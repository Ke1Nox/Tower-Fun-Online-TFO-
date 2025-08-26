using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton (para que solo haya un GameManager en la escena)
    public static GameManager Instance;

    private void Awake()
    {
        // Si ya hay un GameManager, destruye el nuevo
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // No destruir al cambiar de escena
        }
    }

    // Método para cargar la escena 0
    public void CargarEscena0()
    {
        SceneManager.LoadScene(0);
    }

    // Método para cargar la escena 1
    public void CargarEscena1()
    {
        SceneManager.LoadScene(1);
        Debug.Log("Cargando");
    }

    // Método para salir del juego
    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}