using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class MenuPun : MonoBehaviourPunCallbacks
{
    public string gameSceneName;
    public TMP_InputField inputField;
    public Button connectionButton;

    private const string nicknameKey = "playerNickname";
    private string nickname;



    private void Start()
    {
        connectionButton.onClick.AddListener(HandleConnectButton);
        inputField.onValueChanged.AddListener(VerifyName);
    }

    private void VerifyName(string newName)
    {
        if (inputField.text.Length == 0)
        {
            connectionButton.interactable = false;
        }

        if (inputField.text.Length >= 1 && !connectionButton.interactable)
        {
            connectionButton.interactable = true;
        }
        nickname = newName;
    }

    public void HandleConnectButton()
    {
        PlayerPrefs.SetString(nicknameKey, nickname);
        PhotonNetwork.NickName = nickname.ToUpper();
        print(nickname+ "intenta conectarse");
        PhotonNetwork.ConnectUsingSettings();
        connectionButton.interactable = false;

    }

   
    public override void OnConnectedToMaster()
    {
        Debug.Log("conectando al master.....");
        
        SceneManager.LoadScene(gameSceneName);
    }
}
