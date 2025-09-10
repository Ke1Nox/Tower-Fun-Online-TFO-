using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections.Generic;

public class ReadyUpSystem : MonoBehaviourPunCallbacks
{
    public static ReadyUpSystem Instance;

    [SerializeField] private int minPlayersReady = 2;
    [SerializeField] private TextMeshProUGUI logText;

    private HashSet<int> readyPlayers = new HashSet<int>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (logText != null)
        {
            // Mensaje inicial para todos los jugadores
            logText.text = "Presiona ENTER para estar listo.\n";
        }
    }

    void Update()
    {
        if (PhotonNetwork.InRoom && Input.GetKeyDown(KeyCode.Return))
        {
            if (!readyPlayers.Contains(PhotonNetwork.LocalPlayer.ActorNumber))
            {
                photonView.RPC(nameof(RPC_SetPlayerReady), RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber, PhotonNetwork.NickName);
            }
        }
    }

    [PunRPC]
    void RPC_SetPlayerReady(int actorNumber, string nickname)
    {
        readyPlayers.Add(actorNumber);
        int totalPlayers = PhotonNetwork.CurrentRoom.PlayerCount;

        ShowLog($"{nickname} está listo ({readyPlayers.Count}/{totalPlayers})");

        if (PhotonNetwork.IsMasterClient && readyPlayers.Count >= minPlayersReady)
        {
            photonView.RPC(nameof(RPC_StartGame), RpcTarget.AllBuffered);
        }
        else
        {
            int faltan = Mathf.Max(0, minPlayersReady - readyPlayers.Count);
            ShowLog($"Faltan {faltan} jugador(es) más para iniciar.");
        }
    }

    [PunRPC]
    void RPC_StartGame()
    {
        ShowLog("¡La lava comienza a subir!");
        LavaRise lava = FindObjectOfType<LavaRise>();
        if (lava != null)
        {
            lava.StartRising();
        }
    }

    private void ShowLog(string message)
    {
        Debug.Log(message);

        if (logText != null)
        {
            logText.text += "\n" + message;
        }
    }
}
