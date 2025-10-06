using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class MenuPun : MonoBehaviourPunCallbacks
{
    public TMP_InputField inputField;
    public Button connectionButton;

    private const string nicknameKey = "playerNickname";
    private string nickname;

    private void Start()
    {
        connectionButton.onClick.AddListener(HandleConnectButton);
        inputField.onValueChanged.AddListener(VerifyName);
        connectionButton.interactable = false;
    }

    private void VerifyName(string newName)
    {
        connectionButton.interactable = newName.Length > 0;
        nickname = newName;
    }

    public void HandleConnectButton()
    {
        PlayerPrefs.SetString(nicknameKey, nickname);
        PhotonNetwork.NickName = nickname.ToUpper();

        Debug.Log(nickname + " intenta conectarse...");
        LoadingScreen.ShowConnecting();

        connectionButton.interactable = false;
    }
}