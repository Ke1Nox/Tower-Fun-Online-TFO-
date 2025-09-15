using UnityEngine;
using Photon.Pun;

public class PushHitbox : MonoBehaviourPun
{
    [Tooltip("Fuerza del empujon (se pasa por RPC en unidades de impulso)")]
    public float pushForce = 20f;

    [Tooltip("Cuanto dura la hitbox en segundos")]
    public float lifeTime = 0.5f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
       
        if (!photonView.IsMine) return;

       
        PhotonView otherPV = other.GetComponent<PhotonView>();
        if (otherPV == null) return;

      
        if (otherPV.OwnerActorNr == photonView.OwnerActorNr) return;

      
        Vector2 direction = new Vector2(Mathf.Sign(transform.localScale.x), 0f);


        float vx = direction.x * pushForce;
        float vy = direction.y * pushForce;

       
        otherPV.RPC("RPC_ApplyPush", otherPV.Owner, vx, vy);
    }
}
