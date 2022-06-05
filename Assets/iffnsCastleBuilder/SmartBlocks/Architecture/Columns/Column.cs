using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class Column : OnFloorObject
    {
        //Settings:
        int fullCircleVertices = 32;

        //Unity assignments
        //[SerializeField] GameObject MeshMover;

        //Build parameters
        MailboxLineVector2Int BottomLeftPositionParam;
        MailboxLineVector2Int TopRightPositionParam;
        MailboxLineDistinctNamed AnglePreferenceParam;
        MailboxLineDistinctNamed BottomEndingTypeParam;
        MailboxLineDistinctNamed TopEndingTypeParam;
        MailboxLineDistinctNamed ColumnTypeParam;

        BlockGridRectangleOrganizer ModificationNodeOrganizer;

        public override ModificationOrganizer Organizer
        {
            get
            {
                return ModificationNodeOrganizer;
            }
        }

        public enum QuarterAngleTypes
        {
            Deg90,
            Deg180,
            Deg270,
            Deg360
        }

        void SetupAnglePreferenceTypeParam()
        {
            List<string> enumString = new List<string>();

            int enumValues = System.Enum.GetValues(typeof(QuarterAngleTypes)).Length;

            for (int i = 0; i < enumValues; i++)
            {
                QuarterAngleTypes type = (QuarterAngleTypes)i;

                enumString.Add(type.ToString().Replace("Deg", "") + "°");
            }

            AnglePreferenceParam = new MailboxLineDistinctNamed(
                "Angle preference",
                CurrentMailbox,
                Mailbox.ValueType.buildParameter,
                enumString,
                (int)QuarterAngleTypes.Deg360);
        }

        public QuarterAngleTypes AnglePreference
        {
            get
            {
                QuarterAngleTypes returnValue = (QuarterAngleTypes)AnglePreferenceParam.Val;

                return returnValue;
            }
            set
            {
                AnglePreferenceParam.Val = (int)value;
                ApplyBuildParameters();
            }
        }

        public enum EndingTypes
        {
            None,
            Disk,
            Box
        }

        void SetupEndingParams()
        {
            List<string> enumString = new List<string>();

            int enumValues = System.Enum.GetValues(typeof(EndingTypes)).Length;

            for (int i = 0; i < enumValues; i++)
            {
                EndingTypes type = (EndingTypes)i;

                enumString.Add(type.ToString());
            }

            TopEndingTypeParam = new MailboxLineDistinctNamed(
                "Top ending type",
                CurrentMailbox,
                Mailbox.ValueType.buildParameter,
                enumString,
                0);

            BottomEndingTypeParam = new MailboxLineDistinctNamed(
                "Bottom ending type",
                CurrentMailbox,
                Mailbox.ValueType.buildParameter,
                enumString,
                0);
        }

        public EndingTypes TopEndingType
        {
            get
            {
                EndingTypes returnValue = (EndingTypes)TopEndingTypeParam.Val;

                return returnValue;
            }
            set
            {
                TopEndingTypeParam.Val = (int)value;
                ApplyBuildParameters();
            }
        }

        public enum ColumnTypes
        {
            Cylinder,
            Box
        }

        void SetupColumnTypeParam()
        {
            List<string> enumString = new List<string>();

            int enumValues = System.Enum.GetValues(typeof(ColumnTypes)).Length;

            for (int i = 0; i < enumValues; i++)
            {
                ColumnTypes type = (ColumnTypes)i;

                enumString.Add(type.ToString());
            }

            ColumnTypeParam = new MailboxLineDistinctNamed(
                "Column type",
                CurrentMailbox,
                Mailbox.ValueType.buildParameter,
                enumString,
                0);
        }

        public ColumnTypes ColumnType
        {
            get
            {
                ColumnTypes returnValue = (ColumnTypes)ColumnTypeParam.Val;

                return returnValue;
            }
            set
            {
                ColumnTypeParam.Val = (int)value;
                ApplyBuildParameters();
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

        public override void Setup(IBaseObject linkedFloor)
        {
            base.Setup(linkedFloor);

            BottomLeftPositionParam = new MailboxLineVector2Int(name: "Bottom Left Position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            TopRightPositionParam = new MailboxLineVector2Int(name: "Top Right Position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);

            BlockGridPositionModificationNode firstNode = ModificationNodeLibrary.NewBlockGridPositionModificationNode;
            firstNode.Setup(linkedObject: this, value: BottomLeftPositionParam);
            FirstPositionNode = firstNode;

            BlockGridPositionModificationNode secondNode = ModificationNodeLibrary.NewBlockGridPositionModificationNode;
            secondNode.Setup(linkedObject: this, value: TopRightPositionParam, relativeReferenceHolder: BottomLeftPositionParam);
            SecondPositionNode = secondNode;

            SetupAnglePreferenceTypeParam();
            SetupEndingParams();
            SetupColumnTypeParam();

            ModificationNodeOrganizer = new BlockGridRectangleOrganizer(linkedObject: this, firstNode: firstNode, secondNode: secondNode);

            SetupEditButtons();
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
            Failed = false;

            TriangleMeshInfo TopEndingMesh = new TriangleMeshInfo();
            TriangleMeshInfo ColumnMesh = new TriangleMeshInfo();
            TriangleMeshInfo BottomEndingMesh = new TriangleMeshInfo();

            void FinishMeshes()
            {
                StaticMeshManager.AddTriangleInfoIfValid(TopEndingMesh);
                StaticMeshManager.AddTriangleInfoIfValid(ColumnMesh);
                StaticMeshManager.AddTriangleInfoIfValid(BottomEndingMesh);

                BuildAllMeshes();
            }

            ModificationNodeOrganizer.SetLinkedObjectPositionAndOrientation(raiseToFloor: true);
            if (Failed) return;

            Vector2 size = ModificationNodeOrganizer.ObjectOrientationSize;
            Vector2Int gridSize = ModificationNodeOrganizer.ObjectOrientationGridSize;

            float columnHeight = LinkedFloor.CompleteFloorHeight - 0.01f;

            VerticesHolder circularColumnLine;

            float blockIndent = BlockSize * 0.2f;

            if (gridSize.x == 0 || gridSize.y == 0)
            {
                //Always 360
                switch (ColumnType)
                {
                    case ColumnTypes.Cylinder:
                        circularColumnLine = MeshGenerator.Lines.FullCircle(radius: BlockSize / 2, numberOfEdges: fullCircleVertices);
                        circularColumnLine.Rotate(Quaternion.LookRotation(Vector3.up, Vector3.right));
                        circularColumnLine.Scale(new Vector3(size.x + 1, 0, size.y + 1));
                        ColumnMesh.Add(MeshGenerator.MeshesFromLines.ExtrudeLinear(firstLine: circularColumnLine, offset: Vector3.up * columnHeight, closeType: MeshGenerator.ShapeClosingType.closedWithSmoothEdge, smoothTransition: true));
                        break;

                    case ColumnTypes.Box:
                        ColumnMesh.Add(MeshGenerator.FilledShapes.BoxAroundCenter(new Vector3((gridSize.x + 1) * BlockSize - blockIndent, columnHeight, (gridSize.y + 1) * BlockSize - blockIndent)));
                        ColumnMesh.Move(Vector3.up * columnHeight * 0.5f);
                        break;

                    default:
                        Debug.LogWarning("Error: Column type not defined");
                        break;
                }

                //MeshMover.transform.localPosition = new Vector3(gridSize.x + 1, 0, gridSize.y + 1) * (BlockSize * 0.5f);
            }
            else
            {
                switch (ColumnType)
                {
                    case ColumnTypes.Cylinder:
                        //Angle preference arc
                        switch (AnglePreference)
                        {
                            case QuarterAngleTypes.Deg90:
                                circularColumnLine = MeshGenerator.Lines.ArcAroundZ(radius: BlockSize * 0.5f, angleDeg: 90, numberOfEdges: fullCircleVertices);
                                break;
                            case QuarterAngleTypes.Deg180:
                                circularColumnLine = MeshGenerator.Lines.ArcAroundZ(radius: BlockSize * 0.5f, angleDeg: 180, numberOfEdges: fullCircleVertices);
                                break;
                            case QuarterAngleTypes.Deg270:
                                circularColumnLine = MeshGenerator.Lines.ArcAroundZ(radius: BlockSize * 0.5f, angleDeg: 270, numberOfEdges: fullCircleVertices);
                                break;
                            case QuarterAngleTypes.Deg360:
                                circularColumnLine = MeshGenerator.Lines.FullCircle(radius: BlockSize * 0.5f, numberOfEdges: fullCircleVertices);
                                break;
                            default:
                                circularColumnLine = MeshGenerator.Lines.FullCircle(radius: BlockSize * 0.5f, numberOfEdges: fullCircleVertices);
                                Debug.LogWarning("Error: Column type not defined");
                                break;
                        }

                        circularColumnLine.Rotate(Quaternion.LookRotation(Vector3.up, Vector3.right));

                        circularColumnLine.Scale(new Vector3(gridSize.x, 0, gridSize.y));

                        MeshGenerator.ShapeClosingType closeType;
                        if (AnglePreference == QuarterAngleTypes.Deg360) closeType = MeshGenerator.ShapeClosingType.closedWithSmoothEdge;
                        else closeType = MeshGenerator.ShapeClosingType.open;

                        ColumnMesh.Add(MeshGenerator.MeshesFromLines.ExtrudeLinear(firstLine: circularColumnLine, offset: Vector3.up * columnHeight, closeType: closeType, smoothTransition: true));
                        break;

                    case ColumnTypes.Box:
                        ColumnMesh.Add(MeshGenerator.FilledShapes.BoxAroundCenter(new Vector3(gridSize.x * BlockSize - blockIndent, columnHeight, gridSize.y * BlockSize - blockIndent)));
                        ColumnMesh.Move(Vector3.up * columnHeight * 0.5f);
                        break;

                    default:
                        break;
                }


                //MeshMover.transform.localPosition = new Vector3(size.x, 0, size.y) * 0.5f;
            }

            FinishMeshes();
        }

        void SetupEditButtons()
        {
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Rotate Counter-Clockwise", delegate { ModificationNodeOrganizer.RotateCounterClockwise(); }));
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Rotate Clockwise", delegate { ModificationNodeOrganizer.RotateClockwise(); }));
        }

        public override void MoveOnGrid(Vector2Int offset)
        {
            ModificationNodeOrganizer.MoveOnGrid(offset: offset);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}