﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingManager : MonoBehaviour {

    private DataBucket databucket;
    private Image illustration, textbox;
    private Text endingText;

    private void Awake()
    {
        illustration = GameObject.Find("endingIllustration").GetComponent<Image>();
        textbox = GameObject.Find("bottomUI").GetComponent<Image>();
        endingText = GameObject.Find("endingText").GetComponent<Text>();
        databucket = GameObject.Find("DataBucket").GetComponent<DataBucket>();
    }
    // Use this for initialization
    void Start () {
        AudioSource audio = gameObject.AddComponent<AudioSource>();

        switch (databucket.endingCode)
        {
            case "fail":
                //audio.PlayOneShot((AudioClip)Resources.Load("Music/ld41_ending_chicken"));
                illustration.sprite = Resources.Load<Sprite>("Sprites/Endings/badend_clean");
                textbox.sprite = Resources.Load<Sprite>("Sprites/Endings/gameover");
                Vector2 top = endingText.GetComponent<RectTransform>().offsetMax;
                top.y = -20;
                endingText.GetComponent<RectTransform>().offsetMax = top;
                endingText.text = "You ran out of spaces! The glitches have invaded your manuscript, delaying the publication date by weeks! Not that you were around to see it. The bestselling machine will churn on without you.";
                break;

            case "succeed":
                illustration.sprite = Resources.Load<Sprite>("Sprites/Endings/goodend_clean");
                textbox.sprite = Resources.Load<Sprite>("Sprites/Endings/youdidit");

                //audio.PlayOneShot((AudioClip)Resources.Load("Music/ld41_ending_romantic"));
                endingText.text = "You saved the spaces! The book you edited catapults to the top of the bestseller list, and your job is secure. If only you had had time to do a proper edit. Not that it would have helped the story much ...";
                break;
        }

        databucket.endingCode = "";
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
