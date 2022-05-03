using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class RailingArc : OnFloorObject
    {
        public const string constIdentifierString = "Railing arc";
        public override string IdentifierString
        {
            get
            {
                return constIdentifierString;
            }
        }

        [SerializeField] SmartMeshManager TopBorder;
        [SerializeField] SmartMeshManager BottomBorder;
        [SerializeField] GameObject RailingPostTemplate;

        //Build parameters
        MailboxLineVector2Int CenterCoordinateParam;
        MailboxLineVector2Int RadiiCoordinateParam;
        MailboxLineRanged TargetDistanceBetweenCylindersParam;

        readonly float railingHeight = 1.0f;
        readonly float cylinderDiameter = 0.1f;
        readonly float baseHeight = 0.05f;
        readonly float topHeight = 0.05f;
        readonly float width = 0.1f;

        NodeGridRectangleOrganizer ModificationNodeOrganizer;

        public override ModificationOrganizer Organizer
        {
            get
            {




                return ModificationNodeOrganizer;
            }
        }

        public Vector2Int CenterCoordinate
        {
            get
            {
                return CenterCoordinateParam.Val;
            }
            set
            {
                CenterCoordinateParam.Val = value;
                ApplyBuildParameters();
            }
        }

        public Vector2Int EndCoordinate
        {
            get
            {
                return RadiiCoordinateParam.Val;
            }
            set
            {
                RadiiCoordinateParam.Val = value;
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

            CenterCoordinateParam = new MailboxLineVector2Int(name: "Center coordinate", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            RadiiCoordinateParam = new MailboxLineVector2Int(name: "Radii coordinate", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            TargetDistanceBetweenCylindersParam = new MailboxLineRanged(name: "Cylinder distance target [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 20, Min: 0.1f, DefaultValue: 2);

            NodeGridPositionModificationNode firstNode = ModificationNodeLibrary.NewNodeGridPositionModificationNode;
            firstNode.Setup(linkedObject: this, value: CenterCoordinateParam);
            FirstPositionNode = firstNode;

            NodeGridPositionModificationNode secondNode = ModificationNodeLibrary.NewNodeGridPositionModificationNode;
            secondNode.Setup(linkedObject: this, value: RadiiCoordinateParam, relativeReferenceHolder: null);
            SecondPositionNode = secondNode;

            ModificationNodeOrganizer = new NodeGridRectangleOrganizer(linkedObject: this, firstNode: firstNode, secondNode: secondNode);

            SetupEditButtons();


        }

        public void CompleteSetupWithBuildParameters(FloorController linkedFloor, Vector2Int centerCoordinate, Vector2Int radiiCoordinate, float targetCylinderDistance)
        {
            Setup(linkedFloor);

            CenterCoordinateParam.Val = centerCoordinate;
            RadiiCoordinateParam.Val = radiiCoordinate;
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

        public override void ApplyBuildParameters()
        {
            float a;
            a = railingHeight;
            a = cylinderDiameter;
            a = baseHeight;
            a = topHeight;
            a = width;
            Debug.Log("Arc railing updated");

            List<UnityMeshManager> allMeshFilters = new List<UnityMeshManager>();

            TriangleMeshInfo TopBorder = new TriangleMeshInfo();
            TriangleMeshInfo BottomBorder = new TriangleMeshInfo();
            TriangleMeshInfo RailingPostTemplate = new TriangleMeshInfo();

            void FinishMesh()
            {
                StaticMeshManager.AddTriangleInfo(TopBorder);
                StaticMeshManager.AddTriangleInfo(BottomBorder);
                StaticMeshManager.AddTriangleInfo(RailingPostTemplate);

                BuildAllMeshes();
            }

            FinishMesh();

            //AllStaticMeshes.managedMeshes.AddRange();
            UnmanagedMeshes.AddRange(allMeshFilters);
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