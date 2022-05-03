using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class NodeWallEditModNode : NodeWallModificationNode
    {
        NodeWall linkedWall;
        Vector2Int thisCoordinate;
        Vector2Int otherCoordinate;

        PositionTypes positionType;
        public enum PositionTypes
        {
            Start,
            End
        }

        public void Setup(NodeWall linkedWall, PositionTypes positionType)
        {
            //base.setup(linkedObject: linkedWall.LinkedSystem.LinkedFloor);
            this.linkedWall = linkedWall;

            this.positionType = positionType;

            linkedSystem = linkedWall.LinkedSystem;
        }

        public override void UpdatePosition()
        {
            switch (positionType)
            {
                case PositionTypes.Start:
                    thisCoordinate = linkedWall.StartPosition;
                    otherCoordinate = linkedWall.EndPosition;
                    break;
                case PositionTypes.End:
                    thisCoordinate = linkedWall.EndPosition;
                    otherCoordinate = linkedWall.StartPosition;
                    break;
                default:
                    break;
            }

            Vector3 thisPosition = linkedWall.LinkedSystem.LinkedFloor.GetLocalNodePositionFromNodeIndex(thisCoordinate);
            Vector3 otherPosition = linkedWall.LinkedSystem.LinkedFloor.GetLocalNodePositionFromNodeIndex(otherCoordinate);

            transform.localPosition = thisPosition;
            transform.localRotation = Quaternion.LookRotation(thisPosition - otherPosition, Vector3.up);

            float height = linkedSystem.LinkedFloor.CompleteFloorHeight + heightOvershoot;
            transform.localScale = new Vector3(transform.localScale.x, height, transform.localScale.z);
        }

        public Vector2Int NodeCoordinate
        {
            set
            {
                switch (positionType)
                {
                    case PositionTypes.Start:
                        linkedWall.StartPosition = value;
                        break;
                    case PositionTypes.End:
                        linkedWall.EndPosition = value;
                        break;
                    default:
                        break;
                }

                linkedSystem.UpdateModNodePositions();
            }
        }
    }
}