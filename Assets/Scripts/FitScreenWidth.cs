using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitScreenWidth : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Camera.main.orthographicSize = 8.0f * ( (float) Camera.main.pixelHeight / Camera.main.pixelWidth);

        RectTransform RT = GetComponent<RectTransform>();

        float scale = Camera.main.pixelWidth / RT.sizeDelta.x;

        RT.sizeDelta *= scale;


    }
}
