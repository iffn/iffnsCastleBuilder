using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class RectangularRoof : OnFloorObject
    {
        //Mailbox lines
        MailboxLineVector2Int FirstPositionParam;
        MailboxLineVector2Int SecondPositionParam;
        MailboxLineRanged HeightParam;
        MailboxLineRanged HeightThicknessParam;
        MailboxLineRanged RoofOvershootParam;
        MailboxLineDistinctNamed RoofTypeParam;
        MailboxLineBool RaiseToFloorParam;

        MailboxLineMaterial OutsideMaterialParam;
        MailboxLineMaterial InsideMaterialParam;
        MailboxLineMaterial WrapperMaterialParam;

        NodeGridRectangleOrganizer ModificationNodeOrganizer;

        //WallTypes wallType = WallTypes.NodeWall;

        public override ModificationOrganizer Organizer
        {
            get
            {
                return ModificationNodeOrganizer;
            }
        }

        public Vector2Int FirstPosition
        {
            set
            {
                FirstPositionParam.Val = value;
                ApplyBuildParameters();
            }
        }

        public Vector2Int SecondPosition
        {
            set
            {
                SecondPositionParam.Val = value;
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

        public float HeightThickness
        {
            get
            {
                return HeightThicknessParam.Val;
            }
            set
            {
                HeightThicknessParam.Val = value;
                ApplyBuildParameters();
            }
        }

        public float RoofOvershoot
        {
            get
            {
                return RoofOvershootParam.Val;
            }
            set
            {
                RoofOvershootParam.Val = value;
                ApplyBuildParameters();
            }
        }

        public RoofTypes RoofType
        {
            get
            {
                RoofTypes returnValue = (RoofTypes)RoofTypeParam.Val;

                return returnValue;
            }
            set
            {
                RoofTypeParam.Val = (int)value;

                ApplyBuildParameters();
            }
        }

        void SetupRoofTypeParam()
        {
            List<string> enumString = new List<string>();

            int enumValues = System.Enum.GetValues(typeof(RoofTypes)).Length;

            for (int i = 0; i < enumValues; i++)
            {
                RoofTypes type = (RoofTypes)i;

                enumString.Add(type.ToString());
            }

            RoofTypeParam = new MailboxLineDistinctNamed(
                "Roof type",
                CurrentMailbox,
                Mailbox.ValueType.buildParameter,
                enumString,
                0);
        }

        public enum RoofTypes
        {
            FullAngledSquareRoof,
            HalfAngledSquareRoof,
        }

        public override bool RaiseToFloor
        {
            get
            {
                return RaiseToFloorParam.Val;
            }
        }

        public override bool IsStructural
        {
            get
            {
                return true;
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
            if (linkedFloor == null) Debug.LogWarning("Error, linked floor is not a floor. Super object is instead = " + linkedFloor.IdentifierString);

            FirstPositionParam = new MailboxLineVector2Int(name: "First position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            SecondPositionParam = new MailboxLineVector2Int(name: "Second position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            HeightParam = new MailboxLineRanged(name: "Roof height [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 10, Min: 0.5f, DefaultValue: 2);
            HeightThicknessParam = new MailboxLineRanged(name: "Height thickness [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 1f, Min: 0.001f, DefaultValue: 0.1f);
            RoofOvershootParam = new MailboxLineRanged(name: "Roof overshoot [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 10, Min: 0f, DefaultValue: 0);
            RaiseToFloorParam = new MailboxLineBool(name: "Raise to floor", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: true);
            OutsideMaterialParam = new MailboxLineMaterial(name: "Outside material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultRoof);
            InsideMaterialParam = new MailboxLineMaterial(name: "Inside material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultCeiling);
            WrapperMaterialParam = new MailboxLineMaterial(name: "Wrapper material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultPlaster);

            SetupRoofTypeParam();

            NodeGridPositionModificationNode firstNode = ModificationNodeLibrary.NewNodeGridPositionModificationNode;
            firstNode.Setup(linkedObject: this, value: FirstPositionParam);
            FirstPositionNode = firstNode;

            NodeGridPositionModificationNode secondNode = ModificationNodeLibrary.NewNodeGridPositionModificationNode;
            secondNode.Setup(linkedObject: this, value: SecondPositionParam);
            SecondPositionNode = secondNode;

            ModificationNodeOrganizer = new NodeGridRectangleOrganizer(linkedObject: this, firstNode: firstNode, secondNode: secondNode);

            SetupEditButtons();
        }

        public void CompleteSetUpWithBuildParameters(FloorController linkedFloor, Vector2Int firstPosition, Vector2Int secondPosition, float roofHeight, float roofOvershoot, RoofTypes roofType)
        {
            Setup(linkedFloor);

            //Using the accesors would apply build parameters
            FirstPositionParam.Val = firstPosition;
            SecondPositionParam.Val = secondPosition;
            HeightParam.Val = roofHeight;
            RoofTypeParam.Val = (int)roofType;
            RoofOvershoot = roofOvershoot;
        }

        public override void ResetObject()
        {
            baseReset();
        }

        public enum WallTypes
        {
            BlockWall,
            NodeWall
        }
        /*
        public void SetStaticMeshes(List<TriangleMeshInfo> staticMeshes)
        {
            foreach(TriangleMeshInfo mesh in staticMeshes)
            {
                StaticMeshManager.AddTriangleInfo(mesh);
            }
        }
        */

        public void AddStaticMesh(TriangleMeshInfo staticMesh)
        {
            if (staticMesh == null) return;

            StaticMeshManager.AddTriangleInfoIfValid(staticMesh);
        }

        public override void ApplyBuildParameters()
        {
            base.ApplyBuildParameters();

            //Check validity
            if (Failed)
            {
                return;
            }

            if (ModificationNodeOrganizer.ObjectOrientationGridSize.x == 0)
            {
                Failed = true;
                return;
            }

            //Define mesh
            Vector2 size = ModificationNodeOrganizer.ObjectOrientationSize;

            switch (RoofType)
            {
                case RoofTypes.FullAngledSquareRoof:
                    UpdateFullRoof(size: size);
                    break;
                case RoofTypes.HalfAngledSquareRoof:
                    UpdateHalfRoof(size: size);
                    break;
                default:
                    break;
            }

            BuildAllMeshes();
        }

        void UpdateHalfRoof(Vector2 size)
        {
            TriangleMeshInfo RoofOutside;
            TriangleMeshInfo RoofInside;
            TriangleMeshInfo TopWrapper;
            TriangleMeshInfo BottomWrapper;
            TriangleMeshInfo FrontWall;
            TriangleMeshInfo BackWall;

            void FinishMesh()
            {
                RoofOutside.MaterialReference = OutsideMaterialParam;
                RoofInside.MaterialReference = InsideMaterialParam;
                TopWrapper.MaterialReference = WrapperMaterialParam;
                BottomWrapper.MaterialReference = WrapperMaterialParam;
                FrontWall.MaterialReference = WrapperMaterialParam;
                BackWall.MaterialReference = WrapperMaterialParam;

                AddStaticMesh(RoofOutside);
                AddStaticMesh(RoofInside);
                AddStaticMesh(TopWrapper);
                AddStaticMesh(BottomWrapper);
                AddStaticMesh(FrontWall);
                AddStaticMesh(BackWall);
            }

            //Offset calculation
            float xOffset = HeightThickness * size.y / Height;

            //Roof outside
            RoofOutside = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.right * size.x, secondLine: new Vector3(0, Height, -size.y), uvOffset: Vector2.zero);
            RoofOutside.Move(Vector3.forward * size.y);

            //Roof inside
            RoofInside = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.left * size.x, secondLine: new Vector3(0, Height - HeightThickness, -size.y + xOffset), uvOffset: Vector2.zero);
            RoofInside.Move(new Vector3(size.x, 0, size.y - xOffset));

            //Top wrapper
            TopWrapper = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.left * size.x, secondLine: Vector3.up * HeightThickness, uvOffset: Vector2.zero);
            TopWrapper.Move(new Vector3(size.x, Height - HeightThickness, 0));
            TopWrapper.FlipTriangles();

            //Bottom wrapper
            BottomWrapper = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.right * size.x, secondLine: Vector3.forward * xOffset, uvOffset: Vector2.zero);
            BottomWrapper.Move(Vector3.forward * (size.y - xOffset));
            BottomWrapper.FlipTriangles();

            //Side wrapper 1
            FrontWall = MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(points: new List<Vector3>()
            {
                new Vector3(0, 0, size.y - xOffset),
                new Vector3(0,0, size.y),
                new Vector3(0, Height, 0),
                new Vector3(0, Height - HeightThickness, 0),
            }, planar: true);


            //Sided wrapper 2
            BackWall = FrontWall.CloneFlipped;
            BackWall.Move(Vector3.right * size.x);

            RoofOutside.FlipTriangles();
            RoofInside.FlipTriangles();

            FinishMesh();
        }

        void UpdateFullRoof(Vector2 size)
        {
            TriangleMeshInfo RoofOutsideA;
            TriangleMeshInfo RoofOutsideB;
            TriangleMeshInfo RoofInsideA;
            TriangleMeshInfo RoofInsideB;
            TriangleMeshInfo BottomWrapper = new(planar: true);
            TriangleMeshInfo FrontWall;
            TriangleMeshInfo BackWall;

            void FinishMesh()
            {
                RoofOutsideA.MaterialReference = OutsideMaterialParam;
                RoofOutsideB.MaterialReference = OutsideMaterialParam;
                RoofInsideA.MaterialReference = InsideMaterialParam;
                RoofInsideB.MaterialReference = InsideMaterialParam;
                BottomWrapper.MaterialReference = WrapperMaterialParam;
                FrontWall.MaterialReference = WrapperMaterialParam;
                BackWall.MaterialReference = WrapperMaterialParam;

                AddStaticMesh(RoofOutsideA);
                AddStaticMesh(RoofOutsideB);
                AddStaticMesh(RoofInsideA);
                AddStaticMesh(RoofInsideB);
                AddStaticMesh(BottomWrapper);
                AddStaticMesh(FrontWall);
                AddStaticMesh(BackWall);
            }

            float halfWidth = size.y * 0.5f;
            float xOffset = HeightThickness * halfWidth / Height;

            TriangleMeshInfo tempShape;

            //Roof outside
            RoofOutsideA = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.right * size.x, secondLine: new Vector3(0, Height, -halfWidth), uvOffset: Vector2.zero);
            RoofOutsideA.Move(Vector3.forward * size.y);
            RoofOutsideA.FlipTriangles();

            RoofOutsideB = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.left * size.x, secondLine: new Vector3(0, Height, halfWidth), uvOffset: Vector2.zero);
            RoofOutsideB.Move(Vector3.right * size.x);
            RoofOutsideB.FlipTriangles();

            //Roof inside
            RoofInsideA = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.left * size.x, secondLine: new Vector3(0, Height - HeightThickness, -halfWidth + xOffset), uvOffset: Vector2.zero);
            RoofInsideA.Move(new Vector3(size.x, 0, size.y - xOffset));
            RoofInsideA.FlipTriangles();

            RoofInsideB = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.right * size.x, secondLine: new Vector3(0, Height - HeightThickness, halfWidth - xOffset), uvOffset: Vector2.zero);
            RoofInsideB.Move(Vector3.forward * xOffset);
            RoofInsideB.FlipTriangles();

            //Side wrapper 1
            FrontWall = MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(points: new List<Vector3>()
                {
                    new Vector3(0, Height - HeightThickness, halfWidth),
                    new Vector3(0, 0, size.y - xOffset),
                    new Vector3(0, 0, size.y),
                    new Vector3(0, Height, halfWidth),
                    new Vector3(0, 0, 0),
                    new Vector3(0, 0, xOffset)
                }, planar: true);

            //Side wrapper 2
            BackWall = FrontWall.CloneFlipped;
            BackWall.Move(Vector3.right * size.x);

            //Bottom wrapper
            tempShape = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.forward * xOffset, secondLine: Vector3.right * size.x, uvOffset: Vector2.zero);
            BottomWrapper.Add(tempShape);
            tempShape = tempShape.Clone;
            tempShape.Move(Vector3.forward * (size.y - xOffset));
            BottomWrapper.Add(tempShape);

            FinishMesh();
        }


        public override void MoveOnGrid(Vector2Int offset)
        {
            ModificationNodeOrganizer.MoveOnGrid(offset: offset);
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
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Flip Diagonally", delegate { ModificationNodeOrganizer.FlipDiagonally(); }));
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Flip Vertical", delegate { ModificationNodeOrganizer.FlipVertical(); }));
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Flip Horizontal", delegate { ModificationNodeOrganizer.FlipHorizontal(); }));
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

            if (RaiseToFloor)
            {
                Height = LinkedFloor.WallBetweenHeight + nextFloorHeight;
            }
            else
            {
                Height = LinkedFloor.CompleteFloorHeight + nextFloorHeight;
            }
        }

        void SetHeightToNextBottomFloor()
        {
            if (RaiseToFloor)
            {
                Height = LinkedFloor.WallBetweenHeight;
            }
            else
            {
                Height = LinkedFloor.CompleteFloorHeight;
            }
        }
    }
}