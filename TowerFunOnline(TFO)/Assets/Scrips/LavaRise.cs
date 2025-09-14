using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class LavaRise : MonoBehaviour
{
    [SerializeField] private float riseSpeed = 1f;
    private bool isRising = false;

    [Header("Escena al morir por lava (nombre en Build Settings)")]
    public string loseSceneName = "LoseScene";

    void Update()
    {
        if (!isRising) return;
        transform.Translate(Vector3.up * riseSpeed * Time.deltaTime);
    }

    public void StartRising()
    {
        isRising = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView view = other.GetComponent<PhotonView>();
        if (view != null && view.IsMine)
        {
            // Jugador local toca la lava -> escena de derrota
            Debug.Log("LavaRise: Jugador local tocó la lava. Cargando LoseScene...");
            // Opcional: destruir su GameObject para limpiar la escena antes de cambiar
            PhotonNetwork.Destroy(other.gameObject);
            SceneManager.LoadScene(loseSceneName);
        }
        else
        {
            // Si quieres, puedes destruir jugadores remotos también (opcional)
            // PhotonNetwork.Destroy(other.gameObject);
        }
    }
}
