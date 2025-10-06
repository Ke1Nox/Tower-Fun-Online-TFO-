using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class GameStarter : MonoBehaviourPunCallbacks
{
    [Header("Prefabs & Spawns")]
    [SerializeField] private string playerResourcePath = "Prefabs/Player"; //  usa ruta en Resources
    [SerializeField] private Transform playerSpawn;
    [SerializeField] private System.Collections.Generic.List<Transform> playerSpawnPositions = new System.Collections.Generic.List<Transform>();

    private bool hasSpawned = false;
    private int currentSpawnIndex = 0;

    private void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            Debug.Log($"[GameStarter] InRoom={PhotonNetwork.InRoom} ActorNumber={PhotonNetwork.LocalPlayer?.ActorNumber}");
            StartCoroutine(WaitAndSpawn());
        }
        else
        {
            Debug.LogWarning("[GameStarter] Start sin estar en sala. Esperando OnJoinedRoom...");
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"[GameStarter] OnJoinedRoom -> ActorNumber={PhotonNetwork.LocalPlayer?.ActorNumber}, PlayerCount={PhotonNetwork.CurrentRoom.PlayerCount}");
        if (!hasSpawned) StartCoroutine(WaitAndSpawn());
    }

    private IEnumerator WaitAndSpawn()
    {
        // Si dependés de algún índice de spawn del Master
        if (!PhotonNetwork.IsMasterClient)
            yield return new WaitUntil(() => currentSpawnIndex >= 0);

        CreateAndSetUpPlayerInstance();
    }

    private void CreateAndSetUpPlayerInstance()
    {
        if (hasSpawned) return;

        Transform spawn = GetPlayerSpawnPosition() ?? playerSpawn;
        Vector3 spawnPos = (spawn != null ? spawn.position : Vector3.zero);

        // Opcional: separarlos un poco según el ActorNumber para evitar overlap
        int offset = Mathf.Max(0, PhotonNetwork.LocalPlayer.ActorNumber - 1);
        spawnPos += new Vector3(offset * 2.0f, 0f, 0f);

        // IMPORTANTE: usamos ruta en Resources (no el .name de la referencia)
        GameObject player = PhotonNetwork.Instantiate(playerResourcePath, spawnPos, spawn != null ? spawn.rotation : Quaternion.identity, 0);

        hasSpawned = true;

        // Setear nickname (RPC ya existe en tu SimplePlayer)
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
