using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class RailingLinear : OnFloorObject
    {
        [SerializeField] UnityMeshManager TopBorder;
        [SerializeField] UnityMeshManager BottomBorder;
        [SerializeField] GameObject RailingPostTemplate;

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

            //AllStaticMeshes.managedMeshes.AddRange();

            TopBorder.Setup(mainObject: this, currentMaterialReference: TopMaterialParam);
            BottomBorder.Setup(mainObject: this, currentMaterialReference: BottomMaterialParam);
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

        public override void ApplyBuildParameters()
        {
            UnmanagedMeshes.Clear();
            UnmanagedMeshes.Add(TopBorder);
            UnmanagedMeshes.Add(BottomBorder);

            failed = false;

            ModificationNodeOrganizer.SetLinkedObjectPositionAndOrientation(raiseToFloor: true);

            Vector2 size = ModificationNodeOrganizer.ObjectOrientationSize;
            Vector2 gridSize = ModificationNodeOrganizer.ObjectOrientationGridSize;

            if (failed) return;

            if (gridSize.y == 0)
            {
                failed = true;
                return;
            }

            TopBorder.transform.localScale = new Vector3(
                borderWidth,
                topHeight, 
                size.y
                );
            TopBorder.transform.localPosition = new Vector3(
                0,
                railingHeight - topHeight * 0.5f, 
                size.y * 0.5f
                );

            BottomBorder.transform.localScale = new Vector3(
                borderWidth,
                bottomHeight, 
                size.y
                );
            BottomBorder.transform.localPosition = new Vector3(
                0,
                bottomHeight * 0.5f, 
                size.y * 0.5f
                );

            //Railing posts
            while (RailingPosts.Count > 0)
            {
                GameObject post = RailingPosts[0];
                UsuallyActiveColliders.Remove(post.transform.GetChild(0).GetComponent<CapsuleCollider>());
                RailingPosts.Remove(post);
                Destroy(post);
            }

            float baseDistance = size.y;
            float floatingCount = baseDistance / TargetDistanceBetweenCylinders;
            int betweenCount = Mathf.RoundToInt(floatingCount);
            betweenCount = MathHelper.ClampIntMin(value: betweenCount, min: 1);

            float betweenDistance = baseDistance / betweenCount;

            List<MeshFilter> allMeshFilters = new List<MeshFilter>();

            float postHeight = railingHeight - topHeight;
            float halfPostHeight = postHeight * 0.5f;

            for (int i = 0; i <= betweenCount; i++)
            {
                GameObject newPost = Instantiate(original: RailingPostTemplate);

                newPost.transform.parent = transform;
                newPost.transform.localPosition = new Vector3(
                    0,
                    halfPostHeight, 
                    betweenDistance * i
                    );
                newPost.transform.localScale = new Vector3(cylinderDiameter, postHeight, cylinderDiameter);

                RailingPosts.Add(newPost);
                UsuallyActiveColliders.Add(newPost.transform.GetChild(0).GetComponent<CapsuleCollider>());

                UnityMeshManager manager = newPost.transform.GetChild(0).GetComponent<UnityMeshManager>();
                manager.Setup(mainObject: this, currentMaterialReference: PostMaterialParam);
                UnmanagedMeshes.Add(manager);
            }

            //^1 = last index
            RailingPosts[0].transform.localScale = new Vector3(cylinderDiameter, railingHeight + MathHelper.SmallFloat, cylinderDiameter);
            RailingPosts[^1].transform.localScale = new Vector3(cylinderDiameter, railingHeight + MathHelper.SmallFloat, cylinderDiameter);
            RailingPosts[0].transform.localPosition += Vector3.up * (topHeight * 0.5f);
            RailingPosts[^1].transform.localPosition += Vector3.up * (topHeight * 0.5f);

            foreach (UnityMeshManager manager in UnmanagedMeshes)
            {
                manager.UpdateMaterial();
            }
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