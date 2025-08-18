using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Pun.Demo.Cockpit;

public class MenuPun : MonoBehaviourPunCallbacks
{

    private const string nicknameKey = "playerNickname";
    private string nickname = "Prueba";
    public void ConectButton()
    {
        PlayerPrefs.SetString(nicknameKey, nickname);
        PhotonNetwork.NickName = nickname.ToUpper();
        PhotonNetwork.ConnectUsingSettings();


    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("conectando al master.....");
        base.OnConnectedToMaster();
        SceneManager.LoadScene(1);
    }
}
