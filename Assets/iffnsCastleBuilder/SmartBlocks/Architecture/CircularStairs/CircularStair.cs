using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class CircularStair : OnFloorObject
    {
        //Build parameters
        MailboxLineVector2Int PositiveCenterPositionParam;
        MailboxLineRanged OuterRadiusParam;
        MailboxLineRanged InnerRadiusParam;
        MailboxLineRanged StartingAngleDegParam;
        MailboxLineRanged RevolutionAngleDegParam;
        MailboxLineRanged StepHeightTargetParam;
        MailboxLineRanged BackThicknessParam;
        MailboxLineDistinctUnnamed NumberOfFloorsParam;
        MailboxLineMaterial TopSetpTopMaterialParam;
        MailboxLineMaterial OtherSetpsTopMaterialParam;
        MailboxLineMaterial SetpsFrontMaterialParam;
        MailboxLineMaterial SideMaterialParam;
        MailboxLineMaterial BackMaterialParam;

        BlockGridRadiusOrganizer ModificationNodeOrganizer;

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

        public Vector2Int PositiveCenterPosition
        {
            get
            {
                return PositiveCenterPositionParam.Val;
            }
            set
            {
                PositiveCenterPositionParam.Val = value;
                ApplyBuildParameters();
            }
        }

        public float OuterRadius
        {
            get
            {
                return OuterRadiusParam.Val;
            }
            set
            {
                OuterRadiusParam.Val = value;
                ApplyBuildParameters();
            }
        }

        public float InnerRadius
        {
            get
            {
                return InnerRadiusParam.Val;
            }
            set
            {
                InnerRadiusParam.Val = value;
                ApplyBuildParameters();
            }
        }

        public float StartingAngleDeg
        {
            get
            {
                return StartingAngleDegParam.Val;
            }
            set
            {
                StartingAngleDegParam.Val = value;
                ApplyBuildParameters();
            }
        }

        public float RevolutionAngleDeg
        {
            get
            {
                return RevolutionAngleDegParam.Val;
            }
            set
            {
                RevolutionAngleDegParam.Val = value;
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

        public float BackThicknessHeight
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

        public float StairHeight
        {
            get
            {
                float returnValue = LinkedFloor.WallBetweenHeight;

                float topFloorHeight;

                if (LinkedFloor.FloorsAbove <= 0)
                {
                    topFloorHeight = LinkedFloor.BottomFloorHeight;
                }
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

                            returnValue += floor.CompleteFloorHeight;
                        }
                    }
                }

                returnValue += topFloorHeight;

                return returnValue;
            }
        }

        public override float ModificationNodeHeight
        {
            get
            {
                return StairHeight;
            }
        }
        
        void InitializeBuildParameterLines()
        {
            PositiveCenterPositionParam = new MailboxLineVector2Int(name: "Positive center position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            OuterRadiusParam = new MailboxLineRanged(name: "Outer radius [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 80, Min: 0.1f, DefaultValue: 1);
            InnerRadiusParam = new MailboxLineRanged(name: "Inner radius [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 79, Min: 0.01f, DefaultValue: 0.25f);
            StartingAngleDegParam = new MailboxLineRanged(name: "Starting angle [°]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 360, Min: 0, DefaultValue: 0);
            RevolutionAngleDegParam = new MailboxLineRanged(name: "Revolution angle [°]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 3600, Min: -3600, DefaultValue: 360);
            StepHeightTargetParam = new MailboxLineRanged(name: "Step height [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 2f, Min: 0.1f, DefaultValue: 0.25f);
            BackThicknessParam = new MailboxLineRanged(name: "Back thickness [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 2f, Min: 0.1f, DefaultValue: 0.25f);
            NumberOfFloorsParam = new MailboxLineDistinctUnnamed(name: "Number of floors", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 100, Min: 1, DefaultValue: 1);

            TopSetpTopMaterialParam = new MailboxLineMaterial(name: "Top step top material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultWoodPlanks);
            OtherSetpsTopMaterialParam = new MailboxLineMaterial(name: "Other steps top material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultWoodSolid);
            SetpsFrontMaterialParam = new MailboxLineMaterial(name: "Steps front material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultPlaster);
            SideMaterialParam = new MailboxLineMaterial(name: "Side material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultPlaster);
            BackMaterialParam = new MailboxLineMaterial(name: "Back material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultPlaster);
        }

        public override void Setup(IBaseObject linkedFloor)
        {
            base.Setup(linkedFloor);

            LinkedFloor = linkedFloor as FloorController;

            InitializeBuildParameterLines();

            BlockGridPositionModificationNode positionNode = ModificationNodeLibrary.NewBlockGridPositionModificationNode;
            positionNode.Setup(linkedObject: this, value: PositiveCenterPositionParam);
            FirstPositionNode = positionNode;


            RadiusModificationNode radiusNode = ModificationNodeLibrary.NewRadiusModificationNode;
            radiusNode.Setup(linkedObject: this, radiusValue: OuterRadiusParam, localCenter: Vector3.zero, axis: Vector3.up);
            SecondPositionNode = radiusNode;

            ModificationNodeOrganizer = new BlockGridRadiusOrganizer(linkedObject: this, positionNode: positionNode, radiusNode: radiusNode);

            HideModificationNodes();
        }

        public void CompleteSetUpWithBuildParameters(FloorController linkedFloor, Vector2Int PositiveCenterPosition, float OuterRadius = 1, float InnerRadius = 0.1f, float StartingAngleDeg = 0, float RevolutionAngleDeg = 360, float StepHeightTarget = 0.25f, float BackThickness = 0.25f)
        {
            Setup(linkedFloor);

            PositiveCenterPositionParam.Val = PositiveCenterPosition;
            OuterRadiusParam.Val = OuterRadius;
            InnerRadiusParam.Val = InnerRadius;
            StartingAngleDegParam.Val = StartingAngleDeg;
            RevolutionAngleDegParam.Val = RevolutionAngleDeg;
            StepHeightTargetParam.Val = StepHeightTarget;
            BackThicknessParam.Val = BackThickness;
        }

        public override void ResetObject()
        {
            baseReset();
        }

        public override void ApplyBuildParameters()
        {
            base.ApplyBuildParameters();

            //Check validity
            if (Failed) return;

            //Define mesh
            TriangleMeshInfo TopStepTop = new();
            TriangleMeshInfo StepTop = new();
            TriangleMeshInfo StepFront = new();
            TriangleMeshInfo OuterRadiiSide = new();
            TriangleMeshInfo InnerRadiiSide = new();
            TriangleMeshInfo BackFace = new();
            TriangleMeshInfo BackFaceTop = new();
            TriangleMeshInfo ColliderMesh;

            void FinishMeshes()
            {
                StepFront.MaterialReference = SetpsFrontMaterialParam;
                StepTop.MaterialReference = OtherSetpsTopMaterialParam;
                TopStepTop.MaterialReference = TopSetpTopMaterialParam;
                OuterRadiiSide.MaterialReference = SideMaterialParam;
                InnerRadiiSide.MaterialReference = SideMaterialParam;
                BackFace.MaterialReference = BackMaterialParam;
                BackFaceTop.MaterialReference = BackMaterialParam;
                ColliderMesh.AlternativeMaterial = DefaultCastleMaterials.InvisibleMaterial.LinkedMaterial;

                TopStepTop.FixUVCount();
                StepTop.FixUVCount();
                StepFront.FixUVCount();
                OuterRadiiSide.FixUVCount();
                InnerRadiiSide.FixUVCount();
                BackFace.FixUVCount();
                BackFaceTop.FixUVCount();
                ColliderMesh.FixUVCount();

                StepTop.ActiveCollider = TriangleMeshInfo.ColliderStates.VisbleWithoutCollider;
                StepFront.ActiveCollider = TriangleMeshInfo.ColliderStates.VisbleWithoutCollider;
                ColliderMesh.ActiveCollider = TriangleMeshInfo.ColliderStates.InvisibleCollider;

                StaticMeshManager.AddTriangleInfoIfValid(TopStepTop);
                StaticMeshManager.AddTriangleInfoIfValid(StepTop);
                StaticMeshManager.AddTriangleInfoIfValid(StepFront);
                StaticMeshManager.AddTriangleInfoIfValid(OuterRadiiSide);
                StaticMeshManager.AddTriangleInfoIfValid(InnerRadiiSide);
                StaticMeshManager.AddTriangleInfoIfValid(BackFace);
                StaticMeshManager.AddTriangleInfoIfValid(BackFaceTop);
                StaticMeshManager.AddTriangleInfoIfValid(ColliderMesh);

                BuildAllMeshes();
            }

            //transform.localPosition = LinkedFloor.NodePositionFromBlockIndex(PositiveCenterPosition);

            //Calculate basic parameters
            float height = StairHeight;

            int numberOfSteps = Mathf.RoundToInt(height / StepHeightTarget);
            float stepHeight = height / numberOfSteps;
            float stepAngleRad = RevolutionAngleDeg / numberOfSteps * Mathf.Deg2Rad;

            FloorController endFloor = LinkedFloor.LinkedBuildingController.Floor(floorNumber: LinkedFloor.FloorNumber + NumberOfFloors);

            for (int stepNumber = 0; stepNumber < numberOfSteps; stepNumber++)
            {
                //Angles
                float firstAngleRadiants = Mathf.Deg2Rad * StartingAngleDeg + stepAngleRad * stepNumber;
                float firstAngleSin = Mathf.Sin(firstAngleRadiants);
                float firstAngleCos = Mathf.Cos(firstAngleRadiants);

                float secondAngleRadiants = firstAngleRadiants + stepAngleRad;
                float secondAngleSin = Mathf.Sin(secondAngleRadiants);
                float secondAngleCos = Mathf.Cos(secondAngleRadiants);


                //Horizontal positions --> y = z in Vector2
                Vector2 firstInnerLocation = new Vector2(firstAngleCos, firstAngleSin) * InnerRadius;
                Vector2 secondInnerLocation = new Vector2(secondAngleCos, secondAngleSin) * InnerRadius;

                Vector2 firstOuterLocation = new Vector2(firstAngleCos, firstAngleSin) * OuterRadius;
                Vector2 secondOuterLocation = new Vector2(secondAngleCos, secondAngleSin) * OuterRadius;

                //Heights
                float baseHeight = stepHeight * stepNumber;
                float upperHeight = baseHeight + stepHeight;
                
                
                float backLowerHeight;
                float frontLowerHeight;

                if (stepNumber == 0)
                {
                    frontLowerHeight = - LinkedFloor.BottomFloorHeight;
                }
                else
                {
                    frontLowerHeight = baseHeight - BackThicknessHeight;
                }

                if (stepNumber == numberOfSteps - 1)
                {
                    backLowerHeight = upperHeight - endFloor.BottomFloorHeight;
                }
                else
                {
                    backLowerHeight = upperHeight - BackThicknessHeight;
                }

                //if (backLowerHeight < 0) backLowerHeight = 0;
                //if (frontLowerHeight < 0) frontLowerHeight = 0;

                if (stepNumber == 0)
                {
                    StepFront.Add(MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(new List<Vector3>(){
                        new Vector3(firstInnerLocation.x, -LinkedFloor.BottomFloorHeight, firstInnerLocation.y),
                        new Vector3(firstInnerLocation.x, upperHeight, firstInnerLocation.y),
                        new Vector3(firstOuterLocation.x, upperHeight, firstOuterLocation.y),
                        new Vector3(firstOuterLocation.x, -LinkedFloor.BottomFloorHeight, firstOuterLocation.y)
                    }));
                }
                else
                {
                    StepFront.Add(MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(new List<Vector3>(){
                        new Vector3(firstInnerLocation.x, baseHeight, firstInnerLocation.y),
                        new Vector3(firstInnerLocation.x, upperHeight, firstInnerLocation.y),
                        new Vector3(firstOuterLocation.x, upperHeight, firstOuterLocation.y),
                        new Vector3(firstOuterLocation.x, baseHeight, firstOuterLocation.y)
                    }));
                }

                if (stepNumber == numberOfSteps - 1)
                {
                    TopStepTop.Add(MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(new List<Vector3>(){
                        new Vector3(firstInnerLocation.x, upperHeight, firstInnerLocation.y),
                        new Vector3(secondInnerLocation.x, upperHeight, secondInnerLocation.y),
                        new Vector3(secondOuterLocation.x, upperHeight, secondOuterLocation.y),
                        new Vector3(firstOuterLocation.x, upperHeight, firstOuterLocation.y)
                        }));
                }
                else
                {
                    StepTop.Add(MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(new List<Vector3>(){
                        new Vector3(firstInnerLocation.x, upperHeight, firstInnerLocation.y),
                        new Vector3(secondInnerLocation.x, upperHeight, secondInnerLocation.y),
                        new Vector3(secondOuterLocation.x, upperHeight, secondOuterLocation.y),
                        new Vector3(firstOuterLocation.x, upperHeight, firstOuterLocation.y)
                        }));
                }

                OuterRadiiSide.Add(MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(new List<Vector3>(){
                        new Vector3(firstOuterLocation.x, upperHeight, firstOuterLocation.y),
                        new Vector3(secondOuterLocation.x, upperHeight, secondOuterLocation.y),
                        new Vector3(secondOuterLocation.x, backLowerHeight, secondOuterLocation.y),
                        new Vector3(firstOuterLocation.x, frontLowerHeight, firstOuterLocation.y),
                        }));

                InnerRadiiSide.Add(MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(new List<Vector3>(){
                        new Vector3(secondInnerLocation.x, backLowerHeight, secondInnerLocation.y),
                        new Vector3(secondInnerLocation.x, upperHeight, secondInnerLocation.y),
                        new Vector3(firstInnerLocation.x, upperHeight, firstInnerLocation.y),
                        new Vector3(firstInnerLocation.x, frontLowerHeight, firstInnerLocation.y),
                        }));

                if (stepNumber == 0)
                {
                    BackFace.VerticesHolder.Add(new Vector3(firstInnerLocation.x, frontLowerHeight, firstInnerLocation.y));
                    BackFace.VerticesHolder.Add(new Vector3(firstOuterLocation.x, frontLowerHeight, firstOuterLocation.y));
                }

                
                BackFace.Triangles.Add(new TriangleHolder(baseOffset: stepNumber * 2, t1: 0, t2: 1, t3: 3));
                BackFace.Triangles.Add(new TriangleHolder(baseOffset: stepNumber * 2, t1: 0, t2: 3, t3: 2));

                BackFace.VerticesHolder.Add(new Vector3(secondInnerLocation.x, backLowerHeight, secondInnerLocation.y));
                BackFace.VerticesHolder.Add(new Vector3(secondOuterLocation.x, backLowerHeight, secondOuterLocation.y));

                BackFace.UVs.Add(new Vector2(secondInnerLocation.x, secondInnerLocation.y));
                BackFace.UVs.Add(new Vector2(secondOuterLocation.x, secondOuterLocation.y));
            }

            ColliderMesh = BackFace.CloneFlipped;

            float scaleFactor = height / ColliderMesh.AllVerticesDirectly[^1].y;

            ColliderMesh.Scale(new Vector3(1, scaleFactor, 1));
            ColliderMesh.Rotate(Quaternion.Euler(stepAngleRad * Mathf.Rad2Deg * Vector3.up));

            ColliderMesh.RemoveVertexIncludingUV(0);
            ColliderMesh.RemoveVertexIncludingUV(0);

            /*
            ColliderMesh.VerticesHolder.Add(TopStepTop.AllVerticesDirectly[^2]);
            ColliderMesh.VerticesHolder.Add(TopStepTop.AllVerticesDirectly[^3]);
            ColliderMesh.Triangles.Add(new TriangleHolder(baseOffset: BackFace.VerticesHolder.Count, t1: -1, t2: -2, t3: -4));
            ColliderMesh.Triangles.Add(new TriangleHolder(baseOffset: BackFace.VerticesHolder.Count, t1: -2, t2: -3, t3: -4));
            */

            //Debug.Log(BackFace.vertices.Count);

            BackFaceTop = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: (OuterRadius - InnerRadius) * Vector3.right, secondLine: endFloor.BottomFloorHeight * Vector3.down, UVOffset: Vector2.zero);

            BackFaceTop.Move(new Vector3(InnerRadius, height, 0));

            BackFaceTop.Rotate(Quaternion.Euler((StartingAngleDeg + RevolutionAngleDeg) * Vector3.down));


            TopStepTop.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.LinkedBuildingController.transform);

            if (RevolutionAngleDeg < 0)
            {
                StepFront.FlipTriangles();
                StepTop.FlipTriangles();
                OuterRadiiSide.FlipTriangles();
                InnerRadiiSide.FlipTriangles();
                BackFace.FlipTriangles();
            }

            FinishMeshes();
        }

        public override void MoveOnGrid(Vector2Int offset)
        {
            ModificationNodeOrganizer.MoveOnGrid(offset);
        }

        public override void InternalUpdate()
        {
            NonOrderedInternalUpdate();
        }

        public override void PlaytimeUpdate()
        {
            NonOrderedPlaytimeUpdate();
        }

    }
}