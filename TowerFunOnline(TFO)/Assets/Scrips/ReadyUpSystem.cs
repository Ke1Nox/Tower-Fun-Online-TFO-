using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class ReadyUpSystem : MonoBehaviourPunCallbacks
{
    public static ReadyUpSystem Instance;

    [SerializeField] private int minPlayersReady = 2;

    private HashSet<int> readyPlayers = new HashSet<int>();

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (PhotonNetwork.InRoom && Input.GetKeyDown(KeyCode.Return))
        {
            if (!readyPlayers.Contains(PhotonNetwork.LocalPlayer.ActorNumber))
            {
                photonView.RPC(nameof(RPC_SetPlayerReady), RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber);
            }
        }
    }

    [PunRPC]
    void RPC_SetPlayerReady(int actorNumber)
    {
        readyPlayers.Add(actorNumber);
        Debug.Log("Jugador " + actorNumber + " está listo. Total listos: " + readyPlayers.Count);

        if (PhotonNetwork.IsMasterClient && readyPlayers.Count >= minPlayersReady)
        {
            photonView.RPC(nameof(RPC_StartGame), RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void RPC_StartGame()
    {
        LavaRise lava = FindObjectOfType<LavaRise>();
        if (lava != null)
        {
            lava.StartRising();
        }
    }
}