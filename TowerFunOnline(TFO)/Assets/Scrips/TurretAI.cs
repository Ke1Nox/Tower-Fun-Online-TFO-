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
       
        if (!PhotonNetwork.IsMasterClient) return;

        GameObject target = FindClosestPlayer();
        if (target == null) return;

        Vector2 dir = (target.transform.position - transform.position);
        float dist = dir.magnitude;

        if (dist <= detectionRange && Time.time - lastShotTime >= fireRate)
        {
            Shoot(dir);
            lastShotTime = Time.time;
        }
    }

    GameObject FindClosestPlayer()
    {
        SimplePlayer[] players = FindObjectsOfType<SimplePlayer>();

        GameObject closest = null;
        float minDist = Mathf.Infinity;

        foreach (var p in players)
        {
            if (p.CompareTag("Dead")) continue; // ignorar muertos
            if (p.GetComponent<PhotonView>() == null) continue; // seguridad
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

    void Shoot(Vector2 direction)
    {
        GameObject bullet = PhotonNetwork.Instantiate(bulletPrefabPath, transform.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Initialize(direction);
    }
}