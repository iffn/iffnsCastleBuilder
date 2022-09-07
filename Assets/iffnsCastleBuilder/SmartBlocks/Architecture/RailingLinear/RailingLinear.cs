using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class RailingLinear : OnFloorObject
    {
        //Build parameters
        MailboxLineVector2Int StartCoordinateParam;
        MailboxLineVector2Int EndCoordinateParam;
        MailboxLineRanged TargetDistanceBetweenCylindersParam;
        MailboxLineMaterial TopMaterialParam;
        MailboxLineMaterial BottomMaterialParam;
        MailboxLineMaterial PostMaterialParam;

        readonly float railingHeight = 1.0f;
        readonly float cylinderDiameter = 0.1f;
        readonly float bottomHeight = 0.05f;
        readonly float topHeight = 0.05f;
        readonly float borderWidth = 0.1f;

        NodeGridRectangleOrganizer ModificationNodeOrganizer;

        public override ModificationOrganizer Organizer
        {
            get
            {
                return ModificationNodeOrganizer;
            }
        }

        public Vector2Int StartCoordinate
        {
            get
            {
                return StartCoordinateParam.Val;
            }
            set
            {
                StartCoordinateParam.Val = value;
                ApplyBuildParameters();
            }
        }

        public Vector2Int EndCoordinate
        {
            get
            {
                return EndCoordinateParam.Val;
            }
            set
            {
                EndCoordinateParam.Val = value;
                ApplyBuildParameters();
            }
        }

        public float TargetDistanceBetweenCylinders
        {
            get
            {
                return TargetDistanceBetweenCylindersParam.Val;
            }
            set
            {
                TargetDistanceBetweenCylindersParam.Val = value;
                ApplyBuildParameters();
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

        public override void Setup(IBaseObject linkedFloor)
        {
            base.Setup(linkedFloor);

            StartCoordinateParam = new MailboxLineVector2Int(name: "Start coordinate", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            EndCoordinateParam = new MailboxLineVector2Int(name: "End coordinate", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            TargetDistanceBetweenCylindersParam = new MailboxLineRanged(name: "Cylinder distance target [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 20, Min: 0.1f, DefaultValue: BlockSize);

            TopMaterialParam = new MailboxLineMaterial(name: "Top material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultWoodSolid);
            BottomMaterialParam = new MailboxLineMaterial(name: "Bottom material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultWoodSolid);
            PostMaterialParam = new MailboxLineMaterial(name: "Post material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultWoodSolid);

            NodeGridPositionModificationNode firstNode = ModificationNodeLibrary.NewNodeGridPositionModificationNode;
            firstNode.Setup(linkedObject: this, value: StartCoordinateParam);
            FirstPositionNode = firstNode;

            NodeGridPositionModificationNode secondNode = ModificationNodeLibrary.NewNodeGridPositionModificationNode;
            secondNode.Setup(linkedObject: this, value: EndCoordinateParam, relativeReferenceHolder: null);
            SecondPositionNode = secondNode;

            ModificationNodeOrganizer = new NodeGridRectangleOrganizer(linkedObject: this, firstNode: firstNode, secondNode: secondNode);
            ModificationNodeOrganizer.OrientationType = NodeGridRectangleOrganizer.OrientationTypes.NodeGrid;

            SetupEditButtons();
        }

        public void CompleteSetupWithBuildParameters(FloorController linkedFloor, Vector2Int startCoordinate, Vector2Int endCoordinate, float targetCylinderDistance)
        {
            Setup(linkedFloor);

            StartCoordinateParam.Val = startCoordinate;
            EndCoordinateParam.Val = endCoordinate;
            TargetDistanceBetweenCylindersParam.Val = targetCylinderDistance;
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

        List<GameObject> RailingPosts = new List<GameObject>();

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

            Vector2 gridSize = ModificationNodeOrganizer.ObjectOrientationGridSize;

            if (Failed) return;

            if (gridSize.y == 0)
            {
                Failed = true;
                return;
            }

            List<TriangleMeshInfo> TopBorder;
            List<TriangleMeshInfo> BottomBorder;
            List<TriangleMeshInfo> Caps = new();
            TriangleMeshInfo Posts = new(planar: false);

            void FinishMesh()
            {
                Posts.MaterialReference = PostMaterialParam;

                AddStaticMesh(Posts);

                foreach (TriangleMeshInfo info in TopBorder)
                {
                    AddStaticMesh(info);
                    info.MaterialReference = TopMaterialParam;
                }

                foreach (TriangleMeshInfo info in BottomBorder)
                {
                    AddStaticMesh(info);
                    info.MaterialReference = BottomMaterialParam;
                }

                foreach (TriangleMeshInfo info in Caps)
                {
                    AddStaticMesh(info);
                    info.MaterialReference = PostMaterialParam;
                }

                BuildAllMeshes();
            }

            //Define mesh
            Vector2 size = ModificationNodeOrganizer.ObjectOrientationSize;

            TopBorder = MeshGenerator.FilledShapes.BoxAroundCenter(new Vector3(
                borderWidth,
                topHeight,
                size.y
                ));

            BottomBorder = TriangleMeshInfo.GetCloneOfInfoList(infos: TopBorder, flip: false);

            foreach(TriangleMeshInfo info in TopBorder)
            {
                info.Move(new Vector3(
                    0,
                    railingHeight - topHeight * 0.5f,
                    size.y * 0.5f
                    ));
            }

            foreach (TriangleMeshInfo info in BottomBorder)
            {
                info.Move(new Vector3(
                    0,
                    bottomHeight * 0.5f,
                    size.y * 0.5f
                    ));
            }

            //Railing posts
            float baseDistance = size.y;
            float floatingCount = baseDistance / TargetDistanceBetweenCylinders;
            int betweenCount = Mathf.RoundToInt(floatingCount);
            betweenCount = MathHelper.ClampIntMin(value: betweenCount, min: 1);

            float betweenDistance = baseDistance / betweenCount;

            float postHeight = railingHeight - topHeight;
            float halfPostHeight = postHeight * 0.5f;
            float halfRailingHeight = railingHeight * 0.5f;

            TriangleMeshInfo currentPost;
            List<TriangleMeshInfo> currentCap;

            for (int i = 1; i < betweenCount; i++)
            {
                currentPost = MeshGenerator.FilledShapes.CylinderAroundCenterWithoutCap(radius: cylinderDiameter * 0.5f, length: postHeight, direction: Vector3.up, numberOfEdges: 24);

                currentPost.Move(new Vector3(
                    0,
                    halfPostHeight,
                    betweenDistance * i
                    ));

                Posts.Add(currentPost);
            }

            //Start post
            currentPost = MeshGenerator.FilledShapes.CylinderAroundCenterWithoutCap(radius: cylinderDiameter * 0.5f, length: railingHeight, direction: Vector3.up, numberOfEdges: 24);
            currentCap = MeshGenerator.FilledShapes.CylinderCaps(radius: cylinderDiameter * 0.5f, length: railingHeight + MathHelper.SmallFloat, direction: Vector3.up, numberOfEdges: 24);

            currentPost.Move(new Vector3(
                    0,
                    halfRailingHeight,
                    0
                    ));

            foreach(TriangleMeshInfo info in currentCap)
            {
                info.Move(new Vector3(
                    0,
                    halfRailingHeight,
                    0
                    ));
            }

            Caps.AddRange(currentCap);
            Posts.Add(currentPost);

            //End post
            currentPost = MeshGenerator.FilledShapes.CylinderAroundCenterWithoutCap(radius: cylinderDiameter * 0.5f, length: railingHeight, direction: Vector3.up, numberOfEdges: 24);
            currentCap = MeshGenerator.FilledShapes.CylinderCaps(radius: cylinderDiameter * 0.5f, length: railingHeight + MathHelper.SmallFloat, direction: Vector3.up, numberOfEdges: 24);

            currentPost.Move(new Vector3(
                    0,
                    halfRailingHeight,
                    baseDistance
                    ));

            foreach (TriangleMeshInfo info in currentCap)
            {
                info.Move(new Vector3(
                    0,
                    halfRailingHeight,
                    baseDistance
                    ));
            }

            Caps.AddRange(currentCap);
            Posts.Add(currentPost);

            FinishMesh();
        }

        void SetupEditButtons()
        {

        }

        public override void MoveOnGrid(Vector2Int offset)
        {
            ModificationNodeOrganizer.MoveOnGrid(offset: offset);
        }
    }
}