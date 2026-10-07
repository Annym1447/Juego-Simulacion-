using UnityEngine;
using System;

public class ParedResorte : MonoBehaviour
{
    public ControladorPacman pacman;
    public float k = 80.0f;   // Constante de rigidez (resorte)
    public float b = 4.0f;    // Amortiguamiento
    SpriteRenderer sprite;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (pacman == null) return;

        Vector2 posicion = pacman.ParticulaV1.transform.position;
        AplicarResorte(posicion, ref pacman.v1, pacman.m1, pacman.radio);
    }

    void AplicarResorte(Vector2 posicion, ref Vector2 velocidad, float masa, float radio)
    {
        // Rectángulo de la pared (según el tamaño del sprite en la escena)
        Bounds limites = sprite.bounds;
        float minX = limites.min.x;
        float maxX = limites.max.x;
        float minY = limites.min.y;
        float maxY = limites.max.y;

        // Punto de la pared más cercano al centro de Pac-Man
        float cercanoX = Mathf.Clamp(posicion.x, minX, maxX);
        float cercanoY = Mathf.Clamp(posicion.y, minY, maxY);

        float dx = posicion.x - cercanoX;
        float dy = posicion.y - cercanoY;
        float distancia = (float)Math.Sqrt(dx * dx + dy * dy);

        if (distancia < radio)
        {
            // Dirección de empuje (normal de la pared hacia Pac-Man)
            float nx, ny;
            if (distancia > 0.0001f)
            {
                nx = dx / distancia;
                ny = dy / distancia;
            }
            else
            {
                // El centro quedó dentro de la pared: empujar desde su centro
                float cx = posicion.x - limites.center.x;
                float cy = posicion.y - limites.center.y;
                float mag = (float)Math.Sqrt(cx * cx + cy * cy);
                if (mag > 0.0001f) { nx = cx / mag; ny = cy / mag; }
                else { nx = 0f; ny = 1f; }
            }

            // Ley de Hooke: F = k * penetración
            float penetracion = radio - distancia;
            float fResorte = k * penetracion;

            // Amortiguamiento solo en la dirección normal: F = -b * v_n
            float vn = velocidad.x * nx + velocidad.y * ny;
            float fAmort = -b * vn;

            float fx = (fResorte + fAmort) * nx;
            float fy = (fResorte + fAmort) * ny;

            // a = F / m
            velocidad.x = velocidad.x + (fx / masa) * Time.deltaTime;
            velocidad.y = velocidad.y + (fy / masa) * Time.deltaTime;
        }
    }
}