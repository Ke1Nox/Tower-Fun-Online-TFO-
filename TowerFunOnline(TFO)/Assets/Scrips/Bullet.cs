using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private string loseSceneName = "LoseScene";

    private Vector2 direction;

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
            Debug.Log("Bullet: Jugador alcanzado por proyectil.");
            PhotonNetwork.Destroy(other.gameObject);
            UnityEngine.SceneManagement.SceneManager.LoadScene(loseSceneName);
        }

        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Destroy(gameObject);
    }
}