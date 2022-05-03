using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class Door : OnFloorObject
    {
        public const string constIdentifierString = "Door";
        public override string IdentifierString
        {
            get
            {
                return constIdentifierString;
            }
        }

        [SerializeField] RectangularBaseDoor CurrentRectangularBaseDoor;

        //BuildParameters
        MailboxLineVector2Int BottomLeftPositionParam;
        MailboxLineVector2Int TopRightPositionParam;
        MailboxLineRanged DoorHeightParam;
        MailboxLineDistinctNamed WallTypeParam;
        MailboxLineMaterial FrontWallMaterialParm;
        MailboxLineMaterial BackWallMaterialParm;
        MailboxLineMaterial FrameMaterialParam;

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

        public float DoorHeight
        {
            get
            {
                return DoorHeightParam.Val;
            }
            set
            {
                DoorHeightParam.Val = value;
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
            DoorHeightParam = new MailboxLineRanged(name: "Door height [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 20, Min: 0.1f, DefaultValue: 2);
            FrontWallMaterialParm = new MailboxLineMaterial(name: "Front wall material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultPlaster);
            BackWallMaterialParm = new MailboxLineMaterial(name: "Back wall material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultPlaster);
            FrameMaterialParam = new MailboxLineMaterial(name: "Frame material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultWoodSolid);

            SetupWallTypeParam();

            NodeGridPositionModificationNode firstNode = ModificationNodeLibrary.NewNodeGridPositionModificationNode;
            firstNode.Setup(linkedObject: this, value: BottomLeftPositionParam);
            FirstPositionNode = firstNode;

            NodeGridPositionModificationNode secondNode = ModificationNodeLibrary.NewNodeGridPositionModificationNode;
            secondNode.Setup(linkedObject: this, value: TopRightPositionParam, relativeReferenceHolder: BottomLeftPositionParam);
            SecondPositionNode = secondNode;

            ModificationNodeOrganizer = new NodeGridRectangleOrganizer(linkedObject: this, firstNode: firstNode, secondNode: secondNode);

            SetupEditButtons();

            UnmanagedMeshes.Clear();
            UnmanagedMeshes.AddRange(CurrentRectangularBaseDoor.UnmanagedStaticMeshes);

            CurrentRectangularBaseDoor.FrontMaterial = FrontWallMaterialParm;
            CurrentRectangularBaseDoor.BackMaterial = BackWallMaterialParm;

            CurrentRectangularBaseDoor.Setup(mainObject: this, frameMaterial: FrameMaterialParam);
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
            failed = false;
            CurrentRectangularBaseDoor.LinkedDoorController = this;

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
                    CurrentRectangularBaseDoor.completeWidth = ModificationNodeOrganizer.ObjectOrientationSize.x;
                    break;
                case NodeGridRectangleOrganizer.OrientationTypes.NodeGrid:
                    CurrentRectangularBaseDoor.completeWidth = ModificationNodeOrganizer.ParentOrientationSize.magnitude;
                    break;
                default:
                    Debug.LogWarning("Error, enum case not defined");
                    break;
            }

            float currentHeight = LinkedFloor.CompleteFloorHeight;

            int currentFloorNumber = LinkedFloor.FloorNumber;

            while (currentFloorNumber + 1 <= LinkedFloor.LinkedBuildingController.PositiveFloors && currentHeight < DoorHeight)
            {
                currentFloorNumber += 1;
                currentHeight += LinkedFloor.LinkedBuildingController.Floor(floorNumber: currentFloorNumber).CompleteFloorHeight;
            }

            float doorHeightWithFloor = DoorHeight;
            if (ModificationNodeOrganizer.RaiseDueToFloor)
            {
                doorHeightWithFloor += LinkedFloor.BottomFloorHeight;
            }

            if (doorHeightWithFloor < currentHeight)
            {
                CurrentRectangularBaseDoor.doorHeight = doorHeightWithFloor;
                CurrentRectangularBaseDoor.completeHeight = currentHeight;
            }
            else
            {
                CurrentRectangularBaseDoor.doorHeight = currentHeight;
                CurrentRectangularBaseDoor.completeHeight = 0;
            }

            triangleInfo = new List<TriangleMeshInfo>();

            CurrentRectangularBaseDoor.ApplyBuildParameters(LinkedFloor.LinkedBuildingController.transform);

            if (gridSize.y == 0 || WallType == NodeGridRectangleOrganizer.OrientationTypes.NodeGrid)
            {
                CurrentRectangularBaseDoor.betweenDepth = LinkedFloor.CurrentNodeWallSystem.WallThickness + MathHelper.SmallFloat;
                CurrentRectangularBaseDoor.transform.localPosition = Vector3.back * LinkedFloor.CurrentNodeWallSystem.HalfWallThickness;

                foreach (TriangleMeshInfo info in triangleInfo)
                {
                    info.Move(Vector3.back * LinkedFloor.CurrentNodeWallSystem.HalfWallThickness);
                }
            }
            else
            {
                CurrentRectangularBaseDoor.betweenDepth = ModificationNodeOrganizer.ObjectOrientationSize.y;
                CurrentRectangularBaseDoor.transform.localPosition = Vector3.zero;
            }

            foreach (TriangleMeshInfo info in triangleInfo)
            {
                StaticMeshManager.AddTriangleInfo(info);
            }

            CurrentRectangularBaseDoor.FrameMaterail = FrameMaterialParam.Val.LinkedMaterial;

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