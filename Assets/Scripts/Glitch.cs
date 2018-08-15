using UnityEngine;
using UnityEngine.UI;

public class Glitch : MonoBehaviour {

    private int HP;
    private int MaxHP;

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
    private bool momSpawned;

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
        MaxHP = health;
        HP = health;
        document = doc;
        scannerCurrentRow = 99999;
        animationStart = Time.time;



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

            
            if ((loc.x < document.monitorLeft || loc.x > document.monitorRight) && Random.Range(0, 2) == 0)
            {
                location = looperCenter + new Vector3(0, -100);
                looperRotation = Mathf.PI;
            }
            else
            {
                location = looperCenter + new Vector3(0, 100);
                looperRotation = 0;
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
            if (type == GlitchType.mom)
            {
                if (Time.time >= animationStart + .6f && momSpawned == false)
                {
                    momSpawned = true;
                    GameObject newGlitch;
                    Vector3 spawnLocation = location;
                    spawnLocation.y += 25f;
                    Transform monitorTransform = GameObject.Find("Monitor").transform;

                    document.PlaySFX(GlitchSFX.birth);

                    switch (Random.Range(0, 4))
                    {
                        case 0:
                            spawnLocation.y -= 75f;
                            newGlitch = Instantiate((GameObject)Resources.Load("Prefabs/Looper", typeof(GameObject)), monitorTransform);
                            newGlitch.GetComponent<Glitch>().Initiate(GlitchType.looper, spawnLocation, movementSpeed, document);
                            break;
                       case 1:
                            newGlitch = Instantiate((GameObject)Resources.Load("Prefabs/Dasher", typeof(GameObject)), monitorTransform);
                            newGlitch.GetComponent<Glitch>().Initiate(GlitchType.dasher, spawnLocation, movementSpeed, document);
                            break;
                        case 3:
                            newGlitch = Instantiate((GameObject)Resources.Load("Prefabs/Eratic", typeof(GameObject)), monitorTransform);
                            newGlitch.GetComponent<Glitch>().Initiate(GlitchType.eratic, spawnLocation, movementSpeed, document);
                            break;
                        default:
                            newGlitch = Instantiate((GameObject)Resources.Load("Prefabs/Scanner", typeof(GameObject)), monitorTransform);
                            newGlitch.GetComponent<Glitch>().Initiate(GlitchType.scanner, spawnLocation, movementSpeed, document);
                            break;


                    }

                }
                else if(Time.time >= animationStart + 1f)
                {
                    state = GlitchState.moving;
                    animationStart = Time.time;
                    animator.SetBool("Birthing", false);
                    
                }
            }
            else
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
            else if (type == GlitchType.mom)
            {
                if(Time.time >= animationStart + 5f)
                {
                    state = GlitchState.action;
                    animationStart = Time.time;
                    animator.SetBool("Birthing", true);
                    momSpawned = false;
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

        if(state == GlitchState.hit)
        {
            
            if (Time.time > animationStart + .5f)
            {
                animator.SetBool("Hit", false);
                state = GlitchState.moving;
                animationStart = Time.time;
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
                    if (scannerCurrentRow == document.lineStart.Count)
                        scannerCurrentRow = 0;
                    else
                        scannerCurrentRow = Random.Range(0, document.lineStart.Count / 2) * 2;
                    if (location.x < document.monitorLeft)
                        location.y = document.RowHeight(scannerCurrentRow);
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
            case GlitchType.mom:
                //destination.x = Random.Range(document.documentLeft, document.documentRight);
                //destination.y = Random.Range(document.documentTop, document.documentBottom);

                //destination = location + (destination - location).normalized * Random.Range(1f, 3f) / Time.deltaTime * movementSpeed;

                destination = location;

                if (location.y > document.monitorTop)
                    destination.y = document.monitorTop - 50;
                if (location.y < document.monitorBottom)
                    destination.y = document.monitorBottom + 50;
                if (location.x < document.monitorLeft)
                    destination.x = document.monitorLeft + 50;
                if (location.x > document.monitorRight)
                    destination.x = document.monitorRight - 50;

                if (destination == location)
                {
                    //destination = Vector3.zero;

                    if (clockwise)
                    {
                        if (location.y >= document.monitorTop - 60 && location.x < document.monitorRight - 50)
                            destination = new Vector3(document.monitorRight - 50, document.monitorTop - 50);
                        if (location.x >= document.monitorRight - 60 && location.y > document.monitorBottom + 50)
                            destination = new Vector3(document.monitorRight - 50, document.monitorBottom + 50);
                        if (location.y <= document.monitorBottom + 60 && location.x > document.monitorLeft + 50)
                            destination = new Vector3(document.monitorLeft + 50, document.monitorBottom + 50);
                        if (location.x <= document.monitorLeft + 60 && location.y < document.monitorTop - 50)
                            destination = new Vector3(document.monitorLeft + 50, document.monitorTop - 50);
                    }
                    else
                    {
                        if (location.y >= document.monitorTop - 60 && location.x > document.monitorLeft + 50)
                            destination = new Vector3(document.monitorLeft + 50, document.monitorTop - 50);
                        if (location.x <= document.monitorLeft + 60 && location.y > document.monitorBottom + 50)
                            destination = new Vector3(document.monitorLeft + 50, document.monitorBottom + 50);
                        if (location.y <= document.monitorBottom + 60 && location.x < document.monitorRight - 50)
                            destination = new Vector3(document.monitorRight - 50, document.monitorBottom + 50);
                        if (location.x >= document.monitorRight - 60 && location.y < document.monitorTop - 51)
                            destination = new Vector3(document.monitorRight - 50, document.monitorTop - 50);

                    }
                }
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
            animator.SetBool("Hit", true);
            animator.SetFloat("Health", (float)HP / MaxHP);
            document.PlaySFX(GlitchSFX.hit);
            state = GlitchState.hit;
            animationStart = Time.time;
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
public enum GlitchState { moving, action, hit, dying}