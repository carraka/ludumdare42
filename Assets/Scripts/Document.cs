using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Document : MonoBehaviour {

    public List<int> lineStart;
    public List<Space> spaces;

    public Text documentText { get;  private set; }
    private DataBucket db;

    float width;
    float height;

    private void Awake()
    {
        documentText = GetComponent<Text>();
    }

    private void Start()
    {
        db = GameObject.Find("DataBucket").GetComponent<DataBucket>();
    }
    public bool SetUpDocument(int level)
    {

        
        string text = ParseDocument(level);
        Font font = documentText.font;

        if (text == "")
            return false;

        //width = GetComponent<RectTransform>().rect.width;
        //height = GetComponent<RectTransform>().rect.height;

        RectTransform monitorRect = GameObject.Find("Monitor").GetComponent<RectTransform>();
        RectTransform documentRect = GetComponent<RectTransform>();


        width = Camera.main.pixelWidth * 
            (monitorRect.anchorMax.x - monitorRect.anchorMin.x) * 
            (documentRect.anchorMax.x - documentRect.anchorMin.x);

        height = Camera.main.pixelWidth * 0.75f *
            (monitorRect.anchorMax.y - monitorRect.anchorMin.y) *
            (documentRect.anchorMax.y - documentRect.anchorMin.y);


        //Debug.Log("Document size: " + width + " x " + height);

        bool working = true;
        int fontSize = 60;

        while (working)
        {
            GUIStyle style = new GUIStyle();

            style.font = font;
            style.fontSize = fontSize;

            Vector2 charSize = style.CalcSize(new GUIContent("a"));
            float charWidth = charSize.x;
            float charHeight = charSize.y;

            int maxWidth = Mathf.FloorToInt(width / charWidth);
            int maxHeight = Mathf.FloorToInt(height / charHeight);

            string testText = text;

            int lastSpace = -1;
            int lastWordBreak = -1;
            int x = 0;
            int y = 0;

            spaces = new List<Space>();
            lineStart = new List<int>();

            lineStart.Add(0);

            for (int pos = 0;pos < testText.Length;pos++)
            {
                if (testText[pos] == '\t')
                {
                    testText = testText.Substring(0, pos) + "   " + testText.Substring(pos + 1);
                    pos += 2;
                    x += 2;
                    lastWordBreak = pos;
                }
                else if(testText[pos] == '\n')
                {
                    x = -1;
                    y++;
                    lastWordBreak = pos;
                    lineStart.Add(pos + 1);
                }
                else if(testText[pos] == ' ')
                {
                    spaces.Add(new Space(x, y, ' '));
                    lastSpace++;
                    lastWordBreak = pos;
                }

                if (x >= maxWidth)
                {
                    testText = testText.Substring(0, lastWordBreak) + "\n" + testText.Substring(lastWordBreak + 1);
                    spaces.Remove(spaces[lastSpace]);
                    lastSpace--;

                    x = pos - lastWordBreak - 1;
                    lineStart.Add(lastWordBreak + 1);
                    y++;
                }

                if (y >= maxHeight)
                {
                    if (fontSize == 10)
                    {
                        Debug.Log("Too much text");
                        return false;
                    }
                    else
                    {
                        fontSize -= 10;
                        break;
                    }
                }

                if (pos == testText.Length - 1)
                {
                    working = false;
                    //Debug.Log("Parse Successful");
                }

                x++;

            }

            documentText.text = testText;
            documentText.fontSize = fontSize;
        }

        for(int x = 0; x< spaces.Count;x++)
        {
            spaces[x] = FillSpace(spaces[x]);
        }


        return true;

    }

    public string ParseDocument(int level)
    {
        StreamReader reader = new StreamReader("Assets/Resources/epci.txt");
        string text = reader.ReadToEnd();

        int startOfText = text.IndexOf("[" + level + "]");
        int endOfText = text.IndexOf("[" + (level + 1) + "]");

        if (startOfText == -1)
        {
            Debug.Log("An error has occured loading level " + level);
            return "";
        }

        if (endOfText == -1)
            endOfText = text.Length;

        int tagLength = ("[" + level + "]").Length;
        return text.Substring(startOfText + tagLength, endOfText - startOfText - tagLength - 2);
    }

    public Space FillSpace(Space space)
    {
        char letter = (char)(Random.Range((int)'a',(int)'z'));

        return FillSpace(space, letter);
    }

    public Space ClearSpace(Space space)
    {
        return FillSpace(space, ' ');
    }

    private Space FillSpace(Space space, char letter)
    {
        int pos = VectorToPos(space.location);
        space.currentChar = letter;

        string text = documentText.text;

        documentText.text = text.Substring(0, pos) + letter + text.Substring(pos + 1);

        return space;
    }

    private int VectorToPos(Vector2Int location)
    {
        return lineStart[location.y] + location.x;
    }

    public void ClickAt(Vector2Int location)
    {
        if (db.levelDifficulty == Difficulty.easy)
        {
            foreach(Space space in spaces)
            {
                if (space.location == location)
                    ClearSpace(space);
            }
        }
        else
        {
            location = ClampToText(location);
            GetComponentInChildren<CursorController>().MoveCursor(location);
        }
    }
    public Vector2Int ClampToText(Vector2Int location)
    {
        location.y = Mathf.Clamp(location.y, 0, lineStart.Count - 1);
        if (location.x < 0)
            location.x = 0;
        
        if (location.y == lineStart.Count-1)
        {
            if (lineStart[location.y] + location.x >= documentText.text.Length)
                location.x = documentText.text.Length - lineStart[location.y] - 1;
        }
        else
        {
            if (lineStart[location.y] + location.x >= lineStart[location.y + 1] - 1)
                location.x = lineStart[location.y + 1] - lineStart[location.y] - 2;
        }

        return location;
    }

    public char CharAt(Vector2Int location)
    {
        return documentText.text[VectorToPos(location)];
    }
}

public struct Space
{
    public Vector2Int location;
    public char currentChar;

    public Space(Vector2Int loc, char c)
    {
        location = loc;
        currentChar = c;
    }

    public Space(int x, int y, char c)
    {
        location = new Vector2Int(x, y);
        currentChar = c;
    }
}
