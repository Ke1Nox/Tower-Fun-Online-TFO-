using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;

public class RoomPropertiesListener : MonoBehaviourPunCallbacks
{
    [Header("Escenas")]
    public string victorySceneName = "Win";   // asigná en Inspector
    public string loseSceneName = "Lose";  // asigná en Inspector

    private bool handled;

    void Start()
    {
        if (PhotonNetwork.InRoom)
            OnRoomPropertiesUpdate(PhotonNetwork.CurrentRoom.CustomProperties);
    }

    public override void OnRoomPropertiesUpdate(Hashtable changed)
    {
        if (handled) return;

        var snap = PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom.CustomProperties : null;

        bool ended = ReadBool(MatchProps.GAME_ENDED, changed) || ReadBool(MatchProps.GAME_ENDED, snap);
        if (!ended) return;

        bool allLose = ReadBool(MatchProps.ALL_LOSE, changed) || ReadBool(MatchProps.ALL_LOSE, snap);

        if (allLose)
        {
            handled = true;
            SceneManager.LoadScene(loseSceneName);
            return;
        }

        int winner = ReadInt(MatchProps.WINNER, changed);
        if (winner < 0) winner = ReadInt(MatchProps.WINNER, snap);
        if (winner < 0) return;

        handled = true;
        int me = PhotonNetwork.LocalPlayer.ActorNumber;
        SceneManager.LoadScene(me == winner ? victorySceneName : loseSceneName);
    }

    private static bool ReadBool(string key, Hashtable h)
    {
        if (h == null || !h.TryGetValue(key, out var v)) return false;
        if (v is bool b) return b;
        if (v is int i) return i != 0;
        return false;
    }
    private static int ReadInt(string key, Hashtable h)
    {
        if (h == null || !h.TryGetValue(key, out var v)) return -1;
        if (v is int i) return i;
        if (v is string s && int.TryParse(s, out var p)) return p;
        return -1;
    }
}
