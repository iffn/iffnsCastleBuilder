using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class BedGrid : OnFloorObject
    {
        //Unity references
        [SerializeField] BedBase currentBaseBed;

        //Build parameters
        MailboxLineVector2Int BottomLeftPositionParam;
        MailboxLineDistinctNamed MattressSizeParam;
        MailboxLineDistinctNamed OrientationParam;

        GridOrientationOrganizer ModificationNodeOrganizer;

        public override GridModificationOrganizer Organizer
        {
            get
            {
                return ModificationNodeOrganizer;
            }
        }

        public Vector2Int BottomLeftPosition
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

        public BedBase.MattressSizes CurrentMattressSize
        {
            get
            {
                BedBase.MattressSizes returnValue = (BedBase.MattressSizes)MattressSizeParam.Val;

                return returnValue;
            }
            set
            {
                MattressSizeParam.Val = (int)value;
                ApplyBuildParameters();
            }
        }

        public GridOrientation Orientation
        {
            get
            {
                GridOrientation.GridForwardOrientations orientation = (GridOrientation.GridForwardOrientations)OrientationParam.Val;

                GridOrientation returnValue = new GridOrientation(orientation);

                return returnValue;
            }
            set
            {
                OrientationParam.Val = (int)value.ForwardOrientation;
                ApplyBuildParameters();
            }
        }

        public override void Setup(IBaseObject linkedFloor)
        {
            base.Setup(linkedFloor);

            IsStructural = false;

            BottomLeftPositionParam = new MailboxLineVector2Int(name: "Bottom Left Position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            MattressSizeParam = new MailboxLineDistinctNamed(name: "Mattress size", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, entries: BedBase.MattressTypeStrings, DefaultValue: (int)BedBase.MattressSizes.Single);
            OrientationParam = new MailboxLineDistinctNamed(name: "Orientation", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, entries: GridOrientationNode.OrientationStrings, DefaultValue: (int)GridOrientation.GridForwardOrientations.ZPositive);

            SetupEditButtons();

            BlockGridPositionModificationNode firstNode = ModificationNodeLibrary.NewBlockGridPositionModificationNode;
            firstNode.Setup(linkedObject: this, value: BottomLeftPositionParam);
            FirstPositionNode = firstNode;

            GridOrientationNode secondNode = ModificationNodeLibrary.NewGridOrientationNode;
            secondNode.Setup(linkedObject: this, orientation: OrientationParam, positionReference: FirstPositionNode.gameObject, localPositionOffset: Vector3.up * LinkedFloor.CompleteFloorHeight * 0.5f, arrowOffset: 0.73f);
            SecondPositionNode = secondNode;

            ModificationNodeOrganizer = new GridOrientationOrganizer(linkedObject: this, positionNode: firstNode, orientationNode: secondNode);
        }

        public void CompleteSetupWithBuildParameters(FloorController linkedFloor, Vector2Int bottomLeftPosition, BedBase.MattressSizes MattressType)
        {
            Setup(linkedFloor);

            BottomLeftPositionParam.Val = bottomLeftPosition;
            MattressSizeParam.Val = (int)MattressType;
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
            ModificationNodeOrganizer.SetLinkedObjectPositionAndOrientation(raiseToFloor: true);

            currentBaseBed.SetBuildParameters(mattressSize: CurrentMattressSize);

            UpdateModificationNodePositions();

        }

        void SetupEditButtons()
        {
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Convert to float", delegate { ConvertToFloat(); }));
        }

        public override void MoveOnGrid(Vector2Int offset)
        {
            ModificationNodeOrganizer.MoveOnGrid(offset: offset);
        }

        void ConvertToFloat()
        {
            //ToDo
        }
    }
}