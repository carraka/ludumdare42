using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameLoop : MonoBehaviour {

    private DataBucket db;
    private Document document;

    private Text textBox;
    private float fontWidth;
    private float fontHeight;

    private float screenDocumentLeft;
    private float screenDocumentTop;

    private Text clock;
    private Text spaceCounter;

    private CursorController cursor;
    private Vector2Int cursorLocation;

    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    private int errorCount;

    private bool playingLevel;
    private float levelStartTime;
    private float nextSmallWave;
    private float nextBigWave;
    private float nextSurge;
    private int smallWavesSpawned;
    private int bigWavesStarted;
    private int surgesSpawned;

    private GameObject momPrefab;
    private GameObject looperPrefab;
    private GameObject dasherPrefab;
    private GameObject scannerPrefab;
    private GameObject eraticPrefab;

    private Transform monitorTransform;

    // Use this for initialization
    void Awake()
    {
        cursor = GetComponentInChildren<CursorController>();
        db = GameObject.Find("DataBucket").GetComponent<DataBucket>();
        document = GetComponentInChildren<Document>();
        textBox = document.GetComponent<Text>();
        clock = GameObject.Find("Clock").GetComponent<Text>();
        spaceCounter = GameObject.Find("SpaceCounter").GetComponent<Text>();

        //Fetch the Raycaster from the GameObject (the Canvas)
        m_Raycaster = GameObject.Find("BGCanvas").GetComponent<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        m_EventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }

    private void Start()
    {
        document.SetUpDocument(db.level);

        GUIStyle style = new GUIStyle();

        style.font = textBox.font;
        style.fontSize = textBox.fontSize;

        Vector2 fontSize = style.CalcSize(new GUIContent("1\n2\n3\n4\n5\n6\n7\n8\n9\n0"));
        fontWidth = fontSize.x;
        fontHeight = fontSize.y / 10;

        cursor.SetSize(textBox.fontSize, fontWidth, fontHeight);
        cursorLocation = Vector2Int.zero;
        cursor.MoveCursor(cursorLocation);

        RectTransform monitorRect = GetComponent<RectTransform>();
        RectTransform documentRect = GameObject.Find("Document").GetComponent<RectTransform>();

        screenDocumentLeft = Camera.main.pixelWidth * monitorRect.anchorMin.x +
            (Camera.main.pixelWidth * (monitorRect.anchorMax.x - monitorRect.anchorMin.x)) * documentRect.anchorMin.x;

        screenDocumentTop = Camera.main.pixelWidth * 0.75f * monitorRect.anchorMax.y -
            (Camera.main.pixelWidth * 0.75f * (monitorRect.anchorMax.y - monitorRect.anchorMin.y)) * (1 - documentRect.anchorMax.y) -
            (Camera.main.pixelWidth * 0.75f - Camera.main.pixelHeight) / 2;

        if (db.levelData.data[db.level].difficulty == Difficulty.easy)
        {
            cursor.GetComponent<Image>().enabled = false;
        }

        playingLevel = false;

        StartCoroutine(LoadLevel());
    }

    private IEnumerator LoadLevel()
    {

        errorCount = 0;

        smallWavesSpawned = 0;
        bigWavesStarted = 0;
        surgesSpawned = 0;

        db.glitchesKilled = 0;

        clock.text = secondsToClock(db.levelData.data[db.level].levelDuration);
        spaceCounter.text = "Spaces " + document.CountOpenSpaces() + "/" + document.spaces.Count;

        momPrefab = (GameObject) Resources.Load("Prefabs/Mom", typeof(GameObject));
        looperPrefab = (GameObject)Resources.Load("Prefabs/Looper", typeof(GameObject));
        dasherPrefab = (GameObject)Resources.Load("Prefabs/Dasher", typeof(GameObject));
        scannerPrefab = (GameObject)Resources.Load("Prefabs/Scanner", typeof(GameObject));
        eraticPrefab = (GameObject)Resources.Load("Prefabs/Eratic", typeof(GameObject));

        monitorTransform = GameObject.Find("Monitor").transform;

        GameObject ready = GameObject.Find("Ready");

        float readyAnimationStartTime = Time.time;

        ready.GetComponent<RectTransform>().localPosition = Vector3.zero;
        ready.GetComponent<Text>().material = new Material(ready.GetComponent<Text>().material);
        while (Time.time < readyAnimationStartTime + 2f)
        {
            float scale = 1 + Time.time - readyAnimationStartTime;
            ready.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, 1f);
            char letter = (char)((int)'a' + Random.Range(0, 26));
            ready.GetComponent<Text>().text = "GET" + letter + "READY";

            if (Time.time > readyAnimationStartTime + 1f)
            {
                Color color = Color.black;
                color.a = 2f - (Time.time - readyAnimationStartTime);
                ready.GetComponent<Text>().color = color;
            }
            yield return null;
        }

        readyAnimationStartTime = Time.time;
        ready.GetComponent<Text>().color = Color.black;
        ready.GetComponent<Text>().text = "GO!";

        while (Time.time < readyAnimationStartTime + 2f)
        {
            float scale = 1 + Time.time - readyAnimationStartTime;
            ready.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, 1f);

            if (Time.time > readyAnimationStartTime + 1f)
            {
                Color color = Color.black;
                color.a = 2f - (Time.time - readyAnimationStartTime);
                ready.GetComponent<Text>().color = color;
            }
            yield return null;
        }

        ready.GetComponent<RectTransform>().localPosition = new Vector3(3000, 0);

        playingLevel = true;
        levelStartTime = Time.time;

        if (db.levelData.data[db.level].smallWaves > 0)
            nextSmallWave = levelStartTime;
        else nextSmallWave = Mathf.Infinity;

        if (db.levelData.data[db.level].bigWaves > 0)
            nextBigWave = levelStartTime + db.levelData.data[db.level].bigTimeBetweenWaves;
        else nextBigWave = Mathf.Infinity;
    }


