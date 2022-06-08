using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class HorizontalArc : OnFloorObject
    {
        //Build parameters
        MailboxLineVector2Int CenterPositionParam;
        MailboxLineVector2Int OuterRadiiParam;
        MailboxLineDistinctUnnamed WallThicknessParam;
        MailboxLineVector2Int CutoffRangeParam;
        MailboxLineVector2Int UpperIndentParam;
        MailboxLineDistinctNamed BlockTypeParam;
        MailboxLineDistinctNamed EdgeTypeParam;
        MailboxLineMaterial OutsideMaterialParam;
        MailboxLineMaterial InsideMaterialParam;
        MailboxLineMaterial FirstWallMaterialParam;
        MailboxLineMaterial SecondWallMaterialParam;
        MailboxLineMaterial CeilingMaterialParam;
        MailboxLineMaterial FloorMaterialParam;

        NodeGridRectangleOrganizer ModificationNodeOrganizer;

        public override ModificationOrganizer Organizer
        {
            get
            {
                return ModificationNodeOrganizer;
            }
        }

        int QuarterArcVertecies = 12;

        bool noInnerRadius;
        Vector3 InnerWallStartZ;
        Vector3 InnerWallStartX;
        Vector3 noInnerRadiusCenterLocation;

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

        public override bool RaiseToFloor
        {
            get
            {
                return false;
            }
        }

        void SetupBlockTypeParam(BlockTypes blockType = BlockTypes.Wall)
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


        public enum EdgeTypes
        {
            Ring,
            Full,
            InnerGrid,
            OuterGrid
        }

        public EdgeTypes EdgeType
        {
            get
            {
                EdgeTypes returnValue = (EdgeTypes)EdgeTypeParam.Val;

                return returnValue;
            }
            set
            {
                EdgeTypeParam.Val = (int)value;
                ApplyBuildParameters();
            }
        }

        void SetupEdgeTypeParam(EdgeTypes edgeType = EdgeTypes.Ring)
        {
            List<string> enumString = new List<string>();

            int enumValues = System.Enum.GetValues(typeof(EdgeTypes)).Length;

            for (int i = 0; i < enumValues; i++)
            {
                EdgeTypes type = (EdgeTypes)i;

                enumString.Add(type.ToString());
            }

            EdgeTypeParam = new MailboxLineDistinctNamed(
                "Edge type",
                CurrentMailbox,
                Mailbox.ValueType.buildParameter,
                enumString,
                (int)edgeType);
        }

        public Vector2Int CenterPosition
        {
            get
            {
                return CenterPositionParam.Val;
            }
            set
            {
                CenterPositionParam.Val = value;
                ApplyBuildParameters();
            }
        }

        public Vector2Int OuterRadii
        {
            get
            {
                return OuterRadiiParam.Val;
            }
            set
            {
                OuterRadiiParam.Val = value;
                ApplyBuildParameters();
            }
        }

        public int WallThickness
        {
            get
            {
                return WallThicknessParam.Val;
            }
            set
            {
                WallThicknessParam.Val = value;
                ApplyBuildParameters();
            }
        }

        public Vector2Int CutoffRange
        {
            get
            {
                return CutoffRangeParam.Val;
            }
            set
            {
                CutoffRangeParam.Val = value;
                ApplyBuildParameters();
            }
        }

        public Vector2Int UpperIndent
        {
            get
            {
                return UpperIndentParam.Val;
            }
            set
            {
                UpperIndentParam.Val = value;
                ApplyBuildParameters();
            }
        }


        //Settings
        Vector2Int OuterRadiiAbsolute
        {
            get
            {
                return new Vector2Int(Mathf.Abs(OuterRadii.x), Mathf.Abs(OuterRadii.y));
            }
        }

        public override void Setup(IBaseObject linkedFloor)
        {
            base.Setup(linkedFloor);

            CenterPositionParam = new MailboxLineVector2Int(name: "Center position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            OuterRadiiParam = new MailboxLineVector2Int(name: "Outer Radii", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            CutoffRangeParam = new MailboxLineVector2Int(name: "Cutoff range", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            WallThicknessParam = new MailboxLineDistinctUnnamed(name: "Wall thickness", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 80, Min: 0, DefaultValue: 1);
            UpperIndentParam = new MailboxLineVector2Int(name: "Upper indent", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);

            OutsideMaterialParam = new MailboxLineMaterial(name: "Outside material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultStoneBricks);
            InsideMaterialParam = new MailboxLineMaterial(name: "Inside material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultStoneBricks);
            FirstWallMaterialParam = new MailboxLineMaterial(name: "First wall material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultStoneBricks);
            SecondWallMaterialParam = new MailboxLineMaterial(name: "Second wall material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultStoneBricks);
            CeilingMaterialParam = new MailboxLineMaterial(name: "Ceiling material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultCeiling);
            FloorMaterialParam = new MailboxLineMaterial(name: "Floor material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultWoodPlanks);

            SetupBlockTypeParam();
            SetupEdgeTypeParam();

            NodeGridPositionModificationNode firstNode = ModificationNodeLibrary.NewNodeGridPositionModificationNode;
            firstNode.Setup(linkedObject: this, value: CenterPositionParam);
            FirstPositionNode = firstNode;

            NodeGridPositionModificationNode secondNode = ModificationNodeLibrary.NewNodeGridPositionModificationNode;
            secondNode.Setup(linkedObject: this, value: OuterRadiiParam, relativeReferenceHolder: CenterPositionParam);
            SecondPositionNode = secondNode;

            ModificationNodeOrganizer = new NodeGridRectangleOrganizer(linkedObject: this, firstNode: firstNode, secondNode: secondNode);

            //ModificationNodeLibrary.NewGridModificationNode.Setup(linkedObject: this, value: CutoffRangeParam, relativeReferenceHolder: CenterPositionParam, parent: this, modType: GridModificationNode.ModificationType.SizeAdjuster);

            SetupEditButtons();
        }

        public void CompleteSetupWithBuildParameters(
            FloorController linkedFloor,
            Vector2Int centerPosition,
            Vector2Int outerRadii,
            Vector2Int cutoffRange,
            int wallThickness,
            Vector2Int upperIndent,
            BlockTypes blockType,
            EdgeTypes edgeType,
            MaterialManager outsideMaterial,
            MaterialManager insideMaterial,
            MaterialManager firstWallMaterialParam,
            MaterialManager secondWallMaterialParam,
            MaterialManager ceilingMaterialParam,
            MaterialManager floorMaterialParam
            )
        {
            Setup(linkedFloor);

            CenterPositionParam.Val = centerPosition;
            OuterRadiiParam.Val = outerRadii;
            CutoffRangeParam.Val = cutoffRange;
            WallThicknessParam.Val = wallThickness;
            UpperIndentParam.Val = upperIndent;
            BlockTypeParam.Val = (int)blockType;
            EdgeTypeParam.Val = (int)edgeType;
            OutsideMaterialParam.Val = outsideMaterial;
            InsideMaterialParam.Val = insideMaterial;
            FirstWallMaterialParam.Val = firstWallMaterialParam;
            SecondWallMaterialParam.Val = secondWallMaterialParam;
            CeilingMaterialParam.Val = ceilingMaterialParam;
            FloorMaterialParam.Val = floorMaterialParam;
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

        bool noOuterRadius;

        Vector3 OuterWallStartZ;
        Vector3 OuterWallStartX;

        int intermittendVetecies;

        public override void ApplyBuildParameters()
        {
            base.ApplyBuildParameters();

            //Check validity
            if (Failed) return;
            
            Vector2Int gridSize = ModificationNodeOrganizer.ObjectOrientationGridSize;

            if (gridSize.x == 0 || gridSize.y == 0 || CutoffRange.x >= OuterRadiiAbsolute.x || CutoffRange.y >= OuterRadiiAbsolute.y)
            {
                Failed = true;
                return;
            }

            //Define mesh
            Vector2 size = ModificationNodeOrganizer.ObjectOrientationSize;

            TriangleMeshInfo OuterArc = new TriangleMeshInfo();

            TriangleMeshInfo InnerArc = new TriangleMeshInfo();
            TriangleMeshInfo Floor = new TriangleMeshInfo();
            TriangleMeshInfo Ceiling = new TriangleMeshInfo();
            TriangleMeshInfo XWall = new TriangleMeshInfo();
            TriangleMeshInfo ZWall = new TriangleMeshInfo();

            void FinishMesh()
            {
                OuterArc.MaterialReference = OutsideMaterialParam;
                InnerArc.MaterialReference = InsideMaterialParam;
                Ceiling.MaterialReference = CeilingMaterialParam;
                Floor.MaterialReference = FloorMaterialParam;
                XWall.MaterialReference = FirstWallMaterialParam;
                ZWall.MaterialReference = SecondWallMaterialParam;

                Floor.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.transform);
                Ceiling.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.transform);
                XWall.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.transform);
                ZWall.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.transform);

                StaticMeshManager.AddTriangleInfoIfValid(OuterArc);
                StaticMeshManager.AddTriangleInfoIfValid(InnerArc);
                StaticMeshManager.AddTriangleInfoIfValid(Floor);
                StaticMeshManager.AddTriangleInfoIfValid(Ceiling);
                StaticMeshManager.AddTriangleInfoIfValid(XWall);
                StaticMeshManager.AddTriangleInfoIfValid(ZWall);

                BuildAllMeshes();
            }

            //Get basic sizes
            float wallHeight = LinkedFloor.BottomFloorHeight;
            switch (BlockType)
            {
                case BlockTypes.Wall:
                    wallHeight = LinkedFloor.WallHeightWithScaler;
                    break;
                case BlockTypes.Floor:
                    wallHeight = LinkedFloor.BottomFloorHeight;
                    break;
                default:
                    Debug.LogWarning("Error when trying to build horizontal Arc: Block type not defined");
                    break;
            }


            Vector2 CutoffRangeRotated;
            Vector2 IndentRotated;

            if (ModificationNodeOrganizer.Orientation.QuarterOrientation == GridOrientation.GridQuarterOrientations.XPosZPos || ModificationNodeOrganizer.Orientation.QuarterOrientation == GridOrientation.GridQuarterOrientations.XNegZNeg)
            {
                CutoffRangeRotated = new Vector2(BlockSize * CutoffRange.x, BlockSize * CutoffRange.y);
                IndentRotated = new Vector2(BlockSize * UpperIndent.x, BlockSize * UpperIndent.y);
            }
            else
            {
                CutoffRangeRotated = new Vector2(BlockSize * CutoffRange.y, BlockSize * CutoffRange.x);
                IndentRotated = new Vector2(BlockSize * UpperIndent.y, BlockSize * UpperIndent.x);
            }


            if (CutoffRangeRotated.x < 0) CutoffRangeRotated.x = 0;
            if (CutoffRangeRotated.y < 0) CutoffRangeRotated.y = 0;

            //Calculate scaled values where the sides of the arc are 1


            Vector2 lowerMainRadii = size;
            Vector2 upperMainRadii = size - IndentRotated * (wallHeight / LinkedFloor.CompleteFloorHeight);


            switch (EdgeType)
            {
                case EdgeTypes.Ring:
                    //ToDo: Catch Full
                    CreateRing();
                    break;
                case EdgeTypes.Full:
                    CreateFullArc();
                    break;
                case EdgeTypes.InnerGrid:
                    //ToDo: Catch Full
                    CreateInnerGrid();
                    break;
                case EdgeTypes.OuterGrid:
                    CreateOuterGrid();
                    break;
                default:
                    break;
            }

            if (Failed)
            {
                return;
            }

            FinishMesh();

            void CreateFullArc()
            {
                VerticesHolder lowerArc = CreateCutoffArc(radii: lowerMainRadii, cutoffRange: CutoffRangeRotated);
                VerticesHolder upperArc = CreateCutoffArc(radii: upperMainRadii, cutoffRange: CutoffRangeRotated);

                upperArc.Move(Vector3.up * wallHeight);
                lowerArc.Reverse();
                upperArc.Reverse();

                if (lowerArc == null || upperArc == null)
                {
                    Failed = true;
                    Debug.Log("Horizontal arc failed since Cutoff range is too large");
                    return;
                }

                Vector3 centerPoint = new Vector3(CutoffRangeRotated.x, 0, CutoffRangeRotated.y);

                OuterArc = MeshGenerator.MeshesFromLines.KnitLines(firstLine: lowerArc, secondLine: upperArc, closingType: MeshGenerator.ShapeClosingType.open, smoothTransition: true);

                Ceiling = MeshGenerator.MeshesFromLines.KnitLines(point: centerPoint, line: lowerArc, isClosed: false);
                Floor = MeshGenerator.MeshesFromLines.KnitLines(point: centerPoint + Vector3.up * wallHeight, line: upperArc, isClosed: false);
                Floor.FlipTriangles();

                XWall = MeshGenerator.MeshesFromPoints.MeshFrom4Points(
                    upperArc.Vertices[0],
                    centerPoint + Vector3.up * wallHeight,
                    centerPoint,
                    lowerArc.Vertices[0]
                    );

                ZWall = MeshGenerator.MeshesFromPoints.MeshFrom4Points(
                    lowerArc.Vertices[^1],
                    centerPoint,
                    centerPoint + Vector3.up * wallHeight,
                    upperArc.Vertices[^1]
                    );
            }

            void CreateRing()
            {
                Vector2 usedLowerOuterRadii;
                Vector2 usedLowerInnerRadii;
                Vector2 usedUpperOuterRadii;
                Vector2 usedUpperInnerRadii;

                if (WallThickness == 0)
                {
                    float halfWallThickness = LinkedFloor.CurrentNodeWallSystem.HalfWallThickness;

                    usedLowerOuterRadii = lowerMainRadii + Vector2.one * halfWallThickness;
                    usedLowerInnerRadii = lowerMainRadii - Vector2.one * halfWallThickness;

                    usedUpperOuterRadii = upperMainRadii + Vector2.one * halfWallThickness;
                    usedUpperInnerRadii = upperMainRadii - Vector2.one * halfWallThickness;
                }
                else
                {
                    usedLowerOuterRadii = lowerMainRadii;
                    usedLowerInnerRadii = lowerMainRadii - Vector2.one * (WallThickness * BlockSize);

                    usedUpperOuterRadii = upperMainRadii;
                    usedUpperInnerRadii = upperMainRadii - Vector2.one * (WallThickness * BlockSize);
                }

                if (usedLowerInnerRadii.x <= 0 || usedLowerInnerRadii.y <= 0 || usedUpperInnerRadii.x == 0 || usedUpperInnerRadii.y == 0)
                {
                    CreateFullArc();
                    return;
                }

                VerticesHolder lowerOuterArc = CreateCutoffArc(radii: usedLowerOuterRadii, cutoffRange: CutoffRangeRotated);
                VerticesHolder lowerInnerArc = CreateCutoffArc(radii: usedLowerInnerRadii, cutoffRange: CutoffRangeRotated);

                VerticesHolder upperOuterArc = CreateCutoffArc(radii: usedUpperOuterRadii, cutoffRange: CutoffRangeRotated);
                VerticesHolder upperInnerArc = CreateCutoffArc(radii: usedUpperInnerRadii, cutoffRange: CutoffRangeRotated);

                if (lowerOuterArc == null || upperOuterArc == null)
                {
                    Failed = true;
                    return;
                }

                if (lowerInnerArc == null || upperInnerArc == null)
                {
                    CreateFullArc();
                    return;
                }

                upperOuterArc.Move(Vector3.up * wallHeight);
                upperInnerArc.Move(Vector3.up * wallHeight);

                OuterArc = MeshGenerator.MeshesFromLines.KnitLines(firstLine: lowerOuterArc, secondLine: upperOuterArc, closingType: MeshGenerator.ShapeClosingType.open, smoothTransition: true);
                InnerArc = MeshGenerator.MeshesFromLines.KnitLines(firstLine: lowerInnerArc, secondLine: upperInnerArc, closingType: MeshGenerator.ShapeClosingType.open, smoothTransition: true);

                OuterArc.FlipTriangles();

                Ceiling = MeshGenerator.MeshesFromLines.KnitLines(firstLine: lowerOuterArc, secondLine: lowerInnerArc, closingType: MeshGenerator.ShapeClosingType.open, smoothTransition: true);
                Floor = MeshGenerator.MeshesFromLines.KnitLines(firstLine: upperInnerArc, secondLine: upperOuterArc, closingType: MeshGenerator.ShapeClosingType.open, smoothTransition: true);

                XWall = MeshGenerator.MeshesFromPoints.MeshFrom4Points(
                    lowerOuterArc.Vertices[0],
                    lowerInnerArc.Vertices[0],
                    upperInnerArc.Vertices[0],
                    upperOuterArc.Vertices[0]
                    );

                ZWall = MeshGenerator.MeshesFromPoints.MeshFrom4Points(
                    lowerInnerArc.Vertices[^1],
                    lowerOuterArc.Vertices[^1],
                    upperOuterArc.Vertices[^1],
                    upperInnerArc.Vertices[^1]
                    );
            }

            void CreateInnerGrid()
            {
                VerticesHolder outerArc = CreateCutoffArc(radii: size, cutoffRange: CutoffRangeRotated);
                outerArc.Reverse();
                OuterArc = MeshGenerator.MeshesFromLines.ExtrudeLinear(firstLine: outerArc, offset: Vector3.up * wallHeight, closeType: MeshGenerator.ShapeClosingType.open, smoothTransition: true);

                Vector3 RadiusStartingPoint = new Vector3(CutoffRangeRotated.x, 0, Mathf.Sqrt(size.y * size.y - CutoffRangeRotated.x * CutoffRangeRotated.x));
                Vector3 RadiusEndingPoint = new Vector3(Mathf.Sqrt(size.x * size.x - CutoffRangeRotated.y * CutoffRangeRotated.y), 0, CutoffRangeRotated.y);

                Vector3 LadderStartingPoint = RadiusStartingPoint - new Vector3(0, 0, RadiusStartingPoint.z % BlockSize);


                int downSteps = Mathf.RoundToInt((LadderStartingPoint.z - CutoffRangeRotated.y) / BlockSize);

                VerticesHolder ladderPoints = new();

                if (!MathHelper.FloatIsZero(LadderStartingPoint.z - RadiusStartingPoint.z))
                {
                    XWall.Add(MeshGenerator.MeshesFromLines.AddWallBetween2Points(firstClockwiseFloorPoint: RadiusStartingPoint, secondClockwiseFloorPoint: LadderStartingPoint, wallHeight: wallHeight, offset: Vector3.zero));
                    ladderPoints.Add(LadderStartingPoint);
                }



                Vector3 currentStartPoint = LadderStartingPoint;
                Vector3 currentEndPoint = currentStartPoint;

                while (downSteps > 0)
                {
                    //Step right until outside
                    while (true)
                    {
                        Vector3 rightPoint = currentEndPoint + Vector3.right * BlockSize;

                        if (scaledRadius(point: rightPoint, size: size) > 1)
                        {
                            currentStartPoint = currentEndPoint;
                            break;
                        }
                        else
                        {
                            currentEndPoint = rightPoint;
                            InnerArc.Add(MeshGenerator.MeshesFromLines.AddWallBetween2Points(firstClockwiseFloorPoint: currentStartPoint, secondClockwiseFloorPoint: currentEndPoint, wallHeight: wallHeight, offset: Vector3.zero));
                            ladderPoints.Add(currentEndPoint);
                            currentStartPoint = currentEndPoint;
                        }
                    }


                    //Step down until right is inside
                    while (downSteps > 0)
                    {
                        currentEndPoint = currentStartPoint + Vector3.back * BlockSize;
                        downSteps--;

                        InnerArc.Add(MeshGenerator.MeshesFromLines.AddWallBetween2Points(firstClockwiseFloorPoint: currentStartPoint, secondClockwiseFloorPoint: currentEndPoint, wallHeight: wallHeight, offset: Vector3.zero));
                        ladderPoints.Add(currentEndPoint);
                        currentStartPoint = currentEndPoint;

                        Vector3 rightPoint = currentEndPoint + Vector3.right * BlockSize;
                        if (scaledRadius(point: rightPoint, size: size) < 1)
                        {
                            break;
                        }
                    }
                }

                /*
                if (!MathHelper.FloatIsZero((currentEndPoint - innerArc.Vertices[0]).magnitude))
                {
                    XWall.Add(MeshGenerator.MeshesFromLines.AddWallBetween2Points(firstClockwiseFloorPoint: innerArc.Vertices[0], secondClockwiseFloorPoint: currentEndPoint, wallHeight: wallHeight, offset: Vector3.zero));
                }
                */

                ZWall.Add(MeshGenerator.MeshesFromLines.AddWallBetween2Points(firstClockwiseFloorPoint: currentStartPoint, secondClockwiseFloorPoint: RadiusEndingPoint, wallHeight: wallHeight, offset: Vector3.zero));


                Floor = MeshGenerator.MeshesFromLines.KnitLinesWithProximityPreference(firstLine: outerArc, secondLine: ladderPoints, isClosed: false);
                Ceiling = Floor.CloneFlipped;
                Floor.Move(Vector3.up * wallHeight);
            }

            void CreateOuterGrid()
            {
                //Inner arc
                VerticesHolder innerArc = CreateCutoffArc(radii: size, cutoffRange: CutoffRangeRotated);
                InnerArc = MeshGenerator.MeshesFromLines.ExtrudeLinear(firstLine: innerArc, offset: Vector3.up * wallHeight, closeType: MeshGenerator.ShapeClosingType.open, smoothTransition: true);

                //Start and end point
                Vector3 currentStartPoint = innerArc.Vertices[^1];
                Vector3 currentEndPoint = currentStartPoint;
                Vector3 endPoint = innerArc.Vertices[0];

                float offset = currentStartPoint.z % BlockSize;

                VerticesHolder ladderPoints = new();

                bool hasStartCapWall = !(MathHelper.FloatIsZero(offset) || MathHelper.FloatIsZero(offset - BlockSize));

                if (hasStartCapWall)
                {
                    currentEndPoint = currentStartPoint + Vector3.forward * (BlockSize - offset);
                    ZWall.Add(MeshGenerator.MeshesFromLines.AddWallBetween2Points(firstClockwiseFloorPoint: currentEndPoint, secondClockwiseFloorPoint: currentStartPoint, wallHeight: wallHeight, offset: Vector3.zero));
                    ladderPoints.Add(currentEndPoint);
                    currentStartPoint = currentEndPoint;
                }

                int downSteps = Mathf.RoundToInt((currentStartPoint.z - CutoffRangeRotated.y) / BlockSize);
                int rightSteps = Mathf.RoundToInt((endPoint.x - CutoffRangeRotated.x) / BlockSize);

                int maxLoops = 1000;

                while (downSteps > 0 /*&& rightSteps> 0*/)
                {
                    if (maxLoops-- == 0)
                    {
                        Debug.Log("Warning: Max loop breakout!");
                        break;
                    }

                    //Step right until down is inside
                    while (/*rightSteps > 0*/ true)
                    {
                        Vector3 downPoint = currentStartPoint + Vector3.back * BlockSize;

                        float currentScaledRadius = scaledRadius(point: downPoint, size: size);

                        if (currentScaledRadius < 1 && !MathHelper.FloatIsZero(currentScaledRadius - 1))
                        {
                            currentEndPoint = currentStartPoint + Vector3.right * BlockSize;
                            rightSteps--;
                            XWall.Add(MeshGenerator.MeshesFromLines.AddWallBetween2Points(firstClockwiseFloorPoint: currentEndPoint, secondClockwiseFloorPoint: currentStartPoint, wallHeight: wallHeight, offset: Vector3.zero));
                            ladderPoints.Add(currentEndPoint);
                            currentStartPoint = currentEndPoint;
                        }
                        else
                        {
                            break;
                        }
                    }

                    //Step down until right is inside
                    while (downSteps > 0)
                    {
                        Vector3 backPoint = currentStartPoint + Vector3.back * BlockSize;

                        float currentScaledRadius = scaledRadius(point: backPoint, size: size);

                        if (currentScaledRadius > 1 || MathHelper.FloatIsZero(currentScaledRadius - 1))
                        {
                            currentEndPoint = backPoint;
                            downSteps--;
                            ZWall.Add(MeshGenerator.MeshesFromLines.AddWallBetween2Points(firstClockwiseFloorPoint: currentEndPoint, secondClockwiseFloorPoint: currentStartPoint, wallHeight: wallHeight, offset: Vector3.zero));
                            ladderPoints.Add(currentEndPoint);
                            currentStartPoint = currentEndPoint;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                bool hasEndCapWall = !MathHelper.FloatIsZero((currentEndPoint - innerArc.Vertices[0]).magnitude);

                if (hasEndCapWall)
                {
                    XWall.Add(MeshGenerator.MeshesFromLines.AddWallBetween2Points(firstClockwiseFloorPoint: innerArc.Vertices[0], secondClockwiseFloorPoint: currentEndPoint, wallHeight: wallHeight, offset: Vector3.zero));
                }

                ladderPoints.Reverse();

                if (MathHelper.FloatIsZero((ladderPoints.Vertices[0] - innerArc.Vertices[0]).magnitude))
                    ladderPoints.Remove(0);
                if(MathHelper.FloatIsZero((ladderPoints.Vertices[^1] - innerArc.Vertices[^1]).magnitude))
                    ladderPoints.Remove(ladderPoints.Count - 1);

                Floor = MeshGenerator.MeshesFromLines.KnitLinesWithProximityPreference(firstLine: innerArc, secondLine: ladderPoints, isClosed: false);
                Ceiling = Floor.CloneFlipped;
                Floor.Move(Vector3.up * wallHeight);
            }

            float scaledRadius(Vector3 point, Vector2 size)
            {
                return new Vector3(point.x / size.x, 0, point.z / size.y).magnitude;
            }

            VerticesHolder CreateCutoffArc(Vector2 radii, Vector2 cutoffRange)
            {
                VerticesHolder returnValue;

                //Scale down to radius = 1/1:
                Vector2 cutoffRangeScaled = new Vector2(cutoffRange.x / radii.x, cutoffRange.y / radii.y);

                //Calculate angles:
                float startingAngleDeg = Mathf.Asin(cutoffRangeScaled.x) * Mathf.Rad2Deg;
                float endingAngleDeg = Mathf.Acos(cutoffRangeScaled.y) * Mathf.Rad2Deg;
                float betweenAngle = endingAngleDeg - startingAngleDeg;

                //Check validity, return null if not valid:
                if (betweenAngle < 0) return null;

                //Create arc
                returnValue = MeshGenerator.Lines.ArcAroundY(radius: 1, angleDeg: betweenAngle, numberOfEdges: QuarterArcVertecies);
                returnValue.Rotate(Quaternion.Euler(Vector3.up * (-90 + endingAngleDeg)));

                //Scale arc back to the correct size
                returnValue.Scale(new Vector3(radii.x, 0, radii.y));

                return returnValue;
            }

            FinishMesh();
        }


        //Edit buttons
        void SetupEditButtons()
        {
            AddCopyButtons();
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Copy Flip Diagonally", delegate { CopyFlipDiagonally(); }));
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Copy Flip Vertical", delegate { CopyFlipY(); }));
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Copy Flip Horizontal", delegate { CopyFlipX(); }));
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Flip Vertical", delegate { ModificationNodeOrganizer.FlipVertical(); }));
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Flip Horizontal", delegate { ModificationNodeOrganizer.FlipHorizontal(); }));
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Rotate Counter-Clockwise", delegate { ModificationNodeOrganizer.RotateCounterClockwise(); }));
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Rotate Clockwise", delegate { ModificationNodeOrganizer.RotateClockwise(); }));
        }

        void CopyFlipX()
        {
            HorizontalArc newArc = Clone as HorizontalArc;

            newArc.OuterRadii = new Vector2Int(-OuterRadii.x, OuterRadii.y);
        }

        void CopyFlipY()
        {
            HorizontalArc newArc = Clone as HorizontalArc;

            newArc.OuterRadii = new Vector2Int(OuterRadii.x, -OuterRadii.y);
        }

        void CopyFlipDiagonally()
        {
            HorizontalArc newArc = Clone as HorizontalArc;

            newArc.OuterRadii = new Vector2Int(-OuterRadii.x, -OuterRadii.y);
        }

        public override void MoveOnGrid(Vector2Int offset)
        {
            ModificationNodeOrganizer.MoveOnGrid(offset: offset);
        }
    }
}