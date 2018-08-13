using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FeedbackManager : MonoBehaviour {

    private DataBucket databucket;

    private Text spacesSavedText, glitchesKilledText, errorsMadeText, percentSavedText, QAText, ratingText;
    private Image kittyStamp;

    private float spacesSaved, glitchesKilled, totalSpaces, errorsMade;
    private float percentageSaved;

    private AudioSource audio;

    private void Awake()
    {
        databucket = GameObject.Find("DataBucket").GetComponent<DataBucket>();

        spacesSavedText = GameObject.Find("spacesSavedText").GetComponent<Text>();
        glitchesKilledText = GameObject.Find("glitchesKilledText").GetComponent<Text>();
        errorsMadeText = GameObject.Find("errorsMadeText").GetComponent<Text>();
        percentSavedText = GameObject.Find("percentSavedText").GetComponent<Text>();
        QAText = GameObject.Find("QAText").GetComponent<Text>();
        ratingText = GameObject.Find("ratingText").GetComponent<Text>();

        kittyStamp = GameObject.Find("kittyStamp").GetComponent<Image>();

    }
    // Use this for initialization
    void Start () {
        audio = gameObject.AddComponent<AudioSource>();

        UpdateNumbers();
        StartCoroutine("AnimateFeedback");
	}
	
	void UpdateNumbers()
    {
        spacesSaved = databucket.spacesSaved;
        totalSpaces = databucket.levelSpaces;
        errorsMade = databucket.errorsMade;
        percentageSaved = 100f * spacesSaved / (totalSpaces + errorsMade);

        glitchesKilled = databucket.glitchesKilled;

        if (percentageSaved == 100)
        {
            ratingText.text = "Expect Promotion Someday";
            kittyStamp.sprite = Resources.Load<Sprite>("Sprites/Kitty Stamps/perfectkitty1");
            
        }
        else if (percentageSaved > 85)
        {
            ratingText.text = "You Might Be Good in Three Years";
            kittyStamp.sprite = Resources.Load<Sprite>("Sprites/Kitty Stamps/goodkitty1");
        }
        else if (percentageSaved >= databucket.levelData.data[databucket.level].clearPercent * 100f)
        {
            ratingText.text = "Good Enough to Keep Your Job";
            kittyStamp.sprite = Resources.Load<Sprite>("Sprites/Kitty Stamps/badkitty1");
        }
        else
        {
            ratingText.text = "This Cat Edits Better Than You";
            kittyStamp.sprite = Resources.Load<Sprite>("Sprites/Kitty Stamps/ragekitty1");
        }

        if (percentageSaved >= databucket.levelData.data[databucket.level].clearPercent * 100f)
        {
            databucket.levelsCleared = Mathf.Max(databucket.level, databucket.levelsCleared);
        }
    }

    public IEnumerator AnimateFeedback()
    {
        yield return new WaitForSeconds(0.5f);
        spacesSavedText.enabled = true;
        audio.PlayOneShot((AudioClip)Resources.Load("SFX/Keys/Key6-short"));

        //Displays Spaces Saved
        yield return new WaitForSeconds(0.5f);
        for (int i = 1; i <= spacesSaved; i++)
        {
            spacesSavedText.text = "Spaces Saved:       " + i;
            audio.PlayOneShot((AudioClip)Resources.Load("SFX/Keys/Key6-short"));

            yield return new WaitForSeconds(0.06f);
        }
        yield return new WaitForSeconds(0.5f);

        
        audio.PlayOneShot((AudioClip)Resources.Load("SFX/Keys/Key6-short"));
        glitchesKilledText.enabled = true;
        yield return new WaitForSeconds(0.5f);

        //Displays Glitches Killed
        for (int i = 1; i <= glitchesKilled; i++)
        {
            glitchesKilledText.text = "Glitches Killed:     " + i;
            audio.PlayOneShot((AudioClip)Resources.Load("SFX/Keys/Key6-short"));
            yield return new WaitForSeconds(0.06f);
        }
        yield return new WaitForSeconds(0.5f);

        //Displays Errors Made
        if (databucket.levelDifficulty == Difficulty.hard)
        {
            //Displays Percentage Saved
            audio.PlayOneShot((AudioClip)Resources.Load("SFX/Keys/Key6-short"));
            errorsMadeText.enabled = true;
            for (int i = 0; i <= errorsMade; i++)
            {
                errorsMadeText.text = "Errors Made:  " + i;
                audio.PlayOneShot((AudioClip)Resources.Load("SFX/Keys/Key6-short"));
                yield return new WaitForSeconds(0.06f);

            }
            yield return new WaitForSeconds(0.5f);
        }


        //Displays Percentage Saved
        audio.PlayOneShot((AudioClip)Resources.Load("SFX/Keys/Key6-short"));
        percentSavedText.enabled = true;
        for (int i = 0; i <= percentageSaved; i++)
        {
            percentSavedText.text = "Percentage Saved:  " + i + "%";
            audio.PlayOneShot((AudioClip)Resources.Load("SFX/Keys/Key6-short"));
            yield return new WaitForSeconds(0.06f);

        }
        yield return new WaitForSeconds(0.5f);

        QAText.enabled = true;
        audio.PlayOneShot((AudioClip)Resources.Load("SFX/Keys/Key6-short"));
        yield return new WaitForSeconds(0.5f);

        ratingText.enabled = true;
        audio.PlayOneShot((AudioClip)Resources.Load("SFX/Keys/Key6-short"));
        yield return new WaitForSeconds(0.5f);

        kittyStamp.enabled = true;
        
        if (percentageSaved<70)
        {
            audio.PlayOneShot((AudioClip)Resources.Load("Music/00FAIL-ld42"));

        }
        else
        {
            audio.PlayOneShot((AudioClip)Resources.Load("Music/00WIN-ld42"));

        }
    }

    public void DisableVisuals()
    {
        spacesSavedText.enabled = false;
        glitchesKilledText.enabled = false;
        ratingText.enabled = false;
        QAText.enabled = false;
        kittyStamp.enabled = false;
    }
}
