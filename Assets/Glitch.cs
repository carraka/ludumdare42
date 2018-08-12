using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glitch : MonoBehaviour {

    private int HP;

    private GlitchType type;
    private Vector2 location;
    private float movementSpeed;

    private GlitchState state;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

public enum GlitchType { dasher, scanner, looper, eratic, mom}
public enum GlitchState { moving, action, dying}