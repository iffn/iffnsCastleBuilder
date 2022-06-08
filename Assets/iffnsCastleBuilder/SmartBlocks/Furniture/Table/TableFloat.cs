using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class TableFloat : OnFloorObject
    {
        //Unity references

        //Build parameters
        MailboxLineVector2 BottomLeftPositionParam;
        MailboxLineVector2 RelativeSizeParam;
        MailboxLineRanged RotationParam;

        public override GridModificationOrganizer Organizer
        {
            get
            {
                return null;
            }
        }

        public Vector2 BottomLeftPosition
        {
            get
            {
                return BottomLeftPositionParam.Val;
            }
            set
            {
                BottomLeftPositionParam.Val = value;
                ApplyBuildParameters();
            }
        }

        public Vector2 RelativeSize
        {
            get
            {
                return RelativeSizeParam.Val;
            }
            set
            {
                RelativeSizeParam.Val = value;
                ApplyBuildParameters();
            }
        }

        public float Rotation
        {
            get
            {
                return RotationParam.Val;
            }
            set
            {
                RotationParam.Val = value % 360;
                ApplyBuildParameters();
            }
        }

        public override void Setup(IBaseObject linkedFloor)
        {
            base.Setup(linkedFloor);

            IsStructural = false;

            BottomLeftPositionParam = new MailboxLineVector2(name: "Bottom left position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            RelativeSizeParam = new MailboxLineVector2(name: "Relative size", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            RotationParam = new MailboxLineRanged(name: "Rotation", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 360, Min: 0, DefaultValue: 0);

            SetupEditButtons();

            /*
            GridPositionModificationNode firstNode = ModificationNodeLibrary.NewGridPositionModificationNode;
            firstNode.Setup(linkedObject: this, value: BottomLeftPositionParam);
            FirstPositionNode = firstNode;

            GridPositionModificationNode secondNode = ModificationNodeLibrary.NewGridPositionModificationNode;
            secondNode.Setup(linkedObject: this, value: RelativeSize, relativeReferenceHolder: BottomLeftPositionParam);
            SecondPositionNode = secondNode;
            */

            //ModificationNodeLibrary.NewGridModificationNode.Setup(linkedObject: this, value: CutoffRangeParam, relativeReferenceHolder: CenterPositionParam, parent: this, modType: GridModificationNode.ModificationType.SizeAdjuster);

        }

        public void CompleteSetupWithBuildParameters(FloorController linkedFloor, Vector2Int bottomLeftPosition, Vector2Int topRightPosition)
        {
            Setup(linkedFloor);

            BottomLeftPositionParam.Val = bottomLeftPosition;
            RelativeSizeParam.Val = topRightPosition;
        }

        public override void ResetObject()
        {
            baseReset();

            ResetEditButtons();
        }

        public override void InternalUpdate()
        {
            NonOrderedInternalUpdate();
        }

        public override void PlaytimeUpdate()
        {
            NonOrderedPlaytimeUpdate();
        }

        public override void ApplyBuildParameters()
        {
            transform.localPosition = BottomLeftPosition;
            transform.localRotation = Quaternion.Euler(Vector3.up * Rotation);
            transform.localScale = new Vector3(RelativeSize.x, transform.localScale.y, RelativeSize.y);
        }

        void SetupEditButtons()
        {
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Convert to grid", delegate { ConvertToGrid(); }));
        }

        public override void MoveOnGrid(Vector2Int offset)
        {
            BottomLeftPosition += BlockSize * new Vector2(offset.x, offset.y);
        }

        void ConvertToGrid()
        {
            //ToDo
        }
    }
}