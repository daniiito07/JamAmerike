using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class ProceduralMesh : MonoBehaviour
{
    public Transform[] corners;

    private MeshFilter meshFilter; // La forma del objeto
    private Mesh mesh; // La malla del objeto

    private MeshRenderer meshRenderer; // Como lo voy a renderear
    public Material material; // El material del objeto

    void Start()
    {
        // Crear el mesh filter
        meshFilter = gameObject.AddComponent<MeshFilter>();
        mesh = new Mesh();
        mesh.name = "Procedural Mesh";
        meshFilter.mesh = mesh;

        // Asignar informacion a la malla
        MeshData();

        // Crear el Renderer
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = material;
    }

    private void UpdateVertices()
    {
        mesh.vertices = new Vector3[]
        {
            // Cara trasera
            new Vector3(0,0,0), //0
            new Vector3(0,1,0), //1
            new Vector3(1,1,0), //2
            new Vector3(1,0,0), //3
            // Cara frontal
            new Vector3(1,0,1), //4
            new Vector3(1,1,1), //5
            new Vector3(0,1,1), //6
            new Vector3(0,0,1), //7
            // Cara Derecha
            new Vector3(1,0,0), //8
            new Vector3(1,1,0), //9
            new Vector3(1,1,1), //10
            new Vector3(1,0,1), //11
            // Cara Izquierda
            new Vector3(0,0,1), //12
            new Vector3(0,1,1), //13
            new Vector3(0,1,0), //14
            new Vector3(0,0,0), //15
            // Cara Superior
            new Vector3(0,1,0), //16
            new Vector3(0,1,1), //17
            new Vector3(1,1,1), //18
            new Vector3(1,1,0), //19
            // Cara Inferior
            new Vector3(0,0,1), //20
            new Vector3(0,0,0), //21
            new Vector3(1,0,0), //22
            new Vector3(1,0,1), //23
        };
    }

    private void MeshData()
    {
        // Vertices
        UpdateVertices();

        

        // Triangulos
        mesh.triangles = new int[]
        {
            // Cara Trasera
            0,1,2,
            0,2,3,
            // Cara Frontal
            4,5,6,
            4,6,7,
            // Cara Derecha
            8,9,10,
            8,10,11,
            // Cara Izquierda
            12,13,14,
            12,14,15,
            // Cara Superior
            16,17,18,
            16,18,19,
            // Cara Inferior
            20,21,22,
            20,22,23
        };

        // Colores
        mesh.colors = new Color[]
        {
            // Cara Trasera
            Color.black,      //0
            Color.green,    //1 
            Color.blue,     //2
            Color.yellow,   //3
            // Cara Frontal
            Color.gray,      //4
            Color.cyan,    //5 
            Color.magenta,     //6
            Color.white,   //7
            // Cara Derecha
            Color.yellow,      //8
            Color.blue,    //9 
            Color.cyan,     //10
            Color.gray,   //11
            // Cara Izquierda
            Color.white,      //12
            Color.magenta,    //13
            Color.green,     //14
            Color.black,   //15
            // Cara Superior
            Color.green,      //16
            Color.magenta,    //17
            Color.cyan,     //18
            Color.blue,   //19
            // Cara Inferior
            Color.white,      //20
            Color.black,    //21
            Color.yellow,     //22
            Color.gray,   //23
        };

        // UVs
        mesh.uv = new Vector2[]
        {
            // Cara Trasera
            new Vector2(0,0),
            new Vector2(0,1),
            new Vector2(1,1),
            new Vector2(1,0),
            // Cara Frontal
            new Vector2(0,0),
            new Vector2(0,1),
            new Vector2(1,1),
            new Vector2(1,0),
            // Cara Derecha
            new Vector2(0,0),
            new Vector2(0,1),
            new Vector2(1,1),
            new Vector2(1,0),
            // Cara Izquierda
            new Vector2(0,0),
            new Vector2(0,1),
            new Vector2(1,1),
            new Vector2(1,0),
            // Cara Superior
            new Vector2(0,0),
            new Vector2(0,1),
            new Vector2(1,1),
            new Vector2(1,0),
            // Cara Inferior
            new Vector2(0,0),
            new Vector2(0,1),
            new Vector2(1,1),
            new Vector2(1,0)
        };

        // Normales
        mesh.normals = new Vector3[]
        {
            // Cara Trasera
            Vector3.back,
            Vector3.back,
            Vector3.back,
            Vector3.back,
            // Cara Frontal
            Vector3.forward,
            Vector3.forward,
            Vector3.forward,
            Vector3.forward,
            // Cara Derecha
            Vector3.right,
            Vector3.right,
            Vector3.right,
            Vector3.right,
            // Cara Izquierda
            Vector3.left,
            Vector3.left,
            Vector3.left,
            Vector3.left,
            // Cara Superior
            Vector3.up,
            Vector3.up,
            Vector3.up,
            Vector3.up,
            // Cara Inferior
            Vector3.down,
            Vector3.down,
            Vector3.down,
            Vector3.down
        };

        // Actualizar la malla
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
    }
}
