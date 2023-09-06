using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public float speed = 5.0f; // Velocidad de movimiento de la cámara

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveDirection = new Vector3(0, 0, 0); // Dirección inicial de movimiento

        // Movimiento en el eje X con flechas izquierda y derecha
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveDirection.x -= speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveDirection.x += speed * Time.deltaTime;
        }

        // Movimiento en el eje Z con flechas arriba y abajo
        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveDirection.z += speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            moveDirection.z -= speed * Time.deltaTime;
        }

        // Movimiento en el eje Y con teclas W y S
        if (Input.GetKey(KeyCode.W))
        {
            moveDirection.y += speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDirection.y -= speed * Time.deltaTime;
        }

        // Aplica el movimiento a la cámara
        transform.Translate(moveDirection);
        
    }
}
