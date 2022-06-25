using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using iffnsStuff.iffnsBaseSystemForUnity;

public class SmartMeshManager : MeshManager
{
    MeshCollider currentCollider;
    MeshFilter currentMeshFilter;
    MeshRenderer currentMeshRenderer;
    ClickForwarder currentClickForwarder;
    public TriangleMeshInfo LinkedTriangleInfo { get; private set; }

    Mesh currentMesh;
    
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
        //Debug.Log("Setup");

        base.setup(mainObject: mainObject);

        this.LinkedMainObject = mainObject;

        currentMeshFilter = transform.GetComponent<MeshFilter>();
        currentCollider = transform.GetComponent<MeshCollider>();
        currentMeshRenderer = transform.GetComponent<MeshRenderer>();
        currentClickForwarder = transform.GetComponent<ClickForwarder>();
        currentClickForwarder.MainObject = mainObject;

        currentMesh = MeshPoolManager.GetMesh();

        currentMeshFilter.sharedMesh = currentMesh;
    }

    public void SetTriangleInfo(TriangleMeshInfo newInfo)
    {
        LinkedTriangleInfo = newInfo;

        List<Vector3> vertices = newInfo.AllVerticesDirectly;

        if (vertices.Count > 65535)
        {
            //Avoid vertex limit
            //https://answers.unity.com/questions/471639/mesh-with-more-than-65000-vertices.html
            currentMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        }

        currentMesh.vertices = vertices.ToArray();
        currentMesh.triangles = newInfo.AllTrianglesDirectly.ToArray();
        currentMesh.uv = newInfo.UVs.ToArray();

        currentMesh.RecalculateNormals();
        currentMesh.RecalculateTangents();
        currentMesh.RecalculateBounds();

        //currentCollider.isTrigger = !newInfo.ActiveCollider;

        switch (newInfo.ActiveCollider)
        {
            case TriangleMeshInfo.ColliderStates.VisibleCollider:
                currentCollider.enabled = true;
                gameObject.layer = WorldController.LayerManager.GetLayerNumber(WorldController.LayerManager.Layers.Default);
                break;
            case TriangleMeshInfo.ColliderStates.SeeThroughCollider:
                currentCollider.enabled = true;
                gameObject.layer = WorldController.LayerManager.GetLayerNumber(WorldController.LayerManager.Layers.NoSelection);
                break;
            case TriangleMeshInfo.ColliderStates.VisbleWithoutCollider:
                currentCollider.enabled = true;
                gameObject.layer = WorldController.LayerManager.GetLayerNumber(WorldController.LayerManager.Layers.SelectOnly);
                /*
                if (newInfo.Triangles.Count <= 255)
                {
                    currentCollider.convex = true;
                    currentCollider.isTrigger = true;
                }
                else
                {
                    Debug.LogWarning("Warning: Current trigger collider has more than 255 colliders");
                    currentCollider.isTrigger = false;
                    currentCollider.convex = false;
                }
                */
                break;
            default:
                break;
        }

        /*
        if (newInfo.ActiveCollider)
        {
            currentCollider.isTrigger = false;
            currentCollider.convex = false;
        }
        else
        {
            if(newInfo.Triangles.Count <= 255)
            {
                currentCollider.convex = true;
                currentCollider.isTrigger = true;
            }
            else
            {
                Debug.LogWarning("Warning: Current trigger collider has more than 255 colliders");
                currentCollider.isTrigger = false;
                currentCollider.convex = false;
            }
        }
        */

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
            currentCollider.sharedMesh = currentMesh;
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

    public void DestroyMesh()
    {
        MeshPoolManager.ReturnMeshToQueue(currentMesh);

        currentMesh = null;
        
        //Debug.Log("Destroy");
    }
}
