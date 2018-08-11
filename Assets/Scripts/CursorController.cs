using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorController : MonoBehaviour {

    private float StartTime;
    public float blinkTime;

    private Image cursorBG;
    private Text cursorChar;

    float charWidth, charHeight;

	// Use this for initialization
	void Start ()
    {
        StartTime = Time.time;
        cursorBG = GetComponent<Image>();
        cursorChar = GetComponentInChildren<Text>();
	}
	
    public void SetSize(int fontSize, float width, float height)
    {
        cursorChar.fontSize = fontSize;

        GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        charWidth = width;
        charHeight = height;
    }

    public void MoveCursor(Vector2Int location)
    {
        Debug.Log(location);
        GetComponent<RectTransform>().anchoredPosition = new Vector2(location.x * charWidth, location.y * -charHeight);
    }
	// Update is called once per frame
	void Update ()
    {
        if (blinkTime != 0)
        {
            int blinkNum = Mathf.FloorToInt((Time.time - StartTime) / blinkTime);

            if (blinkNum % 2 == 0)
            {
                cursorBG.color = Color.black;
                cursorChar.color = Color.white;
            }
            else
            {
                cursorBG.color = Color.clear;
                cursorChar.color = Color.clear;
            }
        }
        else
        {
            cursorBG.color = Color.black;
            cursorChar.color = Color.white;
        }
    }
}
