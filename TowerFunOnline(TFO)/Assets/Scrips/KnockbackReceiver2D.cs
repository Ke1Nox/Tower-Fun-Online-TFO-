using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody2D))]
public class KnockbackReceiver2D : MonoBehaviourPun
{
    Rigidbody2D rb;
    void Awake() => rb = GetComponent<Rigidbody2D>();

    [PunRPC]
    public void ApplyKnockback2D(float x, float y)
    {
        if (!photonView.IsMine) return;
        rb.velocity = new Vector2(x, y); // empuje simple
    }
}


