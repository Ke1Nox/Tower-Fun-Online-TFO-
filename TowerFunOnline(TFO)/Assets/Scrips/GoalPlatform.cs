using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GoalPlatform : MonoBehaviourPun
{
 
    public GameEndManager endMgr;

    private void Awake()
    {
        if (!endMgr) endMgr = FindObjectOfType<GameEndManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        var pv = other.GetComponent<PhotonView>();
        if (!pv) return;

        if (PhotonNetwork.IsMasterClient)
            endMgr?.EndWithWinner(pv.Owner);
    }
}



