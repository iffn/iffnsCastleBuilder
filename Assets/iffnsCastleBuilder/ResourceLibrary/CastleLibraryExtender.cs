using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using iffnsStuff.iffnsBaseSystemForUnity;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class CastleLibraryExtender : ResourceLibraryExtender
    {
        public override BaseVirtualObject TryGetVirtualObjectFromStringIdentifier(string identifier, IBaseObject superObject)
        {
            BaseVirtualObject returnValue = null;

            switch (identifier)
            {
                //ToDo: Find best way to return objects
                case VirtualBlock.staticIdentifierString:
                    returnValue = new VirtualBlock(superObject: superObject);
                    break;
                case NodeWall.constIdentifierString:
                    returnValue = new NodeWall(superObject: superObject);
                    break;
                default:
                    break;
            }

            return returnValue;

        }
    }
}