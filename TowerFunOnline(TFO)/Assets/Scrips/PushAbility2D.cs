using UnityEngine;
using Photon.Pun;

public class PushAbility2D : MonoBehaviourPun
{
    [Header("Input")]
    [SerializeField] private KeyCode pushKey = KeyCode.E;

    [Header("Zona de empuje (caja delante del player)")]
    [SerializeField] private Vector2 boxHalfExtents = new Vector2(1.2f, 0.8f);
    [SerializeField] private float boxDistance = 1.2f;              // delante del player
    [SerializeField] private LayerMask playerMask;                   // capa "Player"

    [Header("Fuerza de empuje")]
    [SerializeField] private float pushForce = 8f;                   // magnitud del impulso
    [SerializeField] private float verticalLift = 0f;                // leve empuje hacia arriba si querés
    [SerializeField] private float cooldown = 0.35f;

    // Referencia al SimplePlayer para leer su LastMoveDir
    private SimplePlayer simplePlayer;
    private float nextTime;

    void Awake()
    {
        simplePlayer = GetComponent<SimplePlayer>();
    }

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
        // Dirección de empuje: última dir de movimiento o mirar escala X si no hay input
        Vector2 dir = (simplePlayer && simplePlayer.LastMoveDir.sqrMagnitude > 0.0001f)
            ? simplePlayer.LastMoveDir.normalized
            : new Vector2(Mathf.Sign(transform.localScale.x == 0 ? 1 : transform.localScale.x), 0f);

        // Centro de la caja delante del player
        Vector2 center = (Vector2)transform.position + dir * boxDistance;

        // Overlap de otros jugadores
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, boxHalfExtents * 2f, 0f, playerMask);
        if (hits == null || hits.Length == 0) return;

        foreach (var col in hits)
        {
            PhotonView otherPv = col.attachedRigidbody ? col.attachedRigidbody.GetComponentInParent<PhotonView>()
                                                       : col.GetComponentInParent<PhotonView>();
            if (otherPv == null) continue;

            if (otherPv.ViewID == photonView.ViewID) continue; // evitar auto-empuje

            Vector2 knock = (dir + Vector2.up * verticalLift).normalized * pushForce;

            // Mandar RPC SOLO al dueño del otro jugador
            otherPv.RPC(nameof(KnockbackReceiver2D.ApplyKnockback2D), otherPv.Owner, knock.x, knock.y);
        }
    }

    // Gizmos para debug
    void OnDrawGizmosSelected()
    {
        if (!enabled) return;

        Vector2 dir = Vector2.right;
        if (simplePlayer && simplePlayer.photonView != null && simplePlayer.photonView.IsMine)
        {
            dir = (simplePlayer.LastMoveDir.sqrMagnitude > 0.0001f) ? simplePlayer.LastMoveDir.normalized : Vector2.right;
        }
        else
        {
            dir = new Vector2(Mathf.Sign(transform.localScale.x == 0 ? 1 : transform.localScale.x), 0f);
        }

        Vector2 center = (Vector2)transform.position + dir * boxDistance;
        Gizmos.color = new Color(0f, 1f, 1f, 0.25f);
        Gizmos.DrawCube(center, new Vector3(boxHalfExtents.x * 2f, boxHalfExtents.y * 2f, 0.1f));
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(center, new Vector3(boxHalfExtents.x * 2f, boxHalfExtents.y * 2f, 0.1f));
    }
}
