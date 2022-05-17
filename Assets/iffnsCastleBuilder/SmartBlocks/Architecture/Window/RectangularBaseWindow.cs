using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class RectangularBaseWindow : MonoBehaviour
    {
        public float borderWidth = 0.1f;
        public float completeWidth = 1;
        public float windowHeight = 1;
        public float bottomHeight = 0.75f;
        public float completeHeight = 2.2f;
        public float borderDepth = 0.03f;
        public float betweenDepth = 1 / 3;
        public float glassThickness = 0.03f;

        [SerializeField] GameObject TopBorderBlock;
        [SerializeField] GameObject LeftBorderBlock;
        [SerializeField] GameObject RightBorderBlock;
        [SerializeField] GameObject BottomBorderBlock;
        [SerializeField] GameObject Glass;
        [SerializeField] UnityMeshManager GlassMesh;

        public MailboxLineMaterial FrontMaterial;
        public MailboxLineMaterial BackMaterial;


        [SerializeField] List<UnityMeshManager> BorderMeshes;

        public List<UnityMeshManager> UnmanagedStaticMeshes
        {
            get
            {
                List<UnityMeshManager> returnList = new List<UnityMeshManager>(BorderMeshes);

                returnList.Add(GlassMesh);

                return returnList;
            }
        }

        public void Setup(BaseGameObject mainObject, MailboxLineMaterial frameMaterial, MailboxLineMaterial glassMaterial)
        {
            foreach (UnityMeshManager border in BorderMeshes)
            {
                border.Setup(mainObject: mainObject, currentMaterialReference: frameMaterial);
            }

            GlassMesh.Setup(mainObject: mainObject, currentMaterialReference: glassMaterial);
        }

        public void SetBuildParameters(Window linkedWindow, float borderWidth, float completeWidth, float windowHeight, float completeHeight, float bottomHeight, float borderDepth, float betweenDepth, Transform originObject)
        {
            this.borderWidth = borderWidth;
            this.completeWidth = completeWidth;
            this.windowHeight = windowHeight;
            this.bottomHeight = bottomHeight;
            this.completeHeight = completeHeight;
            this.borderDepth = borderDepth;
            this.betweenDepth = betweenDepth;

            ApplyBuildParameters(linkedWindow: linkedWindow, originObject: originObject);
        }

        public void ApplyBuildParameters(Window linkedWindow, Transform originObject)
        {
            TriangleMeshInfo FrontTopWall = new TriangleMeshInfo();
            TriangleMeshInfo FrontBottomWall = new TriangleMeshInfo();
            TriangleMeshInfo BackTopWall = new TriangleMeshInfo();
            TriangleMeshInfo BackBottomkWall = new TriangleMeshInfo();
            TriangleMeshInfo bottomWraper = new TriangleMeshInfo();
            TriangleMeshInfo topWrapper = new TriangleMeshInfo();
            TriangleMeshInfo leftWrapper = new TriangleMeshInfo();
            TriangleMeshInfo rightWrapper = new TriangleMeshInfo();

            void FinishMesh()
            {
                FrontTopWall.MaterialReference = FrontMaterial;
                FrontBottomWall.MaterialReference = FrontMaterial;
                BackTopWall.MaterialReference = BackMaterial;
                BackBottomkWall.MaterialReference = BackMaterial;
                topWrapper.MaterialReference = FrontMaterial;
                leftWrapper.MaterialReference = FrontMaterial;
                rightWrapper.MaterialReference = FrontMaterial;
                bottomWraper.MaterialReference = FrontMaterial;

                linkedWindow.AddStaticMesh(FrontTopWall);
                linkedWindow.AddStaticMesh(FrontBottomWall);
                linkedWindow.AddStaticMesh(BackTopWall);
                linkedWindow.AddStaticMesh(BackBottomkWall);
                linkedWindow.AddStaticMesh(bottomWraper);
                linkedWindow.AddStaticMesh(topWrapper);
                linkedWindow.AddStaticMesh(leftWrapper);
                linkedWindow.AddStaticMesh(rightWrapper);
            }

            TopBorderBlock.transform.localScale = new Vector3(
                betweenDepth + borderDepth * 2,
                borderWidth,
                completeWidth
                );

            TopBorderBlock.transform.localPosition = new Vector3(
                betweenDepth / 2,
                bottomHeight + windowHeight,
                completeWidth / 2 
                );


            LeftBorderBlock.transform.localScale = new Vector3(
                betweenDepth + borderDepth * 2,
                windowHeight,
                borderWidth
                );

            LeftBorderBlock.transform.localPosition = new Vector3(
                betweenDepth / 2,
                bottomHeight,
                0 
                );


            RightBorderBlock.transform.localScale = new Vector3(
                betweenDepth + borderDepth * 2,
                windowHeight,
                borderWidth 
                );

            RightBorderBlock.transform.localPosition = new Vector3(
                betweenDepth / 2,
                bottomHeight,
                completeWidth
                );


            BottomBorderBlock.transform.localScale = new Vector3(
                betweenDepth + borderDepth * 2,
                borderWidth,
                completeWidth
                );

            BottomBorderBlock.transform.localPosition = new Vector3(
                betweenDepth / 2,
                bottomHeight + MathHelper.SmallFloat,
                completeWidth / 2 
                );


            Glass.transform.localScale = new Vector3(
                glassThickness,
                windowHeight,
                completeWidth - borderWidth / 2
                );

            Glass.transform.localPosition = new Vector3(
                betweenDepth / 2,
                bottomHeight,
                completeWidth / 2
                );

            float topHeight = completeHeight - windowHeight - borderWidth - bottomHeight;

            FrontTopWall.Add(MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.forward * completeWidth, secondLine: Vector3.up * topHeight, UVOffset: Vector3.zero));
            FrontTopWall.FlipTriangles();
            FrontTopWall.Move(Vector3.up * (completeHeight - topHeight));

            BackTopWall.Add(FrontTopWall.CloneFlipped);

            FrontBottomWall.Add(MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.forward * completeWidth, secondLine: Vector3.up * (bottomHeight - borderWidth), UVOffset: Vector3.zero)); ;
            FrontBottomWall.FlipTriangles();
            BackBottomkWall.Add(FrontBottomWall.CloneFlipped);
            BackBottomkWall.Move(Vector3.right * betweenDepth);
            BackTopWall.Move(Vector3.right * betweenDepth);

            FrontTopWall.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: originObject);
            BackTopWall.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: originObject);
            FrontBottomWall.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: originObject);
            BackBottomkWall.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: originObject);


            //Wrapper

            bottomWraper = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.right * betweenDepth, secondLine: Vector3.forward * completeWidth, UVOffset: Vector2.zero);
            bottomWraper.FlipTriangles();

            topWrapper = bottomWraper.CloneFlipped;
            topWrapper.Move(Vector3.up * completeHeight);
            bottomWraper.Move(Vector3.up * MathHelper.SmallFloat);

            leftWrapper = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.right * betweenDepth, secondLine: Vector3.down * topHeight, UVOffset: Vector2.zero);
            leftWrapper.FlipTriangles();
            leftWrapper.Move(Vector3.up * completeHeight);
            TriangleMeshInfo leftBottomWrapper = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.right * betweenDepth, secondLine: Vector3.up * (bottomHeight - borderWidth), UVOffset: Vector2.zero);
            leftWrapper.Add(leftBottomWrapper);

            rightWrapper = leftWrapper.CloneFlipped;
            rightWrapper.Move(Vector3.forward * completeWidth);


            /*
            TriangleMeshInfo rightWrapper = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.forward * betweenDepth, secondLine: Vector3.down * topHeight, UVOffset: Vector2.zero);
            rightWrapper.Move(Vector3.up * completeHeight);
            rightWrapper.FlipTriangles();
            rightWrapper.Add(MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.forward * betweenDepth, secondLine: Vector3.up * (bottomHeight - borderWidth), UVOffset: Vector2.zero));
            rightWrapper.Move(Vector3.right * completeWidth);
            rightWrapper.GenerateUVMeshBasedOnCardinalDirections(meshObject: Wrapper.transform, originObject: originObject);
            */

            bottomWraper.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: originObject);
            topWrapper.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: originObject);
            leftWrapper.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: originObject);
            rightWrapper.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: originObject);

            FinishMesh();

            foreach (UnityMeshManager border in BorderMeshes)
            {
                border.UpdateMaterial();
            }
        }

        private void Update()
        {

        }
    }
}