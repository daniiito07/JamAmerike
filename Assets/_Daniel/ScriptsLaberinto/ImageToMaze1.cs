using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ImageToMaze1 : MonoBehaviour
{
    [Header("Configuración de Archivo")]
    public Texture2D mazeTexture;

    [Header("Prefabs")]
    public GameObject wallPrefab; // Asegúrate de que sea un cubo simple
    public float cellSize = 1f;

    [ContextMenu("Generar Laberinto Optimizado")]
    public void Generate()
    {
        // 1. Limpiar todo antes de empezar
        ClearMaze();

        if (mazeTexture == null || wallPrefab == null)
        {
            Debug.LogError("Falta la textura o el Prefab del muro.");
            return;
        }

        // 2. Crear un contenedor temporal para los bloques
        GameObject tempContainer = new GameObject("TempContainer");
        tempContainer.transform.parent = this.transform;

        // 3. Instanciar bloques basados en píxeles
        for (int x = 0; x < mazeTexture.width; x++)
        {
            for (int y = 0; y < mazeTexture.height; y++)
            {
                Color pixelColor = mazeTexture.GetPixel(x, y);
                if (pixelColor.grayscale < 0.5f)
                {
                    Vector3 pos = new Vector3(x * cellSize, 0, y * cellSize);
                    Instantiate(wallPrefab, pos, Quaternion.identity, tempContainer.transform);
                }
            }
        }

        // 4. Combinar todas las mallas en una sola
        CombineMeshes(tempContainer);

        // 5. Limpieza final: Borrar los GameObjects individuales
        DestroyImmediate(tempContainer);

        Debug.Log("¡Laberinto generado y optimizado!");
    }

    void CombineMeshes(GameObject container)
    {
        MeshFilter[] meshFilters = container.GetComponentsInChildren<MeshFilter>();

        // Si no hay muros, no hacemos nada
        if (meshFilters.Length == 0) return;

        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        // Guardamos el material del primer muro para aplicarlo al final (Evita el color rosa)
        MeshRenderer firstRenderer = meshFilters[0].GetComponent<MeshRenderer>();
        Material wallMaterial = firstRenderer != null ? firstRenderer.sharedMaterial : null;

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            // Posición relativa al objeto ImageToMaze1
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix * transform.worldToLocalMatrix;
        }

        // Creamos la malla final
        Mesh finalMesh = new Mesh();
        finalMesh.name = "CombinedMaze";
        finalMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        finalMesh.CombineMeshes(combine);

        // Asignamos la malla al MeshFilter del objeto principal
        GetComponent<MeshFilter>().sharedMesh = finalMesh;

        // Asignamos el material para que NO se vea rosa
        if (wallMaterial != null)
        {
            GetComponent<MeshRenderer>().sharedMaterial = wallMaterial;
        }

        // --- CONFIGURACIÓN DEL COLLIDER ---
        MeshCollider sc = GetComponent<MeshCollider>();
        if (sc == null) sc = gameObject.AddComponent<MeshCollider>();

        // Al asignar la finalMesh, el collider solo existirá donde hay paredes
        sc.sharedMesh = finalMesh;

        Debug.Log("Malla y Colisionador generados correctamente.");
    }

    void ClearMaze()
    {
        GetComponent<MeshFilter>().sharedMesh = null;
        if (GetComponent<MeshCollider>() != null) GetComponent<MeshCollider>().sharedMesh = null;

        // Borrar hijos remanentes
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in transform) children.Add(child.gameObject);
        children.ForEach(child => DestroyImmediate(child));
    }
}
