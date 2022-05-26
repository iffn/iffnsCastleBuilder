using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class RectangularBaseWindow : AssistObjectManager
    {
        public float borderWidth = 0.1f;
        public float completeWidth = 1;
        public float windowHeight = 1;
        public float bottomHeight = 0.75f;
        public float completeHeight = 2.2f;
        public float borderDepth = 0.03f;
        public float betweenDepth = 1 / 3;
        public float glassThickness = 0.03f;

        [HideInInspector] public Window LinkedWindowController;

        public MailboxLineMaterial FrontMaterial;
        public MailboxLineMaterial BackMaterial;
        public MailboxLineMaterial FrameMaterial;
        public MailboxLineMaterial GlassMaterial;

        public void Setup(Window linkedWindow)
        {
            this.LinkedWindowController = linkedWindow;
        }

        public ValueContainer SetBuildParameters(Window linkedWindow, float borderWidth, float completeWidth, float windowHeight, float completeHeight, float bottomHeight, float borderDepth, float betweenDepth, Transform UVBaseObject)
        {
            this.LinkedWindowController = linkedWindow;

            this.borderWidth = borderWidth;
            this.completeWidth = completeWidth;
            this.windowHeight = windowHeight;
            this.bottomHeight = bottomHeight;
            this.completeHeight = completeHeight;
            this.borderDepth = borderDepth;
            this.betweenDepth = betweenDepth;

            return ApplyBuildParameters(UVBaseObject: UVBaseObject);
        }

        public override ValueContainer ApplyBuildParameters(Transform UVBaseObject)
        {
            ValueContainer returnValue = new ValueContainer(originTransform: transform, targetTransform: LinkedWindowController.transform);

            TriangleMeshInfo FrontTopWall = new TriangleMeshInfo();
            TriangleMeshInfo FrontBottomWall = new TriangleMeshInfo();
            TriangleMeshInfo BackTopWall = new TriangleMeshInfo();
            TriangleMeshInfo BackBottomkWall = new TriangleMeshInfo();
            TriangleMeshInfo bottomWraper = new TriangleMeshInfo();
            TriangleMeshInfo topWrapper = new TriangleMeshInfo();
            TriangleMeshInfo leftWrapper = new TriangleMeshInfo();
            TriangleMeshInfo rightWrapper = new TriangleMeshInfo();
            TriangleMeshInfo FrameFront = new TriangleMeshInfo();
            TriangleMeshInfo FrameBack = new TriangleMeshInfo();
            TriangleMeshInfo FrameBorder = new TriangleMeshInfo();
            TriangleMeshInfo Glass = new TriangleMeshInfo();


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
                FrameFront.MaterialReference = FrameMaterial;
                FrameBack.MaterialReference = FrameMaterial;
                FrameBorder.MaterialReference = FrameMaterial;
                Glass.MaterialReference = GlassMaterial;

                returnValue.AddStaticMeshAndConvertToTarget(FrontTopWall);
                returnValue.AddStaticMeshAndConvertToTarget(FrontBottomWall);
                returnValue.AddStaticMeshAndConvertToTarget(BackTopWall);
                returnValue.AddStaticMeshAndConvertToTarget(BackBottomkWall);
                returnValue.AddStaticMeshAndConvertToTarget(bottomWraper);
                returnValue.AddStaticMeshAndConvertToTarget(topWrapper);
                returnValue.AddStaticMeshAndConvertToTarget(leftWrapper);
                returnValue.AddStaticMeshAndConvertToTarget(rightWrapper);
                returnValue.AddStaticMeshAndConvertToTarget(FrameFront);
                returnValue.AddStaticMeshAndConvertToTarget(FrameBack);
                returnValue.AddStaticMeshAndConvertToTarget(FrameBorder);
                returnValue.AddStaticMeshAndConvertToTarget(Glass);
            }

            //Frame
            float completeDepth = betweenDepth + borderDepth * 2;

            VerticesHolder outerFrameBorder = new();
            outerFrameBorder.Add(new Vector3(0, 0, -0.5f));
            outerFrameBorder.Add(new Vector3(0, 1, -0.5f));
            outerFrameBorder.Add(new Vector3(0, 1, 0.5f));
            outerFrameBorder.Add(new Vector3(0, 0, 0.5f));

            outerFrameBorder.Move((completeDepth - borderDepth) * Vector3.right);

            VerticesHolder innerFrameBorder = outerFrameBorder.Clone;

            outerFrameBorder.Scale(new Vector3(1, windowHeight + borderWidth * 2, completeWidth));
            innerFrameBorder.Scale(new Vector3(1, windowHeight, completeWidth - borderWidth * 2));

            outerFrameBorder.Move(new Vector3(0, bottomHeight - borderWidth, completeWidth * 0.5f));
            innerFrameBorder.Move(new Vector3(0, bottomHeight, completeWidth * 0.5f));

            FrameFront = MeshGenerator.MeshesFromLines.KnitLines(firstLine: outerFrameBorder, secondLine: innerFrameBorder, closingType: MeshGenerator.ShapeClosingType.closedWithSharpEdge, smoothTransition: false);
            FrameBack = FrameFront.CloneFlipped;
            FrameBack.Move(completeDepth * Vector3.left);

            FrameBorder.Add(MeshGenerator.MeshesFromLines.ExtrudeLinear(firstLine: outerFrameBorder, offset: (betweenDepth + borderDepth * 2) * Vector3.left, closeType: MeshGenerator.ShapeClosingType.closedWithSharpEdge, smoothTransition: false));
            FrameBorder.FlipTriangles();
            FrameBorder.Add(MeshGenerator.MeshesFromLines.ExtrudeLinear(firstLine: innerFrameBorder, offset: (betweenDepth + borderDepth * 2) * Vector3.left, closeType: MeshGenerator.ShapeClosingType.closedWithSharpEdge, smoothTransition: false));

            //Glass
            Glass = MeshGenerator.FilledShapes.RectangleAroundCenter(baseLineFull: completeWidth * Vector3.forward, secondLineFull: windowHeight * Vector3.up);
            Glass.Move(new Vector3(betweenDepth * 0.5f + glassThickness * 0.5f, bottomHeight + windowHeight * 0.5f, completeWidth * 0.5f));
            TriangleMeshInfo otherGlassSide = Glass.CloneFlipped;
            otherGlassSide.Move(glassThickness * Vector3.left);
            Glass.Add(otherGlassSide);

            //Walls
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

            FrontTopWall.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: UVBaseObject);
            BackTopWall.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: UVBaseObject);
            FrontBottomWall.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: UVBaseObject);
            BackBottomkWall.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: UVBaseObject);

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

            bottomWraper.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: UVBaseObject);
            topWrapper.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: UVBaseObject);
            leftWrapper.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: UVBaseObject);
            rightWrapper.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: UVBaseObject);

            FinishMesh();

            return returnValue;
        }
    }
}