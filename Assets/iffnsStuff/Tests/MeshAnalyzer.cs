using iffnsStuff.iffnsBaseSystemForUnity.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshAnalyzer : MonoBehaviour
{
    [SerializeField] MeshFilter LinkedMeshFilter;
    
    public string verticies;
    public string triangles;
    public string Uvs;

    public int verts;
    public int tris;

    public string ObjText;

    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = LinkedMeshFilter.mesh;

        foreach(Vector2 uv in mesh.uv)
        {
            Uvs += uv + System.Environment.NewLine;
        }

        foreach(Vector3 vertex in mesh.vertices)
        {
            verticies += vertex + System.Environment.NewLine;
        }

        for (int i = 0; i < mesh.triangles.Length; i+= 3)
        {
            triangles += mesh.triangles[i] + "\t" +  mesh.triangles[i + 1] + "\t" + mesh.triangles[i + 2] + System.Environment.NewLine;
        }

        ObjText = string.Join(separator: System.Environment.NewLine, values: ObjExporter.GetObjLines(meshName: mesh.name, vertices: mesh.vertices, uvs: mesh.uv, triangles: mesh.triangles, triangleIndexOffset: 0, upDirection: ObjExporter.UpDirection.Y));

        tris = mesh.triangles.Length;
        verts = mesh.vertices.Length;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
