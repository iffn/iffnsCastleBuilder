using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class RectangularBaseDoor : AssistObjectManager
    {
        public float borderWidth;
        public float completeWidth;
        public float doorHeight;
        public float completeHeight;
        public float borderDepth;
        public float betweenDepth;

        [SerializeField] GameObject Door;

        [HideInInspector] public Door LinkedDoorController;

        public MailboxLineMaterial FrontMaterial;
        public MailboxLineMaterial BackMaterial;
        public MailboxLineMaterial FrameMaterial;

        public void Setup(Door linkedDoor)
        {
            this.LinkedDoorController = linkedDoor;
        }

        public List<TriangleMeshInfo> SetBuildParameters(Door linkedDoor, Transform UVBaseObject, float borderWidth, float completeWidth, float doorHeight, float completeHeight, float borderDepth, float betweenDepth, MailboxLineMaterial frontMaterial, MailboxLineMaterial backMaterial)
        {
            this.borderWidth = borderWidth;
            this.completeWidth = completeWidth;
            this.doorHeight = doorHeight;
            this.completeHeight = completeHeight;
            this.borderDepth = borderDepth;
            this.betweenDepth = betweenDepth;

            FrontMaterial = frontMaterial;
            BackMaterial = backMaterial;

            LinkedDoorController = linkedDoor;

            return ApplyBuildParameters(UVBaseObject: UVBaseObject);
        }

        public override List<TriangleMeshInfo> ApplyBuildParameters(Transform UVBaseObject)
        {
            List<TriangleMeshInfo> returnValue = new();

            TriangleMeshInfo FrontWall = new(planar: true);
            TriangleMeshInfo BackWall = new(planar: true);
            TriangleMeshInfo topWrap = new(planar: true);
            TriangleMeshInfo leftWraper = new(planar: true);
            TriangleMeshInfo rightWraper = new(planar: true);
            TriangleMeshInfo FrameFront = new(planar: true);
            TriangleMeshInfo FrameBack = new(planar: true);
            List<TriangleMeshInfo> FrameBorder = new();

            void FinishMesh()
            {
                FrontWall.MaterialReference = FrontMaterial;
                topWrap.MaterialReference = FrontMaterial;
                leftWraper.MaterialReference = FrontMaterial;
                rightWraper.MaterialReference = FrontMaterial;
                BackWall.MaterialReference = BackMaterial;
                FrameFront.MaterialReference = FrameMaterial;
                FrameBack.MaterialReference = FrameMaterial;

                foreach(TriangleMeshInfo info in FrameBorder)
                {
                    info.MaterialReference = FrameMaterial;
                }

                returnValue.Add(FrontWall);
                returnValue.Add(BackWall);
                returnValue.Add(topWrap);
                returnValue.Add(leftWraper);
                returnValue.Add(rightWraper);
                returnValue.Add(FrameFront);
                returnValue.Add(FrameBack);
                returnValue.AddRange(FrameBorder);

                foreach(TriangleMeshInfo info in returnValue)
                {
                    info.Transorm(origin: transform, target: LinkedDoorController.transform);
                }
            }

            //Frame
            float completeDepth = betweenDepth + borderDepth * 2;

            VerticesHolder outerFrameBorder = new VerticesHolder();
            outerFrameBorder.Add(new Vector3(0, 0, -0.5f));
            outerFrameBorder.Add(new Vector3(0, 1, -0.5f));
            outerFrameBorder.Add(new Vector3(0, 1, 0.5f));
            outerFrameBorder.Add(new Vector3(0, 0, 0.5f));

            outerFrameBorder.Move((completeDepth - borderDepth) * Vector3.right);

            VerticesHolder innerFrameBorder = outerFrameBorder.Clone;

            outerFrameBorder.Scale(new Vector3(1, doorHeight + borderWidth, completeWidth));
            innerFrameBorder.Scale(new Vector3(1, doorHeight, completeWidth - borderWidth * 2));

            
            outerFrameBorder.Move(completeWidth * 0.5f * Vector3.forward);
            innerFrameBorder.Move(completeWidth * 0.5f * Vector3.forward);
            

            VerticesHolder combinedFrameBoder = outerFrameBorder.CloneReversed;
            combinedFrameBoder.Add(innerFrameBorder);

            FrameFront = MeshGenerator.MeshesFromLines.KnitLinesSmooth(firstLine: outerFrameBorder, secondLine: innerFrameBorder, closingType: MeshGenerator.ShapeClosingType.open, planar: true);
            FrameBack = FrameFront.CloneFlipped;
            FrameBack.Move(completeDepth * Vector3.left);

            FrameBorder = MeshGenerator.MeshesFromLines.ExtrudeLinearWithSharpCorners(firstLine: combinedFrameBoder, offset: (betweenDepth + borderDepth * 2) * Vector3.left, closed: true);

            //Wall
            if (completeHeight > doorHeight)
            {
                //FrontWall.transform.localPosition = Vector3.back * betweenDepth / 2;

                float topHeight = completeHeight - doorHeight - borderWidth;

                FrontWall.Add(MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.forward * completeWidth, secondLine: Vector3.up * topHeight, uvOffset: UVBaseObject.transform.InverseTransformDirection(transform.position)));
                FrontWall.Move(Vector3.up * (completeHeight - topHeight));
                FrontWall.FlipTriangles();

                //Wrapper
                topWrap = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.right * betweenDepth, secondLine: Vector3.forward * completeWidth, uvOffset: Vector2.zero);
                topWrap.Move(Vector3.up * completeHeight);

                leftWraper = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.right * betweenDepth, secondLine: Vector3.down * topHeight, uvOffset: Vector2.zero);
                leftWraper.Move(Vector3.up * completeHeight);
                leftWraper.FlipTriangles();

                rightWraper = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.right * betweenDepth, secondLine: Vector3.down * topHeight, uvOffset: Vector2.zero);
                rightWraper.Move(Vector3.up * completeHeight + Vector3.forward * completeWidth);

                BackWall.Add(FrontWall.CloneFlipped);
                BackWall.Move(Vector3.right * betweenDepth);
            }

            FinishMesh();

            return returnValue;
        }
    }
}