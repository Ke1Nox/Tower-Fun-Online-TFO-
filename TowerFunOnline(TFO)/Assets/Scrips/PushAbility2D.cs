using UnityEngine;
using Photon.Pun;

public class PushAbility2D : MonoBehaviourPun
{
    [Header("Input")]
    [SerializeField] private KeyCode pushKey = KeyCode.E;

    [Header("Zona de empuje (caja delante del player)")]
    [SerializeField] private Vector2 boxHalfExtents = new Vector2(1.2f, 0.8f);
    [SerializeField] private float boxDistance = 1.2f;
    [SerializeField] private LayerMask playerMask;

    [Header("Fuerza de empuje")]
    [SerializeField] private float pushForce = 8f;
    [SerializeField] private float verticalLift = 0f;
    [SerializeField] private float cooldown = 0.35f;

    private SimplePlayer simplePlayer;
    private float nextTime;

    void Awake() { simplePlayer = GetComponent<SimplePlayer>(); }

    void Update()
    {
        if (!photonView.IsMine) return;
        if (Time.time < nextTime) return;

        if (Input.GetKeyDown(pushKey))
        {
            Debug.Log("[Push] Key pressed");
            DoPush();
            nextTime = Time.time + cooldown;
        }
    }

    void DoPush()
    {
        Vector2 dir = (simplePlayer && simplePlayer.LastMoveDir.sqrMagnitude > 0.0001f)
                        ? simplePlayer.LastMoveDir.normalized
                        : Vector2.right;

        Vector2 center = (Vector2)transform.position + dir * boxDistance;

        // LOG: dibujar una rayita para ver dirección
        Debug.DrawLine(transform.position, center, Color.cyan, 0.5f);

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, boxHalfExtents * 2f, 0f, playerMask);
        Debug.Log($"[Push] Overlap center={center} size={boxHalfExtents * 2f} mask={(int)playerMask} hits={hits.Length}");

        foreach (var col in hits)
        {
            PhotonView otherPv = col.attachedRigidbody ? col.attachedRigidbody.GetComponentInParent<PhotonView>()
                                                       : col.GetComponentInParent<PhotonView>();
            if (otherPv == null) { Debug.Log("[Push] hit sin PhotonView"); continue; }

            if (otherPv.ViewID == photonView.ViewID) { Debug.Log("[Push] me filtré a mí mismo"); continue; }

            Vector2 knock = (dir + Vector2.up * verticalLift).normalized * pushForce;

            Debug.Log($"[Push] Enviando RPC a {otherPv.Owner?.NickName ?? "owner?"} viewID={otherPv.ViewID} knock={knock}");
            otherPv.RPC(nameof(KnockbackReceiver2D.ApplyKnockback2D), otherPv.Owner, knock.x, knock.y);
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector2 dir = Vector2.right;
        if (simplePlayer) dir = (simplePlayer.LastMoveDir.sqrMagnitude > 0.0001f) ? simplePlayer.LastMoveDir.normalized : Vector2.right;
        Vector2 center = (Vector2)transform.position + dir * boxDistance;

        Gizmos.color = new Color(0f, 1f, 1f, 0.25f);
        Gizmos.DrawCube(center, new Vector3(boxHalfExtents.x * 2f, boxHalfExtents.y * 2f, 0.1f));
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(center, new Vector3(boxHalfExtents.x * 2f, boxHalfExtents.y * 2f, 0.1f));
    }
}
