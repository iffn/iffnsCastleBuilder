using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public abstract class GridModificationNode : ModificationNode
    {
        protected OnFloorObject linkedObject;

        public enum ModificationNodeType
        {
            Block,
            Node
        }

        ModificationNodeType type;

        public abstract ModificationNodeType Type { get; }

        public float Height
        {
            get
            {
                return linkedObject.ModificationNodeHeight + heightOvershoot;
            }
        }

        public OnFloorObject LinkedOnFloorObject
        {
            get
            {
                OnFloorObject returnValue = LinkedObject as OnFloorObject;

                if (returnValue == null) Debug.LogWarning("Error: Linked object of Grid Modification Node is not a OnFloorObject");

                return LinkedObject as OnFloorObject;
            }
        }

        protected override void setup(BaseGameObject linkedObject)
        {
            base.setup(linkedObject: linkedObject);

            if (linkedObject is OnFloorObject)
            {
                this.linkedObject = linkedObject as OnFloorObject;
            }
            else
            {
                Debug.LogWarning("Error: Linked object of Grid Modification Node is not a OnFloorObject");
            }
        }

        public abstract Vector2Int AbsoluteCoordinate { get; set; }

    }
}