// Update is called once per frame
    void Update ()
    {
        if (!playingLevel)
            return;

        SpawnGlitches();

        if (Input.GetMouseButtonDown(0))
        {
            onClick();
        }

        if (db.levelData.data[db.level].difficulty != Difficulty.easy)
        {
            bool errors = cursor.CheckKeys();
            if (db.levelData.data[db.level].difficulty == Difficulty.hard && errors)
            {
                errorCount++;
                //Debug.Log("Error Count: " + errorCount);
            }
        }

        int openSpaces = document.CountOpenSpaces();
        clock.text = secondsToClock(levelStartTime + db.levelData.data[db.level].levelDuration - Time.time);
        spaceCounter.text = "Spaces " + openSpaces + "/" + document.spaces.Count;

        if(Time.time >= levelStartTime + db.levelData.data[db.level].levelDuration)
        {
            playingLevel = false;
            //StartCoroutine(ToEvaluation());
            Debug.Log("Time Up");

            Glitch[] glitches = GetComponentsInChildren<Glitch>();
            foreach (Glitch glitch in glitches)
            {
                glitch.enabled = false;
            }
        }

        if(openSpaces == 0)
        {
            playingLevel = false;
            //StartCoroutine(OutOfSpace());
            Debug.Log("Out of Space[s]");
        }

    }

    private IEnumerator ToEvaluation()
    {
        Glitch[] glitches = GetComponentsInChildren<Glitch>();
        foreach (Glitch glitch in glitches)
            glitch.enabled = false;

        yield return null;
    }

    private IEnumerator OutOfSpace()
    {
        Glitch[] glitches = GetComponentsInChildren<Glitch>();
        foreach (Glitch glitch in glitches)
            glitch.enabled = false;
        yield return null;
    }


    private void onClick()
    {
        //Set up the new Pointer Event
        m_PointerEventData = new PointerEventData(m_EventSystem);
        //Set the Pointer Event Position to that of the mouse position
        m_PointerEventData.position = Input.mousePosition;

        //Create a list of Raycast Results
        List<RaycastResult> results = new List<RaycastResult>();

        //Raycast using the Graphics Raycaster and mouse click position
        m_Raycaster.Raycast(m_PointerEventData, results);

        //For every result returned, output the name of the GameObject on the Canvas hit by the Ray

        bool monitorHit = false;
        int glitchHit = -1;

        for (int pos = 0; pos < results.Count; pos++)
        {
            if (results[pos].gameObject.tag == "Monitor")
                monitorHit = true;
            if (results[pos].gameObject.tag == "Glitch" && glitchHit == -1)
                glitchHit = pos;

            //Debug.Log(results[pos].gameObject.name + " hit!");
        }

        if (monitorHit)
        {
            if (glitchHit >= 0)
            {
                results[glitchHit].gameObject.GetComponent<Glitch>().hit();
            }
            else
            {
                int x = Mathf.FloorToInt((Input.mousePosition.x - screenDocumentLeft) / fontWidth);
                int y = Mathf.FloorToInt((screenDocumentTop - Input.mousePosition.y) / fontHeight);

                cursorLocation = new Vector2Int(x, y);
                document.ClickAt(cursorLocation);
            }
        }
    }

    private void SpawnGlitches()
    {
        if (smallWavesSpawned < db.levelData.data[db.level].smallWaves && Time.time >= nextSmallWave)
        {
            for (int x = 0; x < db.levelData.data[db.level].smallUnitsPerWave; x++)
            {
                GameObject newGlitch;

                if (Random.Range(0f, 1f) < db.levelData.data[db.level].smallMomChance)
                {
                    Vector2 spawnLocation;
                    int rand = Random.Range(0, 4);
                    switch(rand)
                    {
                        case 0:
                            spawnLocation = new Vector2(501, 460);
                            break;
                        case 1:
                            spawnLocation = new Vector2(601, -360);
                            break;
                        case 2:
                            spawnLocation = new Vector2(-501, -460);
                            break;
                        default:
                            spawnLocation = new Vector2(-601, 360);
                            break;

                    }
                    newGlitch = Instantiate(momPrefab, monitorTransform);
                    newGlitch.GetComponent<Glitch>().Initiate(GlitchType.mom, new Vector2(0, 0), db.levelData.data[db.level].glitchSpeed, GetComponentInChildren<Document>(), db.levelData.data[db.level].MomHP);
                }
                else
                {
                    int rand = Random.Range(0, 4);

                    float startPos = Random.Range(0f, (document.monitorTop + document.monitorRight + 50)* 2.0f);
                    //Debug.Log(startPos);
                    Vector2 spawnLocation = Vector2.zero;

                    switch(rand)
                    {
                        case 0:
                            if (startPos < document.monitorRight * 2.0f + 50)
                                spawnLocation = new Vector2(startPos - document.monitorRight - 25, document.monitorTop);
                            else
                                spawnLocation = new Vector2(document.monitorRight + 25, startPos - (document.monitorRight * 2.0f + 50) - document.monitorTop - 25);

                            if (Random.Range(0, 2) == 1)
                                spawnLocation *= -1;

                            newGlitch = Instantiate(dasherPrefab, monitorTransform);
                            newGlitch.GetComponent<Glitch>().Initiate(GlitchType.dasher, spawnLocation, db.levelData.data[db.level].glitchSpeed, GetComponentInChildren<Document>());
                            break;
                        case 1:
                            spawnLocation = new Vector2(document.monitorLeft - 25, document.RowHeight(0));
                            newGlitch = Instantiate(scannerPrefab, monitorTransform);
                            newGlitch.GetComponent<Glitch>().Initiate(GlitchType.scanner, spawnLocation, db.levelData.data[db.level].glitchSpeed, GetComponentInChildren<Document>());
                            break;
                        case 2:
                            if (Random.Range(0, 2) == 1)
                                spawnLocation.x = document.monitorLeft - 25;
                            else spawnLocation.x = document.monitorRight + 25;

                            spawnLocation.y = Random.Range(document.monitorTop - 125f, document.monitorBottom + 125f);

                            newGlitch = Instantiate(looperPrefab, monitorTransform);
                            newGlitch.GetComponent<Glitch>().Initiate(GlitchType.looper, spawnLocation, db.levelData.data[db.level].glitchSpeed, GetComponentInChildren<Document>());
                            break;
                        default:
                            if (startPos < document.monitorRight * 2.0f + 50)
                                spawnLocation = new Vector2(startPos - document.monitorRight - 25, document.monitorTop);
                            else
                                spawnLocation = new Vector2(document.monitorRight + 25, startPos - (document.monitorRight * 2.0f + 50) - document.monitorTop - 25);

                            if (Random.Range(0, 2) == 1)
                                spawnLocation *= -1;

                            newGlitch = Instantiate(eraticPrefab, monitorTransform);
                            newGlitch.GetComponent<Glitch>().Initiate(GlitchType.eratic, spawnLocation, db.levelData.data[db.level].glitchSpeed, GetComponentInChildren<Document>());
                            break;

                    }
                }

            }

            smallWavesSpawned++;

            if (smallWavesSpawned < db.levelData.data[db.level].smallWaves)
                nextSmallWave += db.levelData.data[db.level].smallTimeBetweenWaves;
        }
    }

    private string secondsToClock(float seconds)
    {
        int sec = Mathf.Max(Mathf.FloorToInt(seconds),0);
        int min = sec / 60;
        sec = sec % 60;

        string output = min + ":";

        if (sec < 10)
            output += "0";
        output += sec;
        return output;
    }
}
