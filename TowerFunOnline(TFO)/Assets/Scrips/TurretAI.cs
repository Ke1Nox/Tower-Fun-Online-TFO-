using UnityEngine;
using Photon.Pun;
using System.Linq;

public class TurretAI : MonoBehaviourPun
{
    [Header("Ataque")]
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private string bulletPrefabPath = "Prefabs/bullet";

    private float lastShotTime = -999f;

    void Update()
    {
        // Solo el Master controla las torretas (sincroniza disparos)
        if (!PhotonNetwork.IsMasterClient) return;

        GameObject target = FindClosestAlivePlayer();
        if (target == null) return;

        Vector2 dir = (target.transform.position - transform.position);
        float dist = dir.magnitude;

        if (dist <= detectionRange && Time.time - lastShotTime >= fireRate)
        {
            Shoot(dir);
            lastShotTime = Time.time;
        }
    }

    /// <summary>
    /// Busca el jugador vivo más cercano (ignora fantasmas y eliminados)
    /// </summary>
    GameObject FindClosestAlivePlayer()
    {
        SimplePlayer[] players = FindObjectsOfType<SimplePlayer>();
        GameObject closest = null;
        float minDist = Mathf.Infinity;

        foreach (var p in players)
        {
            if (p == null) continue;
            var view = p.GetComponent<PhotonView>();
            if (view == null || view.Owner == null) continue;

            // Ignorar jugadores fantasmas o eliminados
            var state = PlayerPropsUtil.GetState(view.Owner);
            if (state != PlayerState.Alive) continue;

            // Ignorar si está desactivado
            if (!p.gameObject.activeInHierarchy) continue;

            float d = Vector2.Distance(transform.position, p.transform.position);
            if (d < minDist)
            {
                minDist = d;
                closest = p.gameObject;
            }
        }

        return closest;
    }

    /// <summary>
    /// Instancia una bala y le pasa la dirección del disparo
    /// </summary>
    void Shoot(Vector2 direction)
    {
        GameObject bullet = PhotonNetwork.Instantiate(bulletPrefabPath, transform.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Initialize(direction);
    }
}
