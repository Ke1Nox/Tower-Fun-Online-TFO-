using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class BottonMenu : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    private void Start()
    {
       panel.SetActive(false);
    }
    public void PlayGame()
    {
        SceneManager.LoadScene("Lvl1");
    }
    public void Settings()
    {
        panel.SetActive(true);
    }

    public void BackMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void MutedButton()
    {
        GameManager.Instance.Muted();
    }

    public void DesMuttedButton()
    {
        GameManager.Instance.DesMuted();
    }
}
