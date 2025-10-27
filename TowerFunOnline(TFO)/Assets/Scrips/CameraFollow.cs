using UnityEngine;
using Photon.Pun;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset = new Vector3(0, 2, -10);
    [SerializeField] private float normalSize = 5f;   // tamaño normal
    [SerializeField] private float deadSize = 20f;    // tamaño al morir
    [SerializeField] private float zoomSpeed = 2f;    // velocidad del zoom

    private Transform target;
    private Camera cam;
    private bool isDead = false;

    void Start()
    {
        cam = GetComponent<Camera>();
        FindLocalPlayer();
    }

    void LateUpdate()
    {
        if (target == null)
        {
            FindLocalPlayer();
            return;
        }

        // Detectar si el jugador está muerto
        if (target.CompareTag("Dead"))
            isDead = true;
        else
            isDead = false;

        // Cámara sigue al jugador
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 5f);

        // Zoom in/out según estado
        float targetSize = isDead ? deadSize : normalSize;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * zoomSpeed);
    }

    void FindLocalPlayer()
    {
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