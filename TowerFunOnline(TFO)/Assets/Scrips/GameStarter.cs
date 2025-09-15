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
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom()
    {
        
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
        //UpdateSpawnIndexForAll();
    }

    // Spawner:
    private void CreateAndSetUpPlayerInstance()
    {
        Transform spawn = GetPlayerSpawnPosition();
        if (spawn == null) spawn = playerSpawn;

        // Distancias entre spawns 
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

    //private void UpdateSpawnIndexForAll()
    //{
    //    currentSpawnIndex++;
    //    GetComponent<PhotonView>().RPC(nameof(RPC_UpdateSpawnIndex), RpcTarget.AllBuffered, currentSpawnIndex);
    //}

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
