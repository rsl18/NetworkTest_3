using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class textupdate : NetworkBehaviour {

   NetworkClient nclient;
   CustomManager manager;
   Text text;
   int latency;

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
            latency = nclient.GetRTT();
            text.text = latency.ToString();
        }
	}
}
