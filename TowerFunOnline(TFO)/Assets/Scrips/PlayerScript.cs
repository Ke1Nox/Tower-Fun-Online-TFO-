using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
   
    public float speed = 5f; // Velocidad de movimiento

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal"); // A/D o flechas ← →
        float moveY = Input.GetAxisRaw("Vertical");   // W/S o flechas ↑ ↓

        Vector3 movement = new Vector3(moveX, moveY, 0f).normalized;

        transform.position += movement * speed * Time.deltaTime;
    }
}
