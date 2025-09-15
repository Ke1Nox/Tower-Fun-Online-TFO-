using UnityEngine;
using Photon.Pun;

public class PushAbility : MonoBehaviourPun
{
    [Header("Input / Tecla")]
    [SerializeField] private KeyCode pushKey = KeyCode.F;

    [Header("Detección")]
    [SerializeField] private float range = 1.2f;     // hasta dónde “alcanza” el empujón
    [SerializeField] private float radius = 0.6f;    // ancho del área frente al player
    [SerializeField] private LayerMask playerMask;   // capa donde están los players

    [Header("Fuerza")]
    [SerializeField] private float pushForce = 8f;   // intensidad del empuje
    [SerializeField] private float cooldown = 0.35f; // pequeño CD para no spamear

    private Vector2 lastInputDir = Vector2.right;    // ultima dirección de movimiento
    private float nextPushTime;

    void Update()
    {
        // Solo controlo mi propio personaje
        if (!photonView.IsMine) return;

        // 1) Capturo última dirección de input
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x, y);
        if (dir.sqrMagnitude > 0.01f)
            lastInputDir = dir.normalized;

        // 2) Intento empujar
        if (Time.time >= nextPushTime && Input.GetKeyDown(pushKey))
        {
            TryPush();
            nextPushTime = Time.time + cooldown;
        }
    }

    private void TryPush()
    {
        // Centro del “golpe” desplazado hacia la última dirección de input
        Vector2 origin = (Vector2)transform.position + lastInputDir * (range * 0.5f);

        // Busco posibles objetivos en un círculo frente a mí
        var hits = Physics2D.OverlapCircleAll(origin, radius, playerMask);

        foreach (var hit in hits)
        {
            if (!hit) continue;

            // Identifico el PhotonView del otro
            PhotonView otherPv = hit.GetComponentInParent<PhotonView>();
            if (otherPv == null) continue;

            // IMPORTANTÍSIMO: no me empujo a mí mismo
            if (otherPv.ViewID == photonView.ViewID) continue;

            // Dirección del empuje desde yo -> objetivo
            Vector2 toOther = ((Vector2)otherPv.transform.position - (Vector2)transform.position).normalized;

            // Enviamos el RPC al DUEÑO del objetivo para que él aplique la fuerza localmente
            Vector2 impulse = toOther * pushForce;
            otherPv.RPC(nameof(KnockbackReceiver.RPC_ApplyKnockback), otherPv.Owner, impulse);

            // Si querés empujar a uno solo, salí del loop:
            // break;
        }
    }

#if UNITY_EDITOR
    // Gizmos para ver el área de empuje
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;
        Gizmos.color = Color.white;
        Vector2 origin = (Vector2)transform.position + lastInputDir * (range * 0.5f);
        Gizmos.DrawWireSphere(origin, radius);
    }
#endif
}

