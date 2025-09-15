using UnityEngine;
using Photon.Pun;

public class PushAbility2D : MonoBehaviourPun
{
    [Header("Input")]
    [SerializeField] private KeyCode pushKey = KeyCode.E;

    [Header("Zona de empuje (caja delante del player)")]
    [SerializeField] private Vector2 boxHalfExtents = new Vector2(1.2f, 0.8f);
    [SerializeField] private float boxDistance = 1.2f;
    [SerializeField] private LayerMask playerMask; // poné Player o Everything para probar

    [Header("Fuerza de empuje")]
    [SerializeField] private float pushForce = 10f;
    [SerializeField] private float verticalLift = 0f;
    [SerializeField] private float cooldown = 0.35f;

    Rigidbody2D rb;
    float nextTime;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    void Update()
    {
        if (!photonView.IsMine) return;
        if (Time.time < nextTime) return;

        if (Input.GetKeyDown(pushKey))
        {
            DoPush();
            nextTime = Time.time + cooldown;
        }
    }

    void DoPush()
    {
        // Dirección: si me muevo uso el signo de la velocidad; si no, el de la escala (mirada)
        Vector2 dir;
        if (Mathf.Abs(rb.velocity.x) > 0.01f)
            dir = new Vector2(Mathf.Sign(rb.velocity.x), 0f);
        else
            dir = new Vector2(Mathf.Sign(transform.localScale.x == 0 ? 1 : transform.localScale.x), 0f);

        Vector2 center = (Vector2)transform.position + dir * boxDistance;

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, boxHalfExtents * 2f, 0f, playerMask);
        if (hits == null || hits.Length == 0) return;

        foreach (var col in hits)
        {
            PhotonView otherPv = col.attachedRigidbody ? col.attachedRigidbody.GetComponentInParent<PhotonView>()
                                                       : col.GetComponentInParent<PhotonView>();
            if (otherPv == null) continue;
            if (otherPv.ViewID == photonView.ViewID) continue; // no me empujo a mí mismo

            // knock horizontal simple (si querés Y, subí verticalLift)
            Vector2 knock = new Vector2(dir.x * pushForce, verticalLift);

            otherPv.RPC(nameof(KnockbackReceiver2D.ApplyKnockback2D), otherPv.Owner, knock.x, knock.y);
        }
    }

    // opcional: gizmo de la caja
    void OnDrawGizmosSelected()
    {
        if (!enabled) return;
        Vector2 dir = Vector2.right;
        if (rb) dir = Mathf.Abs(rb.velocity.x) > 0.01f ? new Vector2(Mathf.Sign(rb.velocity.x), 0f)
                                                       : new Vector2(Mathf.Sign(transform.localScale.x == 0 ? 1 : transform.localScale.x), 0f);
        Vector2 center = (Vector2)transform.position + dir * boxDistance;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(center, boxHalfExtents * 2f);
    }
}
