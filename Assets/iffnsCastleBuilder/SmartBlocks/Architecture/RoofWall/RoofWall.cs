using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoofWall : OnFloorObject
{
    public const string constIdentifierString = "Roof wall";

    public override string IdentifierString
    {
        get
        {
            return constIdentifierString;
        }
    }

    MailboxLineVector2Int FirstCoordinatenParam;
    MailboxLineVector2Int SecondCoordinateParam;
    MailboxLineRanged HeightParam;
    MailboxLineMaterial MainSideMaterialParam;
    MailboxLineMaterial OtherSideMaterialParam;
    MailboxLineDistinctNamed RoofTypeParamParam;

    NodeGridRectangleOrganizer ModificationNodeOrganizer;


    public override ModificationOrganizer Organizer
    {
        get
        {
            return ModificationNodeOrganizer;
        }
    }

    public Vector2Int FirstCoordinaten
    {
        set
        {
            FirstCoordinatenParam.Val = value;
            ApplyBuildParameters();
        }
    }

    public Vector2Int SecondCoordinate
    {
        set
        {
            SecondCoordinateParam.Val = value;
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

    void SetupRoofTypeParam()
    {
        List<string> enumString = new List<string>();

        int enumValues = System.Enum.GetValues(typeof(RectangularRoof.RoofTypes)).Length;

        for (int i = 0; i < enumValues; i++)
        {
            RectangularRoof.RoofTypes type = (RectangularRoof.RoofTypes)i;

            enumString.Add(type.ToString());
        }

        RoofTypeParamParam = new MailboxLineDistinctNamed(
            "Roof type",
            CurrentMailbox,
            Mailbox.ValueType.buildParameter,
            enumString,
            0);
    }

    public RectangularRoof.RoofTypes RoofType
    {
        get
        {
            RectangularRoof.RoofTypes returnValue = (RectangularRoof.RoofTypes)RoofTypeParamParam.Val;

            return returnValue;
        }
        set
        {
            RoofTypeParamParam.Val = (int)value;

            ApplyBuildParameters();
        }
    }

    void InitializeBuildParameterLines()
    {
        FirstCoordinatenParam = new MailboxLineVector2Int(name: "First coordinaten", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
        SecondCoordinateParam = new MailboxLineVector2Int(name: "Second coordinate", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
        HeightParam = new MailboxLineRanged(name: "Height [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 5f, Min: 0.3f, DefaultValue: 1.8f);
        MainSideMaterialParam = new MailboxLineMaterial(name: "Main side material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: MaterialLibrary.DefaultPlaster);
        OtherSideMaterialParam = new MailboxLineMaterial(name: "Other side material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: MaterialLibrary.DefaultPlaster);
    }
    public override void Setup(IBaseObject linkedFloor)
    {
        base.Setup(linkedFloor);

        this.LinkedFloor = linkedFloor as FloorController;

        InitializeBuildParameterLines();

        NodeGridPositionModificationNode firstNode = ModificationNodeLibrary.NewNodeGridPositionModificationNode;
        firstNode.Setup(linkedObject: this, value: FirstCoordinatenParam);
        FirstPositionNode = firstNode;

        NodeGridPositionModificationNode secondNode = ModificationNodeLibrary.NewNodeGridPositionModificationNode;
        secondNode.Setup(linkedObject: this, value: SecondCoordinateParam, relativeReferenceHolder: FirstCoordinatenParam);
        SecondPositionNode = secondNode;

        ModificationNodeOrganizer = new NodeGridRectangleOrganizer(linkedObject: this, firstNode: firstNode, secondNode: secondNode);

        SetupRoofTypeParam();

        SetupEditButtons();
    }

    public override void ResetObject()
    {
        baseReset();
    }

    public void AddStaticMesh(TriangleMeshInfo staticMesh)
    {
        if (staticMesh == null) return;

        StaticMeshManager.AddTriangleInfo(staticMesh);
    }

    public override void ApplyBuildParameters()
    {
        failed = false;

        TriangleMeshInfo MainSide = new TriangleMeshInfo();
        TriangleMeshInfo OtherSide = new TriangleMeshInfo();
        TriangleMeshInfo WrapperStartSide = new TriangleMeshInfo();
        TriangleMeshInfo WrapperOtherSide = new TriangleMeshInfo();
        TriangleMeshInfo WrapperBottom = new TriangleMeshInfo();

        void FinishMesh()
        {
            MainSide.MaterialReference = MainSideMaterialParam;
            OtherSide.MaterialReference = OtherSideMaterialParam;
            WrapperStartSide.MaterialReference = MainSideMaterialParam;
            WrapperOtherSide.MaterialReference = MainSideMaterialParam;
            WrapperBottom.MaterialReference = MainSideMaterialParam;

            AddStaticMesh(MainSide);
            AddStaticMesh(OtherSide);
            AddStaticMesh(WrapperStartSide);
            AddStaticMesh(WrapperOtherSide);
            AddStaticMesh(WrapperBottom);
            
            BuildAllMeshes();
        }

        ModificationNodeOrganizer.SetLinkedObjectPositionAndOrientation(raiseToFloor: false);

        Vector2 size = ModificationNodeOrganizer.ObjectOrientationSize;

        if (ModificationNodeOrganizer.ObjectOrientationGridSize.x == 0)
        {
            failed = true;
            return;
        }

        if (ModificationNodeOrganizer.ObjectOrientationGridSize.y == 0)
        {
            size.y = LinkedFloor.CurrentNodeWallSystem.WallThickness;
        }

        float centerPosition = 0.5f;

        switch (RoofType)
        {
            case RectangularRoof.RoofTypes.FullAngledSquareRoof:
                centerPosition = 0.5f;
                break;
            case RectangularRoof.RoofTypes.HalfAngledSquareRoof:
                centerPosition = 0f;
                break;
            default:
                break;
        }

        List<Vector3> trianglePoints = new List<Vector3>();

        trianglePoints.Add(Vector3.zero);
        trianglePoints.Add(new Vector3(size.x * centerPosition, Height, 0));
        trianglePoints.Add(new Vector3(size.x, 0, 0));

        MainSide = MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(points: trianglePoints);
        
        OtherSide = MainSide.CloneFlipped;
        OtherSide.Move(Vector3.forward * size.y);

        WrapperStartSide = MeshGenerator.FilledShapes.RectangleAtCorner(Vector3.forward * size.y, secondLine: trianglePoints[1], UVOffset: Vector2.zero);
        WrapperStartSide.FlipTriangles();
        WrapperOtherSide = MeshGenerator.FilledShapes.RectangleAtCorner(Vector3.forward * size.y, secondLine: trianglePoints[1] - trianglePoints[2], UVOffset: Vector2.zero);
        WrapperOtherSide.Move(trianglePoints[2]);

        WrapperBottom = MeshGenerator.FilledShapes.RectangleAtCorner(Vector3.forward * size.y, secondLine: trianglePoints[2], UVOffset: Vector2.zero);
        WrapperBottom.Move(Vector3.up * MathHelper.SmallFloat);

        if (ModificationNodeOrganizer.ObjectOrientationGridSize.y == 0)
        {
            Vector3 offset = Vector3.back * LinkedFloor.CurrentNodeWallSystem.HalfWallThickness;

            MainSide.Move(offset);
            OtherSide.Move(offset);
            WrapperStartSide.Move(offset);
            WrapperOtherSide.Move(offset);
            WrapperBottom.Move(offset);
        }

        MainSide.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.transform);
        OtherSide.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.transform);

        if(RoofType == RectangularRoof.RoofTypes.HalfAngledSquareRoof)
        {
            WrapperStartSide.GenerateUVMeshBasedOnCardinalDirections(meshObject: transform, originObjectForUV: LinkedFloor.transform);
        }

        FinishMesh();
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
        AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Set height to next bottom floor", delegate { SetHeightToNextBottomFloor(); }));
        AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Set height to next top floor", delegate { SetHeightToNextTopFloor(); }));
    }

    void SetHeightToNextTopFloor()
    {
        float nextFloorHeight;

        if (LinkedFloor.IsTopFloor) nextFloorHeight = LinkedFloor.BottomFloorHeight;
        else nextFloorHeight = LinkedFloor.FloorAbove.BottomFloorHeight;

        Height = LinkedFloor.CompleteFloorHeight + nextFloorHeight;

    }

    void SetHeightToNextBottomFloor()
    {
        Height = LinkedFloor.CompleteFloorHeight;
    }

    public override void MoveOnGrid(Vector2Int offset)
    {
        ModificationNodeOrganizer.MoveOnGrid(offset: offset);
    }
}
