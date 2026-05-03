using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ImageToMaze1 : MonoBehaviour
{
    public Texture2D mazeTexture;
    public Material wallMaterial; // Asigna tu material aquí
    public float cellSize = 1f;

    [ContextMenu("Generar Laberinto Bajo Poligonaje")]
    public void Generate()
    {
        ClearMaze();
        if (mazeTexture == null) return;

        List<CombineInstance> combine = new List<CombineInstance>();

        // Creamos una malla de referencia para una sola cara (Quad)
        Mesh faceMesh = CreateQuadMesh();

        for (int x = 0; x < mazeTexture.width; x++)
        {
            for (int y = 0; y < mazeTexture.height; y++)
            {
                if (IsBlack(x, y))
                {
                    // Solo creamos las caras que dan al vacío (píxeles no negros)
                    // Arriba (Top)
                    AddFace(combine, faceMesh, x, y, Vector3.up, Quaternion.Euler(90, 0, 0));
                    // Abajo (Bottom)
                    AddFace(combine, faceMesh, x, y, Vector3.zero, Quaternion.Euler(-90, 0, 0));

                    // Lados - Solo si el vecino NO es negro
                    if (!IsBlack(x + 1, y)) AddFace(combine, faceMesh, x, y, Vector3.right, Quaternion.Euler(0, -90, 0));
                    if (!IsBlack(x - 1, y)) AddFace(combine, faceMesh, x, y, Vector3.zero, Quaternion.Euler(0, 90, 0));
                    if (!IsBlack(x, y + 1)) AddFace(combine, faceMesh, x, y, new Vector3(0, 0, 1), Quaternion.Euler(0, 180, 0));
                    if (!IsBlack(x, y - 1)) AddFace(combine, faceMesh, x, y, Vector3.zero, Quaternion.Euler(0, 0, 0));
                }
            }
        }

        Mesh finalMesh = new Mesh();
        finalMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        finalMesh.CombineMeshes(combine.ToArray(), true, true);

        GetComponent<MeshFilter>().sharedMesh = finalMesh;
        GetComponent<MeshRenderer>().sharedMaterial = wallMaterial;

        if (GetComponent<MeshCollider>() == null) gameObject.AddComponent<MeshCollider>();
        GetComponent<MeshCollider>().sharedMesh = finalMesh;
    }

    bool IsBlack(int x, int y)
    {
        if (x < 0 || x >= mazeTexture.width || y < 0 || y >= mazeTexture.height) return false;
        return mazeTexture.GetPixel(x, y).grayscale < 0.05f;
    }

    void AddFace(List<CombineInstance> list, Mesh mesh, int x, int y, Vector3 offset, Quaternion rot)
    {
        CombineInstance ci = new CombineInstance();
        ci.mesh = mesh;
        Vector3 position = new Vector3(x * cellSize, 0, y * cellSize) + (offset * cellSize);
        ci.transform = Matrix4x4.TRS(position, rot, Vector3.one * cellSize);
        list.Add(ci);
    }

    Mesh CreateQuadMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 1, 0), new Vector3(0, 1, 0) };
        mesh.triangles = new int[] { 0, 2, 1, 0, 3, 2 };
        mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
        mesh.RecalculateNormals();
        return mesh;
    }

    void ClearMaze()
    {
        GetComponent<MeshFilter>().sharedMesh = null;
        if (GetComponent<MeshCollider>() != null) GetComponent<MeshCollider>().sharedMesh = null;
    }
}