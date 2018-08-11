using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Document : MonoBehaviour {

    public TextAsset textAsset;

    public bool SetUpDocument(int level)
    {
        string text = ParseDocument(level);

        if (text == "")
            return false;

        GetComponent<Text>().text = text;

        return true;

    }

    private string ParseDocument(int level)
    {
        int startOfText = textAsset.text.IndexOf("[" + level + "]");
        int endOfText = textAsset.text.IndexOf("[" + (level + 1) + "]");

        if (startOfText == -1)
        {
            Debug.Log("An error has occured loading level " + level);
            return "";
        }

        if (endOfText == -1)
            endOfText = textAsset.text.Length;

        int tagLength = ("[" + level + "]").Length;
        return textAsset.text.Substring(startOfText + tagLength, endOfText - startOfText - tagLength);
    }
}
