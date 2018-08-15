using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Document : MonoBehaviour {

    public List<int> lineStart { get; private set; }
    public List<Space> spaces;

    public Text documentText { get;  private set; }
    private DataBucket db;
    private AudioSource SFX;

    private List<AudioClip> keys;
    private List<AudioClip> meows;
    AudioClip glitchLetter;
    AudioClip glitchDeath;
    AudioClip glitchHit;
    AudioClip glitchBirth;

    private float width;
    private float height;

    public float charWidth { get; private set; }
    public float charHeight { get; private set; }

    public float monitorLeft { get; private set; }
    public float monitorTop { get; private set; }
    public float monitorRight { get; private set; }
    public float monitorBottom { get; private set; }

    public float documentLeft { get; private set; }
    public float documentTop { get; private set; }
    public float documentRight { get; private set; }
    public float documentBottom { get; private set; }

    private Image paw;

    private void Awake()
    {
        documentText = GetComponent<Text>();

        keys = new List<AudioClip>();

        keys.Add((AudioClip)Resources.Load("SFX/Keys/Key1", typeof(AudioClip)));
        keys.Add((AudioClip)Resources.Load("SFX/Keys/Key2", typeof(AudioClip)));
        keys.Add((AudioClip)Resources.Load("SFX/Keys/Key3", typeof(AudioClip)));
        keys.Add((AudioClip)Resources.Load("SFX/Keys/Key4", typeof(AudioClip)));
        keys.Add((AudioClip)Resources.Load("SFX/Keys/Key5", typeof(AudioClip)));
        keys.Add((AudioClip)Resources.Load("SFX/Keys/Key6", typeof(AudioClip)));

        meows = new List<AudioClip>();

        meows.Add((AudioClip)Resources.Load("SFX/meows/LaCatMeow1", typeof(AudioClip)));
        meows.Add((AudioClip)Resources.Load("SFX/meows/LaCatMeow2", typeof(AudioClip)));
        meows.Add((AudioClip)Resources.Load("SFX/meows/LaCatMeow3", typeof(AudioClip)));
        meows.Add((AudioClip)Resources.Load("SFX/meows/LaCatMeow4", typeof(AudioClip)));
        meows.Add((AudioClip)Resources.Load("SFX/meows/LaCatMeow5", typeof(AudioClip)));
        meows.Add((AudioClip)Resources.Load("SFX/meows/LaCatMeow6", typeof(AudioClip)));

        glitchLetter = (AudioClip)Resources.Load("SFX/glitch-alter", typeof(AudioClip));
        glitchDeath = (AudioClip)Resources.Load("SFX/glitch-death", typeof(AudioClip));
        glitchHit = (AudioClip)Resources.Load("SFX/glitch-hit", typeof(AudioClip));
        glitchBirth = (AudioClip)Resources.Load("SFX/glitch-birth", typeof(AudioClip));
    }

    public bool SetUpDocument(int level)
    {
        paw = GameObject.Find("Kitty").GetComponent<Image>();
        db = GameObject.Find("DataBucket").GetComponent<DataBucket>();
        SFX = GameObject.Find("SFX").GetComponent<AudioSource>();


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

        monitorLeft = Camera.main.pixelWidth * (monitorRect.anchorMax.x - monitorRect.anchorMin.x) / -2.0f;
        monitorRight = Camera.main.pixelWidth * (monitorRect.anchorMax.x - monitorRect.anchorMin.x) / 2.0f;
        monitorTop = Camera.main.pixelWidth * 0.75f * (monitorRect.anchorMax.y - monitorRect.anchorMin.y) / 2.0f;
        monitorBottom = Camera.main.pixelWidth * 0.75f * (monitorRect.anchorMax.y - monitorRect.anchorMin.y) / -2.0f;

        documentLeft = Camera.main.pixelWidth * (monitorRect.anchorMax.x - monitorRect.anchorMin.x) * (documentRect.anchorMin.x - 0.5f);
        documentRight = Camera.main.pixelWidth * (monitorRect.anchorMax.x - monitorRect.anchorMin.x) * (documentRect.anchorMax.x - 0.5f);
        documentTop = Camera.main.pixelWidth * 0.75f * (monitorRect.anchorMax.y - monitorRect.anchorMin.y) * (documentRect.anchorMax.y - 0.5f);
        documentBottom = Camera.main.pixelWidth * 0.75f * (monitorRect.anchorMax.y - monitorRect.anchorMin.y) * (documentRect.anchorMin.y - 0.5f);

        //Debug.Log("Document size: " + width + " x " + height);
        //Debug.Log("Monitor Left: " + monitorLeft + ", Top: " + monitorTop + ", Right: " + monitorRight + ", Bottom: " + monitorBottom);
        //Debug.Log("Document Left: " + documentLeft + ", Top: " + documentTop + ", Right: " + documentRight + ", Bottom: " + documentBottom);

        bool working = true;
        int fontSize = 60;

        while (working)
        {
            int maxWidth;
            int maxHeight;
            
            do
            {

                GUIStyle style = new GUIStyle();

                style.font = font;
                style.fontSize = fontSize;

                Vector2 charSize = style.CalcSize(new GUIContent("1\n2\n3\n4\n5\n6\n7\n8\n9\n0"));
                charWidth = charSize.x;
                charHeight = charSize.y / 10;

                maxWidth = Mathf.FloorToInt(width / charWidth);
                maxHeight = Mathf.FloorToInt(height / charHeight);

                // Debug.Log("Font Size: " + fontSize + ", Char Size: " + charWidth + " x " + charHeight + ", Max Width: " + maxWidth + ", Max Height: " + maxHeight);


                if (maxWidth < 20)
                {
                    if (fontSize == 10)
                    {
                        Debug.Log("Too much text");
                        return false;
                    }
                    else
                    {
                        fontSize -= 5;
                    }
                }

            } while (maxWidth < 20);
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
                    spaces.Add(new Space(x, y, ' ', new Vector2(documentLeft + (x + 0.5f) * charWidth, documentTop - (y + 0.5f) * charHeight)));
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
                        fontSize -= 5;
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

        //Debug.Log("spaces: " + spaces.Count);

        //Debug.Log("level " + db.level);

        int initialFill = Mathf.RoundToInt(spaces.Count * db.levelData.data[db.level].percentInitialSpacesFilled);
        
        for(int x = 0; x< initialFill;x++)
        {
            int random = 0;
            do
            {
                random = Random.Range(0, spaces.Count);
            } while (spaces[random].currentChar != ' ');
            spaces[random] = FillSpace(spaces[random]);
        }


        return true;

    }

    public string ParseDocument(int level)
    {
        TextAsset textFile = Resources.Load("epci") as TextAsset;
        //Debug.Log("Loaded text asset ");
        string text = textFile.text;

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
        SFX.PlayOneShot(keys[Random.Range(0, 5)]);

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
        if (db.levelData.data[db.level].difficulty == Difficulty.easy)
        {
            for(int x = 0;x < spaces.Count;x++)
            {
                if (spaces[x].location == location)
                    spaces[x] = ClearSpace(spaces[x]);
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

    public int GetEmptySpace()
    {
        int count = CountOpenSpaces();
        if (count == 0)
        {
            return Random.Range(0, spaces.Count - 1);
        }
        else
        {
            int slot = Random.Range(0, count) + 1;
            count = 0;
            for (int x = 0; x < spaces.Count; x++)
            {
                if (spaces[x].currentChar == ' ')
                {
                    count++;
                    if (count == slot)
                        return x;
                }
            }

            Debug.Log("Did not find desired space, returning a random one");
            return Random.Range(0, spaces.Count - 1);
        }
        //Debug.Log("Int space " + spaces[rand].location + " Monitor Space: " + spaces[rand].monitorLocation);
    }

    public float RowHeight(int y)
    {
        return documentTop - (y + 0.5f) * charHeight;
    }

    public void ReportKill()
    {
        db.glitchesKilled++;
    }

    public int CountOpenSpaces()
    {
        int count = 0;
        foreach(Space space in spaces)
        {
            if (space.currentChar == ' ')
                count++;
        }
        return count;
    }

    public void PlaySFX(GlitchSFX sfx)
    {
        AudioClip sound = null;
        switch (sfx)
        {
            case GlitchSFX.letter:
                sound = glitchLetter;
                break;
            case GlitchSFX.death:
                sound = glitchDeath;
                break;
            case GlitchSFX.hit:
                sound = glitchHit;
                break;
            case GlitchSFX.birth:
                sound = glitchBirth;
                break;
        }

        SFX.PlayOneShot(sound);

    }

    public void Meow()
    {
        SFX.PlayOneShot(meows[Random.Range(0,6)]);
        paw.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));


    }
}

public struct Space
{
    public Vector2Int location;
    public char currentChar;
    public Vector3 monitorLocation;

    public Space(Vector2Int loc, char c, Vector3 monitor)
    {
        location = loc;
        currentChar = c;
        monitorLocation = monitor;
    }

    public Space(int x, int y, char c, Vector3 monitor)
    {
        location = new Vector2Int(x, y);
        currentChar = c;
        monitorLocation = monitor;
    }
}

public enum GlitchSFX { letter, death, hit, birth};
