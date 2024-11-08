﻿using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class NodeWallSystem : BaseGameObject
    {
        List<NodeWallNode> CylinderNodes = new List<NodeWallNode>();

        FloorController linkedFloor;
        bool showModificationNodes;
        float halfWallthickness = 0.05f;
        List<NodeWallModificationNode> modificationNodes = new List<NodeWallModificationNode>();
        List<List<NodeWallNode>> NodeMatrix;

        public bool IgnoreApplyBuildParameters = false;

        //List<NodeWall> NodeWalls;

        //Build parameters
        MailboxLineMultipleSubObject nodeWallsParam;

        readonly List<DummyNodeWall> dummyNodeWalls = new();
        public List<DummyNodeWall> DummyNodeWalls
        {
            get
            {
                return dummyNodeWalls;
            }
        }

        public Vector2Int NodeGridSize
        {
            get
            {
                return linkedFloor.LinkedBuildingController.NodeGridSize;
            }
        }

        public float HalfWallThickness
        {
            get
            {
                return halfWallthickness;
            }
        }

        public float WallThickness
        {
            get
            {
                return halfWallthickness * 2;
            }
        }

        float wallHeight
        {
            get
            {
                return linkedFloor.WallHeightWithScaler + Random.Range(0.00001f, 0.0001f);
            }
        }

        public FloorController LinkedFloor
        {
            get
            {
                return linkedFloor;
            }
        }

        public override void ResetObject()
        {
            NodeMatrix.Clear();
            nodeWallsParam.ClearAndDestroySubObjects();
            //NodeWalls.Clear();
        }

        void SetupNodeMatrix()
        {
            NodeMatrix.Clear();

            for (int xPos = 0; xPos < linkedFloor.LinkedBuildingController.NodeGridSize.x; xPos++)
            {
                List<NodeWallNode> xLine = new List<NodeWallNode>();

                NodeMatrix.Add(xLine);

                for (int zPos = 0; zPos < linkedFloor.LinkedBuildingController.NodeGridSize.y; zPos++)
                {
                    NodeWallNode currentNode = new NodeWallNode(indexPosition: new Vector2Int(xPos, zPos), linkedNodeWallSystem: this);

                    xLine.Add(currentNode);
                }
            }
        }

        public bool setupWasRun = false;

        public override void Setup(IBaseObject superObject)
        {
            if (setupWasRun)
            {
                Debug.Log("Why run again?");
                return;
            }

            base.Setup(superObject: superObject);

            linkedFloor = superObject as FloorController;

            nodeWallsParam = new MailboxLineMultipleSubObject("Node walls", CurrentMailbox);

            NodeMatrix = new List<List<NodeWallNode>>();
        }

        public NodeWallNode NodeFromCoordinate(Vector2Int indexPosition)
        {
            Vector2Int position = new Vector2Int();

            position.x = MathHelper.ClampInt(value: indexPosition.x, max: linkedFloor.LinkedBuildingController.NodeGridSize.x - 1, min: 0);
            position.y = MathHelper.ClampInt(value: indexPosition.y, max: linkedFloor.LinkedBuildingController.NodeGridSize.y - 1, min: 0);

            return NodeMatrix[position.x][position.y];
        }

        public void AddNodeWall(NodeWall wall)
        {
            if (nodeWallsParam.Contains(wall)) return;

            nodeWallsParam.AddObject(wall);
            //NodeWalls.Add(wall);
        }

        public void RemoveNodeWall(NodeWall wall)
        {
            if (!nodeWallsParam.Contains(wall)) return;

            nodeWallsParam.TryRemoveSubObject(wall);

            //NodeWalls.Remove(wall);

            ApplyBuildParameters();
        }

        public void MoveAllNodeWalls(Vector2Int offset)
        {
            for (int i = 0; i < nodeWallsParam.SubObjects.Count; i++)
            {
                NodeWall wall = nodeWallsParam.SubObjects[i] as NodeWall;

                wall.Move(offset);

                if (wall.Failed)
                {
                    wall.DestroyObject();
                    i--;
                }
            }

            ApplyBuildParameters();
        }

        void UpdateReferences()
        {
            SetupNodeMatrix();

            foreach (IBaseObject wallObject in nodeWallsParam.SubObjects)
            {
                if (wallObject is not NodeWall wall)
                {
                    Debug.LogWarning("Error. Object of type " + wallObject.IdentifierString + " is not a NodeWall");
                    continue;
                }

                Vector2Int startPosition = wall.StartPosition;
                Vector2Int endPosition = wall.EndPosition;

                if (startPosition == endPosition)
                {
                    Debug.LogWarning("Error with node wall: Start and end position are the same");
                    continue;
                }

                NodeFromCoordinate(startPosition).EndPoints.Add(wall);
                NodeFromCoordinate(endPosition).EndPoints.Add(wall);

                Vector2Int offset = new Vector2Int(endPosition.x - startPosition.x, endPosition.y - startPosition.y);
                int greatestCommonFactor = MathHelper.GreatestCommonFactor(offset.x, offset.y);
                offset /= greatestCommonFactor;

                wall.NodesFromStartToEnd.Add(NodeFromCoordinate(startPosition));

                for (int i = 1; i < greatestCommonFactor; i++)
                {
                    Vector2Int position = startPosition + offset * i;

                    NodeFromCoordinate(position).IntermediatePoints.Add(wall);

                    wall.NodesFromStartToEnd.Add(NodeFromCoordinate(position));
                }

                wall.NodesFromStartToEnd.Add(NodeFromCoordinate(endPosition));
            }

            foreach(DummyNodeWall wall in DummyNodeWalls)
            {
                NodeWallNode startNode = NodeFromCoordinate(wall.StartPosition);
                NodeWallNode endNode = NodeFromCoordinate(wall.EndPosition);

                startNode.EndPoints.Add(wall);
                endNode.EndPoints.Add(wall);
            }
        }

        public void CreateNodeWall(Vector2Int startPosition, Vector2Int endPosition)
        {
            NodeWall currentWall = new NodeWall(linkedSystem: this, startPosition: startPosition, endPosition: endPosition);

            AddNodeWall(currentWall);

            ApplyBuildParameters();
        }

        public override void ApplyBuildParameters()
        {
            if (IgnoreApplyBuildParameters) return;

            DestroyInvalidNodeWalls();

            UpdateReferences();

            CylinderNodes.Clear();

            BuildWallsAndDefineCorners();

            BuildCorners();

            GeneratePlanarUVMaps(meshTransform: transform, refernceTransform: linkedFloor.LinkedBuildingController.transform);

            BuildAllMeshes();
        }

        void DestroyInvalidNodeWalls()
        {
            for (int i = 0; i < nodeWallsParam.SubObjects.Count; i++)
            {
                NodeWall currentWall = nodeWallsParam.SubObjects[i] as NodeWall;

                currentWall.EvaluateFailureState();

                if (currentWall.Failed)
                {
                    currentWall.DestroyObject();

                    Debug.Log("Removed failed node wall");
                }
            }
        }

        void BuildWallsAndDefineCorners()
        {
            for (int i = 0; i < nodeWallsParam.SubObjects.Count; i++)
            {
                NodeWall currentWall = nodeWallsParam.SubObjects[i] as NodeWall;

                Vector2 direction = new Vector2(currentWall.EndPosition.x - currentWall.StartPosition.x, currentWall.EndPosition.y - currentWall.StartPosition.y).normalized;

                Vector2 normalDirection = new Vector2(direction.y, -direction.x);

                float startOvershoot = 0;
                float endOvershoot = 0;

                NodeWallNode startCorner = NodeMatrix[currentWall.StartPosition.x][currentWall.StartPosition.y];
                NodeWallNode endCorner = NodeMatrix[currentWall.EndPosition.x][currentWall.EndPosition.y];

                int startCorners = startCorner.EndPoints.Count;
                int endCorners = endCorner.EndPoints.Count;

                if(startCorners < 1)
                {
                    Debug.Log("Error: No corner at start corner. Assignment failed?");
                }
                if (startCorners == 1)
                {
                    startOvershoot = -MathHelper.SmallFloat;
                }
                else if (startCorners == 2)
                {
                    Vector2Int firstWall = startCorner.EndPoints[0].Offset;
                    Vector2Int secondWall = startCorner.EndPoints[1].Offset;

                    if (MathHelper.FloatIsZero(Vector2.Dot(firstWall, secondWall))) //Dot product of 2 vectors = 0 if they are at a 90° angle
                    {
                        startOvershoot = halfWallthickness - MathHelper.SmallFloat;
                    }
                    else
                    {
                        //Checking if walls are alligned
                        float firstWallAngle = Mathf.Atan2(firstWall.y, firstWall.x);
                        float secondWallAngle = Mathf.Atan2(secondWall.y, secondWall.x);

                        if (!MathHelper.FloatIsZero(firstWallAngle - secondWallAngle) && !MathHelper.FloatIsZero(firstWallAngle - secondWallAngle + Mathf.PI) && !MathHelper.FloatIsZero(firstWallAngle - secondWallAngle - Mathf.PI))
                        {
                            if (!CylinderNodes.Contains(startCorner)) CylinderNodes.Add(startCorner);
                        }
                    }
                }
                else
                {
                    if (!CylinderNodes.Contains(startCorner)) CylinderNodes.Add(startCorner);
                }

                if (endCorners < 1)
                {
                    Debug.Log("Error: No corner at start corner. Assignment failed?");
                }
                if (endCorners == 1)
                {
                    endOvershoot = -MathHelper.SmallFloat;
                }
                else if (endCorners == 2)
                {
                    //Check angle
                    Vector2Int firstWall = endCorner.EndPoints[0].Offset;
                    Vector2Int secondWall = endCorner.EndPoints[1].Offset;

                    if (MathHelper.FloatIsZero(Vector2.Dot(firstWall, secondWall))) //Dot product of 2 vectors = 0 if they are at a 90° angle
                    {
                        endOvershoot = halfWallthickness - MathHelper.SmallFloat;
                    }
                    else
                    {
                        float alignmentMagnitude = 1;
                        if (firstWall.x != 0 && secondWall.x != 0)
                        {
                            alignmentMagnitude = (float)firstWall.y / firstWall.x - (float)secondWall.y / secondWall.x;
                        }
                        else if (firstWall.y != 0 && secondWall.y != 0)
                        {
                            alignmentMagnitude = (float)firstWall.x / firstWall.y - (float)secondWall.x / secondWall.y;
                        }

                        if (!MathHelper.FloatIsZero(alignmentMagnitude))
                        {
                            if (!CylinderNodes.Contains(endCorner)) CylinderNodes.Add(endCorner);
                        }
                    }
                }
                else
                {
                    if (!CylinderNodes.Contains(endCorner)) CylinderNodes.Add(endCorner);
                }

                List<Vector2> vertexPoints2D = new List<Vector2>()
                {
                    NodeFromCoordinate(currentWall.StartPosition).LocalPosition2D - normalDirection * halfWallthickness - direction * startOvershoot,
                    NodeFromCoordinate(currentWall.StartPosition).LocalPosition2D + normalDirection * halfWallthickness - direction * startOvershoot,
                    NodeFromCoordinate(currentWall.EndPosition).LocalPosition2D + normalDirection * halfWallthickness + direction * endOvershoot,
                    NodeFromCoordinate(currentWall.EndPosition).LocalPosition2D - normalDirection * halfWallthickness + direction * endOvershoot
                };

                List<Vector3> vertexPoints3D = new List<Vector3>();

                for (int j = 0; j < vertexPoints2D.Count; j++)
                {
                    vertexPoints3D.Add(new Vector3(vertexPoints2D[j].x, 0, vertexPoints2D[j].y));
                }

                VerticesHolder baseLine = new VerticesHolder();

                baseLine.Add(vertexPoints3D);

                //Walls.Add(MeshGenerator.MeshesFromLines.Extrude(firstLine: baseLine, offset: Vector3.up * wallHeight, isClosed: true, isSealed: false, smoothTransition: true));

                Vector3 offset = linkedFloor.transform.localPosition + new Vector3(currentWall.StartPosition.x, 0, currentWall.StartPosition.y) * linkedFloor.BlockSize;


                /*
                TriangleMeshInfo firstWallInfo = MeshGenerator.MeshesFromLines.AddVerticalWallsBetweenMultiplePoints(floorPointsInClockwiseOrder: vertexPoints3D,
                    closed: false, height: wallHeight, offset: offset);
                firstWallInfo.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObject: linkedFloor.LinkedBuildingController.transform);
                firstWallInfo.MaterialReference = currentWall.RightMaterialParam;
                */

                TriangleMeshInfo startCap = MeshGenerator.MeshesFromLines.AddWallBetween2Points(firstClockwiseFloorPoint: vertexPoints3D[0], secondClockwiseFloorPoint: vertexPoints3D[1], wallHeight: wallHeight, uvOffset: offset);
                startCap.MaterialReference = currentWall.RightMaterialParam;

                TriangleMeshInfo firstWallInfo = MeshGenerator.MeshesFromLines.AddWallBetween2Points(firstClockwiseFloorPoint: vertexPoints3D[1], secondClockwiseFloorPoint: vertexPoints3D[2], wallHeight: wallHeight, uvOffset: offset);
                firstWallInfo.MaterialReference = currentWall.RightMaterialParam;

                TriangleMeshInfo endCap = MeshGenerator.MeshesFromLines.AddWallBetween2Points(firstClockwiseFloorPoint: vertexPoints3D[2], secondClockwiseFloorPoint: vertexPoints3D[3], wallHeight: wallHeight, uvOffset: offset);
                endCap.MaterialReference = currentWall.RightMaterialParam;

                TriangleMeshInfo secondWallInfo = MeshGenerator.MeshesFromLines.AddWallBetween2Points(firstClockwiseFloorPoint: vertexPoints3D[3], secondClockwiseFloorPoint: vertexPoints3D[0], wallHeight: wallHeight, uvOffset: offset);
                secondWallInfo.MaterialReference = currentWall.LeftMaterialParam;


                TriangleMeshInfo capBottom = MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(points: vertexPoints3D, planar: true);
                capBottom.Move(MathHelper.SmallFloat * Vector3.up);
                TriangleMeshInfo capTop = capBottom.CloneFlipped;

                capTop.MaterialReference = currentWall.RightMaterialParam;
                capBottom.MaterialReference = currentWall.RightMaterialParam;

                capTop.Move(Vector3.up * wallHeight);

                StaticMeshManager.AddTriangleInfoIfValid(firstWallInfo);
                StaticMeshManager.AddTriangleInfoIfValid(secondWallInfo);

                StaticMeshManager.AddTriangleInfoIfValid(startCap);
                StaticMeshManager.AddTriangleInfoIfValid(endCap);

                StaticMeshManager.AddTriangleInfoIfValid(capTop);
                StaticMeshManager.AddTriangleInfoIfValid(capBottom);
            }

            //Add Define dummy only corners
            foreach (DummyNodeWall wall in DummyNodeWalls)
            {
                NodeWallNode startCorner = NodeFromCoordinate(wall.StartPosition);
                NodeWallNode endCorner = NodeFromCoordinate(wall.EndPosition);

                if (startCorner.EndPoints.Count > 1 && startCorner.AllWallsAreDummy)
                {
                    if(startCorner.EndPoints.Count == 2)
                    {
                        //Checking if walls are alligned
                        Vector2Int firstWall = startCorner.EndPoints[0].Offset;
                        Vector2Int secondWall = startCorner.EndPoints[1].Offset;

                        float firstWallAngle = Mathf.Atan2(firstWall.y, firstWall.x);
                        float secondWallAngle = Mathf.Atan2(secondWall.y, secondWall.x);

                        if (!MathHelper.FloatIsZero(firstWallAngle - secondWallAngle) && !MathHelper.FloatIsZero(firstWallAngle - secondWallAngle + Mathf.PI) && !MathHelper.FloatIsZero(firstWallAngle - secondWallAngle - Mathf.PI))
                        {
                            if (!CylinderNodes.Contains(startCorner)) CylinderNodes.Add(startCorner);
                        }
                    }
                    else
                    {
                        if (!CylinderNodes.Contains(endCorner)) CylinderNodes.Add(endCorner);
                    }
                }

                if (endCorner.EndPoints.Count > 1 && endCorner.AllWallsAreDummy)
                {
                    if (endCorner.EndPoints.Count == 2)
                    {
                        //Checking if walls are alligned
                        Vector2Int firstWall = endCorner.EndPoints[0].Offset;
                        Vector2Int secondWall = endCorner.EndPoints[1].Offset;

                        float firstWallAngle = Mathf.Atan2(firstWall.y, firstWall.x);
                        float secondWallAngle = Mathf.Atan2(secondWall.y, secondWall.x);

                        if (!MathHelper.FloatIsZero(firstWallAngle - secondWallAngle) && !MathHelper.FloatIsZero(firstWallAngle - secondWallAngle + Mathf.PI) && !MathHelper.FloatIsZero(firstWallAngle - secondWallAngle - Mathf.PI))
                        {
                            if (!CylinderNodes.Contains(endCorner)) CylinderNodes.Add(endCorner);
                        }
                    }
                    else
                    {
                        if (!CylinderNodes.Contains(endCorner)) CylinderNodes.Add(endCorner);
                    }
                }
            }
        }

        void BuildCorners()
        {
            foreach (NodeWallNode node in CylinderNodes)
            {
                Vector3 nodePosition = node.LocalPosition3D + 0.5f * wallHeight * Vector3.up;

                //Wall
                TriangleMeshInfo cornerWall = MeshGenerator.FilledShapes.CylinderAroundCenterWithoutCap(radius: halfWallthickness, length: wallHeight, direction: Vector3.up, numberOfEdges: 24);

                cornerWall.Move(nodePosition);

                cornerWall.MaterialReference = node.EndPoints[0].CornerMaterial;

                StaticMeshManager.AddTriangleInfoIfValid(cornerWall);

                //Cap
                List<TriangleMeshInfo> capInfo = MeshGenerator.FilledShapes.CylinderCaps(radius: halfWallthickness, length: wallHeight - 2 * MathHelper.SmallFloat, direction: Vector3.up, numberOfEdges: 24);

                foreach(TriangleMeshInfo info in capInfo)
                {
                    info.Move(nodePosition + MathHelper.SmallFloat * Vector3.up);

                    info.MaterialReference = node.EndPoints[0].CornerMaterial;

                    StaticMeshManager.AddTriangleInfoIfValid(info);
                }
            }
        }

        public override void ShowModificationNodes(bool activateCollider)
        {
            showModificationNodes = true;
            ResetModifactionNodePositions();
        }

        public override void HideModificationNodes()
        {
            showModificationNodes = false;
            ResetModifactionNodePositions();
        }

        public void UpdateModNodePositions()
        {
            foreach (ModificationNode node in modificationNodes)
            {
                node.UpdatePosition();
            }
        }

        void ResetModifactionNodePositions()
        {
            foreach (ModificationNode node in modificationNodes)
            {
                Destroy(node.gameObject);
            }

            modificationNodes.Clear();

            if (!showModificationNodes) return;

            //Edit and remove nodes
            foreach (IBaseObject baseWall in nodeWallsParam.SubObjects)
            {
                NodeWall wall = baseWall as NodeWall;

                //EditMod
                NodeWallEditModNode startNode = ModificationNodeLibrary.NewNodeWallEditModode;
                NodeWallEditModNode endNode = ModificationNodeLibrary.NewNodeWallEditModode;

                startNode.transform.parent = transform;
                endNode.transform.parent = transform;

                startNode.Setup(linkedWall: wall, positionType: NodeWallEditModNode.PositionTypes.Start);
                endNode.Setup(linkedWall: wall, positionType: NodeWallEditModNode.PositionTypes.End);

                modificationNodes.Add(startNode);
                modificationNodes.Add(endNode);

                //Flip
                NodeWallFlipNode flipNode = ModificationNodeLibrary.NewNodeWallFlipNode;
                flipNode.transform.parent = transform;
                flipNode.Setup(linkedWall: wall);
                modificationNodes.Add(flipNode);

                //Remove
                NodeWallRemoveNode remNode = ModificationNodeLibrary.NewNodeWallRemoveNode;
                remNode.transform.parent = transform;
                remNode.Setup(linkedWall: wall);
                modificationNodes.Add(remNode);
            }

            //Multi edit nodes
            foreach (List<NodeWallNode> nodeLine in NodeMatrix)
            {
                foreach (NodeWallNode node in nodeLine)
                {
                    if (node.NonDummyEndPoints.Count > 1)
                    {
                        NodeWallMultiModNode multiNode = ModificationNodeLibrary.NewNodeWallMultiModNode;
                        multiNode.transform.parent = transform;
                        multiNode.Setup(linkedNode: node);
                        modificationNodes.Add(multiNode);
                    }
                }
            }

            foreach (ModificationNode node in modificationNodes)
            {
                node.UpdatePosition();
            }
        }

        public override void InternalUpdate()
        {
            NonOrderedInternalUpdate();
        }

        public override void PlaytimeUpdate()
        {
            NonOrderedPlaytimeUpdate();
        }
    }
}