using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class Triangle : OnFloorObject
    {
        //BuildParameters
        MailboxLineVector2Int FirstPositionParam;
        MailboxLineVector2Int SecondPositionParam;
        MailboxLineVector2Int ThirdPositionParam;
        MailboxLineDistinctNamed BlockTypeParam;
        MailboxLineMaterial TopMaterial;
        MailboxLineMaterial BottomMaterial;
        MailboxLineMaterial MaterialWall12;
        MailboxLineMaterial MaterialWall23;
        MailboxLineMaterial MaterialWall31;

        NodeGridTriangleOrganizer ModificationNodeOrganizer;

        public override GridModificationOrganizer Organizer
        {
            get
            {
                return ModificationNodeOrganizer;
            }
        }

        public enum BlockTypes
        {
            Wall,
            Floor
        }

        public BlockTypes BlockType
        {
            get
            {
                BlockTypes returnValue = (BlockTypes)BlockTypeParam.Val;

                return returnValue;
            }
            set
            {
                BlockTypeParam.Val = (int)value;
                ApplyBuildParameters();
            }
        }

        void SetupBlockTypeParam(BlockTypes blockType = BlockTypes.Floor)
        {
            List<string> enumString = new List<string>();

            int enumValues = System.Enum.GetValues(typeof(BlockTypes)).Length;

            for (int i = 0; i < enumValues; i++)
            {
                BlockTypes type = (BlockTypes)i;

                enumString.Add(type.ToString());
            }

            BlockTypeParam = new MailboxLineDistinctNamed(
                "Block type",
                CurrentMailbox,
                Mailbox.ValueType.buildParameter,
                enumString,
                (int)blockType);
        }

        public override void Setup(IBaseObject linkedFloor)
        {
            base.Setup(linkedFloor);

            FirstPositionParam = new MailboxLineVector2Int(name: "First position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            SecondPositionParam = new MailboxLineVector2Int(name: "Second position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            ThirdPositionParam = new MailboxLineVector2Int(name: "Third position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            SetupBlockTypeParam();

            TopMaterial = new MailboxLineMaterial(name: "Top material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultWoodPlanks);
            BottomMaterial = new MailboxLineMaterial(name: "Bottom material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultCeiling);
            MaterialWall12 = new MailboxLineMaterial(name: "Material Wall 1-2", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultPlaster);
            MaterialWall23 = new MailboxLineMaterial(name: "Material Wall 2-3", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultPlaster);
            MaterialWall31 = new MailboxLineMaterial(name: "Material Wall 3-1", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultPlaster);

            NodeGridPositionModificationNode firstNode = ModificationNodeLibrary.NewNodeGridPositionModificationNode;
            firstNode.Setup(linkedObject: this, value: FirstPositionParam);

            NodeGridPositionModificationNode secondNode = ModificationNodeLibrary.NewNodeGridPositionModificationNode;
            secondNode.Setup(linkedObject: this, value: SecondPositionParam, relativeReferenceHolder: null);

            NodeGridPositionModificationNode thirdNode = ModificationNodeLibrary.NewNodeGridPositionModificationNode;
            thirdNode.Setup(linkedObject: this, value: ThirdPositionParam, relativeReferenceHolder: null);

            FirstPositionNode = secondNode;
            SecondPositionNode = thirdNode;

            ModificationNodeOrganizer = new NodeGridTriangleOrganizer(linkedObject: this, firstNode: firstNode, secondNode: secondNode, thirdNode: thirdNode);

            SetupEditButtons();
        }

        public void CompleteSetupWithBuildParameters(FloorController linkedFloor, Vector2Int firstPosition, Vector2Int secondPosition, Vector2Int thirdPosition, BlockTypes blockType)
        {
            Setup(linkedFloor);

            FirstPositionParam.Val = firstPosition;
            SecondPositionParam.Val = secondPosition;
            ThirdPositionParam.Val = thirdPosition;

            BlockType = blockType;
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
            Failed = false;

            TriangleMeshInfo Top = new TriangleMeshInfo();
            TriangleMeshInfo Wall12 = new TriangleMeshInfo();
            TriangleMeshInfo Wall23 = new TriangleMeshInfo();
            TriangleMeshInfo Wall31 = new TriangleMeshInfo();
            TriangleMeshInfo Bottom = new TriangleMeshInfo();

            void FinishMeshes()
            {
                StaticMeshManager.AddTriangleInfoIfValid(Top);
                StaticMeshManager.AddTriangleInfoIfValid(Wall12);
                StaticMeshManager.AddTriangleInfoIfValid(Wall23);
                StaticMeshManager.AddTriangleInfoIfValid(Wall31);
                StaticMeshManager.AddTriangleInfoIfValid(Bottom);

                BuildAllMeshes();
            }

            ModificationNodeOrganizer.SetLinkedObjectPositionAndOrientation(raiseToFloor: false);

            if (Failed) return;

            float height;

            switch (BlockType)
            {
                case BlockTypes.Wall:
                    height = LinkedFloor.CompleteFloorHeight;
                    break;
                case BlockTypes.Floor:
                    height = LinkedFloor.BottomFloorHeight;
                    break;
                default:
                    height = LinkedFloor.BottomFloorHeight;
                    Debug.LogWarning("Error: Block type not defined");
                    break;
            }

            List<Vector3> ClockwisePoints = new List<Vector3>()
        {
            Vector3.zero,
            new Vector3(ModificationNodeOrganizer.FirstClockwiseOffsetPosition.x, 0, ModificationNodeOrganizer.FirstClockwiseOffsetPosition.y),
            new Vector3(ModificationNodeOrganizer.SecondClockwiseOffsetPosition.x, 0, ModificationNodeOrganizer.SecondClockwiseOffsetPosition.y)
        };

            Top = MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(ClockwisePoints);
            Bottom = Top.CloneFlipped;
            Top.Move(Vector3.up * height);

            Wall12 = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: ClockwisePoints[1], secondLine: Vector3.up * height, UVOffset: Vector2.zero);
            Wall12.FlipTriangles();
            Wall23 = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: ClockwisePoints[2] - ClockwisePoints[1], secondLine: Vector3.up * height, UVOffset: Vector2.zero);
            Wall23.FlipTriangles();
            Wall23.Move(ClockwisePoints[1]);
            Wall31 = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: ClockwisePoints[2], secondLine: Vector3.up * height, UVOffset: Vector2.zero);

            Top.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.LinkedBuildingController.transform);
            Wall12.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.LinkedBuildingController.transform);
            Wall23.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.LinkedBuildingController.transform);
            Wall31.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.LinkedBuildingController.transform);
            Bottom.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.LinkedBuildingController.transform);

            Top.MaterialReference = TopMaterial;
            Bottom.MaterialReference = BottomMaterial;
            Wall12.MaterialReference = MaterialWall12;
            Wall23.MaterialReference = MaterialWall23;
            Wall31.MaterialReference = MaterialWall31;

            FinishMeshes();
        }

        void SetupEditButtons()
        {
            /*
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Flip Diagonally", delegate { ModificationNodeOrganizer.FlipDiagonally(); }));
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Flip Vertical", delegate { ModificationNodeOrganizer.FlipVertical(); }));
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Flip Horizontal", delegate { ModificationNodeOrganizer.FlipHorizontal(); }));
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Rotate Counter-Clockwise", delegate { ModificationNodeOrganizer.RotateCounterClockwise(); }));
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Rotate Clockwise", delegate { ModificationNodeOrganizer.RotateClockwise(); }));
            */
        }

        public override void MoveOnGrid(Vector2Int offset)
        {
            ModificationNodeOrganizer.MoveOnGrid(offset: offset);
        }
    }
}