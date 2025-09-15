using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody2D), typeof(PhotonView))]
public class KnockbackReceiver : MonoBehaviourPun
{
    [SerializeField] private float maxExtraSpeed = 20f; // evita lanzarlo a la luna
    private Rigidbody2D rb;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    [PunRPC]
    public void RPC_ApplyKnockback(Vector2 impulse)
    {
        // Solo el dueño aplica la física (autoridad local)
        if (!photonView.IsMine) return;

        // podes “resetear” algo de velocidad para que se sienta el golpe
        if (rb.velocity.magnitude > maxExtraSpeed)
            rb.velocity = rb.velocity.normalized * maxExtraSpeed;

        rb.AddForce(impulse, ForceMode2D.Impulse);
    }
}

