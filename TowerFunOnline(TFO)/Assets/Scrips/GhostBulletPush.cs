using UnityEngine;
using Photon.Pun;

public class GhostBulletPush : MonoBehaviourPun
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 2.0f;
    [SerializeField] private float pushForce = 8f;

    private Vector2 direction = Vector2.left;

    public void Initialize(Vector2 dir)
    {
        direction = dir.sqrMagnitude > 0.001f ? dir.normalized : Vector2.left;

        
        if (photonView.IsMine)
            Invoke(nameof(DestruirBala), lifeTime);
    }

    void Update()
    {
        // El owner mueve la bala; los demás la ven por PhotonTransformViewClassic
        if (!photonView.IsMine) return;
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Empujar solo a jugadores vivos
        if (!other.CompareTag("Player")) return;

        PhotonView targetPV = other.GetComponent<PhotonView>();
        if (targetPV == null) return;

        // Empuje simple horizontal hacia la izquierda
        float vx = -Mathf.Abs(pushForce);
        float vy = 0f;

        
        targetPV.RPC("RPC_ApplyPush", targetPV.Owner, vx, vy);

        // Destruir la bala en red si sos el owner (NO depende del Master)
        if (photonView.IsMine)
            PhotonNetwork.Destroy(gameObject);
    }

    private void DestruirBala()
    {
        if (photonView != null && photonView.IsMine)
            PhotonNetwork.Destroy(gameObject);
    }
}