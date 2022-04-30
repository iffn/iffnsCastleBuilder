using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshInfoDisplay : MonoBehaviour
{
    [SerializeField] SmartMeshManager Mesh;

    public List<Vector3> vertices;
    public List<int> triangles;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Mesh == null) return;

        if (Mesh.TriangleInfo == null) return;

        vertices = Mesh.TriangleInfo.AllVerticesDirectly;
        triangles = Mesh.TriangleInfo.AllTrianglesDirectly;
        */
    }
}
