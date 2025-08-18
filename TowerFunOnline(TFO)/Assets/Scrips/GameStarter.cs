using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameStarter : MonoBehaviourPunCallbacks
{
    
    void Start()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {

     //   GameObject player = PhotonNetwork.Instantiate(playerPrefab.name,);
        
    }

}
