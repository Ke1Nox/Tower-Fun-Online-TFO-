using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Photon.Pun;

public class LoadingScreen : MonoBehaviourPunCallbacks
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressText;

    private static string targetScene;
    private static bool isConnecting = false;

    public static void ShowConnecting(string nextScene)
    {
        targetScene = nextScene;
        isConnecting = true;
        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Additive);
    }

    public static void LoadScene(string sceneName)
    {
        targetScene = sceneName;
        isConnecting = false;
        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Additive);
    }

    private void Start()
    {
        if (isConnecting)
        {
            if (progressText != null) progressText.text = "Conectando...";
            if (progressBar != null) progressBar.gameObject.SetActive(false);

            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            StartCoroutine(LoadAsync());
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado a Photon Master");
      
        StartCoroutine(LoadAsync());
    }

    private IEnumerator LoadAsync()
    {
        if (progressBar != null) progressBar.gameObject.SetActive(true);

        AsyncOperation op = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Single);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);

            if (progressBar != null) progressBar.value = progress;
            if (progressText != null) progressText.text = (progress * 100f).ToString("F0") + "%";

            if (op.progress >= 0.9f)
            {
                if (progressBar != null) progressBar.value = 1f;
                if (progressText != null) progressText.text = "100%";

                op.allowSceneActivation = true;

                yield return null;
                SceneManager.UnloadSceneAsync("LoadingScene");
            }

            yield return null;
        }
    }
}