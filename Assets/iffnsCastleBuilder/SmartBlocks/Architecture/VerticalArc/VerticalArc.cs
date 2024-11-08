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
        MailboxLineRanged FreeHeightSideParam;
        MailboxLineRanged ArcHeightParam;
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

        float FreeHeightSide
        {
            get
            {
                return FreeHeightSideParam.Val;
            }
        }

        float ArcHeight
        {
            get
            {
                return ArcHeightParam.Val;
            }
        }

        /*
        void LimitHeights()
        {
            float remainingHeight = Height - FreeHeightSideParam.Val - ArcHeightParam.Val;

            if (remainingHeight < 0)
            {
                ArcHeightParam.Val += remainingHeight;

                remainingHeight = Height - FreeHeightSideParam.Val - ArcHeightParam.Val;

                if (remainingHeight < 0)
                {
                    FreeHeightSideParam.Val -= remainingHeight;
                }
            }
        }
        */

        //Derived parameters
        float Height
        {
            get
            {
                float returnValue = LinkedFloor.WallBetweenHeight;
                float minHeight = FreeHeightSide + ArcHeight;

                int currentFloorNumber = LinkedFloor.FloorNumber;

                float lastFloorHeight = 0;

                while (currentFloorNumber + 1 <= LinkedFloor.LinkedBuildingController.PositiveFloors && returnValue < minHeight)
                {
                    currentFloorNumber += 1;

                    FloorController floor = LinkedFloor.LinkedBuildingController.Floor(floorNumber: currentFloorNumber);

                    returnValue += floor.CompleteFloorHeight;
                }

                returnValue -= lastFloorHeight;

                return returnValue;
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
            FreeHeightSideParam = new MailboxLineRanged(name: "FreeHeihgtSide [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 5f, Min: 0f, DefaultValue: 2f);
            ArcHeightParam = new MailboxLineRanged(name: "Arc height [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 2f, Min: 0f, DefaultValue: 0.5f);
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
            TriangleMeshInfo FrontWall;
            TriangleMeshInfo BackWall;
            TriangleMeshInfo RightWall;
            TriangleMeshInfo LeftWall;
            TriangleMeshInfo TopWall;
            TriangleMeshInfo InnerArc;

            void FinishMeshes()
            {
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

            //Inner arc
            VerticesHolder arcLine = MeshGenerator.Lines.ArcAroundX(radius: length * 0.5f, angleDeg: 180, numberOfEdges: 24);
            arcLine.Scale(new Vector3(1, ArcHeightParam.Val / (length * 0.5f), 1));
            arcLine.Move(Vector3.up * FreeHeightSideParam.Val + Vector3.forward * length * 0.5f);

            InnerArc = MeshGenerator.MeshesFromLines.ExtrudeLinearWithSmoothCorners(firstLine: arcLine, offset: Vector3.right * width, closeType: MeshGenerator.ShapeClosingType.open, planar: false);

            //Front wall
            VerticesHolder rightArc = MeshGenerator.Lines.ArcAroundX(radius: length * 0.5f, angleDeg: 90, numberOfEdges: 12);
            rightArc.Scale(new Vector3(1, ArcHeightParam.Val / (length * 0.5f), 1));
            rightArc.Move(Vector3.up * FreeHeightSideParam.Val + Vector3.forward * length * 0.5f);
            Vector3 rightPoint = new Vector3(0, Height, length);

            FrontWall = MeshGenerator.MeshesFromLines.KnitLinesSmooth(point: rightPoint, line: rightArc, isClosed: false, planar: true);

            int currentIndex = FrontWall.VerticesHolder.Count - 1;

            VerticesHolder leftArc = MeshGenerator.Lines.ArcAroundX(radius: length * 0.5f, angleDeg: 90, numberOfEdges: 12);
            leftArc.Rotate(Quaternion.Euler(-90, 0, 0));
            leftArc.Scale(new Vector3(1, ArcHeightParam.Val / (length * 0.5f), 1));
            leftArc.Move(new Vector3(0, FreeHeightSideParam.Val, length * 0.5f));
            Vector3 leftPoint = new Vector3(0, Height, 0);
            leftArc.VerticesDirectly.RemoveAt(0);

            FrontWall.Add(MeshGenerator.MeshesFromLines.KnitLinesSmooth(point: leftPoint, line: leftArc, isClosed: false, planar: true));

            FrontWall.Triangles.Add(new TriangleHolder(baseOffset: currentIndex, t1: 0, t2: 1, t3: 2));
            FrontWall.Triangles.Add(new TriangleHolder(0, currentIndex + 1, currentIndex));

            //FrontWall.FlipTriangles();

            //Back wall
            BackWall = FrontWall.Clone;
            BackWall.Move(Vector3.right * width);
            BackWall.FlipTriangles();

            //Top wall
            TopWall = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.right * width, secondLine: Vector3.forward * length, uvOffset: new Vector2(0, Height));
            TopWall.Move(Vector3.up * Height);

            //Left wall
            LeftWall = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.up * (Height - FreeHeightSideParam.Val), secondLine: Vector3.right * width, uvOffset: Vector2.zero);
            LeftWall.FlipTriangles();
            LeftWall.Move(Vector3.up * FreeHeightSideParam.Val);

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