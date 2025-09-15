using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon; 
using Photon.Realtime;

public class GoalPlatform : MonoBehaviour
{
 
    private const string GAME_ENDED_KEY = "GameEnded";
    private const string WINNER_KEY = "Winner";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv == null) return;

       
        Room currentRoom = PhotonNetwork.CurrentRoom;
        if (currentRoom == null) return;

        object existing;
        if (currentRoom.CustomProperties.TryGetValue(GAME_ENDED_KEY, out existing) && (bool)existing == true)
        {
            //fin
            return;
        }

        
        Hashtable props = new Hashtable
        {
            { GAME_ENDED_KEY, true },
            { WINNER_KEY, pv.Owner.ActorNumber }
        };

       
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        Debug.Log($"GoalPlatform: Jugador {pv.Owner.NickName} (actor {pv.Owner.ActorNumber}) alcanzo la meta.");
    }
}

