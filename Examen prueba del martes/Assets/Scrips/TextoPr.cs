using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextoPr : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float velText;
    [SerializeField] private string texto;

    private void Start()
    {
        text.text = "";

        StartCoroutine(EmpezarText());
    }

    private IEnumerator EmpezarText()
    {
        foreach (var item in texto)
        {
            text.text += item;

            yield return new WaitForSeconds(velText);
        }
    }
}


