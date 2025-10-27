using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class LoadingScreen : MonoBehaviourPunCallbacks
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressText;


    public string level1;
    public string level2;
    public string level3;

    private static bool isConnecting = false;

    //  LoadingScreen.ShowConnecting();
    public static void ShowConnecting()
    {
        isConnecting = true;
        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Additive);
    }

    private void Start()
    {
        var systems = FindObjectsOfType<UnityEngine.EventSystems.EventSystem>();
        for (int i = 1; i < systems.Length; i++)
            Destroy(systems[i].gameObject);

        if (isConnecting)
        {
            if (progressText != null) progressText.text = "Conectando...";
            if (progressBar != null) progressBar.gameObject.SetActive(false);

            PhotonNetwork.AutomaticallySyncScene = true;

            //  Evita reconectar si ya está conectado
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                OnConnectedToMaster(); 
            }
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("LoadingScreen: Conectado a Photon Master -> intentando JoinRandomRoom()");
        //  unirnse a una sala existente
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("LoadingScreen: No se pudo unirse a una sala (creando una). Razón: " + message);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4; 
        PhotonNetwork.CreateRoom(null, roomOptions); // nombre null = Photon crea uno aleatorio
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("LoadingScreen: Se unió a la sala: " + PhotonNetwork.CurrentRoom.Name);

        if (PhotonNetwork.IsMasterClient)
        {
            // El Master decide el nivel aleatorio entre los 3
            int randomIndex = Random.Range(0, 3);
            string selectedScene = (randomIndex == 0) ? level1 : (randomIndex == 1) ? level2 : level3;

            Debug.Log("MasterClient cargará la escena: " + selectedScene);
            if (progressText != null) progressText.text = "Cargando nivel...";
            if (progressBar != null) { progressBar.gameObject.SetActive(true); progressBar.value = 0f; }

            // Carga sincronizada para TODOS los jugadores en la sala
            PhotonNetwork.LoadLevel(selectedScene);
        }
        else
        {
            // Clientes esperan que el host cargue la escena
            if (progressText != null) progressText.text = "Esperando al host...";
            if (progressBar != null) progressBar.gameObject.SetActive(true);
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning("LoadingScreen: Desconectado de Photon: " + cause);
        // Opcional: volver a menú, mostrar error, o descargar la loading scene
        SceneManager.UnloadSceneAsync("LoadingScene");
    }
}