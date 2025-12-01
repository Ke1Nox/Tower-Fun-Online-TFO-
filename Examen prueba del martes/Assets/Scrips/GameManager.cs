using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    AudioSource audioSource;

    [SerializeField]AudioClip[]clips;

   public event System.Action OnPlayerDeath;
   private PlayerController player;
    

    private int coins = 0;

    private void Awake()
    {
        // Si NO existe, este es el GameManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // persiste entre escenas

            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject); // evitar duplicados
        }
    }

    private void Start()
    {
      
        Music();
    }
    public void RegisterPlayer(PlayerController p)
    {
        player = p;
    }

    public void SaveData( float posX, float posY, float posZ)
    {
        PlayerPrefs.SetInt("score", coins);
        PlayerPrefs.SetFloat("posX", posX);
        PlayerPrefs.SetFloat("posY", posY);
        PlayerPrefs.SetFloat("posZ", posZ);
        PlayerPrefs.Save();
    }

   
    public void SumarCoins(int amount)
    {
        coins += amount;
      
        audioSource.PlayOneShot(clips[3]);
        Debug.Log("coins: " + coins);
    }

    public void Music()
    {
        audioSource.Stop();
        audioSource.clip=clips[SceneManager.GetActiveScene().buildIndex];
        audioSource.Play();

    }

    public void Morir()
    {
        audioSource.PlayOneShot(clips[2]);

        OnPlayerDeath?.Invoke();
        OnPlayerDeath = null;

        StartCoroutine(CargarEscena());
       
      
    }

    private IEnumerator CargarEscena()
    {

        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Muted()
    {
        audioSource.enabled = false;
    }

    public void DesMuted()
    {
        audioSource.enabled = true;
    }

    public void ChekResult()
    {
        if (coins > 3)
        {
            Debug.Log("Victoria");

            audioSource.PlayOneShot(clips[5]);
            player.Anim.SetTrigger("Victoria");

        }

        else
        {
            Debug.Log("Derrota");
            audioSource.PlayOneShot(clips[4]);
            player.Anim.SetTrigger("Derrota");
        }
    }
}
