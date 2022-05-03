using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class Window : OnFloorObject
    {
        public const string constIdentifierString = "Window";
        public override string IdentifierString
        {
            get
            {
                return constIdentifierString;
            }
        }

        [SerializeField] RectangularBaseWindow CurrentRectangularBaseWindow;

        //BuildParameters
        MailboxLineVector2Int BottomLeftPositionParam;
        MailboxLineVector2Int TopRightPositionParam;
        MailboxLineRanged BottomHeightParam;
        MailboxLineRanged WindowHeightParam;
        MailboxLineDistinctNamed WallTypeParam;
        MailboxLineMaterial FrontWallMaterialParm;
        MailboxLineMaterial BackWallMaterialParm;
        MailboxLineMaterial FrameMaterialParam;
        MailboxLineMaterial GlassMaterialParam;

        NodeGridRectangleOrganizer ModificationNodeOrganizer;

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

        public float BottomHeight
        {
            get
            {
                return BottomHeightParam.Val;
            }
            set
            {
                BottomHeightParam.Val = value;
                ApplyBuildParameters();
            }
        }

        public float WindowHeight
        {
            get
            {
                return WindowHeightParam.Val;
            }
            set
            {
                WindowHeightParam.Val = value;
                ApplyBuildParameters();
            }
        }

        void SetupWallTypeParam()
        {
            List<string> enumString = new List<string>();

            int enumValues = System.Enum.GetValues(typeof(NodeGridRectangleOrganizer.OrientationTypes)).Length;

            for (int i = 0; i < enumValues; i++)
            {
                NodeGridRectangleOrganizer.OrientationTypes type = (NodeGridRectangleOrganizer.OrientationTypes)i;

                enumString.Add(type.ToString());
            }

            WallTypeParam = new MailboxLineDistinctNamed(
                "Wall type",
                CurrentMailbox,
                Mailbox.ValueType.buildParameter,
                enumString,
                0);
        }

        public NodeGridRectangleOrganizer.OrientationTypes WallType
        {
            get
            {
                NodeGridRectangleOrganizer.OrientationTypes returnValue = (NodeGridRectangleOrganizer.OrientationTypes)WallTypeParam.Val;

                return returnValue;
            }
            set
            {
                WallTypeParam.Val = (int)value;
                ApplyBuildParameters();
            }
        }

        public override void Setup(IBaseObject linkedFloor)
        {
            base.Setup(linkedFloor);

            BottomLeftPositionParam = new MailboxLineVector2Int(name: "Bottom Left Position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            TopRightPositionParam = new MailboxLineVector2Int(name: "Top Right Position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            BottomHeightParam = new MailboxLineRanged(name: "Bottom height [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 500, Min: 0, DefaultValue: 0.75f);
            WindowHeightParam = new MailboxLineRanged(name: "Window height [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 500, Min: 0, DefaultValue: 1.25f);
            FrontWallMaterialParm = new MailboxLineMaterial(name: "Front wall material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultPlaster);
            BackWallMaterialParm = new MailboxLineMaterial(name: "Back wall material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultPlaster);
            FrameMaterialParam = new MailboxLineMaterial(name: "Frame material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultWoodSolid);
            GlassMaterialParam = new MailboxLineMaterial(name: "Glass material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultGlass);

            SetupWallTypeParam();

            NodeGridPositionModificationNode firstNode = ModificationNodeLibrary.NewNodeGridPositionModificationNode;
            firstNode.Setup(linkedObject: this, value: BottomLeftPositionParam);
            FirstPositionNode = firstNode;

            NodeGridPositionModificationNode secondNode = ModificationNodeLibrary.NewNodeGridPositionModificationNode;
            secondNode.Setup(linkedObject: this, value: TopRightPositionParam, relativeReferenceHolder: BottomLeftPositionParam);
            SecondPositionNode = secondNode;

            ModificationNodeOrganizer = new NodeGridRectangleOrganizer(linkedObject: this, firstNode: firstNode, secondNode: secondNode);

            SetupEditButtons();

            CurrentRectangularBaseWindow.Setup(mainObject: this, frameMaterial: FrameMaterialParam, glassMaterial: GlassMaterialParam);
            CurrentRectangularBaseWindow.FrontMaterial = FrontWallMaterialParm;
            CurrentRectangularBaseWindow.BackMaterial = BackWallMaterialParm;

            UnmanagedMeshes.Clear();
            UnmanagedMeshes.AddRange(CurrentRectangularBaseWindow.UnmanagedStaticMeshes);
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

        List<TriangleMeshInfo> triangleInfo;

        public void AddStaticMesh(TriangleMeshInfo staticMesh)
        {
            triangleInfo.Add(staticMesh);
        }

        public override void ApplyBuildParameters()
        {
            ModificationNodeOrganizer.OrientationType = WallType;
            ModificationNodeOrganizer.SetLinkedObjectPositionAndOrientation(raiseToFloor: false);

            Vector2Int gridSize = ModificationNodeOrganizer.ObjectOrientationGridSize;

            if (gridSize.x == 0)
            {
                return;
            }

            switch (WallType)
            {
                case NodeGridRectangleOrganizer.OrientationTypes.BlockGrid:
                    CurrentRectangularBaseWindow.completeWidth = ModificationNodeOrganizer.ObjectOrientationSize.x;
                    break;
                case NodeGridRectangleOrganizer.OrientationTypes.NodeGrid:
                    CurrentRectangularBaseWindow.completeWidth = ModificationNodeOrganizer.ParentOrientationSize.magnitude;
                    break;
                default:
                    Debug.LogWarning("Error, enum case not defined");
                    break;
            }

            triangleInfo = new List<TriangleMeshInfo>();

            float currentHeight = LinkedFloor.CompleteFloorHeight;
            float minWindowHeight = BottomHeight + WindowHeight;

            int currentFloorNumber = LinkedFloor.FloorNumber;

            while (currentFloorNumber + 1 <= LinkedFloor.LinkedBuildingController.PositiveFloors && currentHeight < minWindowHeight)
            {
                currentFloorNumber += 1;
                currentHeight += LinkedFloor.LinkedBuildingController.Floor(floorNumber: currentFloorNumber).CompleteFloorHeight;
            }

            float bottomHeightWithFloor = BottomHeight;
            if (ModificationNodeOrganizer.RaiseDueToFloor)
            {
                bottomHeightWithFloor += LinkedFloor.BottomFloorHeight;
            }

            CurrentRectangularBaseWindow.completeHeight = currentHeight;
            CurrentRectangularBaseWindow.bottomHeight = bottomHeightWithFloor;
            CurrentRectangularBaseWindow.windowHeight = WindowHeight;

            CurrentRectangularBaseWindow.ApplyBuildParameters(linkedWindow: this, originObject: LinkedFloor.LinkedBuildingController.transform);

            if (gridSize.y == 0 || WallType == NodeGridRectangleOrganizer.OrientationTypes.NodeGrid)
            {
                CurrentRectangularBaseWindow.betweenDepth = LinkedFloor.CurrentNodeWallSystem.WallThickness + MathHelper.SmallFloat;
                CurrentRectangularBaseWindow.transform.localPosition = Vector3.back * LinkedFloor.CurrentNodeWallSystem.HalfWallThickness;

                foreach (TriangleMeshInfo info in triangleInfo)
                {
                    info.Move(Vector3.back * LinkedFloor.CurrentNodeWallSystem.HalfWallThickness);
                }
            }
            else
            {
                CurrentRectangularBaseWindow.betweenDepth = ModificationNodeOrganizer.ObjectOrientationSize.y;
                CurrentRectangularBaseWindow.transform.localPosition = Vector3.zero;
            }

            foreach (TriangleMeshInfo info in triangleInfo)
            {
                StaticMeshManager.AddTriangleInfo(info);
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