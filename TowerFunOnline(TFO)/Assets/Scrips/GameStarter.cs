using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
 

public class GameStarter : MonoBehaviourPunCallbacks
{
    [SerializeField] private PhotonView playerPrefab;
    [SerializeField] private Transform playerSpawn;
    [SerializeField] private List<Transform> playerSpawnPsoition = new List<Transform>();

    private int currentSpawnIndex = 0;


    void Start()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {

      GameObject player = PhotonNetwork.Instantiate(playerPrefab.name,GetPlayerSpawnPosition().position,playerSpawn.rotation);
        
    }

    private IEnumerator WaitForSpawnPoint() 
    {

        if (!PhotonNetwork.IsMasterClient)
        {
            yield return new WaitUntil(() => currentSpawnIndex > -1);

        }
        else
        {
            currentSpawnIndex = 0;
        }


        CreateAndSetupPlayerInstance();

        UpdateAndSetupPlayerInstance();
    
    }


}
