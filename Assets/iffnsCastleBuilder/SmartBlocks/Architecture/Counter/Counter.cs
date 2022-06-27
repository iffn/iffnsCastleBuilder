using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class Counter : OnFloorObject
    {
        [SerializeField] BaseCounter LinkedBaseCounter;

        //BuildParameters
        MailboxLineVector2Int BottomLeftPositionParam;
        MailboxLineVector2Int TopRightPositionParam;
        MailboxLineMaterial TopMaterialParam;
        MailboxLineMaterial BaseMaterialParam;

        BlockGridRectangleOrganizer ModificationNodeOrganizer;

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

        public Vector2Int TopRightPosition
        {
            get
            {
                return TopRightPositionParam.Val;
            }
            set
            {
                TopRightPositionParam.Val = value;
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
                return true;
            }
        }

        public override void Setup(IBaseObject linkedFloor)
        {
            base.Setup(linkedFloor);

            BottomLeftPositionParam = new MailboxLineVector2Int(name: "Bottom Left Position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            TopRightPositionParam = new MailboxLineVector2Int(name: "Top Right Position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            TopMaterialParam = new MailboxLineMaterial(name: "Top material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultPlaster);
            BaseMaterialParam = new MailboxLineMaterial(name: "Base material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultWoodSolid);

            BlockGridPositionModificationNode firstNode = ModificationNodeLibrary.NewBlockGridPositionModificationNode;
            firstNode.Setup(linkedObject: this, value: BottomLeftPositionParam);
            FirstPositionNode = firstNode;

            BlockGridPositionModificationNode secondNode = ModificationNodeLibrary.NewBlockGridPositionModificationNode;
            secondNode.Setup(linkedObject: this, value: TopRightPositionParam, relativeReferenceHolder: BottomLeftPositionParam);
            SecondPositionNode = secondNode;

            ModificationNodeOrganizer = new BlockGridRectangleOrganizer(linkedObject: this, firstNode: firstNode, secondNode: secondNode);

            SetupEditButtons();

            LinkedBaseCounter.Setup(mainObject: this);

            LinkedBaseCounter.baseMaterial = BaseMaterialParam;
            LinkedBaseCounter.topMaterial = TopMaterialParam;
        }

        public void CompleteSetupWithBuildParameters(FloorController linkedFloor, Vector2Int bottomLeftPosition, Vector2Int topRightPosition)
        {
            Setup(linkedFloor);

            BottomLeftPositionParam.Val = bottomLeftPosition;
            TopRightPositionParam.Val = topRightPosition;
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

            Vector2Int gridSize = ModificationNodeOrganizer.ObjectOrientationGridSize;

            if (gridSize.x == 0)
            {
                Failed = true;
                return;
            }

            //Define mesh
            Vector2 size = ModificationNodeOrganizer.ObjectOrientationSize;

            AssistObjectManager.ValueContainer baseCounterInfo = LinkedBaseCounter.SetBuildParameters(mainObject: this, UVBaseObject: LinkedFloor.LinkedBuildingController.transform, width: size.y, length: size.x);

            List<TriangleMeshInfo> counterInfo = baseCounterInfo.ConvertedStaticMeshes;

            foreach (TriangleMeshInfo info in counterInfo)
            {
                StaticMeshManager.AddTriangleInfoIfValid(info);
            }

            BuildAllMeshes();
        }

        void SetupEditButtons()
        {
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Flip Diagonally", delegate { ModificationNodeOrganizer.FlipDiagonally(); }));
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Flip Vertical", delegate { ModificationNodeOrganizer.FlipVertical(); }));
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Flip Horizontal", delegate { ModificationNodeOrganizer.FlipHorizontal(); }));
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Rotate Counter-Clockwise", delegate { ModificationNodeOrganizer.RotateCounterClockwise(); }));
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Rotate Clockwise", delegate { ModificationNodeOrganizer.RotateClockwise(); }));
        }

        public override void MoveOnGrid(Vector2Int offset)
        {
            ModificationNodeOrganizer.MoveOnGrid(offset: offset);
        }
    }
}