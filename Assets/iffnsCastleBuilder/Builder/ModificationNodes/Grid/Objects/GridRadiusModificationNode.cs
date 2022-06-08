using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class GridRadiusModificationNode : GridModificationNode
    {
        MailboxLineDistinctUnnamed radiusValue;
        MailboxLineVector2Int centerPosition;

        public override ModificationNodeType Type
        {
            get
            {
                return ModificationNodeType.Block;
            }
        }

        Vector2Int directionPreferenceHolder = Vector2Int.right;
        private Vector2Int DirectionPreference
        {
            set
            {
                int x = Mathf.Clamp(value: value.x, min: -1, max: 1);
                int y = Mathf.Clamp(value: value.y, min: -1, max: 1);

                if (y == 0 && x == 0) x = 1;
                if (y != 0 && x != 0) y = 0;

                directionPreferenceHolder = new Vector2Int(x, y);
            }
            get
            {
                return directionPreferenceHolder;
            }
        }

        public override Vector2Int AbsoluteCoordinate
        {
            get
            {
                return centerPosition.Val + DirectionPreference * radiusValue.Val;
            }
            set
            {
                Vector2Int offset = value - centerPosition.Val;

                if (Mathf.Abs(offset.x) >= Mathf.Abs(offset.y))
                {
                    radiusValue.Val = offset.x;
                    DirectionPreference = new Vector2Int(offset.x, 0);
                }
                else
                {
                    radiusValue.Val = offset.y;
                    DirectionPreference = new Vector2Int(0, offset.y);
                }

                parent.ApplyBuildParameters();
            }
        }

        public void Setup(ModificationOrganizer linkedOrganizer, MailboxLineDistinctUnnamed radiusValue, MailboxLineVector2Int centerPosition)
        {
            base.setup(linkedOrganizer: linkedOrganizer);

            this.radiusValue = radiusValue;
            this.centerPosition = centerPosition;
            transform.parent = parent.transform;
        }

        public override bool ColliderActivationState
        {
            set
            {
                transform.GetComponent<Collider>().enabled = value;
            }
        }

        public override void UpdatePosition()
        {
            transform.parent = parent.LinkedFloor.transform;

            transform.localPosition = parent.LinkedFloor.CenterPositionFromBlockIndex(AbsoluteCoordinate);

            transform.Translate(Vector3.up * parent.LinkedFloor.CompleteFloorHeight / 2);

            transform.localRotation = Quaternion.identity;

            transform.parent = parent.transform;

            float height = parent.LinkedFloor.CompleteFloorHeight + heightOvershoot;
            float width = parent.LinkedFloor.BlockSize + widthOvershoot;
            transform.localScale = new Vector3(width, height, width);
        }
    }
}