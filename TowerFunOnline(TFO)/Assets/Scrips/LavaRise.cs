using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class LavaRise : MonoBehaviour
{
    [SerializeField] private float riseSpeed = 1f;
    private bool isRising = false;

    
    public bool IsRising
    {
        get { return isRising; }
        private set { isRising = value; }
    }

    [Header("Escena al morir por ")]
    public string loseSceneName = "LoseScene";

    void Update()
    {
        if (!isRising) return;
        transform.Translate(Vector3.up * riseSpeed * Time.deltaTime);
    }

    public void StartRising()
    {
        IsRising = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView view = other.GetComponent<PhotonView>();
        if (view != null && view.IsMine)
        {
            
            PhotonNetwork.Destroy(other.gameObject);
            UnityEngine.SceneManagement.SceneManager.LoadScene(loseSceneName);
        }
        else
        {
            
        }
    }
}