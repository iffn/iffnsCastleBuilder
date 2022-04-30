using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiMeshManager : MonoBehaviour
{
    BaseGameObject linkedObject;
    List<SmartMeshManager> MeshManagers;
    List<TriangleMeshInfo> UnusedTriangleInfos;
    List<TriangleMeshInfo> UsedTriangleInfos;

    public void UpdateCardinalUVMapsForAllUnusedTriangleInfos(Transform originObjectForUV)
    {
        foreach(TriangleMeshInfo info in UnusedTriangleInfos)
        {
            info.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: originObjectForUV);
        }
    }

    public void Setup(BaseGameObject linkedObject)
    {
        MeshManagers = new List<SmartMeshManager>();
        UnusedTriangleInfos = new List<TriangleMeshInfo>();
        UsedTriangleInfos = new List<TriangleMeshInfo>();
        this.linkedObject = linkedObject;
    }

    public void Reset()
    {
        DestroyAllMeshManagers();

        UsedTriangleInfos.Clear();
    }

    void DestroyAllMeshManagers()
    {
        while (MeshManagers.Count > 0)
        {
            GameObject.Destroy(MeshManagers[0].gameObject);
            MeshManagers.RemoveAt(0);
        }   
    }

    public void AddTriangleInfo(TriangleMeshInfo newInfo)
    {
        if (newInfo.VerticesHolder.Count == 0) return;

        UnusedTriangleInfos.Add(newInfo);
    }

    public List<TriangleMeshInfo> AllTriangleInfosAsNewList
    {
        get
        {
            List<TriangleMeshInfo> returnList = new List<TriangleMeshInfo>(UsedTriangleInfos);

            return returnList;
        }
    }


    //RebuildBlockMeshes does not work properly
    
    public void RebuildBlockMeshes() //Does not work properly
    {
        DestroyAllMeshManagers();

        List<ManagedMaterialHelper> managedHelpers = new List<ManagedMaterialHelper>();
        List<UnmanagedMaterialHelper> unmanagedHelpers = new List<UnmanagedMaterialHelper>();
        List<TriangleMeshInfo> otherMeshes = new List<TriangleMeshInfo>();

        foreach (TriangleMeshInfo info in UsedTriangleInfos)
        {
            if (!info.IsValid)
            {
                Debug.LogWarning("Error");
                continue;
            }

            bool found = false;

            if (info.MaterialReference != null)
            {
                foreach (ManagedMaterialHelper helper in managedHelpers)
                {
                    if (helper.material == info.MaterialReference)
                    {
                        helper.info.Add(info);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    managedHelpers.Add(new ManagedMaterialHelper(material: info.MaterialReference, info: info));
                }
            }
            else if (info.AlternativeMaterial != null)
            {
                foreach (UnmanagedMaterialHelper helper in unmanagedHelpers)
                {
                    
                    if (helper.material == info.AlternativeMaterial)
                    {
                        helper.info.Add(info.Clone);
                        found = true;
                        break;
                        
                    }
                }

                if (!found)
                {

                    unmanagedHelpers.Add(new UnmanagedMaterialHelper(material: info.AlternativeMaterial, info: info.Clone));
                    
                }
            }
            else
            {
                otherMeshes.Add(info);
            }

        }

        foreach (ManagedMaterialHelper helper in managedHelpers)
        {
            SmartMeshManager newManager = UnityPrefabLibrary.NewMeshManager;

            newManager.Setup(linkedObject);

            MeshManagers.Add(newManager);

            newManager.transform.parent = transform;
            UnityHelper.ResetLocalTransform(newManager.transform);

            newManager.SetTriangleInfo(helper.info);
        }

        foreach (UnmanagedMaterialHelper helper in unmanagedHelpers)
        {
            SmartMeshManager newManager = UnityPrefabLibrary.NewMeshManager;

            newManager.Setup(linkedObject);

            MeshManagers.Add(newManager);

            newManager.transform.parent = transform;
            UnityHelper.ResetLocalTransform(newManager.transform);

            newManager.SetTriangleInfo(helper.info);
        }

        foreach (TriangleMeshInfo info in otherMeshes)
        {
            SmartMeshManager newManager = UnityPrefabLibrary.NewMeshManager;

            newManager.Setup(linkedObject);

            MeshManagers.Add(newManager);

            newManager.transform.parent = transform;
            UnityHelper.ResetLocalTransform(newManager.transform);

            newManager.SetTriangleInfo(info);
        }
    }
    

    public void BuildMeshes()
    {
        Reset();

        List<ManagedMaterialHelper> managedHelpers = new List<ManagedMaterialHelper>();
        List<UnmanagedMaterialHelper> unmanagedHelpers = new List<UnmanagedMaterialHelper>();
        List<TriangleMeshInfo> otherMeshes = new List<TriangleMeshInfo>();

        foreach (TriangleMeshInfo info in UnusedTriangleInfos)
        {
            if (!info.IsValid)
            {
                Debug.LogWarning("Error: Mesh is not valid. Error added to object of type " + linkedObject.IdentifierString);
                continue;
            }

            bool found = false;

            if(info.MaterialReference != null)
            {
                foreach (ManagedMaterialHelper helper in managedHelpers)
                {
                    if (helper.material == info.MaterialReference)
                    {
                        helper.info.Add(info);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    managedHelpers.Add(new ManagedMaterialHelper(material: info.MaterialReference, info: info.Clone));
                }
            }
            else if(info.AlternativeMaterial != null)
            {
                foreach(UnmanagedMaterialHelper helper in unmanagedHelpers)
                {
                    if (helper.material == info.AlternativeMaterial)
                    {
                        helper.info.Add(info);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    unmanagedHelpers.Add(new UnmanagedMaterialHelper(material: info.AlternativeMaterial, info: info.Clone));
                }
            }
            else
            {
                otherMeshes.Add(info);
            }

            UsedTriangleInfos.Add(info);
        }

        foreach(ManagedMaterialHelper helper in managedHelpers)
        {
            SmartMeshManager newManager = UnityPrefabLibrary.NewMeshManager;

            newManager.Setup(linkedObject);

            MeshManagers.Add(newManager);

            newManager.transform.parent = transform;
            UnityHelper.ResetLocalTransform(newManager.transform);

            newManager.SetTriangleInfo(helper.info);
        }

        foreach (UnmanagedMaterialHelper helper in unmanagedHelpers)
        {
            SmartMeshManager newManager = UnityPrefabLibrary.NewMeshManager;

            newManager.Setup(linkedObject);

            MeshManagers.Add(newManager);

            newManager.transform.parent = transform;
            UnityHelper.ResetLocalTransform(newManager.transform);

            newManager.SetTriangleInfo(helper.info);
        }

        foreach(TriangleMeshInfo info in otherMeshes)
        {
            SmartMeshManager newManager = UnityPrefabLibrary.NewMeshManager;

            newManager.Setup(linkedObject);

            MeshManagers.Add(newManager);

            newManager.transform.parent = transform;
            UnityHelper.ResetLocalTransform(newManager.transform);

            newManager.SetTriangleInfo(info);
        }

        UnusedTriangleInfos.Clear();
    }

    class ManagedMaterialHelper
    {
        public MailboxLineMaterial material;
        public TriangleMeshInfo info;

        public ManagedMaterialHelper(MailboxLineMaterial material, TriangleMeshInfo info)
        {
            this.material = material;
            this.info = info;
        }
    }

    class UnmanagedMaterialHelper
    {
        public Material material;
        public TriangleMeshInfo info;

        public UnmanagedMaterialHelper(Material material, TriangleMeshInfo info)
        {
            this.material = material;
            this.info = info;
        }
    }
}
