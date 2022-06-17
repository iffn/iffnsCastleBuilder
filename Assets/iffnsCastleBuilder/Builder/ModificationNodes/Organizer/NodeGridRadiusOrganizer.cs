using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using iffnsStuff.iffnsBaseSystemForUnity;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class NodeGridRadiusOrganizer : ModificationOrganizer
    {
        readonly NodeGridPositionModificationNode positionNode;
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

        public NodeGridRadiusOrganizer(OnFloorObject linkedObject, NodeGridPositionModificationNode positionNode, RadiusModificationNode radiusNode)
        {
            this.linkedObject = linkedObject;
            this.positionNode = positionNode;
            this.radiusNode = radiusNode;

            linkedObject.HideModificationNodes();
        }

        public override void SetLinkedObjectPositionAndOrientation(bool raiseToFloor)
        {
            bool firstOnGrid = linkedObject.LinkedFloor.LinkedBuildingController.NodeCoordinateIsOnGrid(positionNode.AbsoluteCoordinate);

            if (!firstOnGrid)
            {
                linkedObject.Failed = true;
                return;
            }

            linkedObject.transform.localPosition = linkedObject.LinkedFloor.GetLocalNodePositionFromNodeIndex(nodeIndex: positionNode.AbsoluteCoordinate);

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

            //Note: On grid error check is part of apply build parameters

            positionNode.AbsoluteCoordinate = firstNodePosition;
        }
    }
}
