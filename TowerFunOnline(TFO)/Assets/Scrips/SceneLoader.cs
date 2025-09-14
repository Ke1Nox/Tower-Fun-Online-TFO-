using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Nombre de la escena de menú")]
    public string menu = "Menu";

    public void LoadMenu()
    {
        SceneManager.LoadScene(menu);
    }

    public void ReloadCurrent()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}