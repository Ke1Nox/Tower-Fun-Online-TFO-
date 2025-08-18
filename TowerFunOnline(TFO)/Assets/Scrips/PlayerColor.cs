using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColor : MonoBehaviour
{
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();

        // Color aleatorio (RGB con valores entre 0 y 1)
        Color randomColor = new Color(
            Random.value, // R
            Random.value, // G
            Random.value  // B
        );

        rend.material.color = randomColor;
    }
}
