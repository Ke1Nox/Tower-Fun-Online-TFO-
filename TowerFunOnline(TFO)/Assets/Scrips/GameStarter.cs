using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable; // <<< alias obligatorio


public class GameStarter : MonoBehaviourPunCallbacks
{
    [Header("Prefabs & Spawns")]
    [SerializeField] private string playerResourcePath = "Prefabs/Player"; // ruta dentro de Resources
    [SerializeField] private Transform playerSpawn;
    [SerializeField] private List<Transform> playerSpawnPositions = new List<Transform>();

    [Header("Match Reset")]
    [SerializeField] private bool resetRoomPropsOnStart = true;

    private bool hasSpawned = false;
    private int currentSpawnIndex = 0;

    void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            SafeMatchInit();                  // <<< clave para que no dispare Win/Lose al entrar
            StartCoroutine(WaitAndSpawn());
        }
        else
        {
            Debug.LogWarning("[GameStarter] Start sin estar en sala. Esperando OnJoinedRoom...");
        }
    }

    public override void OnJoinedRoom()
    {
        SafeMatchInit();
        if (!hasSpawned) StartCoroutine(WaitAndSpawn());
    }

    /// <summary>
    /// Setea estado Alive del local y (solo Master) limpia props de final de partida.
    /// </summary>
    private void SafeMatchInit()
    {
        // 1) Cada jugador arranca como vivo
        PlayerPropsUtil.SetState(PhotonNetwork.LocalPlayer, PlayerState.Alive);

        // 2) Solo el Master limpia props heredadas (NO tocar ROUND_ACTIVE acá)
        if (resetRoomPropsOnStart && PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
        {
            var clearWinner = new Hashtable { { MatchProps.WINNER, null } };
            PhotonNetwork.CurrentRoom.SetCustomProperties(clearWinner);

            var reset = new Hashtable {
            { MatchProps.GAME_ENDED, false },
            { MatchProps.ALL_LOSE,   false }
        };
            PhotonNetwork.CurrentRoom.SetCustomProperties(reset);
        }

        // 3) Reactivar SIEMPRE el listener (esté quien esté)
        //var listener = FindObjectOfType<RoomPropertiesListener>();
        //if (listener != null && !listener.enabled) listener.enabled = true;
    }


    private IEnumerator WaitAndSpawn()
    {
        // Pequeña espera para garantizar que las props reseteadas se propaguen
        yield return null;

        if (!PhotonNetwork.IsMasterClient)
            yield return new WaitUntil(() => currentSpawnIndex >= 0);

        CreateAndSetUpPlayerInstance();
    }

    private void CreateAndSetUpPlayerInstance()
    {
        if (hasSpawned) return;

        Transform spawn = GetPlayerSpawnPosition() ?? playerSpawn;
        Vector3 spawnPos = (spawn != null ? spawn.position : Vector3.zero);

        // Separación simple por ActorNumber para evitar overlap en el mismo spawn
        int offset = Mathf.Max(0, PhotonNetwork.LocalPlayer.ActorNumber - 1);
        spawnPos += new Vector3(offset * 2.0f, 0f, 0f);

        GameObject player = PhotonNetwork.Instantiate(
            playerResourcePath,
            spawnPos,
            spawn ? spawn.rotation : Quaternion.identity,
            0
        );

        hasSpawned = true;

        // Setear nickname (tu SimplePlayer tiene RPC_SetNickname)
        var view = player.GetComponent<PhotonView>();
        if (view != null)
        {
            string nick = PlayerPrefs.GetString("playerNickname", "PLAYER");
            view.RPC("RPC_SetNickname", RpcTarget.AllBuffered, nick);
        }
    }

    private Transform GetPlayerSpawnPosition()
    {
        if (playerSpawnPositions != null && playerSpawnPositions.Count > 0)
        {
            int safeIndex = Mathf.Abs(currentSpawnIndex) % playerSpawnPositions.Count;
            return playerSpawnPositions[safeIndex];
        }
        return playerSpawn;
    }


}
