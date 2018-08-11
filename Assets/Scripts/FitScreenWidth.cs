using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitScreenWidth : MonoBehaviour {

	// Use this for initialization
	void Start () {
        RectTransform RT = GetComponent<RectTransform>();

        float scale = Camera.main.pixelWidth / RT.sizeDelta.x;

        RT.sizeDelta *= scale;

        Debug.Log(RT.sizeDelta);

    }
}
