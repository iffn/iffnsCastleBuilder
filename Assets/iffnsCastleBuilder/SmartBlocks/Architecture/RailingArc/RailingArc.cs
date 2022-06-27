using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class RailingArc : OnFloorObject
    {
        [SerializeField] SmartMeshManager TopBorder;
        [SerializeField] SmartMeshManager BottomBorder;
        [SerializeField] GameObject RailingPostTemplate;

        //Build parameters
        MailboxLineVector2Int CenterCoordinateParam;
        MailboxLineVector2Int RadiiCoordinateParam;
        MailboxLineRanged TargetDistanceBetweenCylindersParam;
        MailboxLineMaterial TopMaterialParam;
        MailboxLineMaterial BottomMaterialParam;
        MailboxLineMaterial PostMaterialParam;

        readonly float railingHeight = 1.0f;
        readonly float distanceBetweenPosts = 0.4f;
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

            CenterCoordinateParam = new MailboxLineVector2Int(name: "Center coordinate", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            RadiiCoordinateParam = new MailboxLineVector2Int(name: "Radii coordinate", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            TargetDistanceBetweenCylindersParam = new MailboxLineRanged(name: "Cylinder distance target [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 20, Min: 0.1f, DefaultValue: 2);

            TopMaterialParam = new MailboxLineMaterial(name: "Top material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultWoodSolid);
            BottomMaterialParam = new MailboxLineMaterial(name: "Bottom material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultWoodSolid);
            PostMaterialParam = new MailboxLineMaterial(name: "Post material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultWoodSolid);

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
            base.ApplyBuildParameters();

            //Check validity
            if (Failed) return;

            Vector2 gridSize = ModificationNodeOrganizer.ObjectOrientationGridSize;

            if(gridSize.x == 0 || gridSize.y == 0)
            {
                BuildAllMeshes();

                Failed = true;
                return;
            }

            //Define mesh
            Vector2 size = ModificationNodeOrganizer.ObjectOrientationSize;

            TriangleMeshInfo arcMantleTop;
            TriangleMeshInfo arcMantleBottom;
            TriangleMeshInfo posts = new();

            //ToDo: Improve UV mesh

            void FinishMesh()
            {
                arcMantleTop.MaterialReference = TopMaterialParam;
                arcMantleBottom.MaterialReference = BottomMaterialParam;
                posts.MaterialReference = PostMaterialParam;

                StaticMeshManager.AddTriangleInfoIfValid(arcMantleTop);
                StaticMeshManager.AddTriangleInfoIfValid(arcMantleBottom);
                StaticMeshManager.AddTriangleInfoIfValid(posts);

                BuildAllMeshes();
            }

            TriangleMeshInfo currentMesh;

            //Create circle
            VerticesHolder arc = MeshGenerator.Lines.ArcAroundY(radius: 1, angleDeg: 90, numberOfEdges: 12);

            //Scale
            arc.Scale(new Vector3(size.x, 1, size.y));

            //Railing rectangle
            TriangleMeshInfo rectangle = MeshGenerator.FilledShapes.RectangleAroundCenter(baseLineFull: Vector3.right * width, secondLineFull: Vector3.up * topHeight);
            arcMantleTop = MeshGenerator.MeshesFromLines.ExtrudeAlong(sectionLine: rectangle.VerticesHolder, guideLine: arc, sectionIsClosed: true, guideIsClosed: false, sharpGuideEdges: true);
            arcMantleTop.Move((railingHeight - baseHeight * 0.5f) * Vector3.up);

            rectangle = MeshGenerator.FilledShapes.RectangleAroundCenter(baseLineFull: Vector3.right * width, secondLineFull: Vector3.up * baseHeight);
            arcMantleBottom = MeshGenerator.MeshesFromLines.ExtrudeAlong(sectionLine: rectangle.VerticesHolder, guideLine: arc, sectionIsClosed: true, guideIsClosed: false, sharpGuideEdges: true);
            arcMantleBottom.Move((topHeight * 0.5f) * Vector3.up);

            //Posts
            TriangleMeshInfo postTemplate = new TriangleMeshInfo();

            postTemplate.Add(MeshGenerator.FilledShapes.CylinderCaps(radius: width * 0.5f, length: railingHeight - MathHelper.SmallFloat * 2, direction: Vector3.up, numberOfEdges: 24));

            postTemplate.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.LinkedBuildingController.transform);

            postTemplate.Add(MeshGenerator.FilledShapes.CylinderAroundCenterWithoutCap(radius: width * 0.5f, length: railingHeight, direction: Vector3.up, numberOfEdges: 24));


            currentMesh = postTemplate.Clone;
            currentMesh.Move(new Vector3(size.x, 0, 0));
            posts.Add(currentMesh);

            currentMesh = postTemplate.Clone;
            currentMesh.Move(new Vector3(0, 0, size.y));
            posts.Add(currentMesh);

            postTemplate = MeshGenerator.FilledShapes.CylinderAroundCenterWithoutCap(radius: width * 0.5f, length: railingHeight, direction: Vector3.up, numberOfEdges: 24);

            //Posts along arc
            float arcLength = 0;

            List<float> arcLengths = new List<float>();

            for(int i = 0; i < arc.Count - 1; i++)
            {
                arcLengths.Add(arcLength);
                arcLength += (arc.Vertices[i + 1] - arc.Vertices[i]).magnitude;
            }

            arcLengths.Add(arcLength);

            int numberOfPosts = (int)Mathf.Round(arcLength / distanceBetweenPosts);

            float actualDistanceBetweenRailings = arcLength / numberOfPosts;

            for(int postIndex = 1; postIndex < numberOfPosts; postIndex++)
            {
                float postDistance = actualDistanceBetweenRailings * postIndex;

                int basePoint = 0;
                float remainingDistacne = 0;

                for(int arcPoint = 0; arcPoint < arcLengths.Count - 1; arcPoint++)
                {
                    if(postDistance < arcLengths[arcPoint])
                    {
                        basePoint = arcPoint;
                        remainingDistacne = postDistance - arcLengths[arcPoint];
                        break;
                    }
                }

                //ToDo: Improve point accuracy
                Vector3 postPoint = arc.Vertices[basePoint] + remainingDistacne * (arc.Vertices[basePoint + 1] - arc.Vertices[basePoint]).normalized;

                currentMesh = postTemplate.Clone;
                currentMesh.Move(postPoint);
                posts.Add(currentMesh);
            }

            posts.Move(railingHeight * 0.5f * Vector3.up);

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