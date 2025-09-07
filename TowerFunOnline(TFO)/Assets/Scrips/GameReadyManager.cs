using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class GameReadyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private int minPlayersToStart = 2;

    private HashSet<int> readyPlayers = new HashSet<int>(); // guarda IDs de players listos

    public static GameReadyManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (PhotonNetwork.LocalPlayer != null)
            {
                photonView.RPC(nameof(RPC_SetPlayerReady), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
            }
        }
    }

    [PunRPC]
    void RPC_SetPlayerReady(int actorNumber, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        readyPlayers.Add(actorNumber);
        Debug.Log($"Player {actorNumber} está listo ({readyPlayers.Count}/{PhotonNetwork.CurrentRoom.PlayerCount})");

        // Si hay al menos minPlayersToStart listos Y todos los jugadores están listos
        if (PhotonNetwork.CurrentRoom.PlayerCount >= minPlayersToStart &&
            readyPlayers.Count == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            Debug.Log("Todos listos, comenzando lava...");
            photonView.RPC(nameof(RPC_StartGame), RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void RPC_StartGame()
    {
        // Busca la lava en escena y activa su script
        LavaRise lava = FindObjectOfType<LavaRise>();
        if (lava != null) lava.enabled = true; // empieza a subir
    }
}