using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public abstract class GridModificationNode : ModificationNode
    {
        protected OnFloorObject parent;

        public enum ModificationNodeType
        {
            Block,
            Node
        }

        ModificationNodeType type;

        public abstract ModificationNodeType Type { get; }

        /*
        public OnFloorObject LinkedOnFloorObject
        {
            get
            {
                OnFloorObject returnValue = LinkedOrganizer as OnFloorObject;

                if (returnValue == null) Debug.LogWarning("Error: Linked object of Grid Modification Node is not a OnFloorObject");

                return LinkedOrganizer as OnFloorObject;
            }
        }
        */

        protected override void setup(ModificationOrganizer linkedOrganizer)
        {
            base.setup(linkedOrganizer: linkedOrganizer);

            /*
            if (linkedORganizer is OnFloorObject)
            {
                parent = linkedORganizer as OnFloorObject;
            }
            else
            {
                Debug.LogWarning("Error: Linked object of Grid Modification Node is not a OnFloorObject");
            }
            */
        }

        public abstract Vector2Int AbsoluteCoordinate { get; set; }

    }
}