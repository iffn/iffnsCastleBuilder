using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshPoolManager
{
    readonly static Queue MeshQueue = new();

    public static Mesh GetMesh()
    {
        if(MeshQueue.Count == 0)
        {
            return new Mesh();
        }

        return MeshQueue.Dequeue() as Mesh;
    }

    public static void ReturnMeshToQueue(Mesh mesh)
    {
        mesh.Clear();

        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt16;

        MeshQueue.Enqueue(mesh);
    }
}
