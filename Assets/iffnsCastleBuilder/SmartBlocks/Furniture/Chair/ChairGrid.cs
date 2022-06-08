using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class ChairGrid : OnFloorObject
    {
        //Unity references
        [SerializeField] ChairBase currentChairBase;

        //Build parameters
        MailboxLineVector2Int BottomLeftPositionParam;
        MailboxLineDistinctNamed GridSizeParam;
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

        public enum GridSizes
        {
            Size3x3,
            Size2x2
        }

        public GridSizes GridSize
        {
            get
            {
                GridSizes returnValue = (GridSizes)GridSizeParam.Val;

                return returnValue;
            }
            set
            {
                GridSizeParam.Val = (int)value;
                ApplyBuildParameters();
            }
        }

        void SetupGridSizeParam()
        {
            List<string> enumString = new List<string>();

            int enumValues = System.Enum.GetValues(typeof(GridSizes)).Length;

            for (int i = 0; i < enumValues; i++)
            {
                GridSizes type = (GridSizes)i;

                enumString.Add(type.ToString().Replace("Size", ""));
            }

            GridSizeParam = new MailboxLineDistinctNamed(
                "Preference type",
                CurrentMailbox,
                Mailbox.ValueType.buildParameter,
                enumString,
                0);
        }

        public GridOrientation.GridForwardOrientations Orientation
        {
            get
            {
                GridOrientation.GridForwardOrientations returnValue = (GridOrientation.GridForwardOrientations)OrientationParam.Val;

                return returnValue;
            }
            set
            {
                OrientationParam.Val = (int)value;
                ApplyBuildParameters();
            }
        }

        public override void Setup(IBaseObject linkedFloor)
        {
            base.Setup(linkedFloor);

            IsStructural = false;

            BottomLeftPositionParam = new MailboxLineVector2Int(name: "Bottom Left Position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            OrientationParam = new MailboxLineDistinctNamed(name: "Orientation", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, entries: GridOrientationNode.OrientationStrings, DefaultValue: (int)GridOrientation.GridForwardOrientations.ZPositive);

            SetupGridSizeParam();

            SetupEditButtons();

            BlockGridPositionModificationNode firstNode = ModificationNodeLibrary.NewBlockGridPositionModificationNode;
            firstNode.Setup(linkedObject: this, value: BottomLeftPositionParam);
            FirstPositionNode = firstNode;

            GridOrientationNode secondNode = ModificationNodeLibrary.NewGridOrientationNode;
            secondNode.Setup(linkedObject: this, orientation: OrientationParam, positionReference: FirstPositionNode.gameObject, localPositionOffset: Vector3.up * LinkedFloor.CompleteFloorHeight * 0.5f, arrowOffset: 0.73f);
            SecondPositionNode = secondNode;

            ModificationNodeOrganizer = new GridOrientationOrganizer(linkedObject: this, positionNode: firstNode, orientationNode: secondNode);
        }

        public void CompleteSetupWithBuildParameters(FloorController linkedFloor, Vector2Int position, GridSizes GridSize, GridOrientation.GridForwardOrientations gridOrientation)
        {
            Setup(linkedFloor);

            BottomLeftPosition = position;
            this.GridSize = GridSize;
            Orientation = gridOrientation;
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

            //transform.localPosition = new Vector3((BottomLeftPosition.x - 0.5f) * blockSize, LinkedFloor.BottomFloorHeight, (BottomLeftPosition.y - 0.5f) * blockSize);

            switch (GridSize)
            {
                case GridSizes.Size3x3:
                    currentChairBase.transform.localPosition = new Vector3(1, 0, 1) * LinkedFloor.LinkedBuildingController.BlockSize * 0.5f;
                    break;
                case GridSizes.Size2x2:
                    currentChairBase.transform.localPosition = new Vector3(1, 0, 1) * LinkedFloor.LinkedBuildingController.BlockSize;
                    break;
                default:
                    Debug.LogWarning("Error, Undefined Grid size enum: " + GridSize.ToString());
                    break;
            }
            /*
            switch (Orientation)
            {
                case GridOrientation.GridForwardOrientations.ZPositive:
                    transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    break;
                case GridOrientation.GridForwardOrientations.XPositive:
                    transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
                    break;
                case GridOrientation.GridForwardOrientations.ZNegative:
                    transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
                    break;
                case GridOrientation.GridForwardOrientations.XNegative:
                    transform.localRotation = Quaternion.Euler(new Vector3(0, 270, 0));
                    break;
                default:
                    Debug.LogWarning("Error with Switch: Undefined orientation " + Orientation.ToString());
                    break;
            }
            */

            //UpdateModificationNodePositions();
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