using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody2D))]
public class KnockbackReceiver2D : MonoBehaviourPun
{
    private Rigidbody2D rb;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    [PunRPC]
    public void ApplyKnockback2D(float x, float y, PhotonMessageInfo info = default)
    {
        if (!photonView.IsMine) return;

        // Seteamos directamente la velocidad para que se vea el empuje
        rb.velocity = new Vector2(x, y);
    }
}

