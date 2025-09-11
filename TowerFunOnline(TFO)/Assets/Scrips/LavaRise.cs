using UnityEngine;
using Photon.Pun;

public class LavaRise : MonoBehaviour
{
    [SerializeField] private float riseSpeed = 1f; // Velocidad constante hacia arriba


  

    void Update()
    {
        // Mover la lava hacia arriba constantemente
        transform.Translate(Vector3.up * riseSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verificar si colisiona con un jugador
        if (other.CompareTag("Player"))
        {
            PhotonView view = other.GetComponent<PhotonView>();

            // Solo el duelo del objeto puede destruirlo
            if (view != null && view.IsMine)
            {
                PhotonNetwork.Destroy(other.gameObject);

                UnityEngine.SceneManagement.SceneManager.LoadScene("DefeatScene");
            }
        }
    }
}