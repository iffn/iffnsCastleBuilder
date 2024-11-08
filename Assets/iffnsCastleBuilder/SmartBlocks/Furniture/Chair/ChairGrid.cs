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
        MailboxLineMaterial MainMaterialParam;

        GridOrientationOrganizer ModificationNodeOrganizer;

        public override ModificationOrganizer Organizer
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

        public override bool RaiseToFloor
        {
            get
            {
                return true;
            }
        }

        public override bool IsStructural
        {
            get
            {
                return false;
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

            BottomLeftPositionParam = new MailboxLineVector2Int(name: "Bottom Left Position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            OrientationParam = new MailboxLineDistinctNamed(name: "Orientation", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, entries: GridOrientationNode.OrientationStrings, DefaultValue: (int)GridOrientation.GridForwardOrientations.ZPositive);
            MainMaterialParam = new MailboxLineMaterial(name: "Main material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultWoodSolid);

            SetupGridSizeParam();

            SetupEditButtons();

            BlockGridPositionModificationNode firstNode = ModificationNodeLibrary.NewBlockGridPositionModificationNode;
            firstNode.Setup(linkedObject: this, value: BottomLeftPositionParam);
            FirstPositionNode = firstNode;

            GridOrientationNode secondNode = ModificationNodeLibrary.NewGridOrientationNode;
            secondNode.Setup(linkedObject: this, orientation: OrientationParam, positionReference: FirstPositionNode.gameObject, localPositionOffset: Vector3.up * LinkedFloor.CompleteFloorHeight * 0.5f, arrowOffset: 0.73f);
            SecondPositionNode = secondNode;

            ModificationNodeOrganizer = new GridOrientationOrganizer(linkedObject: this, positionNode: firstNode, orientationNode: secondNode);

            foreach (UnityMeshManager manager in UnmanagedMeshes)
            {
                manager.Setup(mainObject: this, currentMaterialReference: MainMaterialParam);
            }
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
            base.ApplyBuildParameters();

            //Check validity
            if (Failed) return;

            //Set parameters
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