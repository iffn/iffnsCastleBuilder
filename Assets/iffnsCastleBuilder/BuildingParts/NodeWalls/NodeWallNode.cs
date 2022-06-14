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

        public List<INodeWallReference> EndPoints;
        public List<INodeWallReference> IntermediatePoints;
        
        public List<NodeWall> NonDummyEndPoints
        {
            get
            {
                List<NodeWall> returnValue = new();

                foreach(INodeWallReference wall in EndPoints)
                {
                    if (wall is NodeWall)
                    {
                        returnValue.Add(wall as NodeWall);
                    }
                }

                return returnValue;
            }
        }

        public bool AllWallsAreDummy
        {
            get
            {
                foreach (INodeWallReference wall in EndPoints)
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

            EndPoints = new List<INodeWallReference>();
            IntermediatePoints = new List<INodeWallReference>();
        }
    }
}