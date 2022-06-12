using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class Door : OnFloorObject
    {
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

        NodeWallSystem LinkedNodeWallSystem;
        DummyNodeWall NodeWallRegister;

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

        public override bool RaiseToFloor
        {
            get
            {
                return true;
            }
        }

        public float CompleteHeight
        {
            get
            {
                float currentHeight = LinkedFloor.WallBetweenHeight;

                int currentFloorNumber = LinkedFloor.FloorNumber;

                while (currentFloorNumber + 1 <= LinkedFloor.LinkedBuildingController.PositiveFloors && currentHeight < DoorHeight)
                {
                    currentFloorNumber += 1;
                    currentHeight += LinkedFloor.LinkedBuildingController.Floor(floorNumber: currentFloorNumber).CompleteFloorHeight;
                }

                return currentHeight;
            }
        }

        public override float ModificationNodeHeight
        {
            get
            {
                return CompleteHeight;
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

            CurrentRectangularBaseDoor.FrontMaterial = FrontWallMaterialParm;
            CurrentRectangularBaseDoor.BackMaterial = BackWallMaterialParm;
            CurrentRectangularBaseDoor.FrameMaterial = FrameMaterialParam;

            CurrentRectangularBaseDoor.Setup(linkedDoor: this);

            LinkedNodeWallSystem = LinkedFloor.CurrentNodeWallSystem;
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
            ModificationNodeOrganizer.OrientationType = WallType;
            
            base.ApplyBuildParameters();

            //Check validity
            if (Failed) return;
            
            Vector2Int gridSize = ModificationNodeOrganizer.ObjectOrientationGridSize;

            if (gridSize.y == 0)
            {
                Failed = true;
                return;
            }

            //Get mesh
            CurrentRectangularBaseDoor.LinkedDoorController = this;


            //Size
            Vector2 size = ModificationNodeOrganizer.ObjectOrientationSize;

            float width;
            float depth;

            if (gridSize.x == 0)
            {
                width = size.y;
                depth = LinkedFloor.CurrentNodeWallSystem.WallThickness;
            }
            else if (WallType == NodeGridRectangleOrganizer.OrientationTypes.NodeGrid)
            {
                width = size.magnitude;
                depth = LinkedFloor.CurrentNodeWallSystem.WallThickness;
            }
            else
            {
                width = size.y;
                depth = size.x;
            }

            //Height
            float completeHeight = CompleteHeight;

            float doorHeightWithFloor = DoorHeight;

            CurrentRectangularBaseDoor.completeWidth = width;
            CurrentRectangularBaseDoor.betweenDepth = depth;

            if (doorHeightWithFloor < completeHeight)
            {
                CurrentRectangularBaseDoor.doorHeight = doorHeightWithFloor;
                CurrentRectangularBaseDoor.completeHeight = completeHeight;
            }
            else
            {
                CurrentRectangularBaseDoor.doorHeight = completeHeight;
                CurrentRectangularBaseDoor.completeHeight = 0;
            }

            if (gridSize.x == 0 || WallType == NodeGridRectangleOrganizer.OrientationTypes.NodeGrid)
            {
                CurrentRectangularBaseDoor.transform.localPosition = Vector3.left * LinkedFloor.CurrentNodeWallSystem.HalfWallThickness;
            }
            else
            {
                CurrentRectangularBaseDoor.transform.localPosition = Vector3.zero;
            }

            //Apply parameters
            AssistObjectManager.ValueContainer BaseDoorInfo = CurrentRectangularBaseDoor.ApplyBuildParameters(LinkedFloor.LinkedBuildingController.transform);

            List<TriangleMeshInfo> DoorInfo = BaseDoorInfo.ConvertedStaticMeshes;

            foreach (TriangleMeshInfo info in DoorInfo)
            {
                StaticMeshManager.AddTriangleInfoIfValid(info);
            }

            BuildAllMeshes();

            UpdateNodeWallRegister();
        }

        void UpdateNodeWallRegister()
        {
            if (WallType == NodeGridRectangleOrganizer.OrientationTypes.NodeGrid || ModificationNodeOrganizer.ObjectOrientationGridSize.x == 0)
            {
                if (NodeWallRegister == null)
                {
                    NodeWallRegister = new DummyNodeWall(startPosition: ModificationNodeOrganizer.FirstCoordinate, endPosition: ModificationNodeOrganizer.SecondCoordinate, cornerMaterial: FrontWallMaterialParm, linkedObject: this);
                    LinkedNodeWallSystem.DummyNodeWalls.Add(NodeWallRegister);
                }
                else
                {
                    NodeWallRegister.StartPosition = ModificationNodeOrganizer.FirstCoordinate;
                    NodeWallRegister.EndPosition = ModificationNodeOrganizer.SecondCoordinate;
                }

                LinkedNodeWallSystem.ApplyBuildParameters(); //ApplyBuildParameters on NodeWallSystem is ignored when updating the entire floor
            }
            else
            {
                RemoveNodeWallRegister();
            }
        }

        public override void DestroyObject()
        {
            if (LinkedNodeWallSystem != null) RemoveNodeWallRegister();

            base.DestroyObject();
        }

        void RemoveNodeWallRegister()
        {
            if (NodeWallRegister != null)
            {
                LinkedNodeWallSystem.DummyNodeWalls.Remove(NodeWallRegister);
                NodeWallRegister = null;
            }
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