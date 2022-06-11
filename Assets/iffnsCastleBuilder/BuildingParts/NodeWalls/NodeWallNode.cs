using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class NodeWallNode
    {
        NodeWallSystem linkedNodeWallSystem;

        public NodeWallSystem LinkedSystem
        {
            get
            {
                return linkedNodeWallSystem;
            }
        }

        Vector2Int indexPosition;


        Vector2 localPosition2D;

        public Vector2 LocalPosition2D
        {
            get
            {
                return localPosition2D;
            }
        }


        Vector3 localPosition3D;

        public Vector3 LocalPosition3D
        {
            get
            {
                return localPosition3D;
            }
        }

        public Vector2Int Coordinate
        {
            get
            {
                return indexPosition;
            }
        }

        public List<NodeWallReference> EndPoints;
        public List<NodeWallReference> IntermediatePoints;
        
        public bool AllWallsAreDummy
        {
            get
            {
                foreach (NodeWallReference wall in EndPoints)
                {
                    if(wall is NodeWall)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public NodeWallNode(Vector2Int indexPosition, NodeWallSystem linkedNodeWallSystem)
        {
            this.indexPosition = indexPosition;
            this.linkedNodeWallSystem = linkedNodeWallSystem;

            localPosition3D = linkedNodeWallSystem.LinkedFloor.GetLocalNodePositionFromNodeIndex(indexPosition);
            localPosition2D = new Vector2(localPosition3D.x, localPosition3D.z);

            EndPoints = new List<NodeWallReference>();
            IntermediatePoints = new List<NodeWallReference>();
        }
    }
}