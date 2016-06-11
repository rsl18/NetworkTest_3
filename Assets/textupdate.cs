using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class textupdate : MonoBehaviour {

    GameObject manager;
    Text text;

	// Use this for initialization
	void Start () {
        manager = GameObject.Find("NetworkManager");
        text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        text.text = manager.GetComponent<NetworkManager>().client.GetRTT().ToString();
	}
}
