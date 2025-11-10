using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class GameEndManager : MonoBehaviour
{
    public static GameEndManager Instance;

    private const string GAME_ENDED_KEY = "GameEnded";
    private const string WINNER_KEY = "Winner";

    void Awake()
    {
        Instance = this;
    }

    public static void MasterCheckForWinner()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (PhotonNetwork.CurrentRoom == null) return;

        // Ya terminó?
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(GAME_ENDED_KEY, out object endedObj) &&
            endedObj is bool ended && ended) return;

        SimplePlayer[] players = FindObjectsOfType<SimplePlayer>();

        int aliveCount = 0;
        Player lastAliveOwner = null;

        foreach (var p in players)
        {
            if (!p.gameObject.activeInHierarchy) continue;
            if (!p.CompareTag("Player")) continue;       // Solo "vivos"
            PhotonView pv = p.GetComponent<PhotonView>();
            if (pv == null) continue;

            aliveCount++;
            lastAliveOwner = pv.Owner;
        }

        if (aliveCount == 1 && lastAliveOwner != null)
        {
            var props = new Hashtable
            {
                { GAME_ENDED_KEY, true },
                { WINNER_KEY, lastAliveOwner.ActorNumber }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            Debug.Log($"[GameEndManager] Winner: {lastAliveOwner.NickName} ({lastAliveOwner.ActorNumber})");
        }

        // (Opcional) Empate: si aliveCount == 0 podrías setear empate con WINNER = -1
    }
}