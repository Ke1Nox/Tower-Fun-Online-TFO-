using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("Player"))
        { GameManager.Instance.SaveData(other.transform.position.x, other.transform.position.y, other.transform.position.z); }
        Debug.Log("Save");
    }
}
