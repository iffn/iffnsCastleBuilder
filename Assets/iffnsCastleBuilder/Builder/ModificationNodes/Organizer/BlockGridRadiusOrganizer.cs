using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using iffnsStuff.iffnsBaseSystemForUnity;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class BlockGridRadiusOrganizer : GridModificationOrganizer
    {
        readonly BlockGridPositionModificationNode positionNode;
        readonly RadiusModificationNode radiusNode;
        readonly OnFloorObject linkedObject;

        // Start is called before the first frame update
        public override Vector3 FirstBuildPositionAbsolute
        {
            set
            {
                positionNode.AbsoluteCoordinate = linkedObject.LinkedFloor.GetBlockCoordinateFromCoordinateAbsolute(value);
            }
        }

        public override Vector3 SecondBuildPositionAbsolute
        {
            set
            {
                radiusNode.AbsolutePosition = value;
            }
        }

        public BlockGridRadiusOrganizer(OnFloorObject linkedObject, BlockGridPositionModificationNode positionNode, RadiusModificationNode radiusNode)
        {
            this.linkedObject = linkedObject;
            this.positionNode = positionNode;
            this.radiusNode = radiusNode;

            //linkedObject.HideModificationNodes();
        }

        public override void SetLinkedObjectPositionAndOrientation(bool raiseToFloor)
        {
            bool firstOnGrid = linkedObject.LinkedFloor.LinkedBuildingController.BlockCoordinateIsOnGrid(positionNode.AbsoluteCoordinate);

            if (!(firstOnGrid))
            {
                Debug.Log("Destroying object since no longer on the grid");

                Debug.Log("First node" + positionNode.AbsoluteCoordinate);
                Debug.Log("GridSize" + linkedObject.LinkedFloor.LinkedBuildingController.GridSize);

                linkedObject.DestroyObject();
                return;
            }

            linkedObject.transform.localPosition = linkedObject.LinkedFloor.NodePositionFromBlockIndex(blockIndex: positionNode.AbsoluteCoordinate);

            if (raiseToFloor) linkedObject.transform.localPosition += Vector3.up * linkedObject.LinkedFloor.BottomFloorHeight;
            //if (raiseToFloor) linkedObject.transform.localPosition += Vector3.up * linkedObject.LinkedFloor.BaseHeightOfBlock(positionNode.AbsoluteCoordinate);
        }

        public bool RaiseDueToFloor
        {
            get
            {
                if (linkedObject.LinkedFloor.BlockAtPosition(positionNode.AbsoluteCoordinate).BlockType == VirtualBlock.BlockTypes.Floor) return true;
                return false;
            }
        }

        public override void MoveOnGrid(Vector2Int offset)
        {
            Vector2Int firstNodePosition = positionNode.AbsoluteCoordinate + offset;

            Vector2Int gridSize = linkedObject.LinkedFloor.LinkedBuildingController.GridSize;

            if (firstNodePosition.x < 0 || firstNodePosition.y < 0
                || firstNodePosition.x >= gridSize.x || firstNodePosition.y >= gridSize.y)
            {
                linkedObject.DestroyObject();
                return;
            }

            positionNode.AbsoluteCoordinate = firstNodePosition;
        }
    }
}