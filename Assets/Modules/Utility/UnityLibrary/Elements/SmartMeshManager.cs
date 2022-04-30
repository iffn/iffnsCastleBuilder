using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SmartMeshManager : MeshManager
{
    MeshCollider currentCollider;
    MeshFilter currentMeshFilter;
    MeshRenderer currentMeshRenderer;
    ClickForwarder currentClickForwarder;
    public TriangleMeshInfo LinkedTriangleInfo { get; private set; }


    public override Material CurrentMaterial
    {
        get
        {
            if (LinkedTriangleInfo == null) return null;
            if (LinkedTriangleInfo.MaterialReference == null) return LinkedTriangleInfo.AlternativeMaterial;
            return LinkedTriangleInfo.MaterialReference.Val.LinkedMaterial;
        }
    }

    public override MailboxLineMaterial CurrentMaterialReference
    {
        get
        {
            if (LinkedTriangleInfo == null) return null;
            return LinkedTriangleInfo.MaterialReference;
        }
    }

    public void Setup(BaseGameObject mainObject)
    {
        base.setup(mainObject: mainObject);

        this.LinkedMainObject = mainObject;

        currentMeshFilter = transform.GetComponent<MeshFilter>();
        currentCollider = transform.GetComponent<MeshCollider>();
        currentMeshRenderer = transform.GetComponent<MeshRenderer>();
        currentClickForwarder = transform.GetComponent<ClickForwarder>();
        currentClickForwarder.MainObject = mainObject;

        currentMeshFilter.mesh.Clear();
    }

    public void SetTriangleInfo(TriangleMeshInfo newInfo)
    {
        LinkedTriangleInfo = newInfo;

        currentMeshFilter.mesh.vertices = newInfo.AllVerticesDirectly.ToArray();
        currentMeshFilter.mesh.triangles = newInfo.AllTrianglesDirectly.ToArray();
        currentMeshFilter.mesh.uv = newInfo.UVs.ToArray();

        currentMeshFilter.mesh.RecalculateNormals();
        currentMeshFilter.mesh.RecalculateTangents();
        currentMeshFilter.mesh.RecalculateBounds();

        //currentCollider.isTrigger = !newInfo.ActiveCollider;
        currentCollider.enabled = newInfo.ActiveCollider;

        if (CurrentMaterial != null)
        {
            currentMeshRenderer.material = CurrentMaterial;
            currentMeshRenderer.enabled = true;
        }
        else
        {
            //currentMeshRenderer.enabled = false; //ToDo: Activate when done
        }

        RefreshCollisionMesh();

    }

    void RefreshCollisionMesh()
    {
        void RefreshCollider()
        {
            currentCollider.sharedMesh = null;
            currentCollider.sharedMesh = currentMeshFilter.mesh;
        }

        bool colliderActivationState = currentCollider.enabled;

        if (!colliderActivationState) //For some reason, the collision meshes do not seem to update when the collider is deactivated during generation
        {
            currentCollider.enabled = true;
            RefreshCollider();
            currentCollider.enabled = false;
        }
        else
        {
            RefreshCollider();
        }
    }
}
