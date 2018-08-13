using UnityEngine;
using UnityEngine.UI;

public class Glitch : MonoBehaviour {

    private int HP;

    private GlitchType type;
    private Vector3 location;
    private float movementSpeed;
    private Document document;

    private GlitchState state;
    private Animator animator;
    float animationStart;

    private Vector3 destination;

    private int spaceToReplace;

    private int dasherTargetIndex;
    private int scannerCurrentRow;

    private Vector3 looperCenter;
    private float looperRotation;
    private float looperRotationalSpeed;

    private bool clockwise;


    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        state = GlitchState.moving;
    }

    public void Initiate(GlitchType gType, Vector2 loc, float mSpeed, Document doc, int health = 1)
    {
        type = gType;
        location = loc;
        movementSpeed = mSpeed;
        HP = health;
        document = doc;
        scannerCurrentRow = 99999;



        //Debug.Log("Spawned a " + gType + " at " + loc);

        if (gType == GlitchType.mom || gType == GlitchType.looper)
        {
            if (Random.Range(0, 2) == 0)
                clockwise = true;
        }

        if (gType == GlitchType.looper)
        {
            looperCenter = loc;
            looperRotationalSpeed = Mathf.PI / 100f * movementSpeed;

            if (Random.Range(0, 2) == 0)
            {
                location = looperCenter + new Vector3(0, 100);
                looperRotation = 0;
            }
            else
            {
                location = looperCenter + new Vector3(0, -100);
                looperRotation = Mathf.PI;
            }
            
        }

        GetComponent<RectTransform>().localPosition = location;

        newDestination();
    }
	
	// Update is called once per frame
	void Update () {
		if (state == GlitchState.dying)
        {
            if (Time.time >= animationStart + 0.5f)
                DestroyImmediate(this.gameObject);
            return;
        }

        if (state == GlitchState.action)
        {
            if ((location - destination).magnitude < movementSpeed)
            {
                location = destination;
                GetComponent<RectTransform>().localPosition = location;
                document.spaces[spaceToReplace] = document.FillSpace(document.spaces[spaceToReplace]);
            }
            else
            {
                location += (destination - location).normalized * movementSpeed;
                GetComponent<RectTransform>().localPosition = location;
            }
            if (Time.time >= animationStart + 1f)
            {
                animator.SetBool("Letter", false);
                state = GlitchState.moving;

                if (type == GlitchType.dasher || type == GlitchType.eratic)
                    newDestination();
            }

            return;
        }

        if (state == GlitchState.moving)
        {
            if (type == GlitchType.dasher) //check for dasher reaching its destination
            {
                if (document.spaces[dasherTargetIndex].currentChar == ' ')
                {
                    if ((destination - location).magnitude <= movementSpeed)
                    {
                        DropLetter(dasherTargetIndex);
                        return;
                    }
                }
                else newDestination();
            }
            else if (type == GlitchType.scanner)
            {
                for (int x = 0; x < document.spaces.Count; x++)
                {
                    if (document.spaces[x].location.y == scannerCurrentRow && document.spaces[x].currentChar == ' ' &&
                        (location - document.spaces[x].monitorLocation).magnitude < 25)
                    {
                        DropLetter(x);
                        return;
                    }
                }
            }
            else
            {

                for (int x = 0; x < document.spaces.Count; x++)
                {
                    if (document.spaces[x].currentChar == ' ' && 
                        (location - document.spaces[x].monitorLocation).magnitude < 25)
                    {
                        DropLetter(x);
                        return;
                    }
                }
            }

            if (type == GlitchType.looper)
            {
                if ((destination - looperCenter).magnitude <= movementSpeed / 2f)
                {
                    looperCenter = destination;
                    if (clockwise)
                        looperRotation += looperRotationalSpeed;
                    else looperRotation -= looperRotationalSpeed;
                    location = new Vector3(looperCenter.x + Mathf.Sin(looperRotation) * 100, looperCenter.y + Mathf.Cos(looperRotation));
                    GetComponent<RectTransform>().localPosition = location;
                    newDestination();
                }
                else
                {
                    looperCenter += (destination - looperCenter).normalized * movementSpeed / 2f;
                    if (clockwise)
                        looperRotation += looperRotationalSpeed;
                    else looperRotation -= looperRotationalSpeed;
                    location = new Vector3(looperCenter.x + Mathf.Sin(looperRotation) * 100, looperCenter.y + Mathf.Cos(looperRotation) * 100);
                    GetComponent<RectTransform>().localPosition = location;
                }
            }
            else
            {
                if ((destination - location).magnitude <= movementSpeed)
                {
                    location = destination;
                    GetComponent<RectTransform>().localPosition = location;
                    newDestination();
                }
                else
                {
                    location += (destination - location).normalized * movementSpeed;
                    GetComponent<RectTransform>().localPosition = location;
                }
            }
        }
    }

    private void DropLetter(int index)
    {
        destination = document.spaces[index].monitorLocation;
        if ((location - destination).magnitude < movementSpeed)
        {
            location = destination;
            GetComponent<RectTransform>().localPosition = location;
        }
        else
        {
            location += (document.spaces[index].monitorLocation - destination).normalized * movementSpeed;
            GetComponent<RectTransform>().localPosition = location;
        }


        animationStart = Time.time;
        spaceToReplace = index;
        animator.SetBool("Letter", true);
        state = GlitchState.action;
        document.PlaySFX(GlitchSFX.letter);
    }

    private void newDestination()
    {

        switch (type)
        {
            case GlitchType.dasher:
                dasherTargetIndex = document.GetEmptySpace();
                destination = document.spaces[dasherTargetIndex].monitorLocation;
                break;
            case GlitchType.eratic:
                destination.x = Random.Range(document.documentLeft, document.documentRight);
                destination.y = Random.Range(document.documentTop, document.documentBottom);

                destination = location + (destination - location).normalized * Random.Range(1f, 3f) / Time.deltaTime * movementSpeed;
                break;
            case GlitchType.scanner:
                if(scannerCurrentRow >= document.lineStart.Count)
                {
                    scannerCurrentRow = 0;
                    destination = new Vector3(document.documentLeft - 25, document.RowHeight(scannerCurrentRow));
                }
                else if(scannerCurrentRow % 2 == 0) //moving right on even rows
                {
                    if (location.x < document.documentRight + 25)
                        destination = new Vector3(document.documentRight + 25, document.RowHeight(scannerCurrentRow));
                    else
                    {
                        scannerCurrentRow++;
                        destination = new Vector3(document.documentRight + 25, document.RowHeight(scannerCurrentRow));
                    }
                }
                else
                {
                    if (location.x > document.documentLeft - 25)
                        destination = new Vector3(document.documentLeft - 25, document.RowHeight(scannerCurrentRow));
                    else
                    {
                        scannerCurrentRow++;
                        destination = new Vector3(document.documentLeft - 25, document.RowHeight(scannerCurrentRow));
                    }
                }

                break;
            case GlitchType.looper:
                if (looperCenter.x > 0)
                    destination.x = document.monitorLeft - 25;
                else destination.x = document.monitorRight + 25;

                destination.y = Random.Range(document.monitorTop - 125f, document.monitorBottom + 125f);
                break;
            default:
                destination = Vector3.zero;
                break;
        }

    }
    public void hit()
    {
        HP--;

        if (HP <= 0)
        {
            die();
            document.ReportKill();
        }
        else
        {
            document.PlaySFX(GlitchSFX.hit);
        }
    }

    public void die()
    {
        animator.SetBool("Dying", true);
        state = GlitchState.dying;
        animationStart = Time.time;
        GetComponent<Image>().raycastTarget = false;
        document.PlaySFX(GlitchSFX.death);
    }
}

public enum GlitchType { dasher, scanner, looper, eratic, mom}
public enum GlitchState { moving, action, dying}