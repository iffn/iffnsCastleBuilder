using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class NodeGridTriangleOrganizer : GridModificationOrganizer
    {
        readonly NodeGridPositionModificationNode firstNode;
        readonly NodeGridPositionModificationNode secondNode;
        readonly NodeGridPositionModificationNode thirdNode;
        readonly OnFloorObject linkedObject;

        float BlockSize
        {
            get
            {
                return linkedObject.LinkedFloor.LinkedBuildingController.BlockSize;
            }
        }

        public Vector2 FirstClockwiseOffsetPosition;
        public Vector2 SecondClockwiseOffsetPosition;

        public NodeGridTriangleOrganizer(OnFloorObject linkedObject, NodeGridPositionModificationNode firstNode, NodeGridPositionModificationNode secondNode, NodeGridPositionModificationNode thirdNode)
        {
            this.linkedObject = linkedObject;
            this.firstNode = firstNode;
            this.secondNode = secondNode;
            this.thirdNode = thirdNode;

            //linkedObject.HideModificationNodes();
        }

        public override void MoveOnGrid(Vector2Int offset)
        {
            firstNode.AbsoluteCoordinate += offset;
            secondNode.AbsoluteCoordinate += offset;
            thirdNode.AbsoluteCoordinate += offset;
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
                Vector2Int secondBuildCoordinate = linkedObject.LinkedFloor.GetNodeCoordinateFromPositionAbsolute(value);

                secondNode.AbsoluteCoordinate = new Vector2Int(secondBuildCoordinate.x, firstNode.AbsoluteCoordinate.y);
                thirdNode.AbsoluteCoordinate = new Vector2Int(firstNode.AbsoluteCoordinate.x, secondBuildCoordinate.y);
            }
        }

        public override void SetLinkedObjectPositionAndOrientation(bool raiseToFloor)
        {
            //Get coordinates from nodes
            Vector2Int firstCoordinate = firstNode.AbsoluteCoordinate;
            Vector2Int secondCoordinate = secondNode.AbsoluteCoordinate;
            Vector2Int thirdCoordinate = thirdNode.AbsoluteCoordinate;


            //Check for errors
            float area = MathHelper.AreaOf2DTriangle(firstCoordinate, secondCoordinate, thirdCoordinate);

            if (MathHelper.FloatIsZero(area))
            {
                linkedObject.Failed = true;
                return;
            }

            bool firstOnGrid = linkedObject.LinkedFloor.LinkedBuildingController.NodeCoordinateIsOnGrid(firstCoordinate);
            bool secondOnGrid = linkedObject.LinkedFloor.LinkedBuildingController.NodeCoordinateIsOnGrid(secondCoordinate);
            bool thirdOnGrid = linkedObject.LinkedFloor.LinkedBuildingController.NodeCoordinateIsOnGrid(thirdCoordinate);

            if (!(firstOnGrid && secondOnGrid && thirdNode && thirdOnGrid))
            {
                linkedObject.Failed = true;
                return;
            }

            //Calculate offsets
            Vector2Int secondRelativeOffset = secondCoordinate - firstCoordinate;
            Vector2Int thirdRelativeOffset = thirdCoordinate - firstCoordinate;

            float secondAngle = Mathf.Atan2(y: secondRelativeOffset.y, x: secondRelativeOffset.x) * Mathf.Rad2Deg;
            float thirdAngle = Mathf.Atan2(y: thirdRelativeOffset.y, x: thirdRelativeOffset.x) * Mathf.Rad2Deg;

            if (MathHelper.FloatIsZero(secondAngle - thirdAngle))
            {
                linkedObject.Failed = true;
                return;
            }

            if (secondAngle > thirdAngle)
            {
                thirdAngle += 360;
            }

            if (thirdAngle - secondAngle > 180)
            {
                FirstClockwiseOffsetPosition = secondRelativeOffset;
                SecondClockwiseOffsetPosition = thirdRelativeOffset;
            }
            else
            {
                FirstClockwiseOffsetPosition = thirdRelativeOffset;
                SecondClockwiseOffsetPosition = secondRelativeOffset;
            }

            FirstClockwiseOffsetPosition *= linkedObject.BlockSize;
            SecondClockwiseOffsetPosition *= linkedObject.BlockSize;

            //Set position
            linkedObject.transform.localPosition = linkedObject.LinkedFloor.NodePositionFromBlockIndex(blockIndex: firstCoordinate);

            if (raiseToFloor)
            {

                Vector2Int offset;

                if (secondAngle < 90)
                {
                    offset = new Vector2Int(0, 0);
                }
                else if (secondAngle < 180)
                {
                    offset = new Vector2Int(-1, 0);
                }
                else if (secondAngle < 270)
                {
                    offset = new Vector2Int(-1, -1);
                }
                else
                {
                    offset = new Vector2Int(0, -1);
                }

                float floorOffset = linkedObject.LinkedFloor.BaseHeightOfBlock(firstNode.AbsoluteCoordinate + offset);

                linkedObject.transform.localPosition += Vector3.up * floorOffset;
            }
        }
    }
}