using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class NodeGridWallOrganizer : ModificationOrganizer
    {
        readonly NodeGridPositionModificationNode firstNode;
        readonly NodeGridPositionModificationNode secondNode;
        readonly OnFloorObject linkedObject;

        float BlockSize
        {
            get
            {
                return linkedObject.LinkedFloor.LinkedBuildingController.BlockSize;
            }
        }

        Vector2Int NodeOffsetSizeAbsolute
        {
            get
            {
                return secondNode.AbsoluteCoordinate - firstNode.AbsoluteCoordinate; //Add absolute 1
            }
        }

        public GridOrientation Orientation
        {
            get
            {
                Vector2 parentOrientation = ParentOrientationSize;

                if (parentOrientation.x == 0 && parentOrientation.y == 0)
                {
                    return new GridOrientation(quarterOrientation: GridOrientation.GridQuarterOrientations.XPosZPos);
                }
                else if (parentOrientation.x > 0 && parentOrientation.y >= 0)
                {
                    return new GridOrientation(quarterOrientation: GridOrientation.GridQuarterOrientations.XPosZPos);
                }
                else if (parentOrientation.x <= 0 && parentOrientation.y > 0)
                {
                    return new GridOrientation(quarterOrientation: GridOrientation.GridQuarterOrientations.XNegZPos);
                }
                else if (parentOrientation.x >= 0 && parentOrientation.y < 0)
                {
                    return new GridOrientation(quarterOrientation: GridOrientation.GridQuarterOrientations.XPosZNeg);
                }
                else //if (parentOrientation.x < 0 && parentOrientation.y < 0)
                {
                    return new GridOrientation(quarterOrientation: GridOrientation.GridQuarterOrientations.XNegZNeg);
                }
            }
        }

        public Vector2Int ParentOrientationGridSize
        {
            get
            {
                Vector2Int offset = new Vector2Int(secondNode.AbsoluteCoordinate.x - firstNode.AbsoluteCoordinate.x, secondNode.AbsoluteCoordinate.y - firstNode.AbsoluteCoordinate.y);

                Vector2Int returnVector = new Vector2Int(offset.x, offset.y);

                return returnVector;
            }
        }

        public Vector2 ParentOrientationSize
        {
            get
            {
                Vector2Int size = ParentOrientationGridSize;

                return new Vector2(size.x, size.y) * BlockSize;
            }
        }

        public float ObjectOrientationSize
        {
            get
            {
                float size = Mathf.Sqrt(ParentOrientationSize.x * ParentOrientationSize.x + ParentOrientationSize.y * ParentOrientationSize.y);

                return size;
            }
        }

        public float BaseFloorHeightBasedOnFirstNode
        {
            get
            {
                return linkedObject.LinkedFloor.BaseHeightOfBlock(firstBlockLocation);
            }
        }

        public float BetweenFloorHeightBasedOnFirstNode
        {
            get
            {
                return linkedObject.LinkedFloor.BetweenFloorHeightOfBlock(firstBlockLocation);
            }
        }

        Vector2Int firstBlockLocation
        {
            get
            {
                Vector2Int block = firstNode.AbsoluteCoordinate;

                switch (Orientation.QuarterOrientation)
                {
                    case GridOrientation.GridQuarterOrientations.XPosZPos:
                        block += Vector2Int.zero;
                        break;
                    case GridOrientation.GridQuarterOrientations.XPosZNeg:
                        block += Vector2Int.down;
                        break;
                    case GridOrientation.GridQuarterOrientations.XNegZNeg:
                        block -= Vector2Int.one;
                        break;
                    case GridOrientation.GridQuarterOrientations.XNegZPos:
                        block += Vector2Int.left;
                        break;
                    default:
                        Debug.LogWarning("Error: Orientation not defined");
                        break;
                }

                return block;
            }
        }

        public override Vector3 FirstBuildPositionAbsolute
        {
            set
            {
                firstNode.AbsoluteCoordinate = linkedObject.LinkedFloor.GetNodeCoordinateFromPositionAbsolute(value);
            }
        }

        public override Vector3 SecondBuildPositionAbsolute
        {
            set
            {
                secondNode.AbsoluteCoordinate = linkedObject.LinkedFloor.GetNodeCoordinateFromPositionAbsolute(value);
            }
        }

        public override void SetLinkedObjectPositionAndOrientation(bool raiseToFloor)
        {
            //Position
            GridOrientation orientation = Orientation;

            bool firstOnGrid = linkedObject.LinkedFloor.LinkedBuildingController.NodeCoordinateIsOnGrid(firstNode.AbsoluteCoordinate);
            bool secondOnGrid = linkedObject.LinkedFloor.LinkedBuildingController.NodeCoordinateIsOnGrid(secondNode.AbsoluteCoordinate);

            if (!(firstOnGrid && secondOnGrid))
            {
                Debug.Log("Destroying object since no longer on the grid");

                Debug.Log("First node" + firstNode.AbsoluteCoordinate);
                Debug.Log("Second node" + secondNode.AbsoluteCoordinate);
                Debug.Log("GridSize" + linkedObject.LinkedFloor.LinkedBuildingController.GridSize);

                linkedObject.DestroyObject();
                return;
            }

            linkedObject.transform.localPosition = (linkedObject.LinkedFloor.NodePositionFromBlockIndex(blockIndex: firstNode.AbsoluteCoordinate) + linkedObject.LinkedFloor.NodePositionFromBlockIndex(blockIndex: secondNode.AbsoluteCoordinate)) * 0.5f;
            if (raiseToFloor) linkedObject.transform.localPosition += Vector3.up * BaseFloorHeightBasedOnFirstNode;

            //Rotation
            float angle = Mathf.Atan2(ParentOrientationSize.x, ParentOrientationSize.y) * Mathf.Rad2Deg + 90;

            linkedObject.transform.localRotation = Quaternion.Euler(Vector3.up * angle);
        }

        public NodeGridWallOrganizer(OnFloorObject linkedObject, NodeGridPositionModificationNode firstNode, NodeGridPositionModificationNode secondNode)
        {
            this.linkedObject = linkedObject;
            this.firstNode = firstNode;
            this.secondNode = secondNode;

            linkedObject.HideModificationNodes();
        }

        public override void MoveOnGrid(Vector2Int offset)
        {
            Vector2Int firstNodePosition = firstNode.AbsoluteCoordinate + offset;
            Vector2Int secondNodePosition = secondNode.AbsoluteCoordinate + offset;

            Vector2Int gridSize = linkedObject.LinkedFloor.LinkedBuildingController.GridSize;

            if (firstNodePosition.x < 0 || firstNodePosition.y < 0 || secondNodePosition.x < 0 || secondNodePosition.y < 0
                || firstNodePosition.x >= gridSize.x || firstNodePosition.y >= gridSize.y || secondNodePosition.x >= gridSize.x || secondNodePosition.y >= gridSize.y)
            {
                linkedObject.DestroyObject();
                return;
            }

            firstNode.AbsoluteCoordinate = firstNodePosition;
            secondNode.AbsoluteCoordinate = secondNodePosition;
        }
    }
}