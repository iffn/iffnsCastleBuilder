using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class VerticalArc : OnFloorObject
    {
        //Assignments

        //Build parameters
        MailboxLineVector2Int BottomLeftPositionParam;
        MailboxLineVector2Int TopRightPositionParam;
        MailboxLineRanged FreeHeightSide;
        MailboxLineRanged ArcHeight;
        MailboxLineMaterial MainMaterialParam;
        MailboxLineMaterial OtherSideMaterialParam;

        NodeGridRectangleOrganizer ModificationNodeOrganizer;

        public override ModificationOrganizer Organizer
        {
            get
            {
                return ModificationNodeOrganizer;
            }
        }

        void LimitHeights()
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
                return LinkedFloor.WallBetweenHeight;
            }
        }

        public override bool RaiseToFloor
        {
            get
            {
                return true;
            }
        }

        public override bool IsStructural
        {
            get
            {
                return true;
            }
        }

        void InitializeBuildParameterLines()
        {
            BottomLeftPositionParam = new MailboxLineVector2Int(name: "Bottom left position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            TopRightPositionParam = new MailboxLineVector2Int(name: "Top right position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            FreeHeightSide = new MailboxLineRanged(name: "FreeHeihgtSide [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 5f, Min: 0f, DefaultValue: 2f);
            ArcHeight = new MailboxLineRanged(name: "Arc height [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 2f, Min: 0.3f, DefaultValue: 0.5f);
            MainMaterialParam = new MailboxLineMaterial(name: "Main material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultStoneBricks);
            OtherSideMaterialParam = new MailboxLineMaterial(name: "Other side material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultStoneBricks);
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
            base.ApplyBuildParameters();

            //Check validity
            if (Failed) return;

            if (ModificationNodeOrganizer.ObjectOrientationGridSize.y == 0)
            {
                Failed = true;
                return;
            }

            //Define mesh
            TriangleMeshInfo FrontWall = new();
            TriangleMeshInfo BackWall = new();
            TriangleMeshInfo RightWall = new();
            TriangleMeshInfo LeftWall = new();
            TriangleMeshInfo TopWall = new();
            TriangleMeshInfo InnerArc = new();

            void FinishMeshes()
            {
                FrontWall.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.LinkedBuildingController.transform);
                BackWall.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.LinkedBuildingController.transform);
                RightWall.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.LinkedBuildingController.transform);
                LeftWall.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.LinkedBuildingController.transform);
                TopWall.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.LinkedBuildingController.transform);

                StaticMeshManager.AddTriangleInfoIfValid(FrontWall);
                StaticMeshManager.AddTriangleInfoIfValid(BackWall);
                StaticMeshManager.AddTriangleInfoIfValid(RightWall);
                StaticMeshManager.AddTriangleInfoIfValid(LeftWall);
                StaticMeshManager.AddTriangleInfoIfValid(TopWall);
                StaticMeshManager.AddTriangleInfoIfValid(InnerArc);

                FrontWall.MaterialReference = MainMaterialParam;
                BackWall.MaterialReference = OtherSideMaterialParam;
                RightWall.MaterialReference = MainMaterialParam;
                LeftWall.MaterialReference = MainMaterialParam;
                TopWall.MaterialReference = MainMaterialParam;
                InnerArc.MaterialReference = MainMaterialParam;

                BuildAllMeshes();
            }

            Vector2 size = ModificationNodeOrganizer.ObjectOrientationSize;

            float length = size.y;
            float width;

            if (ModificationNodeOrganizer.ObjectOrientationGridSize.x == 0)
            {
                width = LinkedFloor.CurrentNodeWallSystem.WallThickness;
            }
            else
            {
                width = size.x;
            }

            LimitHeights();

            //Inner arc
            VerticesHolder arcLine = MeshGenerator.Lines.ArcAroundX(radius: length * 0.5f, angleDeg: 180, numberOfEdges: 24);
            arcLine.Scale(new Vector3(1, ArcHeight.Val / (length * 0.5f), 1));
            arcLine.Move(Vector3.up * FreeHeightSide.Val + Vector3.forward * length * 0.5f);

            InnerArc = MeshGenerator.MeshesFromLines.ExtrudeLinear(firstLine: arcLine, offset: Vector3.right * width, closeType: MeshGenerator.ShapeClosingType.open, smoothTransition: true);

            //Front wall
            VerticesHolder rightArc = MeshGenerator.Lines.ArcAroundX(radius: length * 0.5f, angleDeg: 90, numberOfEdges: 12);
            rightArc.Scale(new Vector3(1, ArcHeight.Val / (length * 0.5f), 1));
            rightArc.Move(Vector3.up * FreeHeightSide.Val + Vector3.forward * length * 0.5f);
            Vector3 rightPoint = new Vector3(0, Height, length);

            FrontWall = MeshGenerator.MeshesFromLines.KnitLines(point: rightPoint, line: rightArc, isClosed: false);

            int currentIndex = FrontWall.VerticesHolder.Count - 1;

            VerticesHolder leftArc = MeshGenerator.Lines.ArcAroundX(radius: length * 0.5f, angleDeg: 90, numberOfEdges: 12);
            leftArc.Rotate(Quaternion.Euler(-90, 0, 0));
            leftArc.Scale(new Vector3(1, ArcHeight.Val / (length * 0.5f), 1));
            leftArc.Move(new Vector3(0, FreeHeightSide.Val, length * 0.5f));
            Vector3 leftPoint = new Vector3(0, Height, 0);
            leftArc.Vertices.RemoveAt(0);

            FrontWall.Add(MeshGenerator.MeshesFromLines.KnitLines(point: leftPoint, line: leftArc, isClosed: false));

            FrontWall.Triangles.Add(new TriangleHolder(baseOffset: currentIndex, t1: 0, t2: 1, t3: 2));
            FrontWall.Triangles.Add(new TriangleHolder(0, currentIndex + 1, currentIndex));

            //FrontWall.FlipTriangles();

            //Back wall
            BackWall = FrontWall.Clone;
            BackWall.Move(Vector3.right * width);
            BackWall.FlipTriangles();

            //Top wall
            TopWall = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.right * width, secondLine: Vector3.forward * length, UVOffset: new Vector2(0, Height));
            TopWall.Move(Vector3.up * Height);

            //Left wall
            LeftWall = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.up * (Height - FreeHeightSide.Val), secondLine: Vector3.right * width, UVOffset: Vector2.zero);
            LeftWall.FlipTriangles();
            LeftWall.Move(Vector3.up * FreeHeightSide.Val);

            //RightWall
            RightWall = LeftWall.Clone;
            RightWall.FlipTriangles();
            RightWall.Move(Vector3.forward * length);

            if (ModificationNodeOrganizer.ObjectOrientationGridSize.x == 0)
            {
                FrontWall.Move(width * 0.5f * Vector3.left);
                BackWall.Move(width * 0.5f * Vector3.left);
                RightWall.Move(width * 0.5f * Vector3.left);
                LeftWall.Move(width * 0.5f * Vector3.left);
                TopWall.Move(width * 0.5f * Vector3.left);
                InnerArc.Move(width * 0.5f * Vector3.left);
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