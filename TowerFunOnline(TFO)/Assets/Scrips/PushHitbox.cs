using UnityEngine;
using Photon.Pun;

public class PushHitbox : MonoBehaviourPun
{
    [Tooltip("Fuerza del empujón (se pasa por RPC en unidades de impulso)")]
    public float pushForce = 8f;

    [Tooltip("Cuánto dura la hitbox en segundos")]
    public float lifeTime = 0.15f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Solo el dueño de la hitbox envía el RPC (evita llamadas duplicadas desde todos los clientes)
        if (!photonView.IsMine) return;

        // Queremos afectar sólo a jugadores que tengan PhotonView
        PhotonView otherPV = other.GetComponent<PhotonView>();
        if (otherPV == null) return;

        // No empujar a nuestro propio jugador (misma owner actor)
        if (otherPV.OwnerActorNr == photonView.OwnerActorNr) return;

        // Dirección desde la hitbox hacia el jugador alcanzado
        Vector2 direction = (other.transform.position - transform.position).normalized;

        float vx = direction.x * pushForce;
        float vy = direction.y * pushForce;

        // Llamar RPC en el dueño del jugador alcanzado para que aplique la fuerza localmente
        otherPV.RPC("RPC_ApplyPush", otherPV.Owner, vx, vy);
    }
}
