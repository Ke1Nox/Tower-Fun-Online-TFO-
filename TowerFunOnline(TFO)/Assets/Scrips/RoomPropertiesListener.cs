using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class RoomPropertiesListener : MonoBehaviourPunCallbacks
{
    [Header("Nombres de escenas")]
    public string victorySceneName = "VictoryScene";
    public string loseSceneName = "LoseScene";

    private const string GAME_ENDED_KEY = "GameEnded";
    private const string WINNER_KEY = "Winner";

    void Start()
    {
        // Si las propiedades ya están establecidas antes de que este script arranque,
        // compruébalas ahora
        if (PhotonNetwork.InRoom)
        {
            object endedObj;
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(GAME_ENDED_KEY, out endedObj) && (bool)endedObj == true)
            {
                object winnerObj;
                PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(WINNER_KEY, out winnerObj);
                HandleGameEnded((int)winnerObj);
            }
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged == null) return;

        if (propertiesThatChanged.ContainsKey(GAME_ENDED_KEY))
        {
            object winnerObj;
            PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(WINNER_KEY, out winnerObj);
            if (winnerObj != null)
            {
                HandleGameEnded((int)winnerObj);
            }
            else
            {
                Debug.LogWarning("RoomPropertiesListener: GameEnded true pero no hay Winner en propiedades.");
            }
        }
    }

    private void HandleGameEnded(int winnerActorNumber)
    {
        int localActor = PhotonNetwork.LocalPlayer.ActorNumber;
        bool iAmWinner = (localActor == winnerActorNumber);

        Debug.Log($"RoomPropertiesListener: Game ended. Winner actor: {winnerActorNumber}. Local actor: {localActor}. IAmWinner: {iAmWinner}");

        // Cargar la escena correspondiente para el jugador local
        if (iAmWinner)
        {
            SceneManager.LoadScene(victorySceneName);
        }
        else
        {
            SceneManager.LoadScene(loseSceneName);
        }
    }
}
