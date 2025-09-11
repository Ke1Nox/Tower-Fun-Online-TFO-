using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryZone : MonoBehaviour
{
    [SerializeField] private string victoryScene = "VictoryScene";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Solo el dueño del jugador en Photon cambia de escena
            var view = other.GetComponent<Photon.Pun.PhotonView>();
            if (view != null && view.IsMine)
            {
                SceneManager.LoadScene(victoryScene);
            }
        }
    }
}