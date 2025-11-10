// Assets/Scrips/MatchProps.cs
using Photon.Realtime;
using ExitGames.Client.Photon;

public enum PlayerState { Alive = 0, Ghost = 1, Eliminated = 2 }

public static class MatchProps
{
    // ROOM
    public const string GAME_ENDED = "GameEnded";
    public const string WINNER = "Winner";
    public const string ALL_LOSE = "AllLose";

    // PLAYER
    public const string PLAYER_STATE = "PS";

    // ROUND
    public const string ROUND_ACTIVE = "RoundActive"; // bool
    public const string START_COUNT = "StartCount";  // int
}

public static class PlayerPropsUtil
{
    public static void SetState(Player p, PlayerState st)
    {
        var h = new Hashtable { { MatchProps.PLAYER_STATE, (int)st } };
        p.SetCustomProperties(h);
    }

    public static PlayerState GetState(Player p)
    {
        if (p?.CustomProperties != null &&
            p.CustomProperties.TryGetValue(MatchProps.PLAYER_STATE, out var v) &&
            v is int i) return (PlayerState)i;

        return PlayerState.Alive;
    }

    public static bool IsAlive(Player p) => GetState(p) == PlayerState.Alive;
}


