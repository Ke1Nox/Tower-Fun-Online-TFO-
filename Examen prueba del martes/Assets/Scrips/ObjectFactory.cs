using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFactory : MonoBehaviour
{
    [SerializeField]GameObject coin;
    [SerializeField]float respawnTimer;
    [SerializeField]Vector3 []positions;

    private void Start()
    {
        StartCoroutine(SpawnItems());
    }

    IEnumerator SpawnItems()
    {
        for (int i = 0; i < 10; i++)
        {
            Instantiate(coin, positions[Random.Range(0,positions.Length)],Quaternion.identity);
            yield return new WaitForSecondsRealtime(respawnTimer);
        }
    }

    
}
