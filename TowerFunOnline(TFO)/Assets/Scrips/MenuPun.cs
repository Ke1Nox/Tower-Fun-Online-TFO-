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

    
    public string level1;
    public string level2;
    public string level3;

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
        // Elegir aleatoriamente uno de los 3 niveles
        int randomIndex = Random.Range(0, 3);
        if (randomIndex == 0) gameSceneName = level1;
        else if (randomIndex == 1) gameSceneName = level2;
        else gameSceneName = level3;

        PlayerPrefs.SetString(nicknameKey, nickname);
        PhotonNetwork.NickName = nickname.ToUpper();
        Debug.Log(nickname + " intenta conectarse a " + gameSceneName + "...");

        LoadingScreen.ShowConnecting(gameSceneName);

        connectionButton.interactable = false;
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.SendRate = 50;
        PhotonNetwork.SerializationRate = 40;
        Debug.Log(nickname + " conectado al master");

        LoadingScreen.LoadScene(gameSceneName);
    }
}