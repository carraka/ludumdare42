using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBucket : MonoBehaviour {

    private static bool Protected = false;

    public int level;
    public int levelsCleared;

	// Use this for initialization
	void Start ()
    {
        if (Protected == false)
        {
            DontDestroyOnLoad(this);
            Protected = true;
        }
        else
            DestroyImmediate(this);
	}
}
