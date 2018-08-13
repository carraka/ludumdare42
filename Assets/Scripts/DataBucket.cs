using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBucket : MonoBehaviour {

    private static bool Protected = false;

    public int level;
    public int levelsCleared;
    public string endingCode;

    public int spacesSaved;
    public int glitchesKilled;
    public int levelSpaces;
    public int errorsMade;

    public LevelDataSO levelData;

    public Difficulty levelDifficulty; 

	void Start ()
    {
        if (Protected == false)
        {
            DontDestroyOnLoad(this);
            Protected = true;
        }
        else
            DestroyImmediate(this);
	}
}

public enum Difficulty { easy, medium, hard};