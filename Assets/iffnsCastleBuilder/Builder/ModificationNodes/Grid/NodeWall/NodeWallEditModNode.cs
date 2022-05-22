using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class NodeWallEditModNode : NodeWallModificationNode
    {
        public NodeWall LinkedWall { get; private set; }
        Vector2Int thisCoordinate;
        Vector2Int otherCoordinate;

        PositionTypes positionType;
        public enum PositionTypes
        {
            Start,
            End
        }

        public bool WouldBeSameAsOtherCoordinate(Vector2Int newCoordinate)
        {
            Vector2Int otherCoordinate = Vector2Int.zero;

            switch (positionType)
            {
                case PositionTypes.Start:
                    otherCoordinate = LinkedWall.EndPosition;
                    break;
                case PositionTypes.End:
                    otherCoordinate = LinkedWall.StartPosition;
                    break;
                default:
                    Debug.LogWarning("Error: State not defined");
                    break;
            }

            return newCoordinate.x == otherCoordinate.x && newCoordinate.y == otherCoordinate.y;
        }

        public bool SamePositionAsOtherNode
        {
            get
            {
                Vector2Int thisCoordinate = Vector2Int.zero;
                Vector2Int otherCoordinate = Vector2Int.zero;

                switch (positionType)
                {
                    case PositionTypes.Start:
                        thisCoordinate = LinkedWall.StartPosition;
                        otherCoordinate = LinkedWall.EndPosition;
                        break;
                    case PositionTypes.End:
                        thisCoordinate = LinkedWall.EndPosition;
                        otherCoordinate = LinkedWall.StartPosition;
                        break;
                    default:
                        Debug.LogWarning("Error: State not defined");
                        break;
                }

                return (thisCoordinate.x == otherCoordinate.x && thisCoordinate.y == otherCoordinate.y);
            }
        }

        public void Setup(NodeWall linkedWall, PositionTypes positionType)
        {
            //base.setup(linkedObject: linkedWall.LinkedSystem.LinkedFloor);
            this.LinkedWall = linkedWall;

            this.positionType = positionType;

            linkedSystem = linkedWall.LinkedSystem;
        }

        public override void UpdatePosition()
        {
            switch (positionType)
            {
                case PositionTypes.Start:
                    thisCoordinate = LinkedWall.StartPosition;
                    otherCoordinate = LinkedWall.EndPosition;
                    break;
                case PositionTypes.End:
                    thisCoordinate = LinkedWall.EndPosition;
                    otherCoordinate = LinkedWall.StartPosition;
                    break;
                default:
                    break;
            }

            Vector3 thisPosition = LinkedWall.LinkedSystem.LinkedFloor.GetLocalNodePositionFromNodeIndex(thisCoordinate);
            Vector3 otherPosition = LinkedWall.LinkedSystem.LinkedFloor.GetLocalNodePositionFromNodeIndex(otherCoordinate);

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
                        LinkedWall.StartPosition = value;
                        break;
                    case PositionTypes.End:
                        LinkedWall.EndPosition = value;
                        break;
                    default:
                        break;
                }

                linkedSystem.UpdateModNodePositions();
            }
        }
    }
}