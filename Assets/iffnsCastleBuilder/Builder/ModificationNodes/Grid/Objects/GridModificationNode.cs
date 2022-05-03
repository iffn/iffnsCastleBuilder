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

        protected override void setup(BaseGameObject linkedObject)
        {
            base.setup(linkedObject: linkedObject);

            if (linkedObject is OnFloorObject)
            {
                parent = linkedObject as OnFloorObject;
            }
            else
            {
                Debug.LogWarning("Error: Linked object of Grid Modification Node is not a OnFloorObject");
            }
        }

        public abstract Vector2Int AbsoluteCoordinate { get; set; }

    }
}