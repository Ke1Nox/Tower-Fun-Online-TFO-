using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LoadingScreen : MonoBehaviourPunCallbacks
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressText;

    [Header("Escenas jugables")]
    public string level1;
    public string level2;
    public string level3;

    private static bool isConnecting = false;
    public static void ShowConnecting()
    {
        isConnecting = true;
        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Additive);
    }

    private void Start()
    {
        var systems = FindObjectsOfType<UnityEngine.EventSystems.EventSystem>();
        for (int i = 1; i < systems.Length; i++) Destroy(systems[i].gameObject);

        if (!isConnecting) return;

        if (progressText) progressText.text = "Conectando...";
        if (progressBar) progressBar.gameObject.SetActive(false);

        PhotonNetwork.AutomaticallySyncScene = true;

        if (!PhotonNetwork.IsConnected) PhotonNetwork.ConnectUsingSettings();
        else OnConnectedToMaster(); // ya conectado: seguir flujo
    }

    // ------- FLOW: conectar -> join con filtro -> si falla, crear room -> cargar nivel (solo master)
    public override void OnConnectedToMaster()
    {
        if (progressText) progressText.text = "Buscando sala...";
        // ÚNETE SOLO a salas no terminadas
        var expected = new Hashtable { { MatchProps.GAME_ENDED, false } };
        PhotonNetwork.JoinRandomRoom(expected, (byte)4);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        if (progressText) progressText.text = "Creando sala...";
        string roomName = "TFO_" + System.Guid.NewGuid().ToString("N").Substring(0, 8);

        // Props iniciales de la sala visibles en lobby para el filtro
        var initial = new Hashtable {
            { MatchProps.GAME_ENDED,   false },
            { MatchProps.ALL_LOSE,     false },
            { MatchProps.ROUND_ACTIVE, false },
            { MatchProps.START_COUNT,  0 }
        };

        var options = new RoomOptions
        {
            MaxPlayers = 4,
            IsOpen = true,
            IsVisible = true,
            CleanupCacheOnLeave = true,
            EmptyRoomTtl = 0,
            PlayerTtl = 0,
            CustomRoomProperties = initial,
            CustomRoomPropertiesForLobby = new[] { MatchProps.GAME_ENDED }
        };

        PhotonNetwork.CreateRoom(roomName, options, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        if (progressText) progressText.text = PhotonNetwork.IsMasterClient ? "Cargando nivel..." : "Esperando al host...";
        if (progressBar) { progressBar.gameObject.SetActive(true); progressBar.value = 0f; }

        if (!PhotonNetwork.IsMasterClient) return;

        // Master elige una de las 3 escenas
        int r = Random.Range(0, 3);
        string selected = (r == 0) ? level1 : (r == 1) ? level2 : level3;

        // Cerrar/invisibilizar la sala en el lobby mientras se juega (opcional, evita joins tardíos)
        PhotonNetwork.CurrentRoom.IsOpen = true;   // si no querés joins tardíos, ponelo en false
        PhotonNetwork.CurrentRoom.IsVisible = true;

        PhotonNetwork.LoadLevel(selected); // sync para todos
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning("LoadingScreen: desconectado -> " + cause);
        SceneManager.UnloadSceneAsync("LoadingScene");
    }
}
