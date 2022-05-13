using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class VerticalArc : OnFloorObject
    {
        public const string constIdentifierString = "Vertical arc";
        public override string IdentifierString
        {
            get
            {
                return constIdentifierString;
            }
        }

        //Assignments


        //Build parameters
        MailboxLineVector2Int BottomLeftPositionParam;
        MailboxLineVector2Int TopRightPositionParam;
        MailboxLineRanged FreeHeightSide;
        MailboxLineRanged ArcHeight;
        MailboxLineMaterial MainMaterialParam;

        NodeGridRectangleOrganizer ModificationNodeOrganizer;

        public override ModificationOrganizer Organizer
        {
            get
            {
                return ModificationNodeOrganizer;
            }
        }

        void LimitSizes()
        {
            float remainingHeight = Height - FreeHeightSide.Val - ArcHeight.Val;

            if (remainingHeight < 0)
            {
                ArcHeight.Val += remainingHeight;

                remainingHeight = Height - FreeHeightSide.Val - ArcHeight.Val;

                if (remainingHeight < 0)
                {
                    FreeHeightSide.Val -= remainingHeight;
                }
            }
        }

        //Derived parameters
        float Height
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
            FreeHeightSide = new MailboxLineRanged(name: "FreeHeihgtSide [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 5f, Min: 0f, DefaultValue: 1.8f);
            ArcHeight = new MailboxLineRanged(name: "Arc height [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 2f, Min: 0.3f, DefaultValue: 0.5f);
            MainMaterialParam = new MailboxLineMaterial(name: "Main material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultStoneBricks);
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
            failed = false;

            TriangleMeshInfo FrontWall;
            TriangleMeshInfo BackWall;
            TriangleMeshInfo RightWall;
            TriangleMeshInfo LeftWall;
            TriangleMeshInfo TopWall;
            TriangleMeshInfo InnerArc;

            void FinishMeshes()
            {
                FrontWall.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.LinkedBuildingController.transform);
                BackWall.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.LinkedBuildingController.transform);
                RightWall.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.LinkedBuildingController.transform);
                LeftWall.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.LinkedBuildingController.transform);
                TopWall.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.LinkedBuildingController.transform);

                StaticMeshManager.AddTriangleInfo(FrontWall);
                StaticMeshManager.AddTriangleInfo(BackWall);
                StaticMeshManager.AddTriangleInfo(RightWall);
                StaticMeshManager.AddTriangleInfo(LeftWall);
                StaticMeshManager.AddTriangleInfo(TopWall);
                StaticMeshManager.AddTriangleInfo(InnerArc);

                FrontWall.MaterialReference = MainMaterialParam;
                BackWall.MaterialReference = MainMaterialParam;
                RightWall.MaterialReference = MainMaterialParam;
                LeftWall.MaterialReference = MainMaterialParam;
                TopWall.MaterialReference = MainMaterialParam;
                InnerArc.MaterialReference = MainMaterialParam;

                BuildAllMeshes();
            }

            ModificationNodeOrganizer.SetLinkedObjectPositionAndOrientation(raiseToFloor: false);

            Vector2 size = ModificationNodeOrganizer.ObjectOrientationSize;

            if (ModificationNodeOrganizer.ObjectOrientationGridSize.x == 0)
            {
                failed = true;
                return;
            }

            LimitSizes();

            if (ModificationNodeOrganizer.ObjectOrientationGridSize.y == 0)
            {
                size.y = LinkedFloor.CurrentNodeWallSystem.WallThickness;
            }

            //Inner arc
            VerticesHolder arcLine = MeshGenerator.Lines.ArcAroundZ(radius: size.x * 0.5f, angleDeg: 180, numberOfEdges: 24);
            arcLine.Scale(new Vector3(1, ArcHeight.Val / (size.x * 0.5f), 1));
            arcLine.Move(Vector3.up * FreeHeightSide.Val + Vector3.right * size.x * 0.5f);

            InnerArc = MeshGenerator.MeshesFromLines.ExtrudeLinear(firstLine: arcLine, offset: Vector3.forward * size.y, closeType: MeshGenerator.ShapeClosingType.open, smoothTransition: true);
            InnerArc.FlipTriangles();

            //Front wall

            VerticesHolder rightArc = MeshGenerator.Lines.ArcAroundZ(radius: size.x * 0.5f, angleDeg: 90, numberOfEdges: 12);
            rightArc.Scale(new Vector3(1, ArcHeight.Val / (size.x * 0.5f), 1));
            rightArc.Move(Vector3.up * FreeHeightSide.Val + Vector3.right * size.x * 0.5f);
            Vector3 rightPoint = new Vector3(size.x, Height, 0);

            FrontWall = MeshGenerator.MeshesFromLines.KnitLines(point: rightPoint, line: rightArc, isClosed: false);

            int currentIndex = FrontWall.VerticesHolder.Count - 1;

            VerticesHolder leftArc = MeshGenerator.Lines.ArcAroundZ(radius: size.x * 0.5f, angleDeg: 90, numberOfEdges: 12);
            leftArc.Rotate(Quaternion.Euler(0, 0, 90));
            leftArc.Scale(new Vector3(1, ArcHeight.Val / (size.x * 0.5f), 1));
            leftArc.Move(new Vector3(size.x * 0.5f, FreeHeightSide.Val, 0));
            Vector3 leftPoint = new Vector3(0, Height, 0);
            leftArc.Vertices.RemoveAt(0);

            FrontWall.Add(MeshGenerator.MeshesFromLines.KnitLines(point: leftPoint, line: leftArc, isClosed: false));

            FrontWall.Triangles.Add(new TriangleHolder(baseOffset: currentIndex, t1: 0, t2: 1, t3: 2));
            FrontWall.Triangles.Add(new TriangleHolder(0, currentIndex + 1, currentIndex));

            FrontWall.FlipTriangles();

            //Back wall
            BackWall = FrontWall.Clone;
            BackWall.Move(Vector3.forward * size.y);
            BackWall.FlipTriangles();

            //Top wall
            TopWall = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.right * size.x, secondLine: Vector3.forward * size.y, UVOffset: new Vector2(0, Height));
            TopWall.Move(Vector3.up * Height);

            //Left wall
            LeftWall = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.up * (Height - FreeHeightSide.Val), secondLine: Vector3.forward * size.y, UVOffset: Vector2.zero);
            LeftWall.Move(Vector3.up * FreeHeightSide.Val);

            //RightWall
            RightWall = LeftWall.Clone;
            RightWall.FlipTriangles();
            RightWall.Move(Vector3.right * size.x);

            if (ModificationNodeOrganizer.ObjectOrientationGridSize.y == 0)
            {
                FrontWall.Move(Vector3.back * LinkedFloor.CurrentNodeWallSystem.HalfWallThickness);
                BackWall.Move(Vector3.back * LinkedFloor.CurrentNodeWallSystem.HalfWallThickness);
                RightWall.Move(Vector3.back * LinkedFloor.CurrentNodeWallSystem.HalfWallThickness);
                LeftWall.Move(Vector3.back * LinkedFloor.CurrentNodeWallSystem.HalfWallThickness);
                TopWall.Move(Vector3.back * LinkedFloor.CurrentNodeWallSystem.HalfWallThickness);
                InnerArc.Move(Vector3.back * LinkedFloor.CurrentNodeWallSystem.HalfWallThickness);
            }

            FinishMeshes();
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