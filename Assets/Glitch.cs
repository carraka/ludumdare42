using UnityEngine;
using UnityEngine.UI;

public class Glitch : MonoBehaviour {

    private int HP;

    private GlitchType type;
    private Vector2 location;
    private float movementSpeed;

    private GlitchState state;
    private Animator animator;
    float animationStart;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();

        HP = 1;
	}
	
	// Update is called once per frame
	void Update () {
		if (state == GlitchState.dying)
        {
            if (Time.time >= animationStart + 0.5f)
                DestroyImmediate(this.gameObject);
        }
	}

    public void hit()
    {
        HP--;

        if(HP <= 0)
            die();
    }

    public void die()
    {
        animator.SetBool("Dying", true);
        state = GlitchState.dying;
        animationStart = Time.time;
        GetComponent<Image>().raycastTarget = false;
    }
}

public enum GlitchType { dasher, scanner, looper, eratic, mom}
public enum GlitchState { moving, action, dying}