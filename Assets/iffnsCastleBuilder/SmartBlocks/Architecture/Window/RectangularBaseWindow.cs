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

        public List<TriangleMeshInfo> SetBuildParameters(Window linkedWindow, float borderWidth, float completeWidth, float windowHeight, float completeHeight, float bottomHeight, float borderDepth, float betweenDepth, Transform UVBaseObject)
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

        public override List<TriangleMeshInfo> ApplyBuildParameters(Transform UVBaseObject)
        {
            List<TriangleMeshInfo> returnValue = new ();

            TriangleMeshInfo FrontTopWall = new(planar: true);
            TriangleMeshInfo FrontBottomWall = new(planar: true);
            TriangleMeshInfo BackTopWall = new(planar: true);
            TriangleMeshInfo BackBottomkWall = new(planar: true);
            TriangleMeshInfo bottomWraper = new(planar: true);
            TriangleMeshInfo topWrapper = new(planar: true);
            TriangleMeshInfo leftWrapper = new(planar: true);
            TriangleMeshInfo rightWrapper = new(planar: true);
            TriangleMeshInfo FrameFront = new(planar: true);
            TriangleMeshInfo FrameBack = new(planar: true);
            List<TriangleMeshInfo> FrameBorder = new();
            TriangleMeshInfo GlassFront = new(planar: true);
            TriangleMeshInfo GlassBack = new(planar: true);


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
                
                foreach(TriangleMeshInfo info in FrameBorder)
                {
                    info.MaterialReference = FrameMaterial;
                }
                
                GlassFront.MaterialReference = GlassMaterial;
                GlassBack.MaterialReference = GlassMaterial;

                GlassFront.ActiveCollider = TriangleMeshInfo.ColliderStates.SeeThroughCollider;
                GlassBack.ActiveCollider = TriangleMeshInfo.ColliderStates.SeeThroughCollider;

                returnValue.Add(FrontTopWall);
                returnValue.Add(FrontBottomWall);
                returnValue.Add(BackTopWall);
                returnValue.Add(BackBottomkWall);
                returnValue.Add(topWrapper);
                returnValue.Add(leftWrapper);
                returnValue.Add(rightWrapper);
                returnValue.Add(bottomWraper);
                returnValue.Add(FrameFront);
                returnValue.Add(FrameBack);
                returnValue.AddRange(FrameBorder);

                foreach (TriangleMeshInfo info in returnValue)
                {
                    info.Transorm(origin: transform, target: LinkedWindowController.transform);
                }
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

            FrameFront = MeshGenerator.MeshesFromLines.KnitLinesSmooth(firstLine: outerFrameBorder, secondLine: innerFrameBorder, closingType: MeshGenerator.ShapeClosingType.closedWithSmoothEdge, planar: true);
            //FrameFront.Move(new Vector3(0, 0, 0));
            FrameBack = FrameFront.CloneFlipped;
            FrameBack.Move(completeDepth * Vector3.left);

            FrameBorder.AddRange(MeshGenerator.MeshesFromLines.ExtrudeLinearWithSharpCorners(firstLine: outerFrameBorder, offset: (betweenDepth + borderDepth * 2) * Vector3.left, closed: true));
            foreach(TriangleMeshInfo info in FrameBorder)
            {
                info.FlipTriangles();
            }
            FrameBorder.AddRange(MeshGenerator.MeshesFromLines.ExtrudeLinearWithSharpCorners(firstLine: innerFrameBorder, offset: (betweenDepth + borderDepth * 2) * Vector3.left, closed: true));

            //Glass
            GlassFront = MeshGenerator.FilledShapes.RectangleAroundCenter(baseLineFull: completeWidth * Vector3.forward, secondLineFull: windowHeight * Vector3.up);
            GlassFront.Move(new Vector3(betweenDepth * 0.5f + glassThickness * 0.5f, bottomHeight + windowHeight * 0.5f, completeWidth * 0.5f));
            GlassBack = GlassFront.CloneFlipped;
            GlassBack.Move(glassThickness * Vector3.left);

            //Walls
            float topHeight = completeHeight - windowHeight - borderWidth - bottomHeight;

            FrontTopWall.Add(MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.forward * completeWidth, secondLine: Vector3.up * topHeight, uvOffset: Vector3.zero));
            FrontTopWall.FlipTriangles();
            FrontTopWall.Move(Vector3.up * (completeHeight - topHeight));

            BackTopWall.Add(FrontTopWall.CloneFlipped);

            FrontBottomWall.Add(MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.forward * completeWidth, secondLine: Vector3.up * (bottomHeight - borderWidth), uvOffset: Vector3.zero)); ;
            FrontBottomWall.FlipTriangles();
            BackBottomkWall.Add(FrontBottomWall.CloneFlipped);
            BackBottomkWall.Move(Vector3.right * betweenDepth);
            BackTopWall.Move(Vector3.right * betweenDepth);

            //Wrapper
            bottomWraper = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.right * betweenDepth, secondLine: Vector3.forward * completeWidth, uvOffset: Vector2.zero);
            bottomWraper.FlipTriangles();

            topWrapper = bottomWraper.CloneFlipped;
            topWrapper.Move(Vector3.up * completeHeight);
            bottomWraper.Move(Vector3.up * MathHelper.SmallFloat);

            leftWrapper = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.right * betweenDepth, secondLine: Vector3.down * topHeight, uvOffset: Vector2.zero);
            leftWrapper.FlipTriangles();
            leftWrapper.Move(Vector3.up * completeHeight);
            TriangleMeshInfo leftBottomWrapper = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.right * betweenDepth, secondLine: Vector3.up * (bottomHeight - borderWidth), uvOffset: Vector2.zero);
            leftWrapper.Add(leftBottomWrapper);

            rightWrapper = leftWrapper.CloneFlipped;
            rightWrapper.Move(Vector3.forward * completeWidth);

            FinishMesh();

            return returnValue;
        }
    }
}