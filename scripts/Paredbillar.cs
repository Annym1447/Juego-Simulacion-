using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class ParedBillarTilemap : MonoBehaviour
{
    public ControladorPacman pacman;
    public Tilemap tilemap;      // Tilemap de las paredes

    public float e = 1.0f;      

    void LateUpdate()
    {
        if (pacman == null || tilemap == null) return;

        Vector2 pos = pacman.ParticulaV1.transform.position;
        float radius = pacman.radio;

        ChoqueParedes(ref pos, ref pacman.v1, radius);

        pacman.ParticulaV1.transform.position = new Vector3(
            pos.x, pos.y, pacman.ParticulaV1.transform.position.z);
    }

    void ChoqueParedes(ref Vector2 pos, ref Vector2 vel, float radius)
    {
       
        Vector3 tam = Vector3.Scale(tilemap.cellSize, tilemap.transform.lossyScale);
        float tamX = Mathf.Abs(tam.x);
        float tamY = Mathf.Abs(tam.y);

        Vector3Int centro = tilemap.WorldToCell(pos);
        int rx = Mathf.CeilToInt(radius / tamX) + 1;
        int ry = Mathf.CeilToInt(radius / tamY) + 1;

        for (int x = -rx; x <= rx; x++)
        {
            for (int y = -ry; y <= ry; y++)
            {
                Vector3Int celda = new Vector3Int(centro.x + x, centro.y + y, 0);
                if (!tilemap.HasTile(celda)) continue;

                Vector3 c = tilemap.GetCellCenterWorld(celda);
                float izquierda = c.x - tamX / 2f;
                float derecha = c.x + tamX / 2f;
                float bajo = c.y - tamY / 2f;
                float alto = c.y + tamY / 2f;

                ChoqueCelda(ref pos, ref vel, radius, izquierda, derecha, bajo, alto);
            }
        }
    }

    void ChoqueCelda(ref Vector2 pos, ref Vector2 vel, float radius,
                     float izquierda, float derecha, float bajo, float alto)
    {

        float cercanoX = Mathf.Clamp(pos.x, izquierda, derecha);
        float cercanoY = Mathf.Clamp(pos.y, bajo, alto);

        float dx = pos.x - cercanoX;
        float dy = pos.y - cercanoY;
        float distancia = (float)Math.Sqrt(dx * dx + dy * dy);

        if (distancia >= radius) return;

    
        float nx, ny;
        if (distancia > 0.0001f)
        {
            nx = dx / distancia;
            ny = dy / distancia;
        }
        else
        {
           
            float dIzq = pos.x - izquierda;
            float dDer = derecha - pos.x;
            float dAba = pos.y - bajo;
            float dArr = alto - pos.y;
            float min = Mathf.Min(Mathf.Min(dIzq, dDer), Mathf.Min(dAba, dArr));

            nx = 0f; ny = 0f;
            if (min == dIzq) nx = -1f;
            else if (min == dDer) nx = 1f;
            else if (min == dAba) ny = -1f;
            else ny = 1f;

            distancia = 0f;
        }

        
        float penetracion = radius - distancia;
        pos.x += nx * penetracion;
        pos.y += ny * penetracion;

        
        float vn = vel.x * nx + vel.y * ny;
        if (vn < 0f)
        {
           
            vel.x -= (1f + e) * vn * nx;
            vel.y -= (1f + e) * vn * ny;
        }
    }
}
