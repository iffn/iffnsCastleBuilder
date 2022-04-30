using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangularRoof : OnFloorObject
{
    public const string constIdentifierString = "Triangular roof";
    public override string IdentifierString
    {
        get
        {
            return constIdentifierString;
        }
    }

    MailboxLineVector2Int FirstPositionParam;
    MailboxLineVector2Int SecondPositionParam;
    MailboxLineVector2Int ThirdPositionParam;
    MailboxLineRanged HeightParam;
    MailboxLineRanged ThicknessParam;
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

    public float Thickness
    {
        get
        {
            return ThicknessParam.Val;
        }
        set
        {
            ThicknessParam.Val = value;
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
        ThicknessParam = new MailboxLineRanged(name: "Thickness", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 1f / 3, Min: 0.001f, DefaultValue: 0.1f);
        RaiseToFloorParam = new MailboxLineBool(name: "Raise to floor", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: true);
        OutsideMaterial = new MailboxLineMaterial(name: "Outside material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: MaterialLibrary.DefaultRoof);
        InsideMaterial = new MailboxLineMaterial(name: "Inside material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: MaterialLibrary.DefaultCeiling);
        WrapperMaterial = new MailboxLineMaterial(name: "Wrapper material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: MaterialLibrary.DefaultPlaster);

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

            StaticMeshManager.AddTriangleInfo(RoofOutside);
            StaticMeshManager.AddTriangleInfo(RoofInside);
            StaticMeshManager.AddTriangleInfo(RoofWrapper);

            BuildAllMeshes();
        }

        ModificationNodeOrganizer.SetLinkedObjectPositionAndOrientation(raiseToFloor: false);

        if (failed) return;

        //Outer roof
        List<Vector3> outerPoints = new List<Vector3>()
        {
            new Vector3(ModificationNodeOrganizer.FirstClockwiseOffsetPosition.x, 0, ModificationNodeOrganizer.FirstClockwiseOffsetPosition.y),
            new Vector3(ModificationNodeOrganizer.SecondClockwiseOffsetPosition.x, 0, ModificationNodeOrganizer.SecondClockwiseOffsetPosition.y),
            Vector3.zero,
        };

        List<Vector3> innerPoints = new List<Vector3>()
        {
            new Vector3(ModificationNodeOrganizer.SecondClockwiseOffsetPosition.x, 0, ModificationNodeOrganizer.SecondClockwiseOffsetPosition.y),
            new Vector3(ModificationNodeOrganizer.FirstClockwiseOffsetPosition.x, 0, ModificationNodeOrganizer.FirstClockwiseOffsetPosition.y),
            Vector3.zero,
        };

        

        switch (RoofType)
        {
            case RoofTypes.TwoAreLow:
                innerPoints[0] -= innerPoints[0].normalized * Thickness;
                innerPoints[1] -= innerPoints[1].normalized * Thickness;
                outerPoints[2] += Vector3.up * Height;
                innerPoints[2] += Vector3.up * (Height - Thickness);
                break;
            case RoofTypes.TwoAreHigh:
                outerPoints[0] += Vector3.up * Height;
                outerPoints[1] += Vector3.up * Height;
                innerPoints[2] += (innerPoints[0] + innerPoints[1]).normalized * Thickness;
                innerPoints[0] += Vector3.up * (Height - Thickness);
                innerPoints[1] += Vector3.up * (Height - Thickness);

                break;
            default:
                break;
        }

        RoofOutside = MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(outerPoints);
        RoofInside = MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(innerPoints);

        RoofWrapper.Add(MeshGenerator.MeshesFromLines.KnitLines(firstLine: new VerticesHolder(new List<Vector3>() {innerPoints[1], innerPoints[0], innerPoints[2] }), secondLine: new VerticesHolder(outerPoints), isClosed: true, isSealed: false, smoothTransition: false));



        if (RoofType == RoofTypes.TwoAreLow)
        {
            RotateUVs(RoofOutside);

            RotateUVs(RoofInside);
        }
        else if (RoofType == RoofTypes.TwoAreHigh)
        {
            //ToDo: Optimize
            RotateUVs(RoofOutside);
            RotateUVs(RoofOutside);
            RotateUVs(RoofOutside);

            RotateUVs(RoofInside);
            RotateUVs(RoofInside);
            RotateUVs(RoofInside);
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

        //Old code
        /*
        //Roof inside
        List<Vector3> innerPoints = new List<Vector3>()
        {
            new Vector3(ModificationNodeOrganizer.FirstClockwiseOffsetPosition.x, 0, ModificationNodeOrganizer.FirstClockwiseOffsetPosition.y),
            new Vector3(ModificationNodeOrganizer.SecondClockwiseOffsetPosition.x, 0, ModificationNodeOrganizer.SecondClockwiseOffsetPosition.y),
            Vector3.zero
        };

        float height = Height - Thickness;

        switch (RoofType)
        {
            case RoofTypes.TwoAreLow:
                innerPoints[2] += Vector3.up * height;
                break;
            case RoofTypes.TwoAreHigh:
                innerPoints[0] += Vector3.up * height;
                innerPoints[1] += Vector3.up * height;
                break;
            default:
                break;
        }

        RoofInside = MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(innerPoints);
        RoofInside.FlipTriangles();

        //Roof outside
        List<Vector3> outerPoints = new List<Vector3>(innerPoints);

        switch (RoofType)
        {
            case RoofTypes.TwoAreLow:
                outerPoints[2] += Vector3.up * Thickness;
                outerPoints[0] += outerPoints[0].normalized * Thickness;
                outerPoints[1] += outerPoints[1].normalized * Thickness;
                break;
            case RoofTypes.TwoAreHigh:
                outerPoints[2] = Vector3.up * Thickness;
                outerPoints[0] += Vector3.up * Thickness;
                outerPoints[1] += Vector3.up * Thickness;
                break;
            default:
                break;
        }

        RoofOutside = MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(outerPoints);

        if(RoofType == RoofTypes.TwoAreLow)
        {
            RotateUVs(RoofOutside);

            RotateUVs(RoofInside);
        }
        else if(RoofType == RoofTypes.TwoAreHigh)
        {
            //ToDo: Optimize
            RotateUVs(RoofOutside);
            RotateUVs(RoofOutside);
            RotateUVs(RoofOutside);
            
            RotateUVs(RoofInside);
            RotateUVs(RoofInside);
            RotateUVs(RoofInside);
        }

        Edge12 = MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(new List<Vector3>() {outerPoints[0], outerPoints[2], innerPoints[2], innerPoints[0] });
        Edge23 = MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(new List<Vector3>() {outerPoints[1], outerPoints[0], innerPoints[0], innerPoints[1] });
        Edge31 = MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(new List<Vector3>() {outerPoints[2], outerPoints[1], innerPoints[1], innerPoints[2] });

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

        FinishMeshes();
        */
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
