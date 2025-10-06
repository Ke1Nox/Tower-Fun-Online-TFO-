using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameStarter : MonoBehaviourPunCallbacks
{
    [Header("Prefabs & Spawns")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerSpawn;
    [SerializeField] private List<Transform> playerSpawnPositions = new List<Transform>();

    private int currentSpawnIndex = 0;

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Conectando a Photon...");
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado al servidor Master. Entrando al lobby...");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Unido al lobby. Intentando unirse o crear una sala...");
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Unido a una sala. Spawneando jugador...");
        StartCoroutine(WaitForSpawnPoint());
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

        CreateAndSetUpPlayerInstance();
    }

    private void CreateAndSetUpPlayerInstance()
    {
        Transform spawn = GetPlayerSpawnPosition();
        if (spawn == null) spawn = playerSpawn;

        int playerIndex = PhotonNetwork.CurrentRoom.PlayerCount - 1;
        Vector3 spawnPosition = spawn.position + new Vector3(playerIndex * 2.5f, 0, 0);

        GameObject player = PhotonNetwork.Instantiate(
            playerPrefab.name,
            spawnPosition,
            spawn.rotation,
            0);

        player.GetComponent<PhotonView>().RPC(
            "RPC_SetNickname",
            RpcTarget.AllBuffered,
            PlayerPrefs.GetString("playerNickname", "Player")
        );
    }

    private Transform GetPlayerSpawnPosition()
    {
        if (playerSpawnPositions == null || playerSpawnPositions.Count == 0)
            return playerSpawn;

        int safeIndex = Mathf.Abs(currentSpawnIndex) % playerSpawnPositions.Count;
        return playerSpawnPositions[safeIndex];
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player {newPlayer.NickName} entered the room");
    }

    [PunRPC]
    public void RPC_UpdateSpawnIndex(int newIndex)
    {
        currentSpawnIndex = newIndex;
    }
}