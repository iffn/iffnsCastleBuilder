using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangularRoof : OnFloorObject
{
    public const string constIdentifierString = "Rectangular roof";
    public override string IdentifierString
    {
        get
        {
            return constIdentifierString;
        }
    }

    //Mailbox lines
    MailboxLineVector2Int FirstBlockPositionParam;
    MailboxLineVector2Int SecondBlockPositionParam;
    MailboxLineRanged HeightParam;
    MailboxLineRanged RoofOvershootParam;
    MailboxLineDistinctNamed RoofTypeParam;
    MailboxLineBool RaiseToFloorParam;

    MailboxLineMaterial OutsideMaterialParam;
    MailboxLineMaterial InsideMaterialParam;
    MailboxLineMaterial WrapperMaterialParam;

    BlockGridRectangleOrganizer ModificationNodeOrganizer;

    //WallTypes wallType = WallTypes.NodeWall;

    public override ModificationOrganizer Organizer
    {
        get
        {
            return ModificationNodeOrganizer;
        }
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

    public float RoofOvershoot
    {
        get
        {
            return RoofOvershootParam.Val;
        }
        set
        {
            RoofOvershootParam.Val = value;
            ApplyBuildParameters();
        }
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

    void SetupRoofTypeParam()
    {
        List<string> enumString = new List<string>();

        int enumValues = System.Enum.GetValues(typeof(RoofTypes)).Length;

        for (int i = 0; i < enumValues; i++)
        {
            RoofTypes type = (RoofTypes)i;

            enumString.Add(type.ToString());
        }

        RoofTypeParam = new MailboxLineDistinctNamed(
            "Roof type",
            CurrentMailbox,
            Mailbox.ValueType.buildParameter,
            enumString,
            0);
    }

    public enum RoofTypes
    {
        FullAngledSquareRoof,
        HalfAngledSquareRoof,
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

        this.LinkedFloor = linkedFloor as FloorController;
        if (linkedFloor == null) Debug.LogWarning("Error, linked floor is not a floor. Super object is instead = " + linkedFloor.IdentifierString);

        FirstBlockPositionParam = new MailboxLineVector2Int(name: "First Block Position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
        SecondBlockPositionParam = new MailboxLineVector2Int(name: "Second Block Position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
        HeightParam = new MailboxLineRanged(name: "Roof height [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 10, Min: 0.5f, DefaultValue: 2);
        RoofOvershootParam = new MailboxLineRanged(name: "Roof overshoot [m]", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, Max: 10, Min: 0f, DefaultValue: 0);
        RaiseToFloorParam = new MailboxLineBool(name: "Raise to floor", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: true);
        OutsideMaterialParam = new MailboxLineMaterial(name: "Outside material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: MaterialLibrary.DefaultRoof);
        InsideMaterialParam = new MailboxLineMaterial(name: "Inside material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: MaterialLibrary.DefaultCeiling);
        WrapperMaterialParam = new MailboxLineMaterial(name: "Wrapper material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: MaterialLibrary.DefaultPlaster);

        SetupRoofTypeParam();

        BlockGridPositionModificationNode firstNode = ModificationNodeLibrary.NewBlockGridPositionModificationNode;
        firstNode.Setup(linkedObject: this, value: FirstBlockPositionParam);
        FirstPositionNode = firstNode;

        BlockGridPositionModificationNode secondNode = ModificationNodeLibrary.NewBlockGridPositionModificationNode;
        secondNode.Setup(linkedObject: this, value: SecondBlockPositionParam);
        SecondPositionNode = secondNode;

        ModificationNodeOrganizer = new BlockGridRectangleOrganizer(linkedObject: this, firstNode: firstNode, secondNode: secondNode);

        SetupEditButtons();
    }

    public void CompleteSetUpWithBuildParameters(FloorController linkedFloor, Vector2Int firstPosition, Vector2Int secondPosition, float roofHeight, float roofOvershoot, RoofTypes roofType)
    {
        Setup(linkedFloor);

        //Using the accesors would apply build parameters
        FirstBlockPositionParam.Val = firstPosition;
        SecondBlockPositionParam.Val = secondPosition;
        HeightParam.Val = roofHeight;
        RoofTypeParam.Val = (int)roofType;
        RoofOvershoot = roofOvershoot;
    }

    public override void ResetObject()
    {
        baseReset();
    }

    public enum WallTypes
    {
        BlockWall,
        NodeWall
    }
    /*
    public void SetStaticMeshes(List<TriangleMeshInfo> staticMeshes)
    {
        foreach(TriangleMeshInfo mesh in staticMeshes)
        {
            StaticMeshManager.AddTriangleInfo(mesh);
        }
    }
    */

    public void AddStaticMesh(TriangleMeshInfo staticMesh)
    {
        if (staticMesh == null) return;

        StaticMeshManager.AddTriangleInfo(staticMesh);
    }

    float roofThickness = 0.1f;

    public override void ApplyBuildParameters()
    {
        ModificationNodeOrganizer.SetLinkedObjectPositionAndOrientation(raiseToFloor: false);
        Vector2 size = ModificationNodeOrganizer.ObjectOrientationSize;
        
        switch (RoofType)
        {
            case RectangularRoof.RoofTypes.FullAngledSquareRoof:
                UpdateFullRoof(size: size);
                break;
            case RectangularRoof.RoofTypes.HalfAngledSquareRoof:
                UpdateHalfRoof(size: size);
                break;
            default:
                break;
        }

        BuildAllMeshes();
    }

    void UpdateHalfRoof(Vector2 size)
    {
        TriangleMeshInfo RoofOutside = new TriangleMeshInfo();
        TriangleMeshInfo RoofInside = new TriangleMeshInfo();
        TriangleMeshInfo RoofWrapper = new TriangleMeshInfo();
        //TriangleMeshInfo FrontWall = new TriangleMeshInfo();
        //TriangleMeshInfo BackWall = new TriangleMeshInfo();

        void FinishMesh()
        {
            RoofOutside.MaterialReference = OutsideMaterialParam;
            RoofInside.MaterialReference = InsideMaterialParam;
            RoofWrapper.MaterialReference = WrapperMaterialParam;

            AddStaticMesh(RoofOutside);
            AddStaticMesh(RoofInside);
            AddStaticMesh(RoofWrapper);
            //AddStaticMesh(FrontWall);
            //AddStaticMesh(BackWall);
        }
        //Base parameters
        float roofAngle = Mathf.Atan(Height / size.x);
        float topAngle = Mathf.PI * 0.5f - roofAngle;

        float xOffset = roofThickness / Mathf.Sin(roofAngle);
        float heightOffset = roofThickness / Mathf.Sin(topAngle);

        //Roof outside
        RoofOutside = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.forward * size.y, secondLine: new Vector3(-size.x, Height, 0), UVOffset: Vector2.zero);
        RoofOutside.Move(Vector3.right * size.x);

        //Roof inside
        RoofInside = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.back * size.y, secondLine: new Vector3(-size.x + xOffset, Height - heightOffset, 0), UVOffset: Vector2.zero);
        RoofInside.Move(new Vector3(size.x - xOffset, 0, size.y));

        //Top wrapper
        TriangleMeshInfo tempWrapper = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.back * size.y, secondLine: Vector3.up * heightOffset, UVOffset: Vector2.zero);
        tempWrapper.Move(new Vector3(0, Height - heightOffset, size.y));
        RoofWrapper.Add(tempWrapper);

        //Bottom wrapper
        tempWrapper = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.forward * size.y, secondLine: Vector3.right * xOffset, UVOffset: Vector2.zero);
        tempWrapper.Move(Vector3.right * (size.x - xOffset));
        RoofWrapper.Add(tempWrapper);

        //Side wrapper 1
        tempWrapper = MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(new List<Vector3>()
        {
            new Vector3(0, Height - heightOffset, 0),
            new Vector3(0, Height, 0),
            new Vector3(size.x, 0, 0),
            new Vector3(size.x - xOffset, 0, 0),
        }
        );
        RoofWrapper.Add(tempWrapper);

        //Sided wrapper 2
        tempWrapper = tempWrapper.CloneFlipped;
        tempWrapper.Move(Vector3.forward * size.y);
        RoofWrapper.Add(tempWrapper);

        if (RaiseToFloor)
        {
            RoofOutside.Move(Vector3.up * LinkedFloor.BottomFloorHeight);
            RoofInside.Move(Vector3.up * LinkedFloor.BottomFloorHeight);
            RoofWrapper.Move(Vector3.up * LinkedFloor.BottomFloorHeight);
        }

        FinishMesh();

        //Old code
        /*
        float h = height;
        float t = roofThickness;
        float w = size.x;

        float h2 = h * h;
        float t2 = t * t;
        float w2 = w * w;

        float rootTerm = Mathf.Sqrt(h2 * t2 * (h2 - t2 + w2));

        float xDiff = (rootTerm + t2 * w) / (h2 - t2);
        float yDiff = (h2 * t2 - w * rootTerm) / (h * t2 - h * w2);

        float innerWidth = size.x;
        float innerHeight = height - yDiff;
        float outerWidth = size.x + xDiff;
        float outerHeight = height;

        //Roof outside
        Vector3 outerRoofOffset = new Vector3(outerWidth, -outerHeight);
        outerRoofOffset += outerRoofOffset.normalized * roofOvershoot;

        RoofOutside = MeshGenerator.FilledShapes.RectangleAtCorner(Vector3.back * size.y, secondLine: outerRoofOffset, UVOffset: Vector2.zero);
        //RoofOutside.Move(Vector3.up * outerHeight + Vector3.forward * size.y * 0.5f);
        //RoofOutside.Move(new Vector3(size.x * 0.5f, outerHeight, size.y * 0.5f));
        RoofOutside.RotateAllUVsCWAroundOrigin(angleDeg: 180);
        RoofOutside.Move(new Vector3(0, outerHeight, size.y));

        Vector3 innerRoofOffset = new Vector3(innerWidth, -innerHeight);
        innerRoofOffset += innerRoofOffset.normalized * roofOvershoot;
        RoofInside = MeshGenerator.FilledShapes.RectangleAtCorner(Vector3.forward * size.y, secondLine: innerRoofOffset, UVOffset: Vector2.zero);
        //RoofInside.Move(Vector3.up * innerHeight + Vector3.back * size.y * 0.5f);
        //RoofInside.Move(new Vector3(size.x * 0.5f, innerHeight, size.y * 0.5f));
        RoofInside.RotateAllUVsCWAroundOrigin(angleDeg: 180);
        RoofInside.Move(new Vector3(0, innerHeight, 0));

        //RoofWrapper.Move(new Vector3(size.x * 0.5f, 0, size.y * 0.5f));
        //FrontWall.Move(new Vector3(size.x * 0.5f, 0, size.y * 0.5f));
        //BackWall.Move(new Vector3(size.x * 0.5f, 0, size.y * 0.5f));
        FinishMesh();
        */
    }

    void UpdateFullRoof(Vector2 size)
    {
        TriangleMeshInfo RoofOutside = new TriangleMeshInfo();
        TriangleMeshInfo RoofInside = new TriangleMeshInfo();
        TriangleMeshInfo RoofWrapper = new TriangleMeshInfo();

        void FinishMesh()
        {
            RoofOutside.MaterialReference = OutsideMaterialParam;
            RoofInside.MaterialReference = InsideMaterialParam;
            RoofWrapper.MaterialReference = WrapperMaterialParam;

            AddStaticMesh(RoofOutside);
            AddStaticMesh(RoofInside);
            AddStaticMesh(RoofWrapper);
        }

        float halfWidth = size.x * 0.5f;
        float roofAngle = Mathf.Atan(Height / (size.x * 0.5f));
        float topAngle = Mathf.PI * 0.5f - roofAngle;

        float xOffset = roofThickness / Mathf.Sin(roofAngle);
        float heightOffset = roofThickness / Mathf.Sin(topAngle);

        TriangleMeshInfo tempShape;

        //Roof outside
        tempShape = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.forward * size.y, secondLine: new Vector3(-halfWidth, Height, 0), UVOffset: Vector2.zero);
        tempShape.Move(Vector3.right * size.x);
        RoofOutside.Add(tempShape);

        tempShape = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.back * size.y, secondLine: new Vector3(halfWidth, Height, 0), UVOffset: Vector2.zero);
        tempShape.Move(Vector3.forward * size.y);
        RoofOutside.Add(tempShape);

        //Roof inside
        tempShape = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.back * size.y, secondLine: new Vector3(-halfWidth + xOffset, Height - heightOffset, 0), UVOffset: Vector2.zero);
        tempShape.Move(new Vector3(size.x - xOffset, 0, size.y));
        RoofInside.Add(tempShape);

        tempShape = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.forward * size.y, secondLine: new Vector3(halfWidth - xOffset, Height - heightOffset, 0), UVOffset: Vector2.zero);
        tempShape.Move(Vector3.right * xOffset);
        RoofInside.Add(tempShape);

        //Side wrapper 1
        tempShape = MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(new List<Vector3>()
        {
            new Vector3(halfWidth, Height - heightOffset, 0),
            new Vector3(xOffset, 0, 0),
            new Vector3(0, 0, 0),
            new Vector3(halfWidth, Height, 0),
            new Vector3(size.x, 0, 0),
            new Vector3(size.x - xOffset, 0, 0)
        }
        );
        RoofWrapper.Add(tempShape);

        //Side wrapper 2
        tempShape = tempShape.CloneFlipped;
        tempShape.Move(Vector3.forward * size.y);
        RoofWrapper.Add(tempShape);

        if (RaiseToFloor)
        {
            RoofOutside.Move(Vector3.up * LinkedFloor.BottomFloorHeight);
            RoofInside.Move(Vector3.up * LinkedFloor.BottomFloorHeight);
            RoofWrapper.Move(Vector3.up * LinkedFloor.BottomFloorHeight);
        }

        FinishMesh();

        //Old code
        /*
        //Roof solved with Wolfram: https://www.wolframalpha.com/input/?i=Solve%5B%7Bcos%28a%29%3D%3Dt%2Fy%2C+sin%28a%29%3D%3Dt%2Fx%2C+tan%28a%29%3D%3Dh%2F%28w%2Bx%29%7D%2C+%7Bx%2Cy%2Ca%7D%5D

        float h = height;
        float t = roofThickness;
        float w = size.x * 0.5f;

        float h2 = h * h;
        float t2 = t * t;
        float w2 = w * w;

        float rootTerm = Mathf.Sqrt(h2*t2*(h2-t2+w2));

        float xDiff = (rootTerm + t2 * w) / (h2 - t2);
        float yDiff = (h2 * t2 - w * rootTerm) / (h * t2 - h * w2);

        float innerWidth = size.x * 0.5f;
        float innerHeight = height - yDiff;
        float outerWidth = size.x * 0.5f + xDiff;
        float outerHeight = height;

        //Roof outside
        Vector3 outerRoofOffset = new Vector3(outerWidth, -outerHeight);
        outerRoofOffset += outerRoofOffset.normalized * roofOvershoot;

        RoofOutside = MeshGenerator.FilledShapes.RectangleAtCorner(Vector3.back * size.y, secondLine: outerRoofOffset, UVOffset: Vector2.zero);
        RoofOutside.Move(Vector3.up * outerHeight + Vector3.forward * size.y * 0.5f);
        RoofOutside.RotateAllUVsCWAroundOrigin(angleDeg: 180);

        TriangleMeshInfo secondRoofSide = RoofOutside.CloneFlipped;
        secondRoofSide.Scale(new Vector3(-1, 1, 1));
        secondRoofSide.FlipAllUVsHorizontallyAroundOrigin();
        RoofOutside.Add(secondRoofSide);

        Vector3 innerRoofOffset = new Vector3(innerWidth, -innerHeight);
        innerRoofOffset += innerRoofOffset.normalized * roofOvershoot;
        RoofInside = MeshGenerator.FilledShapes.RectangleAtCorner(Vector3.forward * size.y, secondLine: innerRoofOffset, UVOffset: Vector2.zero);
        RoofInside.Move(Vector3.up * innerHeight + Vector3.back * size.y * 0.5f);
        RoofInside.RotateAllUVsCWAroundOrigin(angleDeg: 180);

        secondRoofSide = RoofInside.CloneFlipped;
        secondRoofSide.Scale(new Vector3(-1, 1, 1));
        secondRoofSide.FlipAllUVsHorizontallyAroundOrigin();
        RoofInside.Add(secondRoofSide);

        //Walls
        List<Vector3> ClockwiseWallPoints = new List<Vector3>();
        ClockwiseWallPoints.Add(Vector3.up * innerHeight);
        ClockwiseWallPoints.Add(Vector3.right * innerWidth);
        ClockwiseWallPoints.Add(Vector3.left * innerWidth);

        FrontWall = MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(ClockwiseWallPoints);
        FrontWall.Move(Vector3.back * (size.y * 0.5f));

        TriangleMeshInfo secondFrontWallInfo = FrontWall.Clone;
        secondFrontWallInfo.Move(Vector3.forward * (size.y - wallThickness));
        FrontWall.Add(secondFrontWallInfo);

        BackWall= FrontWall.CloneFlipped;
        BackWall.Move(Vector3.forward * wallThickness);

        FrontWall.GenerateUVMeshBasedOnCardinalDirections(meshObject: linkedRoof.transform, originObjectForUV: originObject);
        BackWall.GenerateUVMeshBasedOnCardinalDirections(meshObject: linkedRoof.transform, originObjectForUV: originObject);

        //Wrapper
        List<Vector3> ClockwiseFrontWrapperPoints = new List<Vector3>();
        ClockwiseFrontWrapperPoints.Add(Vector3.up * innerHeight);
        ClockwiseFrontWrapperPoints.Add(Vector3.left * innerWidth + new Vector3(-outerRoofOffset.normalized.x, outerRoofOffset.normalized.y, outerRoofOffset.normalized.z) * roofOvershoot);
        ClockwiseFrontWrapperPoints.Add(Vector3.left * outerWidth + new Vector3(-outerRoofOffset.normalized.x, outerRoofOffset.normalized.y, outerRoofOffset.normalized.z) * roofOvershoot);
        ClockwiseFrontWrapperPoints.Add(Vector3.up * outerHeight);
        ClockwiseFrontWrapperPoints.Add(Vector3.right * outerWidth + outerRoofOffset.normalized * roofOvershoot);
        ClockwiseFrontWrapperPoints.Add(Vector3.right * innerWidth + outerRoofOffset.normalized * roofOvershoot);

        TriangleMeshInfo frontWrapper = MeshGenerator.FilledShapes.PointsClockwiseAroundFirstPoint(ClockwiseFrontWrapperPoints);
        frontWrapper.Move(Vector3.back * (size.y * 0.5f));
        frontWrapper.GenerateUVMeshBasedOnCardinalDirections(meshObject: linkedRoof.transform, originObjectForUV: originObject);

        TriangleMeshInfo backWrapper = frontWrapper.CloneFlipped;
        backWrapper.Move(Vector3.forward * size.y);
        backWrapper.GenerateUVMeshBasedOnCardinalDirections(meshObject: linkedRoof.transform, originObjectForUV: originObject);

        TriangleMeshInfo bottomWrapper = MeshGenerator.FilledShapes.RectangleAroundCenter(baseLine: Vector3.left * xDiff, secondLine: Vector3.forward * size.y);
        bottomWrapper.Move(Vector3.right * (innerWidth + xDiff / 2));
        TriangleMeshInfo otherBottomWrapper = bottomWrapper.Clone;
        bottomWrapper.Move(outerRoofOffset.normalized * roofOvershoot);
        otherBottomWrapper.Move(Vector3.left * (innerWidth * 2 + xDiff));
        otherBottomWrapper.Move(new Vector3(-outerRoofOffset.normalized.x, outerRoofOffset.normalized.y, outerRoofOffset.normalized.z) * roofOvershoot);
        bottomWrapper.Add(otherBottomWrapper);
        bottomWrapper.GenerateUVMeshBasedOnCardinalDirections(meshObject: linkedRoof.transform, originObjectForUV: originObject);

        RoofWrapper = new TriangleMeshInfo();
        RoofWrapper.Add(frontWrapper);
        RoofWrapper.Add(backWrapper);
        RoofWrapper.Add(bottomWrapper);

        RoofOutside.Move(new Vector3(size.x * 0.5f, 0, size.y * 0.5f));
        RoofInside.Move(new Vector3(size.x * 0.5f, 0, size.y * 0.5f));
        RoofWrapper.Move(new Vector3(size.x * 0.5f, 0, size.y * 0.5f));
        FrontWall.Move(new Vector3(size.x * 0.5f, 0, size.y * 0.5f));
        BackWall.Move(new Vector3(size.x * 0.5f, 0, size.y * 0.5f));

        FinishMesh();
        */
    }


    public override void MoveOnGrid(Vector2Int offset)
    {
        ModificationNodeOrganizer.MoveOnGrid(offset: offset);
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
        AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Flip Diagonally", delegate { ModificationNodeOrganizer.FlipDiagonally(); }));
        AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Flip Vertical", delegate { ModificationNodeOrganizer.FlipVertical(); }));
        AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Flip Horizontal", delegate { ModificationNodeOrganizer.FlipHorizontal(); }));
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
