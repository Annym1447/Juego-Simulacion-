using UnityEngine;
using System;

public class ControladorPacman : MonoBehaviour
{
    public GameObject ParticulaV1;
    Vector2 posicion1;
    public Vector2 v1;
    public float m1, fuerzaControl;
    public float e, fricc, radio;
    Camera camara;

    void Start()
    {
        ParticulaV1 = GameObject.Find("CV1");
        posicion1 = ParticulaV1.transform.position;
        camara = Camera.main;
    }

    void Update()
    {
        AplicarFuerza(ref v1, m1);
        AplicarFriccion(ref v1);

        posicion1.x = posicion1.x + v1.x * Time.deltaTime;
        posicion1.y = posicion1.y + v1.y * Time.deltaTime;

        Paredes(ref posicion1, ref v1);

        ParticulaV1.transform.position = posicion1;
    }

    void AplicarFuerza(ref Vector2 velocidad, float masa)
    {
        float fx = Input.GetAxisRaw("Horizontal");
        float fy = Input.GetAxisRaw("Vertical");

        float magnitud = (float)Math.Sqrt(fx * fx + fy * fy);
        if (magnitud > 0f)
        {
            fx = fx / magnitud;
            fy = fy / magnitud;

            // a = F / m
            velocidad.x = velocidad.x + (fx * fuerzaControl / masa) * Time.deltaTime;
            velocidad.y = velocidad.y + (fy * fuerzaControl / masa) * Time.deltaTime;
        }
    }

    void AplicarFriccion(ref Vector2 velocidad)
    {
        float factor = 1 - fricc * Time.deltaTime;
        if (factor < 0)
        {
            factor = 0;
        }
        velocidad.x = velocidad.x * factor;
        velocidad.y = velocidad.y * factor;

        float velocidadCuadrada = velocidad.x * velocidad.x + velocidad.y * velocidad.y;
        if (velocidadCuadrada < 0.0001)
        {
            velocidad.x = 0;
            velocidad.y = 0;
        }
    }

    void Paredes(
        ref Vector2 posicion,
        ref Vector2 velocidad)
    {
        float alto = camara.orthographicSize;
        float ancho = alto * camara.aspect;

        float izquierda = camara.transform.position.x - ancho;
        float derecha = camara.transform.position.x + ancho;
        float abajo = camara.transform.position.y - alto;
        float arriba = camara.transform.position.y + alto;

        if (posicion.x - radio <= izquierda)
        {
            posicion.x = izquierda + radio;
            if (velocidad.x < 0)
            {
                velocidad.x = -velocidad.x * e;
            }
        }
        if (posicion.x + radio >= derecha)
        {
            posicion.x = derecha - radio;
            if (velocidad.x > 0)
            {
                velocidad.x = -velocidad.x * e;
            }
        }
        if (posicion.y - radio <= abajo)
        {
            posicion.y = abajo + radio;
            if (velocidad.y < 0)
            {
                velocidad.y = -velocidad.y * e;
            }
        }
        if (posicion.y + radio >= arriba)
        {
            posicion.y = arriba - radio;
            if (velocidad.y > 0)
            {
                velocidad.y = -velocidad.y * e;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radio);
    }
}