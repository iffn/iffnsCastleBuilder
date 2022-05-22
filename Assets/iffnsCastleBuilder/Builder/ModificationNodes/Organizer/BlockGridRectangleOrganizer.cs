using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class BlockGridRectangleOrganizer : ModificationOrganizer
    {
        readonly BlockGridPositionModificationNode firstNode;
        readonly BlockGridPositionModificationNode secondNode;
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

                Vector2Int returnVector = new Vector2Int(offset.x + System.Math.Sign(offset.x), offset.y + System.Math.Sign(offset.y));

                if (returnVector.x == 0) returnVector.x = 1;
                if (returnVector.y == 0) returnVector.y = 1;

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

        public Vector2Int ObjectOrientationGridSize
        {
            get
            {
                Vector2Int parentOrientationSize = ParentOrientationGridSize;

                switch (Orientation.QuarterOrientation)
                {
                    case GridOrientation.GridQuarterOrientations.XPosZPos:
                        return new Vector2Int(parentOrientationSize.x, parentOrientationSize.y);

                    case GridOrientation.GridQuarterOrientations.XPosZNeg:
                        return new Vector2Int(-parentOrientationSize.y, parentOrientationSize.x);

                    case GridOrientation.GridQuarterOrientations.XNegZNeg:
                        return new Vector2Int(-parentOrientationSize.x, -parentOrientationSize.y);

                    case GridOrientation.GridQuarterOrientations.XNegZPos:
                        return new Vector2Int(parentOrientationSize.y, -parentOrientationSize.x);
                    default:
                        Debug.LogWarning("Error: Grid qarter orientation not defined");
                        return ParentOrientationGridSize;
                }
            }
        }

        public Vector2 ObjectOrientationSize
        {
            get
            {
                Vector2Int size = ObjectOrientationGridSize;

                return new Vector2(size.x, size.y) * BlockSize;
            }
        }

        public override Vector3 FirstBuildPositionAbsolute
        {
            set
            {
                firstNode.AbsoluteCoordinate = linkedObject.LinkedFloor.GetBlockCoordinateFromCoordinateAbsolute(value);
            }
        }

        public override Vector3 SecondBuildPositionAbsolute
        {
            set
            {
                secondNode.AbsoluteCoordinate = linkedObject.LinkedFloor.GetBlockCoordinateFromCoordinateAbsolute(value);
            }
        }

        public bool RaiseDueToFloor
        {
            get
            {
                if (linkedObject.LinkedFloor.BlockAtPosition(firstNode.AbsoluteCoordinate).BlockType == VirtualBlock.BlockTypes.Floor) return true;
                return false;
            }
        }

        public override void SetLinkedObjectPositionAndOrientation(bool raiseToFloor)
        {
            //Position
            GridOrientation orientation = Orientation;

            bool firstOnGrid = linkedObject.LinkedFloor.LinkedBuildingController.BlockCoordinateIsOnGrid(firstNode.AbsoluteCoordinate);
            bool secondOnGrid = linkedObject.LinkedFloor.LinkedBuildingController.BlockCoordinateIsOnGrid(secondNode.AbsoluteCoordinate);

            if (!(firstOnGrid && secondOnGrid))
            {
                linkedObject.failed = true;
                return;
            }

            linkedObject.transform.localPosition = linkedObject.LinkedFloor.NodePositionFromBlockIndex(blockIndex: firstNode.AbsoluteCoordinate, orientation: orientation);
            if (raiseToFloor) linkedObject.transform.localPosition += Vector3.up * linkedObject.LinkedFloor.BaseHeightOfBlock(firstNode.AbsoluteCoordinate);

            //Rotation
            switch (orientation.ForwardOrientation)
            {
                case GridOrientation.GridForwardOrientations.XPositive:
                    linkedObject.transform.localRotation = Quaternion.Euler(Vector3.up * 90);
                    break;
                case GridOrientation.GridForwardOrientations.XNegative:
                    linkedObject.transform.localRotation = Quaternion.Euler(Vector3.up * 270);
                    break;
                case GridOrientation.GridForwardOrientations.ZPositive:
                    linkedObject.transform.localRotation = Quaternion.identity;
                    break;
                case GridOrientation.GridForwardOrientations.ZNegative:
                    linkedObject.transform.localRotation = Quaternion.Euler(Vector3.up * 180);
                    break;
                default:
                    Debug.LogWarning("Error: Orientation not defined");
                    linkedObject.transform.localRotation = Quaternion.identity;
                    break;
            }
        }

        public BlockGridRectangleOrganizer(OnFloorObject linkedObject, BlockGridPositionModificationNode firstNode, BlockGridPositionModificationNode secondNode)
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

        public void RotateClockwise()
        {
            GridOrientation.GridQuarterOrientations currentOrientation = Orientation.QuarterOrientation;

            if (currentOrientation == GridOrientation.GridQuarterOrientations.XPosZPos || currentOrientation == GridOrientation.GridQuarterOrientations.XNegZNeg)
            {
                FlipVertical();
            }
            else
            {
                FlipHorizontal();
            }
        }

        public void RotateCounterClockwise()
        {
            GridOrientation.GridQuarterOrientations currentOrientation = Orientation.QuarterOrientation;

            if (currentOrientation == GridOrientation.GridQuarterOrientations.XPosZPos || currentOrientation == GridOrientation.GridQuarterOrientations.XNegZNeg)
            {
                FlipHorizontal();
            }
            else
            {
                FlipVertical();
            }
        }

        public void FlipHorizontal()
        {
            Vector2Int oldFirstPosition = new Vector2Int(firstNode.AbsoluteCoordinate.x, firstNode.AbsoluteCoordinate.y);
            Vector2Int oldSecondPosition = new Vector2Int(secondNode.AbsoluteCoordinate.x, secondNode.AbsoluteCoordinate.y);

            firstNode.AbsoluteCoordinate = new Vector2Int(oldSecondPosition.x, oldFirstPosition.y);
            secondNode.AbsoluteCoordinate = new Vector2Int(oldFirstPosition.x, oldSecondPosition.y);

            linkedObject.ApplyBuildParameters();
        }

        public void FlipVertical()
        {
            Vector2Int oldFirstPosition = new Vector2Int(firstNode.AbsoluteCoordinate.x, firstNode.AbsoluteCoordinate.y);
            Vector2Int oldSecondPosition = new Vector2Int(secondNode.AbsoluteCoordinate.x, secondNode.AbsoluteCoordinate.y);

            firstNode.AbsoluteCoordinate = new Vector2Int(oldFirstPosition.x, oldSecondPosition.y);
            secondNode.AbsoluteCoordinate = new Vector2Int(oldSecondPosition.x, oldFirstPosition.y);

            linkedObject.ApplyBuildParameters();
        }

        public void FlipDiagonally()
        {
            Vector2Int oldFirstPosition = new Vector2Int(firstNode.AbsoluteCoordinate.x, firstNode.AbsoluteCoordinate.y);
            Vector2Int oldSecondPosition = new Vector2Int(secondNode.AbsoluteCoordinate.x, secondNode.AbsoluteCoordinate.y);

            firstNode.AbsoluteCoordinate = new Vector2Int(oldSecondPosition.x, oldSecondPosition.y);
            secondNode.AbsoluteCoordinate = new Vector2Int(oldFirstPosition.x, oldFirstPosition.y);

            linkedObject.ApplyBuildParameters();
        }
    }
}