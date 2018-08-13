using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour {

    private Image tutorialbg;
    private Text tutorialText;

    private int slideNumber;

    private void Awake()
    {
        tutorialbg = GameObject.Find("tutorialbg").GetComponent<Image>();
        tutorialText = GameObject.Find("tutorialText").GetComponent<Text>();
    }
    // Use this for initialization
    void Start () {
        slideNumber = 1;
        tutorialText.text = "It's a calm day at the office, until you're assigned to copyedit the manuscript for one of the market's worst hacks. You open the document and notice something's not quite right ...";

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void NextSlide()
    {
        slideNumber++;
        tutorialbg.sprite = Resources.Load<Sprite>("Tutorial/tutorial" + slideNumber);
        switch (slideNumber)
        {
            case 2:
                tutorialText.text = "That extra O! It just leaps out at you! AoHistorically?! Why is there a letter O instead of a space? Why is the space gone? Such an egregious mistake, just on the title page? Who is Mr. Plum's typist?";
                return;
            case 3:
                tutorialText.text = "Strangely, when you click on the O, it vanishes, leaving the document pristine. (Don't click on that letter now. This is just a tutorial. Just know that you can erase errors by clicking on them.";
                return;
            case 4:
                tutorialText.text = "That's when you notice the glitch. You thought it was an illustration. But it's moving. In fact, it's making a beeline for the nearest space! It must be stealing the spaces!";
                return;
            case 5:
                tutorialText.text = "You click madly on the glitch, and it too vanishes. (Click on the glitches to destroy them before they invade your spaces.) That was odd. Perhaps you're hallucinating. (You're not.)";
                return;
            case 6:
                tutorialText.text = "As more glitches appear, you realize you must remove all the glitches and the errors they make before you hit your deadline. You're an editor. You can do this. How bad can it get?";
                return;
            case 7:
                tutorialText.text = "That's it! Remove glitches on the page and errors in the text by clicking on them. Save enough spaces to advance to the next page. Run out of spaces, and bad things happen.";
                return;
            case 8:
                StartCoroutine("LastSlide");
                return;
            default:
                return;
        }

    }

    IEnumerator LastSlide()
    {
        for (int i = 0; i < 7; i++)
        {
            if (i%2 == 0)
            {
                tutorialbg.sprite = Resources.Load<Sprite>("Tutorial/tutorial9");
                tutorialText.text = "Everything is fine.";
                yield return new WaitForSeconds(0.3f);

            }
            else
            {
                tutorialbg.sprite = Resources.Load<Sprite>("Tutorial/tutorial8");
                tutorialText.text = "THIS IS FINE. EVERYTHING IS FINE.";
                yield return new WaitForSeconds(0.1f);

            }


        }
        SceneManager.LoadScene("levelselect");

    }
}
