using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class RectangularStairs : OnFloorObject
    {
        //Build parameters
        MailboxLineVector2Int BottomLeftPositionParam;
        MailboxLineVector2Int TopRightPositionParam;
        MailboxLineRanged StepHeightTargetParam;
        MailboxLineRanged BackThicknessParam;
        MailboxLineDistinctNamed TopCutoffTypeParam;
        MailboxLineDistinctUnnamed NumberOfFloorsParam;
        MailboxLineMaterial TopSetpTopMaterialParam;
        MailboxLineMaterial OtherSetpsTopMaterialParam;
        MailboxLineMaterial SetpsFrontMaterialParam;
        MailboxLineMaterial SideMaterialParam;
        MailboxLineMaterial BackMaterialParam;

        NodeGridRectangleOrganizer ModificationNodeOrganizer;

        public override ModificationOrganizer Organizer
        {
            get
            {
                return ModificationNodeOrganizer;
            }
        }

        public TopCutoffTypes TopCutoffType
        {
            get
            {
                return (TopCutoffTypes)TopCutoffTypeParam.Val;
            }
            set
            {
                TopCutoffTypeParam.Val = (int)value;

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


        void SetupCuttofTypeParam()
        {
            List<string> enumString = new List<string>();

            int enumValues = System.Enum.GetValues(typeof(TopCutoffTypes)).Length;

            for (int i = 0; i < enumValues; i++)
            {
                TopCutoffTypes type = (TopCutoffTypes)i;

                enumString.Add(type.ToString());
            }

            TopCutoffTypeParam = new MailboxLineDistinctNamed(
                "Top cutoff type",
                CurrentMailbox,
                Mailbox.ValueType.buildParameter,
                enumString,
                0);
        }

        public enum TopCutoffTypes
        {
            Vertical,
            Horizontal
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

        public float StepHeightTarget
        {
            get
            {
                return StepHeightTargetParam.Val;
            }
            set
            {
                StepHeightTargetParam.Val = value;
                ApplyBuildParameters();
            }
        }

        public float BackThickness
        {
            get
            {
                return BackThicknessParam.Val;
            }
            set
            {
                BackThicknessParam.Val = value;
                ApplyBuildParameters();
            }
        }

        //Derived parameters
        float completeFloorHeight
        {
            get
            {
                return LinkedFloor.CompleteFloorHeight;
            }
        }


        void InitializeBuildParameterLines()
        {
            BottomLeftPositionParam = new MailboxLineVector2Int(name: "Bottom left position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            TopRightPositionParam = new MailboxLineVector2Int(name: "Top right position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            StepHeightTargetParam = new MailboxLineRanged(name: "Step height [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 2f, Min: 0.1f, DefaultValue: 0.25f);
            BackThicknessParam = new MailboxLineRanged(name: "Back thickness [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 2f, Min: 0.1f, DefaultValue: 0.25f);
            NumberOfFloorsParam = new MailboxLineDistinctUnnamed(name: "Number of floors", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 100, Min: 1, DefaultValue: 1);

            TopSetpTopMaterialParam = new MailboxLineMaterial(name: "Top step top material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultWoodPlanks);
            OtherSetpsTopMaterialParam = new MailboxLineMaterial(name: "Other steps top material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultWoodSolid);
            SetpsFrontMaterialParam = new MailboxLineMaterial(name: "Steps front material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultPlaster);
            SideMaterialParam = new MailboxLineMaterial(name: "Side material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultPlaster);
            BackMaterialParam = new MailboxLineMaterial(name: "Back material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultPlaster);

            SetupCuttofTypeParam();
        }

        public override void Setup(IBaseObject linkedFloor)
        {
            base.Setup(linkedFloor);

            LinkedFloor = linkedFloor as FloorController;

            InitializeBuildParameterLines();

            NodeGridPositionModificationNode firstNode = ModificationNodeLibrary.NewNodeGridPositionModificationNode;
            firstNode.Setup(linkedObject: this, value: BottomLeftPositionParam);
            FirstPositionNode = firstNode;

            NodeGridPositionModificationNode secondNode = ModificationNodeLibrary.NewNodeGridPositionModificationNode;
            secondNode.Setup(linkedObject: this, value: TopRightPositionParam);
            SecondPositionNode = secondNode;

            ModificationNodeOrganizer = new NodeGridRectangleOrganizer(linkedObject: this, firstNode: firstNode, secondNode: secondNode);

            SetupEditButtons();
        }

        public override void ResetObject()
        {
            baseReset();
        }

        public override void ApplyBuildParameters()
        {
            Failed = false;

            TriangleMeshInfo StepFront = new TriangleMeshInfo();
            TriangleMeshInfo TopStepTop = new TriangleMeshInfo();
            TriangleMeshInfo StepTop = new TriangleMeshInfo();
            TriangleMeshInfo LeftSide = new TriangleMeshInfo();
            TriangleMeshInfo RightSide = new TriangleMeshInfo();
            TriangleMeshInfo BackFaceAngle = new TriangleMeshInfo();
            TriangleMeshInfo BackFaceVertical = new TriangleMeshInfo();
            TriangleMeshInfo BottomFrontFace = new TriangleMeshInfo();
            TriangleMeshInfo TopCollider = new TriangleMeshInfo();

            void FinishMesh()
            {
                StepFront.MaterialReference = SetpsFrontMaterialParam;
                StepTop.MaterialReference = OtherSetpsTopMaterialParam;
                TopStepTop.MaterialReference = TopSetpTopMaterialParam;
                LeftSide.MaterialReference = SideMaterialParam;
                RightSide.MaterialReference = SideMaterialParam;
                BackFaceAngle.MaterialReference = BackMaterialParam;
                BackFaceVertical.MaterialReference = BackMaterialParam;
                BottomFrontFace.MaterialReference = SetpsFrontMaterialParam;
                TopCollider.AlternativeMaterial = DefaultCastleMaterials.InvisibleMaterial.LinkedMaterial;
                //TopCollider.AlternativeMaterial = DefaultCastleMaterials.InvisibleMaterial.LinkedMaterial;

                StepFront.ActiveCollider = false;
                StepTop.ActiveCollider = false;

                StepFront.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.LinkedBuildingController.transform);
                TopStepTop.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.LinkedBuildingController.transform);
                StepTop.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.LinkedBuildingController.transform);
                LeftSide.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.LinkedBuildingController.transform);
                RightSide.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.LinkedBuildingController.transform);
                //BackFaceAngle.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.LinkedBuildingController.transform);
                BackFaceVertical.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.LinkedBuildingController.transform);
                BottomFrontFace.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.LinkedBuildingController.transform);

                StaticMeshManager.AddTriangleInfoIfValid(StepFront);
                StaticMeshManager.AddTriangleInfoIfValid(TopStepTop);
                StaticMeshManager.AddTriangleInfoIfValid(StepTop);
                StaticMeshManager.AddTriangleInfoIfValid(LeftSide);
                StaticMeshManager.AddTriangleInfoIfValid(RightSide);
                StaticMeshManager.AddTriangleInfoIfValid(BackFaceAngle);
                StaticMeshManager.AddTriangleInfoIfValid(BackFaceVertical);
                StaticMeshManager.AddTriangleInfoIfValid(BottomFrontFace);
                StaticMeshManager.AddTriangleInfoIfValid(TopCollider);

                BuildAllMeshes();
            }

            ModificationNodeOrganizer.SetLinkedObjectPositionAndOrientation(raiseToFloor: false);
            if (Failed) return;

            if(ModificationNodeOrganizer.ObjectOrientationGridSize.x == 0)
            {
                Failed = true;
                return;
            }

            Vector2 size = ModificationNodeOrganizer.ObjectOrientationSize;

            //Setup containers

            //Calculate basic parameters
            float stairHeight = LinkedFloor.WallBetweenHeight;

            float topFloorHeight;

            if (LinkedFloor.FloorsAbove <= 0) topFloorHeight = LinkedFloor.BottomFloorHeight;
            else
            {


                if (NumberOfFloors == 1)
                {
                    topFloorHeight = LinkedFloor.LinkedBuildingController.Floor(LinkedFloor.FloorNumber + 1).BottomFloorHeight;
                }
                else
                {
                    int usedNumberOfFloors = NumberOfFloors;

                    usedNumberOfFloors = Mathf.Clamp(value: usedNumberOfFloors, min: 1, max: LinkedFloor.FloorsAbove + 1);

                    topFloorHeight = LinkedFloor.LinkedBuildingController.Floor(LinkedFloor.FloorNumber + usedNumberOfFloors - 1).BottomFloorHeight;

                    for (int i = LinkedFloor.FloorNumber + 1; i < LinkedFloor.FloorNumber + usedNumberOfFloors; i++)
                    {
                        FloorController floor = LinkedFloor.LinkedBuildingController.Floor(i);

                        stairHeight += floor.CompleteFloorHeight;
                    }
                }
            }

            stairHeight += topFloorHeight;

            //Build mesh
            float length = size.y;
            float width = size.x;
            //float slopeRatio = stairHeight / length;
            int numberOfSteps = Mathf.RoundToInt(stairHeight / StepHeightTarget);
            float stepHeight = stairHeight / numberOfSteps;
            float stepLength = length / numberOfSteps;

            TriangleMeshInfo frontStepTemplate = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.right * width, secondLine: Vector3.up * stepHeight, UVOffset: Vector2.zero);
            TriangleMeshInfo topStepInfo = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.right * width, secondLine: Vector3.forward * stepLength, UVOffset: Vector2.zero);
            Vector3 stepOffset = new Vector3(0, stepHeight, stepLength);

            //Move for the floor offset
            frontStepTemplate.Move(Vector3.up * LinkedFloor.BottomFloorHeight);
            topStepInfo.Move(Vector3.up * (LinkedFloor.BottomFloorHeight + stepHeight));

            //Front steps
            for (int stepNumber = 0; stepNumber < numberOfSteps; stepNumber++)
            {
                StepFront.Add(frontStepTemplate.Clone);
                frontStepTemplate.Move(stepOffset);
            }

            //TopSteps -> 1 less due to special Top step top
            for (int stepNumber = 0; stepNumber < numberOfSteps - 1; stepNumber++)
            {
                StepTop.Add(topStepInfo);
                topStepInfo.Move(stepOffset);
            }

            //TopStepTop
            TopStepTop.Add(topStepInfo);

            //End faces
            BottomFrontFace.Add(MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.right * width, secondLine: Vector3.up * LinkedFloor.BottomFloorHeight, UVOffset: Vector2.zero));

            Vector3 TopBackEnd = new Vector3(0, stairHeight - topFloorHeight + LinkedFloor.BottomFloorHeight, length);

            BackFaceVertical.Add(MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.right * width, secondLine: Vector3.up * topFloorHeight, UVOffset: Vector2.zero));
            BackFaceVertical.Move(TopBackEnd);
            BackFaceVertical.FlipTriangles();

            BackFaceAngle.Add(MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.right * width, secondLine: TopBackEnd, UVOffset: Vector2.zero));
            BackFaceAngle.FlipTriangles();

            //Side faces
            List<Vector3> SidePoints = new List<Vector3>();

            SidePoints.Add(Vector3.zero);

            for (int stepNumber = 1; stepNumber <= numberOfSteps; stepNumber++)
            {
                SidePoints.Add(new Vector3(0, stepHeight * stepNumber + LinkedFloor.BottomFloorHeight, stepLength * stepNumber));
            }

            RightSide.Add(MeshGenerator.FilledShapes.PointsClockwiseAroundStartPoint(startPoint: TopBackEnd, points: SidePoints));

            //Invisible collider
            TopCollider.Add(MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.right * width, secondLine: new Vector3(0, stepHeight * numberOfSteps, length - stepLength), UVOffset: Vector3.zero));
            TopCollider.Move(Vector3.up * LinkedFloor.BottomFloorHeight);

            //Add step triangles
            for (int i = 0; i < SidePoints.Count; i++)
            {
                SidePoints[i] += Vector3.up * stepHeight;
            }

            SidePoints[0] += Vector3.up * LinkedFloor.BottomFloorHeight;

            int originalVerticiesCount = RightSide.VerticesHolder.Count;

            foreach (Vector3 point in SidePoints)
            {
                RightSide.VerticesHolder.Add(point);
            }

            for (int i = 0; i < numberOfSteps; i++)
            {
                RightSide.Triangles.Add(new TriangleHolder(
                    triangle1: i + 1,
                    triangle2: originalVerticiesCount + i, //Also works:   triangle2: numberOfSteps + 2 + i,
                    triangle3: i + 2));
            }

            LeftSide = RightSide.CloneFlipped;
            RightSide.Move(Vector3.right * width);

            //Finish mesh
            FinishMesh();
        }

        public override void InternalUpdate()
        {
            NonOrderedInternalUpdate();
        }

        public override void PlaytimeUpdate()
        {
            NonOrderedPlaytimeUpdate();
        }

        void SetupEditButtons()
        {
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Flip", delegate { ModificationNodeOrganizer.FlipDiagonally(); }));
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Rotate Counter-Clockwise", delegate { ModificationNodeOrganizer.RotateCounterClockwise(); }));
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Rotate Clockwise", delegate { ModificationNodeOrganizer.RotateClockwise(); }));
        }

        public override void MoveOnGrid(Vector2Int offset)
        {
            ModificationNodeOrganizer.MoveOnGrid(offset: offset);
        }
    }
}