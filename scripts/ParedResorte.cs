using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class ParedResorteTilemap : MonoBehaviour
{
    public ControladorPacman pacman;
    public Tilemap tilemap;          // Arrastra aquí el Tilemap de las paredes
    public float k = 80.0f;          // Constante de rigidez
    public float b = 4.0f;           // Amortiguamiento

    void Update()
    {
        if (pacman == null || tilemap == null) return;

        Vector2 posicion = pacman.ParticulaV1.transform.position;
        float radio = pacman.radio;

        // Tamaño real de una celda en el mundo
        Vector3 tam = Vector3.Scale(tilemap.cellSize, tilemap.transform.lossyScale);

        // Celda donde está Pac-Man y cuántas celdas alrededor revisar
        Vector3Int centro = tilemap.WorldToCell(posicion);
        int rx = Mathf.CeilToInt(radio / Mathf.Abs(tam.x)) + 1;
        int ry = Mathf.CeilToInt(radio / Mathf.Abs(tam.y)) + 1;

        for (int x = -rx; x <= rx; x++)
        {
            for (int y = -ry; y <= ry; y++)
            {
                Vector3Int celda = new Vector3Int(centro.x + x, centro.y + y, 0);
                if (!tilemap.HasTile(celda)) continue;

                // Rectángulo de esta celda (equivale al "sprite.bounds" de una pared)
                Vector3 c = tilemap.GetCellCenterWorld(celda);
                Bounds limites = new Bounds(c, new Vector3(Mathf.Abs(tam.x), Mathf.Abs(tam.y), 1f));

                AplicarResorte(posicion, limites, ref pacman.v1, pacman.m1, radio);
            }
        }
    }

    void AplicarResorte(Vector2 posicion, Bounds limites, ref Vector2 velocidad, float masa, float radio)
    {
        float cercanoX = Mathf.Clamp(posicion.x, limites.min.x, limites.max.x);
        float cercanoY = Mathf.Clamp(posicion.y, limites.min.y, limites.max.y);

        float dx = posicion.x - cercanoX;
        float dy = posicion.y - cercanoY;
        float distancia = (float)Math.Sqrt(dx * dx + dy * dy);

        if (distancia < radio)
        {
            float nx, ny;
            if (distancia > 0.0001f)
            {
                nx = dx / distancia;
                ny = dy / distancia;
            }
            else
            {
                float cx = posicion.x - limites.center.x;
                float cy = posicion.y - limites.center.y;
                float mag = (float)Math.Sqrt(cx * cx + cy * cy);
                if (mag > 0.0001f) { nx = cx / mag; ny = cy / mag; }
                else { nx = 0f; ny = 1f; }
            }

            float penetracion = radio - distancia;
            float fResorte = k * penetracion;

            float vn = velocidad.x * nx + velocidad.y * ny;
            float fAmort = -b * vn;

            float fx = (fResorte + fAmort) * nx;
            float fy = (fResorte + fAmort) * ny;

            velocidad.x += (fx / masa) * Time.deltaTime;
            velocidad.y += (fy / masa) * Time.deltaTime;
        }
    }
}
