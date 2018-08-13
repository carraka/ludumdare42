using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorController : MonoBehaviour {

    private float StartTime;
    public float blinkTime;

    private Image cursorBG;
    private Text cursorChar;
    private Document document;

    float charWidth, charHeight;

    private Vector2Int cursorLocation;

	// Use this for initialization
	void Awake ()
    {
        StartTime = Time.time;
        cursorBG = GetComponent<Image>();
        cursorChar = GetComponentInChildren<Text>();
        document = GetComponentInParent<Document>();
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
        cursorLocation = location;
        GetComponent<RectTransform>().anchoredPosition = new Vector2(location.x * charWidth, location.y * -charHeight);
        cursorChar.text = document.CharAt(location).ToString();
        StartTime = Time.time;
    }

    public bool CheckKeys()
    {
        bool moved = false;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (cursorLocation.x > 0)
            {
                cursorLocation.x--;
                moved = true;
            }
            else if (cursorLocation.y > 0)
            {
                cursorLocation.y--;
                cursorLocation.x = document.lineStart[cursorLocation.y + 1] - document.lineStart[cursorLocation.y] - 2;
                moved = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (cursorLocation.y < document.lineStart.Count - 1)
            {
                moved = true;
                cursorLocation.x++;
                if (document.lineStart[cursorLocation.y] + cursorLocation.x + 1 >= document.lineStart[cursorLocation.y + 1])
                {
                    cursorLocation.x = 0;
                    cursorLocation.y++;
                }
            }
            else if (document.lineStart[cursorLocation.y] + cursorLocation.x + 1 < document.documentText.text.Length)
            {
                cursorLocation.x++;
                moved = true;
            }

        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            cursorLocation.y--;
            moved = true;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            cursorLocation.y++;
            moved = true;
        }

        if (moved == true)
        { 
            cursorLocation = document.ClampToText(cursorLocation);

            MoveCursor(cursorLocation);
        }

        for(int x = 0; x< document.spaces.Count;x++)
        {
            if (document.spaces[x].location == cursorLocation)
            {
                //Debug.Log("'" + document.spaces[x].currentChar + "'");

                if(Input.GetKeyDown((KeyCode)((int)KeyCode.A + (int)document.spaces[x].currentChar - (int)'a')))
                {
                    document.spaces[x] = document.ClearSpace(document.spaces[x]);
                    cursorChar.text = " ";
                    return false;
                }
            }
        }

        for (int x = 0;x < 26;x++)
        {
            if (Input.GetKeyDown((KeyCode)((int)KeyCode.A + x)))
            {
                return true;
            }
        }

        return false;
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
