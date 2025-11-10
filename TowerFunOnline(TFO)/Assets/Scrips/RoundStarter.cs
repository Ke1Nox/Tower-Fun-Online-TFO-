using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class RoundStarter : MonoBehaviourPunCallbacks
{
    [Tooltip("Esperar este tiempo tras detectar 2 jugadores antes de activar la ronda.")]
    public float startDelay = 0.5f;

    void Start()
    {
        if (!PhotonNetwork.InRoom) return;

        // Limpieza inicial (además de la que ya hacés en GameStarter)
        if (PhotonNetwork.IsMasterClient)
        {
            var reset = new Hashtable {
                { MatchProps.ROUND_ACTIVE, false },
                { MatchProps.START_COUNT,  0 }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(reset);
        }

        TryStartIfReady();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) => TryStartIfReady();

    private async void TryStartIfReady()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // Activar solo con 2+ jugadores
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            await System.Threading.Tasks.Task.Delay(System.TimeSpan.FromSeconds(startDelay));

            var h = new Hashtable {
                { MatchProps.ROUND_ACTIVE, true },
                { MatchProps.START_COUNT,  PhotonNetwork.CurrentRoom.PlayerCount }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(h);
            Debug.Log($"[RoundStarter] ROUND_ACTIVE=true, START_COUNT={PhotonNetwork.CurrentRoom.PlayerCount}");
        }
    }
}
