using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCounter : MonoBehaviour
{
    [SerializeField] GameObject Top;
    [SerializeField] GameObject Base;
    [SerializeField] UnityMeshManager TopMesh;
    [SerializeField] UnityMeshManager BaseMesh;

    BaseGameObject mainObject;
    
    public float Width = 2f/3f;
    public float Length = 1f;
    public float Indent = 0.05f;
    public float topHeight = 0.1f;
    public float totalHeight = 0.8f;

    public List<UnityMeshManager> UnmanagedStaticMeshes
    {
        get
        {
            List<UnityMeshManager> returnList = new List<UnityMeshManager>()
            {
                TopMesh,
                BaseMesh
            };

            return returnList;
        }
    }

    public void Setup(BaseGameObject mainObject, MailboxLineMaterial baseMaterial, MailboxLineMaterial topMaterial)
    {
        this.mainObject = mainObject;
        TopMesh.Setup(mainObject: mainObject, currentMaterialReference: topMaterial);
        BaseMesh.Setup(mainObject: mainObject, currentMaterialReference: baseMaterial);
    }

    public void SetMainParameters(float width, float length)
    {
        this.Width = width;
        this.Length = length;

        ApplyBuildParameters();
    }

    public void ApplyBuildParameters()
    {
        Base.transform.localPosition = new Vector3(Length / 2, (totalHeight - topHeight) / 2, Width / 2);
        Base.transform.localScale = new Vector3(Length - Indent * 2, totalHeight - topHeight, Width - Indent * 2);

        Top.transform.localPosition = new Vector3(Length / 2, totalHeight - topHeight / 2, Width / 2);
        Top.transform.localScale = new Vector3(Length, topHeight, Width);

        TopMesh.UpdateMaterial();
        BaseMesh.UpdateMaterial();
    }

    private void Update()
    {
        ApplyBuildParameters();
    }
}
