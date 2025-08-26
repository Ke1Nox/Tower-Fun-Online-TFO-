using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(PhotonView))]
public class GameStarter : MonoBehaviourPunCallbacks
{
    [Header("Prefabs & Spawns")]
    [SerializeField] private GameObject playerPrefab;       // Debe estar en una carpeta Resources y el nombre exacto
    [SerializeField] private Transform playerSpawn;          // Fallback si no hay lista
    [SerializeField] private List<Transform> playerSpawnPositions = new List<Transform>();

    private int currentSpawnIndex = -1;                      // <- arranca inválido para que los clientes esperen

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
        if (PhotonNetwork.IsMasterClient)
        {
            if (currentSpawnIndex < 0) currentSpawnIndex = 0;   // master arranca en 0
        }
        else
        {
            // Esperar a que el master lo sincronice por RPC
            yield return new WaitUntil(() => currentSpawnIndex >= 0);
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
            0
        );

        // Seteo de nick en el player
        var pv = player.GetComponent<PhotonView>();
        if (pv != null)
        {
            pv.RPC("RPC_SetNickname", RpcTarget.AllBuffered,
                PlayerPrefs.GetString("playerNickname", "Player"));
        }
        else
        {
            Debug.LogWarning("El prefab del Player no tiene PhotonView en la raíz.");
        }
    }

    private Transform GetPlayerSpawnPosition()
    {
        if (playerSpawnPositions == null || playerSpawnPositions.Count == 0)
            return playerSpawn;

        int safeIndex = Mathf.Abs(currentSpawnIndex) % playerSpawnPositions.Count;
        return playerSpawnPositions[safeIndex];
    }

    private void UpdateSpawnIndexForAll()
    {
        currentSpawnIndex++;
        // Usa el photonView heredado (requiere que este objeto tenga PhotonView)
        photonView.RPC(nameof(RPC_UpdateSpawnIndex), RpcTarget.AllBuffered, currentSpawnIndex);
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
