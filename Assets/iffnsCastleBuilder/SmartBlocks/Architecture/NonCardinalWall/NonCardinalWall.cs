﻿using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class NonCardinalWall : OnFloorObject
    {
        //Build parameters
        MailboxLineVector2Int FirstBlockPositionParam;
        MailboxLineVector2Int SecondBlockPositionParam;
        MailboxLineDistinctNamed PreferenceTypeParam;
        MailboxLineMaterial MainMaterial;
        MailboxLineMaterial SecondMaterial;
        MailboxLineMaterial CeilingMaterial;

        BlockGridRectangleOrganizer ModificationNodeOrganizer;

        public override ModificationOrganizer Organizer
        {
            get
            {
                return ModificationNodeOrganizer;
            }
        }

        public PreferenceTypes PreferenceType
        {
            get
            {
                PreferenceTypes returnValue = (PreferenceTypes)PreferenceTypeParam.Val;

                return returnValue;
            }
            set
            {
                PreferenceTypeParam.Val = (int)value;
                ApplyBuildParameters();
            }
        }

        void SetupPreferenceTypeParam()
        {
            List<string> enumString = new List<string>();

            int enumValues = System.Enum.GetValues(typeof(PreferenceTypes)).Length;

            for (int i = 0; i < enumValues; i++)
            {
                PreferenceTypes type = (PreferenceTypes)i;

                enumString.Add(type.ToString());
            }

            PreferenceTypeParam = new MailboxLineDistinctNamed(
                "Preference type",
                CurrentMailbox,
                Mailbox.ValueType.buildParameter,
                enumString,
                0);
        }

        //Enum definitions
        public enum PreferenceTypes
        {
            Full,
            AttemptXWall,
            AttemptZWall
        }

        public override bool RaiseToFloor
        {
            get
            {
                return false;
            }
        }

        public override bool IsStructural
        {
            get
            {
                return true;
            }
        }

        float wallHeight
        {
            get
            {
                return LinkedFloor.WallHeightWithScaler + Random.Range(0.0015f, 0.001f);
            }
        }

        /*
        //Status parameters
        DoneTypes doneType;
        public DoneTypes DoneType
        {
            get
            {
                return doneType;
            }
        }
        */

        public enum DoneTypes
        {
            None,
            Cardinal,
            CreatedXWall,
            CreatedZWall,
            CreatedFullWall
        }

        void initializeBuildParameterLines()
        {
            FirstBlockPositionParam = new MailboxLineVector2Int(name: "First Block Position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            SecondBlockPositionParam = new MailboxLineVector2Int(name: "Second Block Position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            MainMaterial = new MailboxLineMaterial(name: "Main material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultStoneBricks);
            SecondMaterial = new MailboxLineMaterial(name: "Second material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultStoneBricks);
            CeilingMaterial = new MailboxLineMaterial(name: "Ceiling material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultCeiling);

            SetupPreferenceTypeParam();
        }


        public override void Setup(IBaseObject linkedFloor)
        {
            base.Setup(linkedFloor);

            LinkedFloor = linkedFloor as FloorController;

            initializeBuildParameterLines();

            BlockGridPositionModificationNode firstNode = ModificationNodeLibrary.NewBlockGridPositionModificationNode;
            firstNode.Setup(linkedObject: this, value: FirstBlockPositionParam);
            FirstPositionNode = firstNode;

            BlockGridPositionModificationNode secondNode = ModificationNodeLibrary.NewBlockGridPositionModificationNode;
            secondNode.Setup(linkedObject: this, value: SecondBlockPositionParam);
            SecondPositionNode = secondNode;

            ModificationNodeOrganizer = new BlockGridRectangleOrganizer(linkedObject: this, firstNode: firstNode, secondNode: secondNode);

            SetupEditButtons();
        }

        public void CompleteSetUpWithBuildParameters(FloorController linkedFloor, Vector2Int firstPosition, Vector2Int secondPosition)
        {
            Setup(linkedFloor);

            FirstBlockPositionParam.Val = firstPosition;
            SecondBlockPositionParam.Val = secondPosition;
        }

        public override void ResetObject()
        {
            baseReset();
        }

        public Vector2Int FirstPosition
        {
            set
            {
                FirstBlockPositionParam.Val = value;
                ApplyBuildParameters();
            }
        }

        public Vector2Int SecondPosition
        {
            set
            {
                SecondBlockPositionParam.Val = value;
                ApplyBuildParameters();
            }
        }

        TriangleMeshInfo TopCap2;
        TriangleMeshInfo Walls2;
        TriangleMeshInfo BottomCap2;

        //Generate wall frorm parameters
        public override void ApplyBuildParameters()
        {
            base.ApplyBuildParameters();

            //Check validity
            if (Failed) return;

            Vector2Int gridSize = ModificationNodeOrganizer.ObjectOrientationGridSize;

            if (gridSize.x <= 1 && gridSize.y <= 1
                || gridSize.x == 1 && gridSize.y == 2
                || gridSize.x == 2 && gridSize.y == 1)
            {
                Failed = true;
                return;
            }

            //Define mesh
            Vector2 size = ModificationNodeOrganizer.ObjectOrientationSize;
            float height = LinkedFloor.WallHeightWithScaler;

            TriangleMeshInfo TopCap = new(planar: true);
            List<TriangleMeshInfo> Walls = new();
            TriangleMeshInfo BottomCap = new(planar: true);

            void FinishMesh()
            {
                StaticMeshManager.AddTriangleInfoIfValid(TopCap);
                TopCap.MaterialReference = MainMaterial;

                foreach (TriangleMeshInfo info in Walls)
                {
                    info.MaterialReference = MainMaterial;
                    StaticMeshManager.AddTriangleInfoIfValid(info);
                }

                Walls[Walls.Count - 2].MaterialReference = SecondMaterial;

                StaticMeshManager.AddTriangleInfoIfValid(BottomCap);
                BottomCap.MaterialReference = CeilingMaterial;

                BuildAllMeshes();
            }


            List<Vector3> clockwiseEdgePoints = new List<Vector3>();

            bool planar = false;

            if (gridSize.x == 1)
            {
                SetZWall();
                planar = true;
            }
            else if (gridSize.y == 1)
            {
                SetXWall();
                planar = true;
            }
            else if(gridSize.x == 2 && gridSize.y == 2)
            {
                SetButterfly();
                FinishMesh();
                return;
            }
            else
            {
                SetDiagonalWall();
            }

            TopCap = MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(points: clockwiseEdgePoints, planar: true);
            BottomCap = TopCap.CloneFlipped;
            TopCap.Move(Vector3.up * height);

            Walls = MeshGenerator.MeshesFromLines.AddVerticalWallsBetweenMultiplePoints(floorPointsInClockwiseOrder: clockwiseEdgePoints, height: height, closed: true, uvOffset: transform.localPosition);

            foreach(TriangleMeshInfo info in Walls)
            {
                info.planar = planar;
            }

            foreach (TriangleMeshInfo info in Walls)
            {
                info.FlipTriangles();
            }

            void SetXWall()
            {
                clockwiseEdgePoints.Add(new Vector3(BlockSize, 0, BlockSize));
                clockwiseEdgePoints.Add(new Vector3(size.x - BlockSize, 0, BlockSize));
                clockwiseEdgePoints.Add(new Vector3(size.x - BlockSize, 0, 0));
                clockwiseEdgePoints.Add(new Vector3(BlockSize, 0, 0));
            }

            void SetZWall()
            {
                clockwiseEdgePoints.Add(new Vector3(0, 0, BlockSize));
                clockwiseEdgePoints.Add(new Vector3(0, 0, size.y - BlockSize));
                clockwiseEdgePoints.Add(new Vector3(BlockSize, 0, size.y - BlockSize));
                clockwiseEdgePoints.Add(new Vector3(BlockSize, 0, BlockSize));
            }

            void SetButterfly()
            {
                SetDiagonalWall();
                
                Walls = MeshGenerator.MeshesFromLines.AddVerticalWallsBetweenMultiplePoints(floorPointsInClockwiseOrder: clockwiseEdgePoints, height: height, closed: true, uvOffset: transform.localPosition);

                foreach (TriangleMeshInfo info in Walls)
                {
                    info.FlipTriangles();
                    info.planar = false;
                }

                TopCap.Add(MeshGenerator.MeshesFromPoints.MeshFrom3Points(clockwiseEdgePoints[0], clockwiseEdgePoints[1], clockwiseEdgePoints[2]));
                TopCap.Add(MeshGenerator.MeshesFromPoints.MeshFrom3Points(clockwiseEdgePoints[0], clockwiseEdgePoints[4], clockwiseEdgePoints[5]));
                BottomCap = TopCap.CloneFlipped;
                TopCap.Move(height * Vector3.up);
            }

            void SetDiagonalWall()
            {
                clockwiseEdgePoints.Add(new Vector3(BlockSize, 0, BlockSize));
                clockwiseEdgePoints.Add(new Vector3(0, 0, BlockSize));
                clockwiseEdgePoints.Add(new Vector3(size.x - BlockSize, 0, size.y));
                clockwiseEdgePoints.Add(new Vector3(size.x - BlockSize, 0, size.y - BlockSize));
                clockwiseEdgePoints.Add(new Vector3(size.x, 0, size.y - BlockSize));
                clockwiseEdgePoints.Add(new Vector3(BlockSize, 0, 0));
            }

            FinishMesh();
        }

        void SetupEditButtons()
        {
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Flip Vertical", delegate { ModificationNodeOrganizer.FlipDiagonally(); }));
        }

        public override void MoveOnGrid(Vector2Int offset)
        {
            ModificationNodeOrganizer.MoveOnGrid(offset);

            /*
            Vector2Int firstNodePosition = FirstBlockPositionParam.Val + offset;
            Vector2Int secondNodePosition = SecondBlockPositionParam.Val + offset;

            Vector2Int gridSize = LinkedFloor.LinkedBuildingController.GridSize;

            if (firstNodePosition.x < 0 || firstNodePosition.y < 0 || secondNodePosition.x < 0 || secondNodePosition.y < 0
                || firstNodePosition.x >= gridSize.x || firstNodePosition.y >= gridSize.y || secondNodePosition.x >= gridSize.x || secondNodePosition.y >= gridSize.y)
            {
                DestroyObject();
                return;
            }

            FirstBlockPositionParam.Val = firstNodePosition;
            SecondBlockPositionParam.Val = secondNodePosition;
            */
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