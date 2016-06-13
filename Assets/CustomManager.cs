using UnityEngine.Networking;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CustomManager : NetworkManager
{

	Transform startPos;
	public GameObject cameraRig;
    public GameObject UI;
	public Vector3 cameraRigoffset;

    public bool isHost;
    public bool isClient;
    public bool isServer;

	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
	{
		//GetSpawnPoint();
		startPos = GetStartPosition();

		var player = (GameObject)GameObject.Instantiate(playerPrefab, startPos.position, startPos.rotation);
		NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);

	}

    public override void OnStartHost()
    {
        base.OnStartHost();
        isHost = true;
        Debug.Log("HOST");
    }

    public override void OnStartServer()
    {
        cameraRig.SetActive(false);
        base.OnStartServer();
        isServer = true;
        Debug.Log("SERVER");
    }

    public override void OnStartClient(NetworkClient client)
    {
        cameraRig.SetActive(true);
        UI.SetActive(true);
        base.OnStartClient(client);
        isClient = true;
        Debug.Log("CLIENT");
        
    }


}