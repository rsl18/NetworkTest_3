using UnityEngine.Networking;
using UnityEngine;
using System.Collections.Generic;


public class CustomNetworkManager : NetworkManager
{
    int spawnPosIndex = 0;
    Transform startPos;
    public 
    List<Transform> spawnPosList = new List<Transform>();


    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GetSpawnPoint();

        var player = (GameObject)GameObject.Instantiate(playerPrefab, startPos.position, startPos.rotation);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);

        



    }

    void  GetSpawnPoint()
    {
        spawnPosList = startPositions;

        if (playerSpawnMethod == PlayerSpawnMethod.RoundRobin && spawnPosList.Count > 0)
        {
            if (spawnPosIndex >= spawnPosList.Count)
            {
                spawnPosIndex = 0;
            }
​
            startPos = spawnPosList[spawnPosIndex];
            spawnPosIndex += 1;
        }
    }
}
