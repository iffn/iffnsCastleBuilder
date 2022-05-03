using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class GridScalerOrganizer : ModificationNode
    {
        HumanBuildingController linkedController;

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

        public void Setup(HumanBuildingController linkedController)
        {
            this.linkedController = linkedController;

            xPosNode.Setup(linkedController: linkedController, organizer: this, directionType: GridScaleNode.DirectionTypes.xPos);
            xNegNode.Setup(linkedController: linkedController, organizer: this, directionType: GridScaleNode.DirectionTypes.xNeg);
            zPosNode.Setup(linkedController: linkedController, organizer: this, directionType: GridScaleNode.DirectionTypes.zPos);
            zNegNode.Setup(linkedController: linkedController, organizer: this, directionType: GridScaleNode.DirectionTypes.zNeg);
        }

        public override void UpdatePosition()
        {
            xPosNode.UpdatePosition();
            xNegNode.UpdatePosition();
            zPosNode.UpdatePosition();
            zNegNode.UpdatePosition();
        }

        public override bool ColliderActivationState
        {
            set
            {
                xPosNode.ColliderActivationState = value;
                xNegNode.ColliderActivationState = value;
                zPosNode.ColliderActivationState = value;
                zNegNode.ColliderActivationState = value;
            }
        }
    }
}