using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using iffnsStuff.iffnsBaseSystemForUnity;

public class UnityMeshManager : MeshManager
{
    MeshFilter currentMeshFilter;
    MeshRenderer currentMeshRenderer;
    ClickForwarder currentClickForwarder;
    MailboxLineMaterial currentMaterialReference;

    public override MailboxLineMaterial CurrentMaterialReference
    {
        get
        {
            return currentMaterialReference;
        }
    }
    public override Material CurrentMaterial
    {
        get
        {
            if (currentMaterialReference == null) return null;
            return currentMaterialReference.Val.LinkedMaterial;
        }
    }

    public TriangleMeshInfo TriangleInfo
    {
        get
        {
            TriangleMeshInfo returnValue = new TriangleMeshInfo();

            List<Vector3> localVerticies = currentMeshFilter.mesh.vertices.OfType<Vector3>().ToList();
            List<Vector3> correctVerticies = new List<Vector3>();

            foreach (Vector3 vertex in localVerticies)
            {
                Vector3 globalVertex = transform.TransformPoint(vertex);

                correctVerticies.Add(LinkedMainObject.transform.InverseTransformPoint(globalVertex));
            }

            returnValue.AllVerticesDirectly = correctVerticies;
            returnValue.AllTrianglesDirectly = currentMeshFilter.mesh.triangles.OfType<int>().ToList();
            returnValue.UVs = currentMeshFilter.mesh.uv.OfType<Vector2>().ToList();
            returnValue.MaterialReference = currentMaterialReference;

            return returnValue;
        }
    }

    public void Setup(BaseGameObject mainObject, MailboxLineMaterial currentMaterialReference)
    {
        base.setup(mainObject: mainObject);
        this.currentMaterialReference = currentMaterialReference;

        //currentMeshFilter = transform.GetComponent<MeshFilter>();
        //currentCollider = transform.GetComponent<MeshCollider>();
        currentMeshFilter = transform.GetComponent<MeshFilter>();
        currentMeshRenderer = transform.GetComponent<MeshRenderer>();
        currentClickForwarder = transform.GetComponent<ClickForwarder>();
        if (currentClickForwarder != null) currentClickForwarder.MainObject = mainObject;

        UpdateMaterial();
    }

    public void UpdateMaterial()
    {
        if (currentMaterialReference == null) return;
        if (currentMeshRenderer == null) return;
        currentMeshRenderer.material = currentMaterialReference.Val.LinkedMaterial;
    }

    
}
