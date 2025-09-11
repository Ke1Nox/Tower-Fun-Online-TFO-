using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressText;

    private static string targetScene;

    /// <summary>
    /// Llamar a esta función para cargar una escena con pantalla de carga.
    /// </summary>
    public static void LoadScene(string sceneName)
    {
        targetScene = sceneName;

        // Cargar la pantalla de carga encima de la actual
        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Additive);
    }

    private void Start()
    {
        StartCoroutine(LoadAsync());
    }

    private IEnumerator LoadAsync()
    {
        // 🔹 Cargar la escena objetivo en segundo plano
        AsyncOperation op = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Single);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);

            if (progressBar != null) progressBar.value = progress;
            if (progressText != null) progressText.text = (progress * 100f).ToString("F0") + "%";

            // Cuando ya terminó de cargar
            if (op.progress >= 0.9f)
            {
                if (progressBar != null) progressBar.value = 1f;
                if (progressText != null) progressText.text = "100%";

                // Activar la escena objetivo
                op.allowSceneActivation = true;

                // 🔹 Esperar un frame para que se active y después descargar LoadingScene
                yield return null;
                SceneManager.UnloadSceneAsync("LoadingScene");
            }

            yield return null;
        }
    }
}
