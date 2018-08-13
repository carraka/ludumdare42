using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class load_game : MonoBehaviour {

    //public bool instructionsOpen = false;
    //public Image blackbg;
    //public Image whiteborder;
    //public Text actualinstructions;
    //private AudioSource buttonSound;
    private DataBucket databucket;


    private AudioSource menuMusic;
    public int currPage;


	// Use this for initialization
	void Awake(){
        //buttonSound = GameObject.Find ("ButtonSound").GetComponent<AudioSource> ();
        databucket = GameObject.Find("DataBucket").GetComponent<DataBucket>();

    }

    void Start () {

	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void LoadLevel() {
		if (SceneManager.GetActiveScene().name == "title")
		{
			SceneManager.LoadScene ("levelselect");
		}
		else
		{
            switch (databucket.level)
            {
                case 1:
                    if (!databucket.easyTutorialPlayed)
                    {
                        databucket.easyTutorialPlayed = true;
                        SceneManager.LoadScene("tutorial");
                    }
                    else
                    {
                        SceneManager.LoadScene("playlevel");
                    }
                    return;
                case 2:
                case 3:
                case 4:
                case 5:
                    SceneManager.LoadScene("playlevel");
                    return;
                case 6:
                    if (!databucket.mediumTutorialPlayed)
                    {
                        databucket.mediumTutorialPlayed = true;
                        SceneManager.LoadScene("tutorial");
                    }
                    else
                    {
                        SceneManager.LoadScene("playlevel");
                    }
                    return;
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                    SceneManager.LoadScene("playlevel");
                    return;
                case 16:
                    if (!databucket.hardTutorialPlayed)
                    {
                        databucket.hardTutorialPlayed = true;
                        SceneManager.LoadScene("tutorial");
                    }
                    else
                    {
                        SceneManager.LoadScene("playlevel");
                    }
                    return;
                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                    SceneManager.LoadScene("playlevel");
                    return;
                default:
                    Debug.Log("level does not exist");
                    return;
            }
			//GameObject.Find ("mainmenu_loop").GetComponent<AudioSource> ().Stop ();
		}

		
	}
	public void LoadCredits() {
		//buttonSound.Play ();
		SceneManager.LoadScene("credits");

		if (SceneManager.GetActiveScene().name == "ending" || SceneManager.GetActiveScene().name == "Level 1")
		{
			//GameObject.Find ("mainmenu_loop").GetComponent<AudioSource> ().Play ();
        }
	}

	public void LoadTitle() {
		
		//buttonSound.Play ();
		SceneManager.LoadScene("title");
        //stop in-game music


        if (SceneManager.GetActiveScene().name == "Level 1" || SceneManager.GetActiveScene().name == "ending")
            //play menu music
            return;
		
	}

    public void RestartLevel()
    {
        SceneManager.LoadScene("playlevel");
    }

    /*public void Instructions(){
		//buttonSound.Play ();

		if (instructionsOpen)
		{
			instructionsOpen = false;
			blackbg.enabled = false;
			whiteborder.enabled = false;
			actualinstructions.enabled = false;
		}
		else
		{
			blackbg.enabled = true;
			whiteborder.enabled = true;
			actualinstructions.enabled = true;
			instructionsOpen = true;

		}
	}*/
}
