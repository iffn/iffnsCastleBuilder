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

    [SerializeField] BaseLadder LinkedBaseLadder;

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

        
    }

    public void CompleteSetupWithBuildParameters(FloorController linkedFloor, Vector2Int bottomLeftPosition, Vector2Int topRightPosition)
    {
        Setup(linkedFloor);

        BottomLeftPositionParam.Val = bottomLeftPosition;
        TopRightPositionParam.Val = topRightPosition;

        LinkedBaseLadder.Setup(mainObject: this, edgeMaterial: EdgeMaterialParam, stepMaterial: StepMaterialParam);
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
        ModificationNodeOrganizer.SetLinkedObjectPositionAndOrientation(raiseToFloor: true);

        Vector2Int gridSize = ModificationNodeOrganizer.ParentOrientationGridSize;

        if (gridSize.x == 0 && gridSize.y == 0)
        {
            return;
        }

        float size = ModificationNodeOrganizer.ObjectOrientationSize;

        
        LinkedBaseLadder.SetMainParameters(width: size, height: LinkedFloor.CompleteFloorHeight);

        UnmanagedMeshes.Clear();
        UnmanagedMeshes.AddRange(LinkedBaseLadder.UnmanagedStaticMeshes);
    }

    void SetupEditButtons()
    {

    }

    public override void MoveOnGrid(Vector2Int offset)
    {
        ModificationNodeOrganizer.MoveOnGrid(offset: offset);
    }
}
