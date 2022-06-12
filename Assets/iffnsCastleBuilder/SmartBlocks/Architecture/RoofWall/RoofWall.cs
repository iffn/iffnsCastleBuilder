using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class RoofWall : OnFloorObject
    {
        MailboxLineVector2Int FirstCoordinatenParam;
        MailboxLineVector2Int SecondCoordinateParam;
        MailboxLineRanged HeightParam;
        MailboxLineMaterial MainSideMaterialParam;
        MailboxLineMaterial OtherSideMaterialParam;
        MailboxLineDistinctNamed RoofTypeParamParam;
        MailboxLineBool RaiseToFloorParam;

        NodeGridRectangleOrganizer ModificationNodeOrganizer;


        public override ModificationOrganizer Organizer
        {
            get
            {
                return ModificationNodeOrganizer;
            }
        }

        public Vector2Int FirstCoordinaten
        {
            set
            {
                FirstCoordinatenParam.Val = value;
                ApplyBuildParameters();
            }
        }

        public Vector2Int SecondCoordinate
        {
            set
            {
                SecondCoordinateParam.Val = value;
                ApplyBuildParameters();
            }
        }

        public float Height
        {
            get
            {
                return HeightParam.Val;
            }
            set
            {
                HeightParam.Val = value;
                ApplyBuildParameters();
            }
        }

        void SetupRoofTypeParam()
        {
            List<string> enumString = new List<string>();

            int enumValues = System.Enum.GetValues(typeof(RectangularRoof.RoofTypes)).Length;

            for (int i = 0; i < enumValues; i++)
            {
                RectangularRoof.RoofTypes type = (RectangularRoof.RoofTypes)i;

                enumString.Add(type.ToString());
            }

            RoofTypeParamParam = new MailboxLineDistinctNamed(
                "Roof type",
                CurrentMailbox,
                Mailbox.ValueType.buildParameter,
                enumString,
                0);
        }

        public RectangularRoof.RoofTypes RoofType
        {
            get
            {
                RectangularRoof.RoofTypes returnValue = (RectangularRoof.RoofTypes)RoofTypeParamParam.Val;

                return returnValue;
            }
            set
            {
                RoofTypeParamParam.Val = (int)value;

                ApplyBuildParameters();
            }
        }

        void InitializeBuildParameterLines()
        {
            FirstCoordinatenParam = new MailboxLineVector2Int(name: "First coordinaten", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            SecondCoordinateParam = new MailboxLineVector2Int(name: "Second coordinate", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            HeightParam = new MailboxLineRanged(name: "Height [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 5f, Min: 0.3f, DefaultValue: 2f);
            RaiseToFloorParam = new MailboxLineBool(name: "Raise to floor", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: true);
            MainSideMaterialParam = new MailboxLineMaterial(name: "Main side material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultPlaster);
            OtherSideMaterialParam = new MailboxLineMaterial(name: "Other side material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultPlaster);
        }

        public override bool RaiseToFloor
        {
            get
            {
                return RaiseToFloorParam.Val;
            }
        }

        public override float ModificationNodeHeight
        {
            get
            {
                if (RaiseToFloor)
                {
                    return Height + LinkedFloor.BottomFloorHeight;
                }
                else
                {
                    return Height;
                }
            }
        }

        public override void Setup(IBaseObject linkedFloor)
        {
            base.Setup(linkedFloor);

            LinkedFloor = linkedFloor as FloorController;

            InitializeBuildParameterLines();

            NodeGridPositionModificationNode firstNode = ModificationNodeLibrary.NewNodeGridPositionModificationNode;
            firstNode.Setup(linkedObject: this, value: FirstCoordinatenParam);
            FirstPositionNode = firstNode;

            NodeGridPositionModificationNode secondNode = ModificationNodeLibrary.NewNodeGridPositionModificationNode;
            secondNode.Setup(linkedObject: this, value: SecondCoordinateParam, relativeReferenceHolder: FirstCoordinatenParam);
            SecondPositionNode = secondNode;

            ModificationNodeOrganizer = new NodeGridRectangleOrganizer(linkedObject: this, firstNode: firstNode, secondNode: secondNode);

            SetupRoofTypeParam();

            SetupEditButtons();
        }

        public override void ResetObject()
        {
            baseReset();
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

            if (ModificationNodeOrganizer.ObjectOrientationGridSize.y == 0)
            {
                Failed = true;
                return;
            }

            //Define mesh
            TriangleMeshInfo MainSide;
            TriangleMeshInfo OtherSide;
            TriangleMeshInfo WrapperStartSide;
            TriangleMeshInfo WrapperOtherSide;
            TriangleMeshInfo WrapperBottom;

            void FinishMesh()
            {
                MainSide.MaterialReference = MainSideMaterialParam;
                OtherSide.MaterialReference = OtherSideMaterialParam;
                WrapperStartSide.MaterialReference = MainSideMaterialParam;
                WrapperOtherSide.MaterialReference = MainSideMaterialParam;
                WrapperBottom.MaterialReference = MainSideMaterialParam;

                AddStaticMesh(MainSide);
                AddStaticMesh(OtherSide);
                AddStaticMesh(WrapperStartSide);
                AddStaticMesh(WrapperOtherSide);
                AddStaticMesh(WrapperBottom);

                BuildAllMeshes();
            }

            Vector2 size = ModificationNodeOrganizer.ObjectOrientationSize;

            float width = size.y;
            float thickness = size.x;


            if (ModificationNodeOrganizer.ObjectOrientationGridSize.x == 0)
            {
                thickness = LinkedFloor.CurrentNodeWallSystem.WallThickness;
            }

            float centerPosition = 0.5f;

            switch (RoofType)
            {
                case RectangularRoof.RoofTypes.FullAngledSquareRoof:
                    centerPosition = 0.5f;
                    break;
                case RectangularRoof.RoofTypes.HalfAngledSquareRoof:
                    centerPosition = 0f;
                    break;
                default:
                    break;
            }

            List<Vector3> trianglePoints = new List<Vector3>();

            trianglePoints.Add(Vector3.zero);
            trianglePoints.Add(width * Vector3.forward);
            trianglePoints.Add(new Vector3(0, Height, width * centerPosition));

            //trianglePoints[2] += new Vector3(MathHelper.SmallFloat, -MathHelper.SmallFloat, -MathHelper.SmallFloat);

            MainSide = MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(points: trianglePoints);
            OtherSide = MainSide.CloneFlipped;

            MainSide.Move(MathHelper.SmallFloat * Vector3.left);
            OtherSide.Move((thickness + MathHelper.SmallFloat) * Vector3.right);

            WrapperStartSide = MeshGenerator.FilledShapes.RectangleAtCorner(thickness * Vector3.right, secondLine: trianglePoints[2], UVOffset: Vector2.zero);
            WrapperStartSide.Move(MathHelper.SmallFloat * Vector3.forward);

            WrapperOtherSide = MeshGenerator.FilledShapes.RectangleAtCorner(thickness * Vector3.right, secondLine: trianglePoints[1] - trianglePoints[2], UVOffset: Vector2.zero);
            WrapperOtherSide.Move(trianglePoints[2] + MathHelper.SmallFloat * Vector3.back);

            WrapperBottom = MeshGenerator.FilledShapes.RectangleAtCorner(thickness * Vector3.right, secondLine: trianglePoints[1], UVOffset: Vector2.zero);
            WrapperBottom.Move(Vector3.up * MathHelper.SmallFloat);
            WrapperBottom.FlipTriangles();

            if (ModificationNodeOrganizer.ObjectOrientationGridSize.x == 0)
            {
                Vector3 offset = LinkedFloor.CurrentNodeWallSystem.HalfWallThickness * Vector3.left;

                MainSide.Move(offset);
                OtherSide.Move(offset);
                WrapperStartSide.Move(offset);
                WrapperOtherSide.Move(offset);
                WrapperBottom.Move(offset);
            }

            MainSide.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.transform);
            OtherSide.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.transform);

            if (RoofType == RectangularRoof.RoofTypes.HalfAngledSquareRoof)
            {
                WrapperStartSide.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.transform);
            }

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
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Set height to next bottom floor", delegate { SetHeightToNextBottomFloor(); }));
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Set height to next top floor", delegate { SetHeightToNextTopFloor(); }));
        }

        void SetHeightToNextTopFloor()
        {
            float nextFloorHeight;

            if (LinkedFloor.IsTopFloor) nextFloorHeight = LinkedFloor.BottomFloorHeight;
            else nextFloorHeight = LinkedFloor.FloorAbove.BottomFloorHeight;

            Height = LinkedFloor.CompleteFloorHeight + nextFloorHeight;

        }

        void SetHeightToNextBottomFloor()
        {
            Height = LinkedFloor.CompleteFloorHeight;
        }

        public override void MoveOnGrid(Vector2Int offset)
        {
            ModificationNodeOrganizer.MoveOnGrid(offset: offset);
        }
    }
}