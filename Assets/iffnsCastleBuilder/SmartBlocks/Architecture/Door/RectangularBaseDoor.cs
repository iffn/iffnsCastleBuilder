using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangularBaseDoor : MonoBehaviour
{
    public float borderWidth;
    public float completeWidth;
    public float doorHeight;
    public float completeHeight;
    public float borderDepth;
    public float betweenDepth;

    [SerializeField] GameObject TopBorderBlock;
    [SerializeField] GameObject LeftBorderBlock;
    [SerializeField] GameObject RightBorderBlock;

    [SerializeField] List<UnityMeshManager> BorderMeshes;


    [SerializeField] GameObject Door;

    public Door LinkedDoorController;

    public MailboxLineMaterial FrontMaterial;
    public MailboxLineMaterial BackMaterial;

    public Material FrameMaterail
    {
        set
        {
            TopBorderBlock.transform.GetChild(0).GetComponent<MeshRenderer>().material = value;
            LeftBorderBlock.transform.GetChild(0).GetComponent<MeshRenderer>().material = value;
            RightBorderBlock.transform.GetChild(0).GetComponent<MeshRenderer>().material = value;
        }
    }

    public List<UnityMeshManager> UnmanagedStaticMeshes
    {
        get
        {
            return new List<UnityMeshManager>(BorderMeshes);
        }
    }

    public void Setup(BaseGameObject mainObject, MailboxLineMaterial frameMaterial)
    {
        foreach(UnityMeshManager border in BorderMeshes)
        {
            border.Setup(mainObject: mainObject, currentMaterialReference: frameMaterial);
        }
    }

    public void SetBuildParameters(Door linkedDoor, float borderWidth, float completeWidth, float doorHeight, float completeHeight, float borderDepth, float betweenDepth, Transform UVBaseObject, MailboxLineMaterial frontMaterial, MailboxLineMaterial backMaterial)
    {
        this.borderWidth = borderWidth;
        this.completeWidth = completeWidth;
        this.doorHeight = doorHeight;
        this.completeHeight = completeHeight;
        this.borderDepth = borderDepth;
        this.betweenDepth = betweenDepth;

        this.FrontMaterial = frontMaterial;
        this.BackMaterial = backMaterial;

        this.LinkedDoorController = linkedDoor;

        ApplyBuildParameters(UVBaseObject: UVBaseObject);
    }

    public void ApplyBuildParameters(Transform UVBaseObject)
    {
        TriangleMeshInfo FrontWall = new TriangleMeshInfo();
        TriangleMeshInfo BackWall = new TriangleMeshInfo();
        TriangleMeshInfo topWrap = new TriangleMeshInfo();
        TriangleMeshInfo leftWraper = new TriangleMeshInfo();
        TriangleMeshInfo rightWraper = new TriangleMeshInfo();

        void FinishMesh()
        {
            FrontWall.MaterialReference = FrontMaterial;
            topWrap.MaterialReference = FrontMaterial;
            leftWraper.MaterialReference = FrontMaterial;
            rightWraper.MaterialReference = FrontMaterial;
            BackWall.MaterialReference = BackMaterial;

            LinkedDoorController.AddStaticMesh(FrontWall);
            LinkedDoorController.AddStaticMesh(BackWall);
            LinkedDoorController.AddStaticMesh(topWrap);
            LinkedDoorController.AddStaticMesh(leftWraper);
            LinkedDoorController.AddStaticMesh(rightWraper);
        }

        TopBorderBlock.transform.localScale = new Vector3(completeWidth, borderWidth, betweenDepth + borderDepth * 2);
        TopBorderBlock.transform.localPosition = new Vector3(completeWidth / 2, doorHeight, betweenDepth / 2);

        LeftBorderBlock.transform.localScale = new Vector3(borderWidth, doorHeight, betweenDepth + borderDepth * 2);
        LeftBorderBlock.transform.localPosition = new Vector3(0, MathHelper.SmallFloat, betweenDepth / 2);
        
        RightBorderBlock.transform.localScale = new Vector3(borderWidth, doorHeight, betweenDepth + borderDepth * 2);
        RightBorderBlock.transform.localPosition = new Vector3(completeWidth, MathHelper.SmallFloat, betweenDepth / 2);

        if (completeHeight > doorHeight)
        {
            //FrontWall.transform.localPosition = Vector3.back * betweenDepth / 2;
            
            float topHeight = completeHeight - doorHeight - borderWidth;

            FrontWall.Add(MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.right * completeWidth, secondLine: Vector3.up * topHeight, UVOffset: UVBaseObject.transform.InverseTransformDirection(transform.position)));
            FrontWall.Move(Vector3.up * (completeHeight - topHeight));

            BackWall.Add(FrontWall.CloneFlipped);

            //Wrapper
            topWrap = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.forward * betweenDepth, secondLine: Vector3.right * completeWidth, UVOffset: Vector2.zero);
            topWrap.Move(Vector3.up * completeHeight);
            topWrap.FlipTriangles();

            leftWraper = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.forward * betweenDepth, secondLine: Vector3.down * topHeight, UVOffset: Vector2.zero);
            leftWraper.Move(Vector3.up * completeHeight);

            rightWraper = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.forward * betweenDepth, secondLine: Vector3.down * topHeight, UVOffset: Vector2.zero);
            rightWraper.Move(Vector3.up * completeHeight + Vector3.right * completeWidth);
            rightWraper.FlipTriangles();

            BackWall.Move(Vector3.forward * betweenDepth);

            FrontWall.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: UVBaseObject);
            BackWall.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: UVBaseObject);

            topWrap.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: UVBaseObject);
            leftWraper.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: UVBaseObject);
            rightWraper.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: UVBaseObject);

            FinishMesh();
        }

        foreach (UnityMeshManager border in BorderMeshes)
        {
            border.UpdateMaterial();
        }
    }

    private void Update()
    {
        //ApplyBuildParameters();
    }
}
