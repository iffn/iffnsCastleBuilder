using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class TriangularRoof : OnFloorObject
    {
        MailboxLineVector2Int FirstPositionParam;
        MailboxLineVector2Int SecondPositionParam;
        MailboxLineVector2Int ThirdPositionParam;
        MailboxLineRanged HeightParam;
        MailboxLineRanged HeightThicknessParam;
        MailboxLineDistinctNamed RoofTypeParam;
        MailboxLineBool RaiseToFloorParam;

        MailboxLineMaterial OutsideMaterial;
        MailboxLineMaterial InsideMaterial;
        MailboxLineMaterial WrapperMaterial;

        NodeGridTriangleOrganizer ModificationNodeOrganizer;

        public override ModificationOrganizer Organizer
        {
            get
            {
                return ModificationNodeOrganizer;
            }
        }

        public enum RoofTypes
        {
            TwoAreLow,
            TwoAreHigh
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

        void SetupRoofTypeParam(RoofTypes roofType = RoofTypes.TwoAreLow)
        {
            List<string> enumString = new List<string>()
        {
            "Two are low",
            "TwoAreHigh"
        };

            RoofTypeParam = new MailboxLineDistinctNamed(
                "Block type",
                CurrentMailbox,
                Mailbox.ValueType.buildParameter,
                enumString,
                (int)roofType);
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

        public bool RaiseToFloor
        {
            get
            {
                return RaiseToFloorParam.Val;
            }
            set
            {
                RaiseToFloorParam.Val = value;
                ApplyBuildParameters();
            }
        }

        public override void Setup(IBaseObject linkedFloor)
        {
            base.Setup(linkedFloor);

            FirstPositionParam = new MailboxLineVector2Int(name: "First position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            SecondPositionParam = new MailboxLineVector2Int(name: "Second position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            ThirdPositionParam = new MailboxLineVector2Int(name: "Third position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            HeightParam = new MailboxLineRanged(name: "Height [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 10f, Min: 0.2f, DefaultValue: 2f);
            HeightThicknessParam = new MailboxLineRanged(name: "Height thickness", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 1f, Min: 0.001f, DefaultValue: 0.1f);
            RaiseToFloorParam = new MailboxLineBool(name: "Raise to floor", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: true);
            OutsideMaterial = new MailboxLineMaterial(name: "Outside material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultRoof);
            InsideMaterial = new MailboxLineMaterial(name: "Inside material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultCeiling);
            WrapperMaterial = new MailboxLineMaterial(name: "Wrapper material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultPlaster);

            SetupRoofTypeParam();

            NodeGridPositionModificationNode firstNode = ModificationNodeLibrary.NewNodeGridPositionModificationNode;
            firstNode.Setup(linkedObject: this, value: FirstPositionParam);

            NodeGridPositionModificationNode secondNode = ModificationNodeLibrary.NewNodeGridPositionModificationNode;
            secondNode.Setup(linkedObject: this, value: SecondPositionParam, relativeReferenceHolder: null);

            NodeGridPositionModificationNode thirdNode = ModificationNodeLibrary.NewNodeGridPositionModificationNode;
            thirdNode.Setup(linkedObject: this, value: ThirdPositionParam, relativeReferenceHolder: null);

            FirstPositionNode = secondNode;
            SecondPositionNode = thirdNode;

            ModificationNodeOrganizer = new NodeGridTriangleOrganizer(linkedObject: this, firstNode: firstNode, secondNode: secondNode, thirdNode: thirdNode);

            SetupEditButtons();
        }

        public void CompleteSetupWithBuildParameters(FloorController linkedFloor, Vector2Int firstPosition, Vector2Int secondPosition, Vector2Int thirdPosition, RoofTypes roofType)
        {
            Setup(linkedFloor);

            FirstPositionParam.Val = firstPosition;
            SecondPositionParam.Val = secondPosition;
            ThirdPositionParam.Val = thirdPosition;

            RoofType = roofType;
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
            failed = false;

            TriangleMeshInfo RoofOutside;
            TriangleMeshInfo RoofInside;
            TriangleMeshInfo RoofWrapper = new TriangleMeshInfo();

            void FinishMeshes()
            {
                RoofOutside.MaterialReference = OutsideMaterial;
                RoofInside.MaterialReference = InsideMaterial;
                RoofWrapper.MaterialReference = WrapperMaterial;

                StaticMeshManager.AddTriangleInfoIfValid(RoofOutside);
                StaticMeshManager.AddTriangleInfoIfValid(RoofInside);
                StaticMeshManager.AddTriangleInfoIfValid(RoofWrapper);

                BuildAllMeshes();
            }

            ModificationNodeOrganizer.SetLinkedObjectPositionAndOrientation(raiseToFloor: false);

            if (failed) return;

            //Outer roof without height
            List<Vector3> outerPoints = new List<Vector3>()
        {
            new Vector3(ModificationNodeOrganizer.FirstClockwiseOffsetPosition.x, 0, ModificationNodeOrganizer.FirstClockwiseOffsetPosition.y),
            new Vector3(ModificationNodeOrganizer.SecondClockwiseOffsetPosition.x, 0, ModificationNodeOrganizer.SecondClockwiseOffsetPosition.y),
            Vector3.zero,
        };

            List<Vector3> innerPoints = new List<Vector3>();

            //Point positions
            switch (RoofType)
            {
                case RoofTypes.TwoAreLow:
                    outerPoints[2] += Vector3.up * Height;

                    innerPoints.Add(new Vector3(ModificationNodeOrganizer.SecondClockwiseOffsetPosition.x, 0, ModificationNodeOrganizer.SecondClockwiseOffsetPosition.y));
                    innerPoints.Add(new Vector3(ModificationNodeOrganizer.FirstClockwiseOffsetPosition.x, 0, ModificationNodeOrganizer.FirstClockwiseOffsetPosition.y));
                    innerPoints.Add(Vector3.zero);

                    innerPoints[0] -= HeightThickness / Height * innerPoints[0];
                    innerPoints[1] -= HeightThickness / Height * innerPoints[1];
                    innerPoints[2] += Vector3.up * (Height - HeightThickness);
                    break;
                case RoofTypes.TwoAreHigh:
                    outerPoints[0] += Vector3.up * Height;
                    outerPoints[1] += Vector3.up * Height;

                    innerPoints.Add(outerPoints[1] + HeightThickness * Vector3.down);
                    innerPoints.Add(outerPoints[0] + HeightThickness * Vector3.down);
                    innerPoints.Add(HeightThickness / Height * new Vector3(outerPoints[0].x, 0, outerPoints[0].z));
                    innerPoints.Add(HeightThickness / Height * new Vector3(outerPoints[1].x, 0, outerPoints[1].z));

                    break;
                default:
                    break;
            }

            RoofOutside = MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(outerPoints);
            RoofInside = MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(innerPoints);

            //Wrapper
            switch (RoofType)
            {
                case RoofTypes.TwoAreLow:
                    RoofWrapper.Add(MeshGenerator.MeshesFromLines.KnitLines(
                        firstLine: new VerticesHolder(new List<Vector3>() { innerPoints[1], innerPoints[0], innerPoints[2] }),
                        secondLine: new VerticesHolder(outerPoints),
                        closingType: MeshGenerator.ShapeClosingType.closedWithSharpEdge, smoothTransition: false));
                    break;
                case RoofTypes.TwoAreHigh:
                    RoofWrapper.Add(MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(new List<Vector3>() { outerPoints[1], outerPoints[0], innerPoints[1], innerPoints[0] }));
                    RoofWrapper.Add(MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(new List<Vector3>() { outerPoints[2], outerPoints[1], innerPoints[0], innerPoints[3] }));
                    RoofWrapper.Add(MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(new List<Vector3>() { outerPoints[0], outerPoints[2], innerPoints[2], innerPoints[1] }));
                    RoofWrapper.Add(MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(new List<Vector3>() { outerPoints[2], innerPoints[3], innerPoints[2] }));
                    break;
                default:
                    break;
            }

            //Rotate UVs
            switch (RoofType)
            {
                case RoofTypes.TwoAreLow:
                    RotateUVs(RoofOutside);
                    RotateUVs(RoofInside);
                    break;
                case RoofTypes.TwoAreHigh:
                    //ToDo: Optimize
                    RotateUVs(RoofOutside);
                    RotateUVs(RoofOutside);
                    RotateUVs(RoofOutside);
                    RotateUVs(RoofInside);
                    RotateUVs(RoofInside);
                    RotateUVs(RoofInside);
                    break;
                default:
                    break;
            }

            if (RaiseToFloor)
            {
                RoofOutside.Move(Vector3.up * LinkedFloor.BottomFloorHeight);
                RoofInside.Move(Vector3.up * LinkedFloor.BottomFloorHeight);
                RoofWrapper.Move(Vector3.up * LinkedFloor.BottomFloorHeight);
            }

            FinishMeshes();

            void RotateUVs(TriangleMeshInfo info)
            {
                List<Vector2> UVs;

                UVs = info.UVs;

                for (int i = 0; i < UVs.Count; i++)
                {
                    UVs[i] = new Vector2(-UVs[i].y, UVs[i].x);
                }

                info.UVs = UVs;
            }
        }
        public override void MoveOnGrid(Vector2Int offset)
        {
            ModificationNodeOrganizer.MoveOnGrid(offset: offset);
        }

        void SetupEditButtons()
        {
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