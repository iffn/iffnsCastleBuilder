using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class GridOrientationNode : ModificationNode
    {
        //Unity assignments
        [SerializeField] GameObject XPositiveArrow;
        [SerializeField] GameObject XNegativeArrow;
        [SerializeField] GameObject ZPositiveArrow;
        [SerializeField] GameObject ZNegativArrow;

        public static List<string> OrientationStrings
        {
            get
            {
                List<string> enumString = new List<string>();

                int enumValues = System.Enum.GetValues(typeof(GridOrientation.GridForwardOrientations)).Length;

                for (int i = 0; i < enumValues; i++)
                {
                    GridOrientation.GridForwardOrientations type = (GridOrientation.GridForwardOrientations)i;

                    enumString.Add(type.ToString());
                }

                return enumString;
            }
        }

        public void SetXPositive()
        {
            CurrentOrientation = new GridOrientation(GridOrientation.GridForwardOrientations.XPositive);
        }

        public void SetXNegative()
        {
            CurrentOrientation = new GridOrientation(GridOrientation.GridForwardOrientations.XNegative);
        }

        public void SetZPositive()
        {
            CurrentOrientation = new GridOrientation(GridOrientation.GridForwardOrientations.ZPositive);
        }

        public void SetZNegative()
        {
            CurrentOrientation = new GridOrientation(GridOrientation.GridForwardOrientations.ZNegative);
        }


        public GridOrientation CurrentOrientation
        {
            get
            {
                GridOrientation returnValue = new GridOrientation(forwardOrientation: (GridOrientation.GridForwardOrientations)OrientationValue.Val);

                return returnValue;
            }
            set
            {
                OrientationValue.Val = (int)value.ForwardOrientation;
                //LinkedOrganizer.ApplyBuildParameters();
            }
        }

        MailboxLineDistinctNamed OrientationValue;
        Vector3 localPositionOffset;
        GameObject positionReference;
        float arrowOffset;


        //public void Setup(BaseGameObject linkedObject, MailboxLineDistinctNamed orientation, Vector3 localPosition, float arrowOffset)
        public void Setup(ModificationOrganizer linkedOrganizer, MailboxLineDistinctNamed orientation, GameObject positionReference, Vector3 localPositionOffset, float arrowOffset)
        {
            base.setup(linkedOrganizer: linkedOrganizer);

            OrientationValue = orientation;

            transform.parent = linkedOrganizer.LinkedObject.transform;

            this.positionReference = positionReference;

            this.localPositionOffset = localPositionOffset;
            this.arrowOffset = arrowOffset;

            UpdatePosition();
        }

        public override void UpdatePosition()
        {
            //transform.parent = LinkedOrganizer.transform.parent; //Set parent for positioning
            transform.position = positionReference.transform.position;
            transform.localPosition += localPositionOffset;
            transform.localRotation = Quaternion.identity;

            //transform.parent = LinkedOrganizer.transform; //Set correct parent

            ZPositiveArrow.transform.localPosition = Vector3.forward * arrowOffset;
            ZNegativArrow.transform.localPosition = Vector3.back * arrowOffset;
            XPositiveArrow.transform.localPosition = Vector3.right * arrowOffset;
            XNegativeArrow.transform.localPosition = Vector3.left * arrowOffset;
        }

        public override bool ColliderActivationState
        {
            set
            {
                XPositiveArrow.transform.GetComponent<Collider>().enabled = value;
                XNegativeArrow.transform.GetComponent<Collider>().enabled = value;
                ZPositiveArrow.transform.GetComponent<Collider>().enabled = value;
                ZNegativArrow.transform.GetComponent<Collider>().enabled = value;
            }
        }
    }
}