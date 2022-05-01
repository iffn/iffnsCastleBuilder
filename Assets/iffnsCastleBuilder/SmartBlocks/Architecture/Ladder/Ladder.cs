using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : OnFloorObject
{
    public const string constIdentifierString = "Ladder";
    public override string IdentifierString
    {
        get
        {
            return constIdentifierString;
        }
    }

    //[SerializeField] BaseLadder LinkedBaseLadder;

    //BuildParameters
    MailboxLineVector2Int BottomLeftPositionParam;
    MailboxLineVector2Int TopRightPositionParam;

    NodeGridWallOrganizer ModificationNodeOrganizer;

    MailboxLineMaterial EdgeMaterialParam;
    MailboxLineMaterial StepMaterialParam;

    public override ModificationOrganizer Organizer
    {
        get
        {
            return ModificationNodeOrganizer;
        }
    }

    public Vector2Int BottomLeftPosition
    {
        get
        {
            return BottomLeftPositionParam.Val;
        }
        set
        {
            BottomLeftPositionParam.Val = value;
            ApplyBuildParameters();
        }
    }

    public Vector2Int TopRightPosition
    {
        get
        {
            return TopRightPositionParam.Val;
        }
        set
        {
            TopRightPositionParam.Val = value;
            ApplyBuildParameters();
        }
    }

    public override void Setup(IBaseObject linkedFloor)
    {
        base.Setup(linkedFloor);

        IsStructural = false;

        BottomLeftPositionParam = new MailboxLineVector2Int(name: "Bottom Left Position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
        TopRightPositionParam = new MailboxLineVector2Int(name: "Top Right Position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
        
        EdgeMaterialParam = new MailboxLineMaterial(name: "Edge material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: MaterialLibrary.DefaultWoodSolid);
        StepMaterialParam = new MailboxLineMaterial(name: "Step material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: MaterialLibrary.DefaultWoodSolid);


        NodeGridPositionModificationNode firstNode = ModificationNodeLibrary.NewNodeGridPositionModificationNode;
        firstNode.Setup(linkedObject: this, value: BottomLeftPositionParam);
        FirstPositionNode = firstNode;

        NodeGridPositionModificationNode secondNode = ModificationNodeLibrary.NewNodeGridPositionModificationNode;
        secondNode.Setup(linkedObject: this, value: TopRightPositionParam, relativeReferenceHolder: BottomLeftPositionParam);
        SecondPositionNode = secondNode;

        ModificationNodeOrganizer = new NodeGridWallOrganizer(linkedObject: this, firstNode: firstNode, secondNode: secondNode);

        SetupEditButtons();

        //UnmanagedMeshes.Clear();
        //UnmanagedMeshes.AddRange(LinkedBaseLadder.UnmanagedStaticMeshes);
    }

    public void CompleteSetupWithBuildParameters(FloorController linkedFloor, Vector2Int bottomLeftPosition, Vector2Int topRightPosition)
    {
        Setup(linkedFloor);

        BottomLeftPositionParam.Val = bottomLeftPosition;
        TopRightPositionParam.Val = topRightPosition;

        //LinkedBaseLadder.Setup(mainObject: this, edgeMaterial: EdgeMaterialParam, stepMaterial: StepMaterialParam);
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

    [SerializeField] float DistanceBetweenSteps = 0.5f;
    [SerializeField] float SideThickness = 0.1f;
    [SerializeField] float StepThickness = 0.1f;

    public void AddStaticMesh(TriangleMeshInfo staticMesh)
    {
        if (staticMesh == null) return;

        StaticMeshManager.AddTriangleInfo(staticMesh);
    }

    public override void ApplyBuildParameters()
    {
        failed = false;

        ModificationNodeOrganizer.SetLinkedObjectPositionAndOrientation(raiseToFloor: true);

        Vector2Int gridSize = ModificationNodeOrganizer.ParentOrientationGridSize;

        if (gridSize.x == 0 && gridSize.y == 0)
        {
            failed = true;
            return;
        }

        float width = ModificationNodeOrganizer.ObjectOrientationSize;
        float height = LinkedFloor.CompleteFloorHeight; //ToDo: Multi floor ladder

        TriangleMeshInfo OriginSide = new TriangleMeshInfo();
        TriangleMeshInfo OtherSide = new TriangleMeshInfo();
        TriangleMeshInfo Steps = new TriangleMeshInfo();

        void FinishMesh()
        {
            OriginSide.MaterialReference = EdgeMaterialParam;
            OtherSide.MaterialReference = EdgeMaterialParam;
            Steps.MaterialReference = StepMaterialParam;

            AddStaticMesh(OriginSide);
            AddStaticMesh(OtherSide);
            AddStaticMesh(Steps);

            BuildAllMeshes();
        }

        OriginSide = MeshGenerator.FilledShapes.BoxAroundCenter(size: new Vector3(SideThickness, height, SideThickness));
        OtherSide = OriginSide.Clone;

        OriginSide.Move(new Vector3((-width + SideThickness) * 0.5f, height * 0.5f, 0));
        OtherSide.Move(new Vector3((width - SideThickness) * 0.5f, height * 0.5f, 0));

        int numberOfSteps = (int)(height / DistanceBetweenSteps);

        TriangleMeshInfo baseStep = MeshGenerator.FilledShapes.CylinderAroundCenterWithoutCap(radius: StepThickness * 0.5f, length: width, direction: Vector3.right, numberOfEdges: 12);

        //baseStep.Move(width * 0.5f * Vector3.left);

        for (int i = 0; i < numberOfSteps; i++)
        {
            baseStep.Move(DistanceBetweenSteps * Vector3.up);
            Steps.Add(baseStep.Clone);
        }

        FinishMesh();

        //LinkedBaseLadder.SetMainParameters(width: size, height: LinkedFloor.CompleteFloorHeight);

        /*
        UnmanagedMeshes.Clear();
        UnmanagedMeshes.AddRange(LinkedBaseLadder.UnmanagedStaticMeshes);
        */
    }

    void SetupEditButtons()
    {

    }

    public override void MoveOnGrid(Vector2Int offset)
    {
        ModificationNodeOrganizer.MoveOnGrid(offset: offset);
    }
}
