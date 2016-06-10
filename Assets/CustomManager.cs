﻿using UnityEngine.Networking;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CustomManager : NetworkManager
{
	int spawnPosIndex = 0;
	Transform startPos;
	GameObject cameraRig;
	public Vector3 cameraRigoffset;
	List<Transform> spawnPosList = new List<Transform>();


	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
	{
		//GetSpawnPoint();
		startPos = GetStartPosition();

		var player = (GameObject)GameObject.Instantiate(playerPrefab, startPos.position, startPos.rotation);
		NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);

		cameraRig = GameObject.Find ("[CameraRig]");
		cameraRig.transform.position = startPos.position + cameraRigoffset;
		cameraRig.transform.rotation = startPos.rotation;

	}
		

}