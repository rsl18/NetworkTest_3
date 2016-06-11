using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class textupdate : MonoBehaviour {

   NetworkClient nclient;
   CustomManager manager;
   Text text;

	// Use this for initialization
	void Start () {
        manager = GameObject.Find("NetworkManager").GetComponent<CustomManager>();
        nclient = manager.client;
        text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (nclient != null)
        {
            int latency = nclient.GetRTT();
            text.text = latency.ToString();
        }
	}
}
