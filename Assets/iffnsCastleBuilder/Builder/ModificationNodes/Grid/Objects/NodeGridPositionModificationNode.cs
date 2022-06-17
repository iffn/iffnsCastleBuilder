using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class NodeGridPositionModificationNode : GridModificationNode
    {
        [SerializeField] Renderer CurrentRenderer;

        MailboxLineVector2Int valueHolder;
        MailboxLineVector2Int relativeReferenceHolder;

        public bool RelativeNode
        {
            get
            {
                return relativeReferenceHolder != null;
            }
        }

        public override ModificationNodeType Type
        {
            get
            {
                return ModificationNodeType.Node;
            }
        }

        public override Vector2Int AbsoluteCoordinate
        {
            get
            {
                if (relativeReferenceHolder == null)
                {
                    return new Vector2Int(valueHolder.Val.x, valueHolder.Val.y);
                }
                else
                {
                    return new Vector2Int(valueHolder.Val.x + relativeReferenceHolder.Val.x, valueHolder.Val.y + relativeReferenceHolder.Val.y);
                }

            }
            set
            {
                if (RelativeNode)
                {
                    valueHolder.Val = new Vector2Int(value.x - relativeReferenceHolder.Val.x, value.y - relativeReferenceHolder.Val.y);
                }
                else
                {
                    valueHolder.Val = new Vector2Int(value.x, value.y);
                }

                linkedObject.ApplyBuildParameters();
            }
        }

        public enum ModificationType
        {
            ObjectMover,
            SizeAdjuster
        }

        ModificationType modType;

        public void Setup(BaseGameObject linkedObject, MailboxLineVector2Int value, MailboxLineVector2Int relativeReferenceHolder = null)
        {
            base.setup(linkedObject: linkedObject);

            valueHolder = value;
            this.relativeReferenceHolder = relativeReferenceHolder;
            transform.parent = base.linkedObject.transform;

            if (relativeReferenceHolder == null)
            {
                modType = ModificationType.ObjectMover;
            }
            else
            {
                modType = ModificationType.SizeAdjuster;
            }

            UpdateColors();
        }


        protected override bool NodeColliderState
        {
            set
            {
                transform.GetComponent<Collider>().enabled = value;
            }
        }

        public override void UpdatePosition()
        {
            transform.parent = linkedObject.LinkedFloor.transform;

            transform.localPosition = linkedObject.LinkedFloor.NodePositionFromBlockIndex(blockIndex: AbsoluteCoordinate);

            transform.localRotation = Quaternion.identity;

            transform.parent = linkedObject.transform;

            float width = linkedObject.LinkedFloor.CurrentNodeWallSystem.WallThickness + widthOvershoot;
            transform.localScale = new Vector3(width, Height, width);
        }

        void UpdateColors()
        {
            if (CurrentRenderer == null) return;

            switch (modType)
            {
                case ModificationType.ObjectMover:
                    CurrentRenderer.material.color = ColorLibrary.ModificationNodeObjectMoverColor;
                    break;
                case ModificationType.SizeAdjuster:
                    CurrentRenderer.material.color = ColorLibrary.ModificationNodeSizeAdjusterColor;
                    break;
                default:
                    Debug.LogWarning("Error: Color not defined for this type");
                    break;
            }
        }
    }
}