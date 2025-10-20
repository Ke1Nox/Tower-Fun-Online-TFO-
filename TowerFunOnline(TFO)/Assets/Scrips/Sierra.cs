using UnityEngine;
using Photon.Pun;

public class Sierra : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 3f;     // Velocidad de movimiento
    [SerializeField] private float moveDistance = 5f;  // Distancia total entre extremos

    private Vector3 startPos;
    private bool movingRight = true;

    [Header("Escena de derrota")]
    public string loseSceneName = "LoseScene";

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        MoveSaw();
    }

    private void MoveSaw()
    {
        // Mueve la sierra de un lado a otro
        if (movingRight)
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            if (transform.position.x >= startPos.x + moveDistance)
                movingRight = false;
        }
        else
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            if (transform.position.x <= startPos.x - moveDistance)
                movingRight = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView view = other.GetComponent<PhotonView>();
        if (view != null && view.IsMine)
        {
            Debug.Log("SawTrap: Jugador local tocó la sierra. Cargando LoseScene...");
            PhotonNetwork.Destroy(other.gameObject);
            UnityEngine.SceneManagement.SceneManager.LoadScene(loseSceneName);
        }
    }
}