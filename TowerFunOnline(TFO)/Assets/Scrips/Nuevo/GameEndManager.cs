using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class GameEndManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private bool log = true;
    [SerializeField] private int minPlayersToWin = 2; // no declarar ganador si la ronda empezó con menos

    private bool ended;
    private bool roundActive;
    private int startCount;

    void Start()
    {
        ReadRoundProps(PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom.CustomProperties : null);
        TryResolveEnd("Start");
    }

    void Update()
    {
        if (!ended && PhotonNetwork.IsMasterClient) TryResolveEnd("Update");
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        ReadRoundProps(PhotonNetwork.CurrentRoom.CustomProperties);
        if (PhotonNetwork.IsMasterClient && !ended) TryResolveEnd("Props");
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (PhotonNetwork.IsMasterClient && !ended) TryResolveEnd("PlayerProps");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient && !ended) TryResolveEnd("PlayerLeft");
    }

    private void ReadRoundProps(Hashtable snap)
    {
        roundActive = ReadBool(MatchProps.ROUND_ACTIVE, snap);
        startCount = ReadInt(MatchProps.START_COUNT, snap);
    }

    private void TryResolveEnd(string from)
    {
        if (!PhotonNetwork.IsMasterClient || ended || !PhotonNetwork.InRoom) return;

        //  No evaluar si la ronda no está activa
        if (!roundActive) return;

        var players = PhotonNetwork.PlayerList;
        var alive = players.Where(PlayerPropsUtil.IsAlive).ToList();
        int ghosts = players.Count(p => PlayerPropsUtil.GetState(p) == PlayerState.Ghost);
        int elim = players.Count(p => PlayerPropsUtil.GetState(p) == PlayerState.Eliminated);

        if (alive.Count == 0)
        {
            EndAllLose();
        }
        else if (alive.Count == 1)
        {
            // Solo declarar ganador si la ronda empezó con 2+ o ya hubo bajas
            if (startCount >= minPlayersToWin || ghosts + elim > 0)
                EndWithWinner(alive[0]);
        }
        // >1 vivos: sigue
    }

    public void EndWithWinner(Player winner)
    {
        if (ended || !PhotonNetwork.InRoom || !PhotonNetwork.IsMasterClient) return;
        ended = true;

        var h = new Hashtable {
        { MatchProps.GAME_ENDED, true },
        { MatchProps.ALL_LOSE,   false },
        { MatchProps.WINNER,     winner.ActorNumber },
        { MatchProps.ROUND_ACTIVE, false } // opcional
    };

        // 1) Publicar fin
        PhotonNetwork.CurrentRoom.SetCustomProperties(h);

        // 2) Cerrar e invisibilizar la sala
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        if (log) Debug.Log($"[End] Winner={winner.ActorNumber}");

        // enviar score
        photonView.RPC(nameof(RPC_SubmitWinnerScore), RpcTarget.All);
    }

    public void EndAllLose()
    {
        if (ended || !PhotonNetwork.InRoom || !PhotonNetwork.IsMasterClient) return;
        ended = true;

        var h = new Hashtable {
        { MatchProps.GAME_ENDED, true },
        { MatchProps.ALL_LOSE,   true },
        { MatchProps.WINNER,     null },          // limpiar ganador si hubiera
        { MatchProps.ROUND_ACTIVE, false }        // opcional
    };

        PhotonNetwork.CurrentRoom.SetCustomProperties(h);

        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        if (log) Debug.Log("[End] ALL LOSE");
    }


    private static bool ReadBool(string k, Hashtable h)
    {
        if (h == null || !h.TryGetValue(k, out var v)) return false;
        if (v is bool b) return b;
        if (v is int i) return i != 0;
        return false;
    }
    private static int ReadInt(string k, Hashtable h)
    {
        if (h == null || !h.TryGetValue(k, out var v)) return 0;
        if (v is int i) return i;
        if (v is string s && int.TryParse(s, out var p)) return p;
        return 0;
    }


    [PunRPC]
    void RPC_SubmitWinnerScore()
    {
      
        var local = PhotonNetwork.LocalPlayer;
        if (local == null) return;

        if (PhotonNetwork.LocalPlayer.ActorNumber !=
            (int)PhotonNetwork.CurrentRoom.CustomProperties[MatchProps.WINNER])
            return;

        // +1 punto por victoria
        LeaderboardService.SubmitScore
            (
            1,
            "global_highscore"
        );
    }
}


