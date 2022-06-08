using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class GridScalerOrganizer : ModificationOrganizer
    {
        //HumanBuildingController linkedController;

        //Public since used as assignment and script reference
        public GridScaleNode xPosNode;
        public GridScaleNode xNegNode;
        public GridScaleNode zPosNode;
        public GridScaleNode zNegNode;

        /*
        public override void UpdateNodeSize()
        {
            xPosNode.UpdateNodeSize();
            xNegNode.UpdateNodeSize();
            zPosNode.UpdateNodeSize();
            zNegNode.UpdateNodeSize();
        }
        */

        public GridScalerOrganizer(HumanBuildingController linkedBuilding) : base(linkedBuilding)
        {
            xPosNode.Setup(linkedBuilding: linkedBuilding, organizer: this, directionType: GridScaleNode.DirectionTypes.xPos);
            xNegNode.Setup(linkedBuilding: linkedBuilding, organizer: this, directionType: GridScaleNode.DirectionTypes.xNeg);
            zPosNode.Setup(linkedBuilding: linkedBuilding, organizer: this, directionType: GridScaleNode.DirectionTypes.zPos);
            zNegNode.Setup(linkedBuilding: linkedBuilding, organizer: this, directionType: GridScaleNode.DirectionTypes.zNeg);
        }
    }
}