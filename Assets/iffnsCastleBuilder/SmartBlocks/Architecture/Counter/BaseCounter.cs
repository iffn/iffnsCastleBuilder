using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

        public List<TriangleMeshInfo> SetBuildParameters(BaseGameObject mainObject, Transform UVBaseObject, float width, float length, float totalHeight, Counter.ShapeTypes shapeType)
        {
            this.width = width;
            this.length = length;
            this.totalHeight = totalHeight;
            this.shapeType = shapeType;

            return ApplyBuildParameters(UVBaseObject: UVBaseObject);
        }

        public override List<TriangleMeshInfo> ApplyBuildParameters(Transform UVBaseObject)
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

            return new List<TriangleMeshInfo>();
        }

        public List<TriangleMeshInfo> GetBaseBox()
        {
            //ValueContainer returnValue = new ValueContainer(originTransform: transform, targetTransform: mainObject.transform);
            List<TriangleMeshInfo> returnValue = new();

            returnValue = MeshGenerator.FilledShapes.BoxAroundCenter(size: new Vector3(
                width,
                totalHeight,
                length
                ));

            foreach (TriangleMeshInfo mesh in returnValue)
            {
                mesh.Move(offset: new Vector3(
                    width / 2,
                    (totalHeight) / 2,
                    length / 2
                ));

                mesh.MaterialReference = baseMaterial;

                mesh.Transorm(origin: transform, target: mainObject.transform);
            }

            return returnValue;
        }

        public List<TriangleMeshInfo> GetFlatTop()
        {
            List<TriangleMeshInfo> returnValue = new();

            List<TriangleMeshInfo> baseMesh;
            List<TriangleMeshInfo> topMesh;


            baseMesh = MeshGenerator.FilledShapes.BoxAroundCenter(size: new Vector3(
                width - Indent * 2,
                totalHeight - topHeight,
                length - Indent * 2
                ));

            foreach (TriangleMeshInfo mesh in baseMesh)
            {
                mesh.Move(offset: new Vector3(
                    width / 2,
                    (totalHeight - topHeight) / 2,
                    length / 2
                ));

                mesh.MaterialReference = baseMaterial;

                mesh.Transorm(origin: transform, target: mainObject.transform);
            }

            topMesh = MeshGenerator.FilledShapes.BoxAroundCenter(size: new Vector3(
                width - MathHelper.SmallFloat,
                topHeight,
                length - MathHelper.SmallFloat
                ));

            foreach (TriangleMeshInfo mesh in topMesh)
            {
                mesh.Move(offset: new Vector3(
                    width / 2,
                    totalHeight - topHeight / 2,
                    length / 2
                ));

                mesh.MaterialReference = topMaterial;

                mesh.Transorm(origin: transform, target: mainObject.transform);
            }

            returnValue.AddRange(baseMesh);
            returnValue.AddRange(topMesh);

            return returnValue;
        }

        public List<TriangleMeshInfo> GetRoofTop()
        {
            List<TriangleMeshInfo> returnValue = new();

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

            //House faces
            TriangleMeshInfo coverFaceFront = MeshGenerator.MeshesFromLines.KnitLinesSmooth(point: topPoint, line: baseRectangle, isClosed: false, planar: true);
            coverFaceFront.GenerateUVMeshBasedOnCardinalDirectionsWithoutReference();
            coverFaceFront.MaterialReference = baseMaterial;
            TriangleMeshInfo coverFaceBack = coverFaceFront.CloneFlipped;
            coverFaceBack.Move(extrudeVector);


            //Base faces
            List<TriangleMeshInfo> baseFaces = MeshGenerator.MeshesFromLines.ExtrudeLinearWithSharpCorners(firstLine: baseRectangle, offset: extrudeVector, closed: false);

            foreach(TriangleMeshInfo info in baseFaces)
            {
                info.MaterialReference = baseMaterial;
            }

            //Roof faces
            VerticesHolder roofOutline = new VerticesHolder();
            roofOutline.Add(baseRectangle.VerticesDirectly[3]);
            roofOutline.Add(topPoint);
            roofOutline.Add(baseRectangle.VerticesDirectly[0]);
            List<TriangleMeshInfo> roofShape = MeshGenerator.MeshesFromLines.ExtrudeLinearWithSharpCorners(firstLine: roofOutline, offset: extrudeVector, closed: false);
            
            foreach (TriangleMeshInfo info in roofShape)
            {
                info.MaterialReference = topMaterial;
            }

            returnValue.Add(coverFaceFront);
            returnValue.Add(coverFaceBack);
            returnValue.AddRange(baseFaces);
            returnValue.AddRange(roofShape);

            foreach(TriangleMeshInfo info in returnValue)
            {
                info.Transorm(origin: transform, target: mainObject.transform);
            }

            return returnValue;
        }
    }
}