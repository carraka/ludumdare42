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

    private float minX;
    private float maxY;

    private CursorController cursor;
    private Vector2Int cursorLocation;

    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    private int errorCount;

    // Use this for initialization
    void Awake ()
    {
        cursor = GetComponentInChildren<CursorController>();
        db = GameObject.Find("DataBucket").GetComponent<DataBucket>();
        document = GetComponentInChildren<Document>();
        textBox = document.GetComponent<Text>();

        //Fetch the Raycaster from the GameObject (the Canvas)
        m_Raycaster = GameObject.Find("Canvas").GetComponent<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        m_EventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }

    private void Start()
    {
        document.SetUpDocument(db.level);

        GUIStyle style = new GUIStyle();

        style.font = textBox.font;
        style.fontSize = textBox.fontSize;

        Vector2 fontSize = style.CalcSize(new GUIContent("a"));
        fontWidth = fontSize.x;
        fontHeight = fontSize.y;

        cursor.SetSize(textBox.fontSize, fontWidth, fontHeight);
        cursorLocation = Vector2Int.zero;
        cursor.MoveCursor(cursorLocation);

        RectTransform monitorRect = GetComponent<RectTransform>();
        RectTransform documentRect = GameObject.Find("Document").GetComponent<RectTransform>();

        minX = Camera.main.pixelWidth * monitorRect.anchorMin.x +
            (Camera.main.pixelWidth * (monitorRect.anchorMax.x - monitorRect.anchorMin.x)) * documentRect.anchorMin.x;

        maxY = Camera.main.pixelWidth * 0.75f * monitorRect.anchorMax.y -
            (Camera.main.pixelWidth * 0.75f * (monitorRect.anchorMax.y - monitorRect.anchorMin.y)) * (1 - documentRect.anchorMax.y) -
            (Camera.main.pixelWidth * 0.75f - Camera.main.pixelHeight) / 2;


        errorCount = 0;

        if (db.levelDifficulty == Difficulty.easy)
        {
            cursor.GetComponent<Image>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update ()
    {

        if (Input.GetMouseButtonDown(0))
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
            foreach (RaycastResult result in results)
            {
                Debug.Log("Hit " + result.gameObject.name);
            }

            int x = Mathf.FloorToInt((Input.mousePosition.x - minX) / fontWidth);
            int y = Mathf.FloorToInt((maxY - Input.mousePosition.y) / fontHeight);

            cursorLocation = new Vector2Int(x, y);
            document.ClickAt(cursorLocation);
        }

        if (db.levelDifficulty != Difficulty.easy)
        {
            bool errors = cursor.CheckKeys();
            if (db.levelDifficulty == Difficulty.hard && errors)
            {
                errorCount++;
                //Debug.Log("Error Count: " + errorCount);
            }
        }

    }
}
