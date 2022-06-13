using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class NodeWallFlipNode : NodeWallModificationNode
    {
        NodeWall linkedWall;

        public void Setup(NodeWall linkedWall)
        {
            this.linkedWall = linkedWall;

            linkedSystem = linkedWall.LinkedSystem;
        }

        public override void UpdatePosition()
        {
            FloorController linkedFloor = linkedWall.LinkedSystem.LinkedFloor;

            Vector3 thisPosition = linkedFloor.GetLocalNodePositionFromNodeIndex(linkedWall.StartPosition);
            Vector3 otherPosition = linkedFloor.GetLocalNodePositionFromNodeIndex(linkedWall.EndPosition);

            transform.localPosition = (thisPosition + otherPosition) * 0.5f + (linkedFloor.WallBetweenHeight * 0.6666667f + linkedFloor.BottomFloorHeight) * Vector3.up;
            transform.localRotation = Quaternion.LookRotation(thisPosition - otherPosition, Vector3.up);

            float width = linkedSystem.WallThickness + widthOvershoot * 2;
            transform.localScale = new Vector3(width, transform.localScale.y, transform.localScale.z);
            //Warning: UpdteNodeSize calls UpdatePosition() -> Do not add here
        }

        public void FlipNodeWall()
        {
            linkedWall.FlipPositions();
        }
    }
}