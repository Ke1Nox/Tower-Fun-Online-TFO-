using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class LVL1Manager : MonoBehaviour
{
    public UnityEvent evento;
   [SerializeField] private float CurrentTime ;

    //public GameObject player;


    private void Start()
    {
        GameManager.Instance.Music();

        StartCoroutine(Tiempo());
    }

    private IEnumerator Tiempo()
    {
        while (CurrentTime > 0)
        {
            CurrentTime -= 1;

            yield return new WaitForSeconds(1);
            Debug.Log("Tiempo: " + CurrentTime);
        }
        GameManager.Instance.ChekResult();
       
    }

    //private void SpawPlayer()
    //{
    //    Instantiate (player);
    //}

}
