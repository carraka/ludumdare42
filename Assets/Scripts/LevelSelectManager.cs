﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour {

    private DataBucket databucket;
    private load_game load_game;
    private Document document;
    private Text currText, nextText, prevText;

    public int maxLevel, mediumFirstLevel, hardFirstLevel, currentLevelNumber;

    private Image prevPageImage, nextPageImage, mediumLockImage, hardLockImage, nextLockImage, prevButtonImage, nextButtonImage;
    private Button prevButton, nextButton;

    //private Animator currAnim, prevAnim, nextAnim;

    private void Awake()
    {
        databucket = GameObject.Find("DataBucket").GetComponent<DataBucket>();
        document = GameObject.Find("Document").GetComponent<Document>();
        load_game = GameObject.Find("LoadLevelManager").GetComponent<load_game>();

        currText = GameObject.Find("currPageText").GetComponent<Text>();
        nextText = GameObject.Find("nextPageText").GetComponent<Text>();
        prevText = GameObject.Find("prevPageText").GetComponent<Text>();

        prevPageImage = GameObject.Find("prevPage").GetComponent<Image>();
        nextPageImage = GameObject.Find("nextPage").GetComponent<Image>();
        mediumLockImage = GameObject.Find("mediumLock").GetComponent<Image>();
        hardLockImage = GameObject.Find("hardLock").GetComponent<Image>();
        nextLockImage = GameObject.Find("nextLock").GetComponent<Image>();
        prevButtonImage = GameObject.Find("prevButton").GetComponent<Image>();
        nextButtonImage = GameObject.Find("nextButton").GetComponent<Image>();


        prevButton = GameObject.Find("prevButton").GetComponent<Button>();
        nextButton = GameObject.Find("nextButton").GetComponent<Button>();

        //currAnim = GameObject.Find("currPage").GetComponent<Animator>();
        //nextAnim = GameObject.Find("nextPage").GetComponent<Animator>();
        //prevAnim = GameObject.Find("prevPage").GetComponent<Animator>();


    }
    // Use this for initialization
    void Start () {
        currentLevelNumber = 1;

        maxLevel = 15;
        mediumFirstLevel = 5;
        hardFirstLevel = 10;

        ResetVisuals();


        //AnimNextPage();
        //for (int i = 0; i < databucket.levelsCleared; i++)
        //{
        //    AnimNextPage();
        //}
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AnimNextPage()
    {
        //currAnim.SetTrigger("turnToNextPage");
        //nextAnim.SetTrigger("turnToNextPage");
        //prevAnim.SetTrigger("turnToNextPage");
        currentLevelNumber++;
        ResetVisuals();
          
    }

    public void AnimPrevPage()
    {
        //currAnim.SetTrigger("turnToPrevPage");
        //nextAnim.SetTrigger("turnToPrevPage");
        //prevAnim.SetTrigger("turnToPrevPage");

        currentLevelNumber--;
        ResetVisuals();

    }

    public void goToPage(string difficulty)
    {
        switch (difficulty)
        {
            case "easy":
                currentLevelNumber = 1;
                ResetVisuals();
                return;

            case "medium":
                if (mediumFirstLevel > databucket.levelsCleared)
                {
                    return;
                }
                else
                {
                    currentLevelNumber = mediumFirstLevel;
                    ResetVisuals();
                    return;
                }

            case "hard":
                if (hardFirstLevel > databucket.levelsCleared)
                {
                    return;
                }
                else
                {
                    currentLevelNumber = hardFirstLevel;
                    ResetVisuals();
                    return;
                }
            default:
                return;
        }
    }

    private void ResetVisuals()
    {
        ResetText();
        ResetPagesAndButtons();
    }

    private void ResetText()
    {
        currText.text = document.ParseDocument(currentLevelNumber);
        if (currentLevelNumber == 1)
        {
            prevText.text = "";
            nextText.text = document.ParseDocument(currentLevelNumber + 1);

        }
        else if (currentLevelNumber == maxLevel)
        {
            prevText.text = document.ParseDocument(currentLevelNumber - 1);
            nextText.text = "";
        }
        else
        {
            prevText.text = document.ParseDocument(currentLevelNumber - 1);
            nextText.text = document.ParseDocument(currentLevelNumber + 1);
        }


    }

    private void ResetPagesAndButtons()
    {
        if (currentLevelNumber == 1)
        {
            prevPageImage.enabled = false;
            prevButtonImage.enabled = false;
            prevButton.enabled = false;
        }
        else if (currentLevelNumber == maxLevel)
        {
            nextPageImage.enabled = false;
            nextButtonImage.enabled = false;
            nextButton.enabled = false;
        }
        else
        {
            prevPageImage.enabled = true;
            prevButtonImage.enabled = true;
            prevButton.enabled = true;
            nextPageImage.enabled = true;
            nextButtonImage.enabled = true;
            nextButton.enabled = true;
        }

        if (currentLevelNumber == databucket.levelsCleared)
        {
            nextLockImage.enabled = true;
        }
        else
        {
            nextLockImage.enabled = false;
        }

        if (databucket.levelsCleared < mediumFirstLevel)
        {
            mediumLockImage.enabled = true;
        }
        else
        {
            mediumLockImage.enabled = false;
        }

        if (databucket.levelsCleared < hardFirstLevel)
        {
            hardLockImage.enabled = true;
        }
        else
        {
            hardLockImage.enabled = false;
        }
    }
}
