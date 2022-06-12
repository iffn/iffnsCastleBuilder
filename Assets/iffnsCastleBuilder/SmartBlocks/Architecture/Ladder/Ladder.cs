using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class Ladder : OnFloorObject
    {
        //BuildParameters
        MailboxLineVector2Int BottomLeftPositionParam;
        MailboxLineVector2Int TopRightPositionParam;
        MailboxLineDistinctUnnamed NumberOfFloorsParam;
        MailboxLineRanged HeightOvershootParam;
        MailboxLineRanged DistanceBetweenStepsParam;

        MailboxLineMaterial EdgeMaterialParam;
        MailboxLineMaterial StepMaterialParam;

        NodeGridWallOrganizer ModificationNodeOrganizer;

        [SerializeField] float SideThickness = 0.05f;
        [SerializeField] float StepThickness = 0.05f;

        public override bool RaiseToFloor
        {
            get
            {
                return true;
            }
        }

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

        public int NumberOfFloors
        {
            get
            {
                return NumberOfFloorsParam.Val;
            }
            set
            {
                NumberOfFloorsParam.Val = value;
                ApplyBuildParameters();
            }
        }

        public float HeightOvershoot
        {
            get
            {
                return HeightOvershootParam.Val;
            }
            set
            {
                HeightOvershootParam.Val = value;
                ApplyBuildParameters();
            }
        }

        public float DistanceBetweenSteps
        {
            get
            {
                return DistanceBetweenStepsParam.Val;
            }
            set
            {
                DistanceBetweenStepsParam.Val = value;
                ApplyBuildParameters();
            }
        }

        float CompleteHeight
        {
            get
            {
                float height = LinkedFloor.CompleteFloorHeight; //ToDo: Multi floor ladder

                if (LinkedFloor.FloorsAbove >= 0)
                {
                    if (NumberOfFloors > 1)
                    {
                        int usedNumberOfFloors = NumberOfFloors;

                        if (usedNumberOfFloors > LinkedFloor.FloorsAbove + 1) usedNumberOfFloors = LinkedFloor.FloorsAbove + 1;

                        for (int i = LinkedFloor.FloorNumber + 1; i < LinkedFloor.FloorNumber + usedNumberOfFloors; i++)
                        {
                            FloorController floor = LinkedFloor.LinkedBuildingController.Floor(i);

                            height += floor.CompleteFloorHeight;
                        }
                    }
                }

                return height + HeightOvershoot;
            }
        }

        public override float ModificationNodeHeight
        {
            get
            {
                return CompleteHeight + LinkedFloor.BottomFloorHeight;
            }
        }

        public override void Setup(IBaseObject linkedFloor)
        {
            base.Setup(linkedFloor);

            IsStructural = false;

            BottomLeftPositionParam = new MailboxLineVector2Int(name: "Bottom Left Position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            TopRightPositionParam = new MailboxLineVector2Int(name: "Top Right Position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            NumberOfFloorsParam = new MailboxLineDistinctUnnamed(name: "Number of floors", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 100, Min: 1, DefaultValue: 1);
            HeightOvershootParam = new MailboxLineRanged(name: "Height overshoot [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 5, Min: 0, DefaultValue: 1.1f);
            DistanceBetweenStepsParam = new MailboxLineRanged(name: "Distance between steps [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 2f, Min: 0.1f, DefaultValue: 0.5f);

            EdgeMaterialParam = new MailboxLineMaterial(name: "Edge material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultWoodSolid);
            StepMaterialParam = new MailboxLineMaterial(name: "Step material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultWoodSolid);

            NodeGridPositionModificationNode firstNode = ModificationNodeLibrary.NewNodeGridPositionModificationNode;
            firstNode.Setup(linkedObject: this, value: BottomLeftPositionParam);
            FirstPositionNode = firstNode;

            NodeGridPositionModificationNode secondNode = ModificationNodeLibrary.NewNodeGridPositionModificationNode;
            secondNode.Setup(linkedObject: this, value: TopRightPositionParam, relativeReferenceHolder: BottomLeftPositionParam);
            SecondPositionNode = secondNode;

            ModificationNodeOrganizer = new NodeGridWallOrganizer(linkedObject: this, firstNode: firstNode, secondNode: secondNode);

            SetupEditButtons();

            //UnmanagedMeshes.Clear();
            //UnmanagedMeshes.AddRange(LinkedBaseLadder.UnmanagedStaticMeshes);
        }

        public void CompleteSetupWithBuildParameters(FloorController linkedFloor, Vector2Int bottomLeftPosition, Vector2Int topRightPosition)
        {
            Setup(linkedFloor);

            BottomLeftPositionParam.Val = bottomLeftPosition;
            TopRightPositionParam.Val = topRightPosition;

            //LinkedBaseLadder.Setup(mainObject: this, edgeMaterial: EdgeMaterialParam, stepMaterial: StepMaterialParam);
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

        public void AddStaticMesh(TriangleMeshInfo staticMesh)
        {
            if (staticMesh == null) return;

            StaticMeshManager.AddTriangleInfoIfValid(staticMesh);
        }

        public override void ApplyBuildParameters()
        {
            base.ApplyBuildParameters();

            //Check validity
            if (Failed) return;

            Vector2Int gridSize = ModificationNodeOrganizer.ParentOrientationGridSize;

            if (gridSize.x == 0 && gridSize.y == 0)
            {
                Failed = true;
                return;
            }

            //Define mesh
            float width = ModificationNodeOrganizer.ObjectOrientationSize;

            float completeHeight = CompleteHeight;
            float height = completeHeight - HeightOvershoot;

            TriangleMeshInfo OriginSide = new();
            TriangleMeshInfo OtherSide = new();
            TriangleMeshInfo Steps = new();

            void FinishMesh()
            {
                OriginSide.MaterialReference = EdgeMaterialParam;
                OtherSide.MaterialReference = EdgeMaterialParam;
                Steps.MaterialReference = StepMaterialParam;

                AddStaticMesh(OriginSide);
                AddStaticMesh(OtherSide);
                AddStaticMesh(Steps);

                BuildAllMeshes();
            }

            OriginSide = MeshGenerator.FilledShapes.BoxAroundCenter(size: new Vector3(SideThickness, completeHeight, SideThickness));
            OtherSide = OriginSide.Clone;

            OriginSide.Move(new Vector3((-width + SideThickness) * 0.5f, completeHeight * 0.5f, 0));
            OtherSide.Move(new Vector3((width - SideThickness) * 0.5f, completeHeight * 0.5f, 0));

            int numberOfSteps = (int)(height / DistanceBetweenSteps);

            TriangleMeshInfo baseStep = MeshGenerator.FilledShapes.CylinderAroundCenterWithoutCap(radius: StepThickness * 0.5f, length: width, direction: Vector3.right, numberOfEdges: 12);

            //baseStep.Move(width * 0.5f * Vector3.left);

            for (int i = 0; i < numberOfSteps; i++)
            {
                baseStep.Move(DistanceBetweenSteps * Vector3.up);
                Steps.Add(baseStep.Clone);
            }

            FinishMesh();

            //LinkedBaseLadder.SetMainParameters(width: size, height: LinkedFloor.CompleteFloorHeight);

            /*
            UnmanagedMeshes.Clear();
            UnmanagedMeshes.AddRange(LinkedBaseLadder.UnmanagedStaticMeshes);
            */
        }

        void SetupEditButtons()
        {

        }

        public override void MoveOnGrid(Vector2Int offset)
        {
            ModificationNodeOrganizer.MoveOnGrid(offset: offset);
        }
    }
}