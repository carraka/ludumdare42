using UnityEngine;
using UnityEngine.UI;

public class manuscriptViewer : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Text>().text = GetComponent<Document>().ParseDocument(GameObject.Find("DataBucket").GetComponent<DataBucket>().level);
    }
	
	// Update is called once per frame
	void Update () {

	}
}
