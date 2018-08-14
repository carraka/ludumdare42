using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBucket : MonoBehaviour {

    public static DataBucket instance = null;

    public int level;
    public int levelsCleared;
    public string endingCode = null;

    public int spacesSaved;
    public int glitchesKilled;
    public int levelSpaces;
    public int errorsMade;

    public bool easyTutorialPlayed;
    public bool mediumTutorialPlayed;
    public bool hardTutorialPlayed;

    public LevelDataSO levelData;

	void Awake ()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this);
            instance = this;
        }
        else
            Destroy(gameObject);
	}
}

public enum Difficulty { easy, medium, hard};