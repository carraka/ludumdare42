using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBucket : MonoBehaviour {

    private static bool Protected = false;

    public bool easyTutorialPlayed = false;
    public bool mediumTutorialPlayed = false;
    public bool hardTutorialPlayed = false;


    public static DataBucket instance = null;
    private AudioSource audioSource;
    public int level;
    public int levelsCleared;
    public string endingCode;
    public int glitchesKilled;

    public LevelDataSO levelData;
    
    public Difficulty levelDifficulty;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
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