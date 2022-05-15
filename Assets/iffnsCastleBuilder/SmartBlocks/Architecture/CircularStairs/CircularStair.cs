﻿using iffnsStuff.iffnsBaseSystemForUnity;
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
        MailboxLineMaterial TopSetpTopMaterialParam;
        MailboxLineMaterial OtherSetpsTopMaterialParam;
        MailboxLineMaterial SetpsFrontMaterialParam;
        MailboxLineMaterial SideMaterialParam;
        MailboxLineMaterial BackMaterialParam;
        MailboxLineBool RaiseToFloorParam;

        BlockGridRadiusOrganizer ModificationNodeOrganizer;

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

        //Derived parameters
        float completeFloorHeight
        {
            get
            {
                return LinkedFloor.CompleteFloorHeight;
            }
        }
        public bool RaiseToFloor
        {
            get
            {
                return RaiseToFloorParam.Val;
            }
            set
            {
                RaiseToFloorParam.Val = value;
                ApplyBuildParameters();
            }
        }

        void initializeBuildParameterLines()
        {
            PositiveCenterPositionParam = new MailboxLineVector2Int(name: "Positive center position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            OuterRadiusParam = new MailboxLineRanged(name: "Outer radius [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 80, Min: 0.1f, DefaultValue: 1);
            InnerRadiusParam = new MailboxLineRanged(name: "Inner radius [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 79, Min: 0.01f, DefaultValue: 0.25f);
            StartingAngleDegParam = new MailboxLineRanged(name: "Starting angle [°]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 360, Min: 0, DefaultValue: 0);
            RevolutionAngleDegParam = new MailboxLineRanged(name: "Revolution angle [°]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 3600, Min: -3600, DefaultValue: 360);
            StepHeightTargetParam = new MailboxLineRanged(name: "Step height [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 2f, Min: 0.1f, DefaultValue: 0.25f);
            BackThicknessParam = new MailboxLineRanged(name: "Back thickness [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 2f, Min: 0.1f, DefaultValue: 0.25f);

            TopSetpTopMaterialParam = new MailboxLineMaterial(name: "Top step top material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultWoodPlanks);
            OtherSetpsTopMaterialParam = new MailboxLineMaterial(name: "Other steps top material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultWoodSolid);
            SetpsFrontMaterialParam = new MailboxLineMaterial(name: "Steps front material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultPlaster);
            SideMaterialParam = new MailboxLineMaterial(name: "Side material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultPlaster);
            BackMaterialParam = new MailboxLineMaterial(name: "Back material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultPlaster);
            RaiseToFloorParam = new MailboxLineBool(name: "Raise to floor", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: true);
        }

        public override void Setup(IBaseObject linkedFloor)
        {
            base.Setup(linkedFloor);

            LinkedFloor = linkedFloor as FloorController;

            initializeBuildParameterLines();

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
            failed = false;

            ModificationNodeOrganizer.SetLinkedObjectPositionAndOrientation(raiseToFloor: RaiseToFloor);

            TriangleMeshInfo TopStepTop = new TriangleMeshInfo();
            TriangleMeshInfo StepTop = new TriangleMeshInfo();
            TriangleMeshInfo StepFront = new TriangleMeshInfo();
            TriangleMeshInfo OuterRadiiSide = new TriangleMeshInfo();
            TriangleMeshInfo InnerRadiiSide = new TriangleMeshInfo();
            TriangleMeshInfo BackFace = new TriangleMeshInfo();

            void FinishMeshes()
            {
                StepFront.MaterialReference = SetpsFrontMaterialParam;
                StepTop.MaterialReference = OtherSetpsTopMaterialParam;
                TopStepTop.MaterialReference = TopSetpTopMaterialParam;
                OuterRadiiSide.MaterialReference = SideMaterialParam;
                InnerRadiiSide.MaterialReference = SideMaterialParam;
                BackFace.MaterialReference = BackMaterialParam;

                TopStepTop.FixUVCount();
                StepTop.FixUVCount();
                StepFront.FixUVCount();
                OuterRadiiSide.FixUVCount();
                InnerRadiiSide.FixUVCount();
                BackFace.FixUVCount();

                StaticMeshManager.AddTriangleInfo(TopStepTop);
                StaticMeshManager.AddTriangleInfo(StepTop);
                StaticMeshManager.AddTriangleInfo(StepFront);
                StaticMeshManager.AddTriangleInfo(OuterRadiiSide);
                StaticMeshManager.AddTriangleInfo(InnerRadiiSide);
                StaticMeshManager.AddTriangleInfo(BackFace);

                BuildAllMeshes();
            }

            //transform.localPosition = LinkedFloor.NodePositionFromBlockIndex(PositiveCenterPosition);

            //Calculate basic parameters
            int numberOfSteps = Mathf.RoundToInt(completeFloorHeight / StepHeightTarget);
            float stepHeight = completeFloorHeight / numberOfSteps;
            float stepAngleRad = RevolutionAngleDeg / numberOfSteps * Mathf.Deg2Rad;

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
                float backLowerHeight = upperHeight - BackThicknessHeight;
                float frontLowerHeight = baseHeight - BackThicknessHeight;

                if (backLowerHeight < 0) backLowerHeight = 0;
                if (frontLowerHeight < 0) frontLowerHeight = 0;

                StepFront.Add(MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(new List<Vector3>(){
                    new Vector3(firstInnerLocation.x, baseHeight, firstInnerLocation.y),
                    new Vector3(firstInnerLocation.x, upperHeight, firstInnerLocation.y),
                    new Vector3(firstOuterLocation.x, upperHeight, firstOuterLocation.y),
                    new Vector3(firstOuterLocation.x, baseHeight, firstOuterLocation.y)
                    }));

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

                if (stepNumber == 0)
                {

                    OuterRadiiSide.Add(MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(new List<Vector3>(){
                        new Vector3(firstOuterLocation.x, upperHeight, firstOuterLocation.y),
                        new Vector3(secondOuterLocation.x, upperHeight, secondOuterLocation.y),
                        new Vector3(secondOuterLocation.x, baseHeight, secondOuterLocation.y),
                        new Vector3(firstOuterLocation.x, baseHeight, firstOuterLocation.y),
                        }));

                    InnerRadiiSide.Add(MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(new List<Vector3>(){
                        new Vector3(secondInnerLocation.x, baseHeight, secondInnerLocation.y),
                        new Vector3(secondInnerLocation.x, upperHeight, secondInnerLocation.y),
                        new Vector3(firstInnerLocation.x, upperHeight, firstInnerLocation.y),
                        new Vector3(firstInnerLocation.x, baseHeight, firstInnerLocation.y),
                        }));

                    BackFace.VerticesHolder.Add(new Vector3(firstInnerLocation.x, 0, firstInnerLocation.y));
                    BackFace.VerticesHolder.Add(new Vector3(firstOuterLocation.x, 0, firstOuterLocation.y));

                    /*
                    BackFace.AddPointsClockwiseAroundFirstPoint(new List<Vector3>(){
                        new Vector3(secondOuterLocation.x, baseHeight, secondOuterLocation.y),
                        new Vector3(secondInnerLocation.x, baseHeight, secondInnerLocation.y),
                        new Vector3(firstInnerLocation.x, baseHeight, firstInnerLocation.y),
                        new Vector3(firstOuterLocation.x, baseHeight, firstOuterLocation.y),
                    });
                    */
                }
                else
                {
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

                    /*
                    BackFace.AddPointsClockwiseAroundFirstPoint(new List<Vector3>(){
                        new Vector3(secondOuterLocation.x, backLowerHeight, secondOuterLocation.y),
                        new Vector3(secondInnerLocation.x, backLowerHeight, secondInnerLocation.y),
                        new Vector3(firstInnerLocation.x, frontLowerHeight, firstInnerLocation.y),
                        new Vector3(firstOuterLocation.x, frontLowerHeight, firstOuterLocation.y),
                    });
                    */
                }

                BackFace.Triangles.Add(new TriangleHolder(baseOffset: stepNumber * 2, t1: 0, t2: 1, t3: 3));
                BackFace.Triangles.Add(new TriangleHolder(baseOffset: stepNumber * 2, t1: 0, t2: 3, t3: 2));

                BackFace.VerticesHolder.Add(new Vector3(secondInnerLocation.x, backLowerHeight, secondInnerLocation.y));
                BackFace.VerticesHolder.Add(new Vector3(secondOuterLocation.x, backLowerHeight, secondOuterLocation.y));
            }

            //Debug.Log(BackFace.vertices.Count);

            BackFace.VerticesHolder.Add(BackFace.AllVerticesDirectly[BackFace.VerticesHolder.Count - 2]);
            BackFace.VerticesHolder.Add(BackFace.AllVerticesDirectly[BackFace.VerticesHolder.Count - 2]);

            BackFace.VerticesHolder.Add(TopStepTop.AllVerticesDirectly[TopStepTop.VerticesHolder.Count - 2]);
            BackFace.VerticesHolder.Add(TopStepTop.AllVerticesDirectly[TopStepTop.VerticesHolder.Count - 3]);

            BackFace.Triangles.Add(new TriangleHolder(baseOffset: BackFace.VerticesHolder.Count, t1: -2, t2: -1, t3: -4));
            BackFace.Triangles.Add(new TriangleHolder(baseOffset: BackFace.VerticesHolder.Count, t1: -3, t2: -2, t3: -4));

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
            Vector2Int firstNodePosition = PositiveCenterPositionParam.Val + offset;

            Vector2Int gridSize = LinkedFloor.LinkedBuildingController.GridSize;

            if (firstNodePosition.x < 0 || firstNodePosition.y < 0
                || firstNodePosition.x >= gridSize.x || firstNodePosition.y >= gridSize.y)
            {
                DestroyObject();
                return;
            }

            PositiveCenterPositionParam.Val = firstNodePosition;
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