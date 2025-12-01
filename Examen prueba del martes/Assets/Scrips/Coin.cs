using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int puntajeCoins = 1;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           
            GameManager.Instance.SumarCoins(puntajeCoins);
            Destroy(gameObject);
        }
    }
}
