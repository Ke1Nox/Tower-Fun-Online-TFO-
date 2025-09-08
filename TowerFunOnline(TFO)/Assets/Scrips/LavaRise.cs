using UnityEngine;
using Photon.Pun;

public class LavaRise : MonoBehaviour
{
    [SerializeField] private float riseSpeed = 1f;
    private bool isRising = false;

    void Update()
    {
        if (!isRising) return;
        transform.Translate(Vector3.up * riseSpeed * Time.deltaTime);
    }

    public void StartRising()
    {
        isRising = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView view = other.GetComponent<PhotonView>();
            if (view != null && view.IsMine)
            {
                PhotonNetwork.Destroy(other.gameObject);
            }
        }
    }
}