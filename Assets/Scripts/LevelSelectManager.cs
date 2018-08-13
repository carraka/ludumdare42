using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour {

    private DataBucket databucket;
    private load_game load_game;
    private Document document;
    private Text currText, nextText, prevText, prevButtonText, nextButtonText;

    public int maxLevel, mediumFirstLevel, hardFirstLevel;

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
        prevButtonText = GameObject.Find("prevButtonText").GetComponent<Text>();
        nextButtonText = GameObject.Find("nextButtonText").GetComponent<Text>();



        prevButton = GameObject.Find("prevButton").GetComponent<Button>();
        nextButton = GameObject.Find("nextButton").GetComponent<Button>();

        //currAnim = GameObject.Find("currPage").GetComponent<Animator>();
        //nextAnim = GameObject.Find("nextPage").GetComponent<Animator>();
        //prevAnim = GameObject.Find("prevPage").GetComponent<Animator>();


    }
    // Use this for initialization
    void Start () {
        databucket.level = 1;
        maxLevel = 23;
        mediumFirstLevel = 8;
        hardFirstLevel = 16;

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
        databucket.level++;
        ResetVisuals();
          
    }

    public void AnimPrevPage()
    {
        //currAnim.SetTrigger("turnToPrevPage");
        //nextAnim.SetTrigger("turnToPrevPage");
        //prevAnim.SetTrigger("turnToPrevPage");
        Debug.Log("animating previous page");
        databucket.level--;
        ResetVisuals();
        Debug.Log("animating previous page");

    }

    public void goToPage(string difficulty)
    {
        switch (difficulty)
        {
            case "easy":
                databucket.level = 1;
                ResetVisuals();
                return;

            case "medium":
                if (mediumFirstLevel > databucket.levelsCleared)
                {
                    return;
                }
                else
                {
                    databucket.level = mediumFirstLevel;
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
                    databucket.level = hardFirstLevel;
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
        currText.text = document.ParseDocument(databucket.level);
        if (databucket.level == 1)
        {
            prevText.text = "";
            nextText.text = document.ParseDocument(databucket.level + 1);

        }
        else if (databucket.level == maxLevel)
        {
            prevText.text = document.ParseDocument(databucket.level - 1);
            nextText.text = "";
        }
        else
        {
            prevText.text = document.ParseDocument(databucket.level - 1);
            nextText.text = document.ParseDocument(databucket.level + 1);
        }


    }

    private void ResetPagesAndButtons()
    {
        if (databucket.level == 1)
        {
            prevPageImage.enabled = false;
            prevButtonImage.enabled = false;
            prevButton.enabled = false;
            prevButtonText.enabled = false;
        }
        else if (databucket.level == maxLevel)
        {
            nextPageImage.enabled = false;
            nextButtonImage.enabled = false;
            nextButton.enabled = false;
            nextButtonText.enabled = false;
        }
        else
        {
            prevPageImage.enabled = true;
            prevButtonImage.enabled = true;
            prevButton.enabled = true;
            prevButtonText.enabled = true;

            nextPageImage.enabled = true;
            nextButtonImage.enabled = true;
            nextButton.enabled = true;
            nextButtonText.enabled = true;
        }

        if (databucket.level == databucket.levelsCleared)
        {
            nextLockImage.enabled = true;
            nextButton.enabled = false;
        }
        else
        {
            nextLockImage.enabled = false;
            nextButton.enabled = true;
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
