using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class ReturnToMenu : MonoBehaviourPunCallbacks
{
    public string menuScene = "Menu"; // asigná en Inspector si cambia

    public void GoToMenu()
    {
        if (PhotonNetwork.InRoom) { PhotonNetwork.LeaveRoom(); return; }
        SceneManager.LoadScene(menuScene);
    }

    public override void OnLeftRoom() { SceneManager.LoadScene(menuScene); }
}
