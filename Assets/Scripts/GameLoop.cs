using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameLoop : MonoBehaviour {

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

    // Use this for initialization
    void Start ()
    {
        textBox = GetComponentInChildren<Text>();
        cursor = GetComponentInChildren<CursorController>();

        GUIStyle style = new GUIStyle();

        style.font = textBox.font;
        style.fontSize = textBox.fontSize;

        Vector2 fontSize = style.CalcSize(new GUIContent("a"));
        fontWidth = fontSize.x;
        fontHeight = fontSize.y + 1;

        cursor.SetSize(textBox.fontSize, fontWidth, fontHeight);
        cursorLocation = Vector2Int.zero;
        cursor.MoveCursor(cursorLocation);

        minX = Camera.main.pixelWidth * 0.2f;
        maxY = (Camera.main.pixelWidth * 0.75f) * 0.8f - (Camera.main.pixelWidth * 0.75f - Camera.main.pixelHeight)/2;

        //Fetch the Raycaster from the GameObject (the Canvas)
        m_Raycaster = GameObject.Find("Canvas").GetComponent<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        m_EventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
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
            cursor.MoveCursor(cursorLocation);
        }
	}
}
