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

        var pv = other.GetComponent<PhotonView>();
        if (pv == null) return;

        // Marca Eliminated (no ghost)
        PlayerPropsUtil.SetState(pv.Owner, PlayerState.Eliminated);

        if (pv.IsMine)
            PhotonNetwork.Destroy(other.gameObject);
        // No cargues Lose aquí. El fin lo resuelve el Master.
    }
}