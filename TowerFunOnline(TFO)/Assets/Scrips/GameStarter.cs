using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameStarter : MonoBehaviourPunCallbacks
{
    [Header("Prefabs & Spawns")]
    [SerializeField] private GameObject playerPrefab;           // usar GameObject para PhotonNetwork.Instantiate
    [SerializeField] private Transform playerSpawn;             // fallback si la lista está vacía
    [SerializeField] private List<Transform> playerSpawnPositions = new List<Transform>();

    private int currentSpawnIndex = 0;

    void Start()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom()
    {
        // Usar la corrutina que maneja el índice y la creación
        StartCoroutine(WaitForSpawnPoint());
    }

    private IEnumerator WaitForSpawnPoint()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            // Espera a que el master haya seteado y replicado el índice
            yield return new WaitUntil(() => currentSpawnIndex > -1);
        }
        else
        {
            currentSpawnIndex = 0;
        }

        CreateAndSetUpPlayerInstance();
        UpdateSpawnIndexForAll();
    }

    private void CreateAndSetUpPlayerInstance()
    {
        Transform spawn = GetPlayerSpawnPosition();
        if (spawn == null) spawn = playerSpawn;

        GameObject player = PhotonNetwork.Instantiate(
            playerPrefab.name,
            spawn != null ? spawn.position : Vector3.zero,
            spawn != null ? spawn.rotation : Quaternion.identity,
            0);

        // Este RPC debe existir en el script del jugador
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

        // index seguro aunque haya más players que puntos
        int safeIndex = Mathf.Abs(currentSpawnIndex) % playerSpawnPositions.Count;
        return playerSpawnPositions[safeIndex];
    }

    private void UpdateSpawnIndexForAll()
    {
        currentSpawnIndex++;
        GetComponent<PhotonView>().RPC(nameof(RPC_UpdateSpawnIndex), RpcTarget.AllBuffered, currentSpawnIndex);
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
