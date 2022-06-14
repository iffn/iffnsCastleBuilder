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
        float baseHeight = 0.2f;

        //Unity assignments
        //[SerializeField] GameObject MeshMover;

        //Build parameters
        MailboxLineVector2Int BottomLeftPositionParam;
        MailboxLineVector2Int TopRightPositionParam;
        MailboxLineDistinctUnnamed NumberOfFloorsParam;
        //MailboxLineDistinctNamed AnglePreferenceParam;
        MailboxLineDistinctNamed BottomEndingTypeParam;
        MailboxLineDistinctNamed TopEndingTypeParam;
        MailboxLineDistinctNamed ColumnTypeParam;
        //MailboxLineDistinctNamed SizeTypeParam;
        MailboxLineMaterial ColumnMaterialParam;

        NodeGridRectangleOrganizer ModificationNodeOrganizer;

        public override ModificationOrganizer Organizer
        {
            get
            {
                return ModificationNodeOrganizer;
            }
        }

        public override bool RaiseToFloor
        {
            get
            {
                return true;
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

        /*
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
        */

        public enum EndingTypes
        {
            None,
            Cylinder,
            Box
        }

        void SetupColumnAndEndingParams()
        {
            List<string> endingEnumString = new List<string>();

            int endingEnumValues = System.Enum.GetValues(typeof(EndingTypes)).Length;

            for (int i = 0; i < endingEnumValues; i++)
            {
                EndingTypes type = (EndingTypes)i;

                endingEnumString.Add(type.ToString());
            }

            List<string> columnEnumString = new List<string>();

            int columnEnumValues = System.Enum.GetValues(typeof(ColumnTypes)).Length;

            for (int i = 0; i < columnEnumValues; i++)
            {
                ColumnTypes type = (ColumnTypes)i;

                columnEnumString.Add(type.ToString());
            }

            TopEndingTypeParam = new MailboxLineDistinctNamed(
                "Top ending type",
                CurrentMailbox,
                Mailbox.ValueType.buildParameter,
                endingEnumString,
                (int)EndingTypes.Box);

            ColumnTypeParam = new MailboxLineDistinctNamed(
                "Column type",
                CurrentMailbox,
                Mailbox.ValueType.buildParameter,
                columnEnumString,
                (int)ColumnTypes.Cylinder);

            BottomEndingTypeParam = new MailboxLineDistinctNamed(
                "Bottom ending type",
                CurrentMailbox,
                Mailbox.ValueType.buildParameter,
                endingEnumString,
                (int)EndingTypes.Box);
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

        public EndingTypes BottomEndingType
        {
            get
            {
                EndingTypes returnValue = (EndingTypes)BottomEndingTypeParam.Val;

                return returnValue;
            }
            set
            {
                BottomEndingTypeParam.Val = (int)value;
                ApplyBuildParameters();
            }
        }

        public enum ColumnTypes
        {
            Cylinder,
            Box
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

        /*
        public enum SizeTypes
        {
            Diameter,
            Radius
        }

        void SetupSizeTypeParam()
        {
            List<string> enumString = new List<string>();

            int enumValues = System.Enum.GetValues(typeof(SizeTypes)).Length;

            for (int i = 0; i < enumValues; i++)
            {
                SizeTypes type = (SizeTypes)i;

                enumString.Add(type.ToString());
            }

            SizeTypeParam = new MailboxLineDistinctNamed(
                "Size type",
                CurrentMailbox,
                Mailbox.ValueType.buildParameter,
                enumString,
                0);
        }

        public SizeTypes SizeType
        {
            get
            {
                SizeTypes returnValue = (SizeTypes)SizeTypeParam.Val;

                return returnValue;
            }
            set
            {
                SizeTypeParam.Val = (int)value;
                ApplyBuildParameters();
            }
        }
        */

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

        public float CompleteHeight
        {
            get
            {
                float completeHeight = LinkedFloor.WallBetweenHeight;

                if (LinkedFloor.FloorsAbove > 0 && NumberOfFloors > 1)
                {
                    int usedNumberOfFloors = NumberOfFloors;

                    usedNumberOfFloors = Mathf.Clamp(value: usedNumberOfFloors, min: 1, max: LinkedFloor.FloorsAbove + 1);

                    for (int i = LinkedFloor.FloorNumber + 1; i < LinkedFloor.FloorNumber + usedNumberOfFloors; i++)
                    {
                        FloorController floor = LinkedFloor.LinkedBuildingController.Floor(i);

                        completeHeight += floor.CompleteFloorHeight;
                    }
                }

                return completeHeight;
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
            NumberOfFloorsParam = new MailboxLineDistinctUnnamed(name: "Number of floors", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 100, Min: 1, DefaultValue: 1);
            ColumnMaterialParam = new MailboxLineMaterial(name: "Column material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultPlaster);

            //SetupAnglePreferenceTypeParam();
            SetupColumnAndEndingParams();
            //SetupSizeTypeParam();

            NodeGridPositionModificationNode firstNode = ModificationNodeLibrary.NewNodeGridPositionModificationNode;
            firstNode.Setup(linkedObject: this, value: BottomLeftPositionParam);
            FirstPositionNode = firstNode;

            NodeGridPositionModificationNode secondNode = ModificationNodeLibrary.NewNodeGridPositionModificationNode;
            secondNode.Setup(linkedObject: this, value: TopRightPositionParam, relativeReferenceHolder: BottomLeftPositionParam);
            SecondPositionNode = secondNode;

            ModificationNodeOrganizer = new NodeGridRectangleOrganizer(linkedObject: this, firstNode: firstNode, secondNode: secondNode);

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
            base.ApplyBuildParameters();

            //Check validity
            if (Failed) return;

            if(ModificationNodeOrganizer.ObjectOrientationGridSize.x == 0)
            {
                Failed = true;
                return;
            }

            //Sizes
            Vector2 radii = 0.5f * ModificationNodeOrganizer.ObjectOrientationSize;
            Vector3 offset = new Vector3(radii.x, 0, radii.y);

            float completeHeight = CompleteHeight;

            float bottomHeight = 0;
            float topHeight = 0;

            if (TopEndingType != EndingTypes.None) topHeight = baseHeight;
            if (BottomEndingType != EndingTypes.None) bottomHeight = baseHeight;

            float middleHeight = completeHeight - bottomHeight - topHeight;

            if(middleHeight < 0.1f)
            {
                topHeight = completeHeight / 3;
                middleHeight = completeHeight / 3;
                bottomHeight = completeHeight / 3;
            }

            float columnRadiusFactor = 0.9f;

            if (TopEndingType == EndingTypes.None && BottomEndingType == EndingTypes.None) columnRadiusFactor = 1;
            if (TopEndingType == EndingTypes.Cylinder || BottomEndingType == EndingTypes.Cylinder)
            {
                if(ColumnType == ColumnTypes.Box)
                {
                    columnRadiusFactor = 0.707f;
                }
                else
                {
                    columnRadiusFactor = 0.8f;
                }
            }


            //Define mesh
            TriangleMeshInfo TopEndingMesh = new();
            TriangleMeshInfo ColumnMesh = new();
            TriangleMeshInfo BottomEndingMesh = new();

            void FinishMeshes()
            {
                TopEndingMesh.MaterialReference = ColumnMaterialParam;
                ColumnMesh.MaterialReference = ColumnMaterialParam;
                BottomEndingMesh.MaterialReference = ColumnMaterialParam;

                StaticMeshManager.AddTriangleInfoIfValid(TopEndingMesh);
                StaticMeshManager.AddTriangleInfoIfValid(ColumnMesh);
                StaticMeshManager.AddTriangleInfoIfValid(BottomEndingMesh);

                BuildAllMeshes();
            }

            switch (TopEndingType)
            {
                case EndingTypes.None:
                    break;
                case EndingTypes.Cylinder:
                    TopEndingMesh.Add(MeshGenerator.FilledShapes.CylinderAroundCenterWithoutCap(radius: 1, length: 1, direction: Vector3.up, numberOfEdges: fullCircleVertices));
                    TopEndingMesh.Add(MeshGenerator.FilledShapes.CylinderCaps(radius: 1, length: 1, direction: Vector3.up, numberOfEdges: fullCircleVertices));
                    break;
                case EndingTypes.Box:
                    TopEndingMesh.Add(MeshGenerator.FilledShapes.BoxAroundCenter(size: new Vector3(2, 1, 2)));
                    break;
                default:
                    break;
            }

            switch (ColumnType)
            {
                case ColumnTypes.Cylinder:
                    ColumnMesh.Add(MeshGenerator.FilledShapes.CylinderAroundCenterWithoutCap(radius: columnRadiusFactor, length: 1, direction: Vector3.up, numberOfEdges: fullCircleVertices));

                    if (TopEndingType == EndingTypes.None || BottomEndingType == EndingTypes.None)
                    {
                        ColumnMesh.Add(MeshGenerator.FilledShapes.CylinderCaps(radius: columnRadiusFactor, length: 1, direction: Vector3.up, numberOfEdges: fullCircleVertices));
                    }

                    break;
                case ColumnTypes.Box:
                    ColumnMesh.Add(MeshGenerator.FilledShapes.BoxAroundCenter(size: new Vector3(2 * columnRadiusFactor, 1, 2 * columnRadiusFactor)));
                    break;
                default:
                    break;
            }

            switch (BottomEndingType)
            {
                case EndingTypes.None:
                    break;
                case EndingTypes.Cylinder:
                    BottomEndingMesh.Add(MeshGenerator.FilledShapes.CylinderAroundCenterWithoutCap(radius: 1, length: 1, direction: Vector3.up, numberOfEdges: fullCircleVertices));
                    BottomEndingMesh.Add(MeshGenerator.FilledShapes.CylinderCaps(radius: 1, length: 1, direction: Vector3.up, numberOfEdges: fullCircleVertices));
                    break;
                case EndingTypes.Box:
                    BottomEndingMesh.Add(MeshGenerator.FilledShapes.BoxAroundCenter(size: new Vector3(2, 1, 2)));
                    break;
                default:
                    break;
            }

            TopEndingMesh.Scale(new Vector3(radii.x, topHeight, radii.y));
            TopEndingMesh.Move((bottomHeight + middleHeight + topHeight * 0.5f) * Vector3.up);

            ColumnMesh.Scale(new Vector3(radii.x, middleHeight, radii.y));
            ColumnMesh.Move((bottomHeight + middleHeight * 0.5f) * Vector3.up);

            BottomEndingMesh.Scale(new Vector3(radii.x, bottomHeight, radii.y));
            BottomEndingMesh.Move((bottomHeight * 0.5f) * Vector3.up);

            TopEndingMesh.Move(offset);
            ColumnMesh.Move(offset);
            BottomEndingMesh.Move(offset);

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