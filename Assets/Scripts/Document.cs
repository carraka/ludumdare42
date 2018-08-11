using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Document : MonoBehaviour {

    List<string> lines;
    List<Space> spaces;

    float width;
    float height;

    public bool SetUpDocument(int level)
    {
        string text = ParseDocument(level);
        Font font = GetComponent<Text>().font;

        if (text == "")
            return false;

        width = GetComponent<RectTransform>().rect.width;
        height = GetComponent<RectTransform>().rect.height;

        Debug.Log("Document size: " + width + " x " + height);

        bool working = true;
        int fontSize = 60;

        while (working)
        {
            GUIStyle style = new GUIStyle();

            style.font = font;
            style.fontSize = fontSize;

            Vector2 charSize = style.CalcSize(new GUIContent("a"));
            float charWidth = charSize.x;
            float charHeight = charSize.y + 1;

            int maxWidth = Mathf.FloorToInt(width / charWidth);
            int maxHeight = Mathf.FloorToInt(height / charHeight);

            string testText = text;

            int lastSpace = -1;
            int lastWordBreak = -1;
            int x = 0;
            int y = 0;

            spaces = new List<Space>();

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
                    lastSpace--;
                    spaces.Remove(spaces[lastSpace]);
                    x = pos - lastWordBreak - 1;
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
                    Debug.Log("Parse Successful");

                    foreach (Space space in spaces)
                    {
                        Debug.Log("Space found at " + space.location);
                    }
                }

            }

            GetComponent<Text>().text = testText;
        }

        return true;

    }

    private string ParseDocument(int level)
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
