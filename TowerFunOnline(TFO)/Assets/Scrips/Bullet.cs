using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifeTime = 5f;

    private Vector2 direction;

    // Posición de respawn
    private Vector3 respawnPosition = new Vector3(17.9799995f, -2.82999992f, 0f);

    public void Initialize(Vector2 dir)
    {
        direction = dir.normalized;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView view = other.GetComponent<PhotonView>();
        if (view != null && view.IsMine)
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Desactivar gravedad
                rb.gravityScale = 0f;

                // Detener movimiento
                rb.velocity = Vector2.zero;

                // Teletransportar jugador
                other.transform.position = respawnPosition;
                // Cambiar su tag a "Dead" para que la torreta lo ignore
                other.tag = "Dead";


                SimplePlayer.isdead = true;
            }
        }

        // Destruir la bala solo si es el master
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Destroy(gameObject);
    }
}