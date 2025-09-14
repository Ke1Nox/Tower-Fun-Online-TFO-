using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon; // Hashtable
using Photon.Realtime;

public class GoalPlatform : MonoBehaviour
{
    // Nombre de la propiedad de sala que usaremos
    private const string GAME_ENDED_KEY = "GameEnded";
    private const string WINNER_KEY = "Winner";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv == null) return;

        // Evitar que alguien intente terminar el juego si ya terminó
        Room currentRoom = PhotonNetwork.CurrentRoom;
        if (currentRoom == null) return;

        object existing;
        if (currentRoom.CustomProperties.TryGetValue(GAME_ENDED_KEY, out existing) && (bool)existing == true)
        {
            // Ya terminado
            return;
        }

        // Solo el primer que llegue configurará la propiedad de sala
        Hashtable props = new Hashtable
        {
            { GAME_ENDED_KEY, true },
            { WINNER_KEY, pv.Owner.ActorNumber }
        };

        // SetCustomProperties fusiona con propiedades existentes; se propaga a todos
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        Debug.Log($"GoalPlatform: Jugador {pv.Owner.NickName} (actor {pv.Owner.ActorNumber}) alcanzó la meta.");
    }
}

