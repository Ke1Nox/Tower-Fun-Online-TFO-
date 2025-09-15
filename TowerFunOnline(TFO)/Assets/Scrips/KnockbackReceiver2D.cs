using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody2D))]
public class KnockbackReceiver2D : MonoBehaviourPun
{
    [SerializeField] private float maxHorizontalSpeed = 14f;
    [SerializeField] private float maxVerticalSpeed = 20f;
    private Rigidbody2D rb;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    [PunRPC]
    public void ApplyKnockback2D(float x, float y, PhotonMessageInfo info = default)
    {
        // Solo el dueño mueve su propio personaje
        if (!photonView.IsMine) return;

        Vector2 impulse = new Vector2(x, y);

        // Impulso instantáneo independiente de masa
        rb.AddForce(impulse, ForceMode2D.Impulse);

        // Clamp opcional de velocidades
        Vector2 v = rb.velocity;
        v.x = Mathf.Clamp(v.x, -maxHorizontalSpeed, maxHorizontalSpeed);
        v.y = Mathf.Clamp(v.y, -maxVerticalSpeed, maxVerticalSpeed);
        rb.velocity = v;
    }
}
