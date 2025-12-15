using LootLocker.Requests;
using UnityEngine;

public class LeaderboardService : MonoBehaviour
{
    public static void SubmitScore(int score, string leaderboardKey, System.Action<bool> onDone = null)
    {
        // Seguridad: no mandar score si no está conectado
        if (!LootLockerBootstrap.SessionStarted)
        {
            Debug.LogWarning("LootLocker no conectado todavía");
            onDone?.Invoke(false);
            return;
        }

        LootLockerSDKManager.SubmitScore("", score, leaderboardKey, response =>
        {
            if (!response.success)
            {
                Debug.LogError("Fallo al enviar score a LootLocker");
                onDone?.Invoke(false);
                return;
            }

            Debug.Log("Score enviado correctamente a LootLocker");
            onDone?.Invoke(true);
        });
    }
}

