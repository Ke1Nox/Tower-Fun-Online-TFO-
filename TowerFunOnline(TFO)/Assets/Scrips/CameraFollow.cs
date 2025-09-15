using UnityEngine;
using Photon.Pun;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset = new Vector3(0, 2, -10);
    private Transform target;

    void Start()
    {
        
        FindLocalPlayer();
    }

    void LateUpdate()
    {
        if (target == null)
        {
            FindLocalPlayer();
            return;
        }

        // Camara sigue aal target
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 5f);
    }

    void FindLocalPlayer()
    {
        // Buscar  jugadores en la escena for
        SimplePlayer[] players = FindObjectsOfType<SimplePlayer>();
        foreach (var p in players)
        {
            PhotonView view = p.GetComponent<PhotonView>();
            if (view != null && view.IsMine)
            {
                target = p.transform;
                break;
            }
        }
    }
}
