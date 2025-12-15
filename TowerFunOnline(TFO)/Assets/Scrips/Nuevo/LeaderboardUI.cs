using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Text;


public class LeaderboardUI : MonoBehaviour
{
    [Header("Leaderboard")]
    [SerializeField] private string leaderboardKey = "global_highscore";
    [SerializeField] private int count = 10;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI tableText;

    private void Start()
    {
        StartCoroutine(Empezar());
      
    }

    IEnumerator Empezar()
    {
        yield return new WaitForSeconds(3);
        Refresh();
    }
    public void Refresh()
    {
        if (!LootLockerBootstrap.SessionStarted)
        {
            tableText.text = "Conectando a LootLocker...";
            return;
        }

        LootLockerSDKManager.GetScoreList(leaderboardKey, count, 0, response =>
        {
            if (!response.success)
            {
                tableText.text = "Error al cargar leaderboard";
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("RANK   NOMBRE           SCORE");
            sb.AppendLine("-------------------------------");

            if (response.items == null || response.items.Length == 0)
            {
                sb.AppendLine("Sin registros aún");
            }
            else
            {
                foreach (var item in response.items)
                {
                    string name = string.IsNullOrEmpty(item.player.name)
                        ? "Player " + item.player.id
                        : item.player.name;

                    sb.AppendLine($"{item.rank,4}   {name,-15} {item.score,5}");
                }
            }

            tableText.text = sb.ToString();
        });
    }
}
