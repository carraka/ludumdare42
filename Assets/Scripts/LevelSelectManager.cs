using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour {

    private DataBucket databucket;
    private load_game load_game;
    private Text currText, nextText, prevText;

    private Animator currAnim, prevAnim, nextAnim;

    private void Awake()
    {
        databucket = GameObject.Find("DataBucket").GetComponent<DataBucket>();
        currText = GameObject.Find("currPageText").GetComponent<Text>();
        nextText = GameObject.Find("nextPageText").GetComponent<Text>();
        prevText = GameObject.Find("prevPageText").GetComponent<Text>();
        load_game = GameObject.Find("LoadLevelManager").GetComponent<load_game>();

        currAnim = GameObject.Find("currPage").GetComponent<Animator>();
        nextAnim = GameObject.Find("nextPage").GetComponent<Animator>();
        prevAnim = GameObject.Find("prevPage").GetComponent<Animator>();


    }
    // Use this for initialization
    void Start () {
        AnimNextPage();
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
        currAnim.SetTrigger("turnToNextPage");
        nextAnim.SetTrigger("turnToNextPage");
        prevAnim.SetTrigger("turnToNextPage");
        load_game.currPage++;
          
    }

    public void AnimPrevPage()
    {
        currAnim.SetTrigger("turnToPrevPage");
        nextAnim.SetTrigger("turnToPrevPage");
        prevAnim.SetTrigger("turnToPrevPage");

        load_game.currPage--;

    }

    public void ResetPagePositions()
    {

    }
}
