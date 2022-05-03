using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public abstract class NodeWallModificationNode : ModificationNode
    {
        List<NodeWall> linkedWalls;

        protected NodeWallSystem linkedSystem;

        public override bool ColliderActivationState
        {
            set
            {

            }
        }
    }
}