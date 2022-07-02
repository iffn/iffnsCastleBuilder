using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class HollowedPreviewBlock : MonoBehaviour
    {
        [SerializeField] MeshFilter LinkedMeshFilter;

        Mesh currentMesh;

        public void SetCardinalPreviewBlock(VirtualBlock firstBlock, VirtualBlock secondBlock, CastleController currentBuilding)
        {
            transform.position = currentBuilding.CurrentFloorObject.transform.position;
            transform.rotation = currentBuilding.CurrentFloorObject.transform.rotation;

            float wallHeight = currentBuilding.CurrentFloorObject.CompleteFloorHeight;

            TriangleMeshInfo MeshInfo = new();

            Vector3 firstCenterPosition = currentBuilding.CurrentFloorObject.CenterPositionFromBlockIndex(firstBlock.Coordinate);
            Vector3 secondCenterPosition = currentBuilding.CurrentFloorObject.CenterPositionFromBlockIndex(secondBlock.Coordinate);

            float xMin, xMax, zMin, zMax;
            
            if (firstCenterPosition.x > secondCenterPosition.x)
            {
                xMax = firstCenterPosition.x;
                xMin = secondCenterPosition.x;
            }
            else
            {
                xMax = secondCenterPosition.x;
                xMin = firstCenterPosition.x;
            }

            if (firstCenterPosition.z > secondCenterPosition.z)
            {
                zMax = firstCenterPosition.z;
                zMin = secondCenterPosition.z;
            }
            else
            {
                zMax = secondCenterPosition.z;
                zMin = firstCenterPosition.z;
            }

            float halfBlockSize = currentBuilding.BlockSize * 0.5f + MathHelper.SmallFloat;

            List<Vector3> outerPoints = new()
            {
                new Vector3(xMax + halfBlockSize, 0, zMax + halfBlockSize),
                new Vector3(xMin - halfBlockSize, 0, zMax + halfBlockSize),
                new Vector3(xMin - halfBlockSize, 0, zMin - halfBlockSize),
                new Vector3(xMax + halfBlockSize, 0, zMin - halfBlockSize)
            };

            List<Vector3> innerPoints = new()
            {
                new Vector3(xMax - halfBlockSize, 0, zMax - halfBlockSize),
                new Vector3(xMin + halfBlockSize, 0, zMax - halfBlockSize),
                new Vector3(xMin + halfBlockSize, 0, zMin + halfBlockSize),
                new Vector3(xMax - halfBlockSize, 0, zMin + halfBlockSize)
            };

            List<Vector3> innerPointsReversed = new()
            {
                new Vector3(xMax - halfBlockSize, 0, zMax - halfBlockSize),
                new Vector3(xMax - halfBlockSize, 0, zMin + halfBlockSize),
                new Vector3(xMin + halfBlockSize, 0, zMin + halfBlockSize),
                new Vector3(xMin + halfBlockSize, 0, zMax - halfBlockSize)
            };


            MeshInfo.Add(MeshGenerator.MeshesFromLines.AddVerticalWallsBetweenMultiplePoints(floorPointsInClockwiseOrder: outerPoints, height: wallHeight, closed: true, uvOffset: Vector3.zero));
            MeshInfo.Add(MeshGenerator.MeshesFromLines.AddVerticalWallsBetweenMultiplePoints(floorPointsInClockwiseOrder: innerPointsReversed, height: wallHeight, closed: true, uvOffset: Vector3.zero));
            
            VerticesHolder outerPointHolder = new(outerPoints);
            VerticesHolder innerPointHolder = new(innerPoints);

            TriangleMeshInfo capBottom = MeshGenerator.MeshesFromLines.KnitLines(firstLine: outerPointHolder, secondLine: innerPointHolder, closingType: MeshGenerator.ShapeClosingType.closedWithSmoothEdge, smoothTransition: true);
            TriangleMeshInfo capTop = capBottom.CloneFlipped;

            capTop.Move((wallHeight + MathHelper.SmallFloat) * Vector3.up);
            capBottom.Move(MathHelper.SmallFloat * Vector3.down);

            MeshInfo.Add(capTop);
            MeshInfo.Add(capBottom);
            

            //Build mesh

            if(currentMesh == null) currentMesh = MeshPoolManager.GetMesh();

            LinkedMeshFilter.sharedMesh = currentMesh;

            List<Vector3> vertices = MeshInfo.AllVerticesDirectly;

            if (vertices.Count > 65535)
            {
                //Avoid vertex limit
                //https://answers.unity.com/questions/471639/mesh-with-more-than-65000-vertices.html
                currentMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            }

            currentMesh.vertices = vertices.ToArray();
            currentMesh.triangles = MeshInfo.AllTrianglesDirectly.ToArray();

            currentMesh.RecalculateNormals();
            currentMesh.RecalculateTangents();
            currentMesh.RecalculateBounds();
        }

        public void Clear()
        {
            MeshPoolManager.ReturnMeshToQueue(currentMesh);
            currentMesh = null;
            Destroy(transform.gameObject);
        }
    }
}