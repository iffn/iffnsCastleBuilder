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

        public ValueContainer SetBuildParameters(Door linkedDoor, Transform UVBaseObject, float borderWidth, float completeWidth, float doorHeight, float completeHeight, float borderDepth, float betweenDepth, MailboxLineMaterial frontMaterial, MailboxLineMaterial backMaterial)
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

        public override ValueContainer ApplyBuildParameters(Transform UVBaseObject)
        {
            ValueContainer returnValue = new ValueContainer(originTransform: transform, targetTransform: LinkedDoorController.transform);

            TriangleMeshInfo FrontWall = new TriangleMeshInfo();
            TriangleMeshInfo BackWall = new TriangleMeshInfo();
            TriangleMeshInfo topWrap = new TriangleMeshInfo();
            TriangleMeshInfo leftWraper = new TriangleMeshInfo();
            TriangleMeshInfo rightWraper = new TriangleMeshInfo();
            TriangleMeshInfo FrameFront = new TriangleMeshInfo();
            TriangleMeshInfo FrameBack = new TriangleMeshInfo();
            TriangleMeshInfo FrameBorder = new TriangleMeshInfo();

            void FinishMesh()
            {
                FrontWall.MaterialReference = FrontMaterial;
                topWrap.MaterialReference = FrontMaterial;
                leftWraper.MaterialReference = FrontMaterial;
                rightWraper.MaterialReference = FrontMaterial;
                BackWall.MaterialReference = BackMaterial;
                FrameFront.MaterialReference = FrameMaterial;
                FrameBack.MaterialReference = FrameMaterial;
                FrameBorder.MaterialReference = FrameMaterial;

                FrameFront.GenerateUVMeshBasedOnCardinalDirections(meshObject: LinkedDoorController.transform, originObjectForUV: UVBaseObject);
                FrameBack.GenerateUVMeshBasedOnCardinalDirections(meshObject: LinkedDoorController.transform, originObjectForUV: UVBaseObject);

                returnValue.AddStaticMeshAndConvertToTarget(FrontWall);
                returnValue.AddStaticMeshAndConvertToTarget(BackWall);
                returnValue.AddStaticMeshAndConvertToTarget(topWrap);
                returnValue.AddStaticMeshAndConvertToTarget(leftWraper);
                returnValue.AddStaticMeshAndConvertToTarget(rightWraper);
                returnValue.AddStaticMeshAndConvertToTarget(FrameFront);
                returnValue.AddStaticMeshAndConvertToTarget(FrameBack);
                returnValue.AddStaticMeshAndConvertToTarget(FrameBorder);
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

            FrameFront = MeshGenerator.MeshesFromLines.KnitLines(firstLine: outerFrameBorder, secondLine: innerFrameBorder, closingType: MeshGenerator.ShapeClosingType.open, smoothTransition: false);
            FrameBack = FrameFront.CloneFlipped;
            FrameBack.Move(completeDepth * Vector3.left);

            FrameBorder = MeshGenerator.MeshesFromLines.ExtrudeLinear(firstLine: combinedFrameBoder, offset: (betweenDepth + borderDepth * 2) * Vector3.left, closeType: MeshGenerator.ShapeClosingType.closedWithSharpEdge, smoothTransition: false);

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

                FrontWall.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: UVBaseObject);
                BackWall.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: UVBaseObject);

                topWrap.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: UVBaseObject);
                leftWraper.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: UVBaseObject);
                rightWraper.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: UVBaseObject);
            }

            FinishMesh();

            return returnValue;
        }
    }
}