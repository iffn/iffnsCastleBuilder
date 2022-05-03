using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class NodeWallMultiModNode : NodeWallModificationNode
    {
        List<NodeWall> linkedStartWalls;
        List<NodeWall> linkedEndWalls;

        Vector2Int corrdinate;

        NodeWallNode linkedNode;

        public void Setup(NodeWallNode linkedNode)
        {
            this.linkedNode = linkedNode;

            linkedSystem = linkedNode.LinkedSystem;
        }

        public override void UpdatePosition()
        {
            transform.localPosition = linkedNode.LocalPosition3D;

            float height = linkedSystem.LinkedFloor.CompleteFloorHeight + heightOvershoot + MathHelper.SmallFloat * 2;
            float width = linkedSystem.WallThickness + widthOvershoot;
            transform.localScale = new Vector3(width, height, width);
        }

        /*
        public override void UpdateNodeSize()
        {
            //X and Z scale defined in Unity
            transform.localScale = new Vector3(transform.localScale.x, linkedSystem.LinkedFloor.CompleteFloorHeight + heightOvershoot, transform.localScale.z);
        }
        */

        public Vector2Int NodeCoordinate
        {
            set
            {
                foreach (NodeWall wall in linkedNode.EndPoints)
                {
                    if (wall.StartPosition.x == linkedNode.Coordinate.x && wall.StartPosition.y == linkedNode.Coordinate.y)
                    {
                        wall.StartPosition = value;
                    }
                    else if (wall.EndPosition.x == linkedNode.Coordinate.x && wall.EndPosition.y == linkedNode.Coordinate.y)
                    {
                        wall.EndPosition = value;
                    }
                }

                linkedNode = linkedSystem.NodeFromCoordinate(value);

                linkedSystem.ApplyBuildParameters();

                linkedSystem.UpdateModNodePositions();
            }
        }
    }
}