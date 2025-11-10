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
        
        if (PhotonNetwork.IsMasterClient)
            Invoke(nameof(NetworkDestroyByLifetime), lifeTime);
    }

    void Update()
    {
        
        if (!photonView.IsMine) return;
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView targetView = other.GetComponent<PhotonView>();
        if (targetView == null) return;

        // rpc jugador impactado fantasma y se teletransporte
        targetView.RPC("RPC_BecomeGhost", targetView.Owner, respawnPosition.x, respawnPosition.y);

        
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Destroy(gameObject);
    }

    private void NetworkDestroyByLifetime()
    {
        if (PhotonNetwork.IsMasterClient && photonView != null && photonView.IsMine)
            PhotonNetwork.Destroy(gameObject);
    }
}