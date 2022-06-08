using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public abstract class GridModificationOrganizer : ModificationOrganizer
    {
        public abstract void SetLinkedObjectPositionAndOrientation(bool RaiseToFloor);

        public abstract void MoveOnGrid(Vector2Int offset);

        public abstract Vector3 FirstBuildPositionAbsolute { set; }
        public abstract Vector3 SecondBuildPositionAbsolute { set; }


    }
}