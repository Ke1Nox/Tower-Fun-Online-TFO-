using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class SimplePlayer : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI nicknameUI;
    [SerializeField] private float moveSpeed;
    [SerializeField] private LayerMask collisionMask;

    private PhotonView photonView;
    public PhotonView PhotonView => photonView ??= GetComponent<PhotonView>();

    void Start()
    {
       photonView = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (photonView.IsMine)
        {
           float horizontal = Input.GetAxis("Horizontal"); 
            float vertical = Input.GetAxis("Vertical");

            Vector2 movement = new Vector2(horizontal, vertical) * moveSpeed * Time.deltaTime;

            transform.Translate(movement);
        }
       
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!PhotonView.IsMine)
            return;

        if ((collisionMask.value & (1 << collision.transform.gameObject.layer)) > 0)
        {
            PhotonView.RPC(
                "RPC_InformCollision",
                RpcTarget.AllBuffered,
                PhotonNetwork.LocalPlayer.NickName
            );
        }
    }

    [PunRPC]
    public void RPC_SetNickname(string nickname)
    {
        nicknameUI.text = nickname;
    }
}
