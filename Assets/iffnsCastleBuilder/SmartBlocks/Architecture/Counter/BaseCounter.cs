using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static iffnsStuff.iffnsBaseSystemForUnity.BaseGameObject;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class BaseCounter : AssistObjectManager
    {
        BaseGameObject mainObject;

        public float width = 2f / 3f;
        public float length = 1f;
        public float Indent = 0.05f;
        public float topHeight = 0.1f;
        public float totalHeight = 0.8f;
        Counter.ShapeTypes shapeType;

        public MailboxLineMaterial baseMaterial;
        public MailboxLineMaterial topMaterial;

        public void Setup(BaseGameObject mainObject)
        {
            this.mainObject = mainObject;
        }

        public ValueContainer SetBuildParameters(BaseGameObject mainObject, Transform UVBaseObject, float width, float length, float totalHeight, Counter.ShapeTypes shapeType)
        {
            this.width = width;
            this.length = length;
            this.totalHeight = totalHeight;
            this.shapeType = shapeType;

            return ApplyBuildParameters(UVBaseObject: UVBaseObject);
        }

        public override ValueContainer ApplyBuildParameters(Transform UVBaseObject)
        {
            switch (shapeType)
            {
                case Counter.ShapeTypes.BaseBox:
                    return GetBaseBox();
                case Counter.ShapeTypes.FlatTop:
                    return GetFlatTop();
                case Counter.ShapeTypes.RoofTop:
                    return GetRoofTop();
                default:
                    Debug.LogWarning($"Error: Counter shape {shapeType} not defined during mesh creation");
                    break;
            }

            return new ValueContainer(originTransform: transform, targetTransform: mainObject.transform);
        }

        public ValueContainer GetBaseBox()
        {
            ValueContainer returnValue = new ValueContainer(originTransform: transform, targetTransform: mainObject.transform);

            TriangleMeshInfo baseMesh;
            void FinishMesh()
            {
                baseMesh.MaterialReference = baseMaterial;

                returnValue.AddStaticMeshAndConvertToTarget(baseMesh);
            }

            baseMesh = MeshGenerator.FilledShapes.BoxAroundCenter(size: new Vector3(
                width,
                totalHeight,
                length
                ));
            baseMesh.Move(offset: new Vector3(
                width / 2,
                (totalHeight) / 2,
                length / 2
                ));

            FinishMesh();

            return returnValue;
        }

        public ValueContainer GetFlatTop()
        {
            ValueContainer returnValue = new ValueContainer(originTransform: transform, targetTransform: mainObject.transform);

            TriangleMeshInfo baseMesh;
            TriangleMeshInfo topMesh;

            void FinishMesh()
            {
                baseMesh.MaterialReference = baseMaterial;
                topMesh.MaterialReference = topMaterial;

                returnValue.AddStaticMeshAndConvertToTarget(baseMesh);
                returnValue.AddStaticMeshAndConvertToTarget(topMesh);
            }

            baseMesh = MeshGenerator.FilledShapes.BoxAroundCenter(size: new Vector3(
                width - Indent * 2,
                totalHeight - topHeight,
                length - Indent * 2
                ));
            baseMesh.Move(offset: new Vector3(
                width / 2,
                (totalHeight - topHeight) / 2,
                length / 2
                ));

            topMesh = MeshGenerator.FilledShapes.BoxAroundCenter(size: new Vector3(
                width - MathHelper.SmallFloat,
                topHeight,
                length - MathHelper.SmallFloat
                ));
            topMesh.Move(offset: new Vector3(
                width / 2,
                totalHeight - topHeight / 2,
                length / 2
                ));

            FinishMesh();

            return returnValue;
        }

        public ValueContainer GetRoofTop()
        {
            ValueContainer returnValue = new ValueContainer(originTransform: transform, targetTransform: mainObject.transform);

            float heightRatio = 0.66f;

            //Create base rectangle and shape
            VerticesHolder baseRectangle = new VerticesHolder(new List<Vector3>()
            {
                new Vector3(0, totalHeight * heightRatio, 0),
                new Vector3(0, 0, 0),
                new Vector3(width, 0, 0),
                new Vector3(width, totalHeight * heightRatio)
            }) ;

            Vector3 topPoint = new Vector3(width * 0.5f, totalHeight);
            Vector3 extrudeVector = length * Vector3.forward;

            //Cover faces
            TriangleMeshInfo coverFaceFront = MeshGenerator.MeshesFromLines.KnitLines(point: topPoint, line: baseRectangle, isClosed: false);
            coverFaceFront.MaterialReference = baseMaterial;
            TriangleMeshInfo coverFaceBack = coverFaceFront.CloneFlipped;
            coverFaceBack.Move(extrudeVector);

            //Base faces
            TriangleMeshInfo baseFaces = MeshGenerator.MeshesFromLines.ExtrudeLinear(firstLine: baseRectangle, offset: extrudeVector, closeType: MeshGenerator.ShapeClosingType.open, smoothTransition: false); 
            baseFaces.MaterialReference = baseMaterial;

            //Roof faces
            VerticesHolder roofOutline = new VerticesHolder();
            roofOutline.Add(baseRectangle.VerticesDirectly[3]);
            roofOutline.Add(topPoint);
            roofOutline.Add(baseRectangle.VerticesDirectly[0]);
            TriangleMeshInfo roofShape = MeshGenerator.MeshesFromLines.ExtrudeLinear(firstLine: roofOutline, offset: extrudeVector, closeType: MeshGenerator.ShapeClosingType.open, smoothTransition: false);
            roofShape.MaterialReference = topMaterial;

            returnValue.AddStaticMeshAndConvertToTarget(coverFaceFront);
            returnValue.AddStaticMeshAndConvertToTarget(coverFaceBack);
            returnValue.AddStaticMeshAndConvertToTarget(baseFaces);
            returnValue.AddStaticMeshAndConvertToTarget(roofShape);

            return returnValue;
        }
    }
}