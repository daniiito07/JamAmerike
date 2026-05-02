using UnityEngine;

public class ImageToMaze : MonoBehaviour
{
    [Header("Configuración de Archivo")]
    public Texture2D mazeTexture; // La imagen del laberinto

    [Header("Prefabs")]
    public GameObject wallPrefab;
    public float cellSize = 1f;

    [ContextMenu("Generar Laberinto")]
    public void Generate()
    {
        // Limpiar laberinto previo (opcional)
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }

        if (mazeTexture == null) return;

        for (int x = 0; x < mazeTexture.width; x++)
        {
            for (int y = 0; y < mazeTexture.height; y++)
            {
                Color pixelColor = mazeTexture.GetPixel(x, y);

                // Si el píxel es oscuro (casi negro), ponemos un muro
                if (pixelColor.grayscale < 0.5f)
                {
                    Vector3 pos = new Vector3(x * cellSize, 0, y * cellSize);
                    Instantiate(wallPrefab, pos, Quaternion.identity, transform);
                }
            }
        }
    }
}