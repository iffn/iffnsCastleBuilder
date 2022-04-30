using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnFloorObject : BaseGameObject
{
    public FloorController LinkedFloor { get; protected set; }

    /*
        New implementation checklist
        - Script
            - Derived from OnFloorObject
            - All dependencies implemented
            - Add modification nodes
            - Add EdditButtonFunction
        - Template setup
            - Create GameObject
            - Attach script
            - Set up custom elements
        - Template mesh
            - Add gameplay object with MeshManager, MeshFilter, MeshRenderer, ClickForwarder and Material
            - Link ClickForwarder to Main object
            - Add meshes to usually active colliders
        - Template implementation
            - Save as template
            - Add to Recource library
        - UI button
            - SVG image
            - PNG export (Page = 378x378)
            - Texture type = Sprite (2D and UI)
        - SmartBlockBuilderTool
            - Add SmartBlockToolType enum option
            - Implement enum option in UpdateCurrentTemplate() function
        - SmartBlockUI
            - Add buttons
            - Assign to SmartBlockUI
        - Canvas
            - Douplicate existing button
            - Rename
            - Put into correct position
            - Change icon
        
        Old implementation checklist
        - Object
            - Referes to OnFloorObject
            - Setup implemented with linkedFloor function
            - Implements ObjectTypeName
        - Click forwarder
            - Object
            - Enum
            - Linked floor
            - Linked object
            - Add linker to colliders
        - Human building resources reference
            - Create reference in script
            - Add template to prefab
        - Floor controller implementation
            - Add and remove
            - Save
            - Load
        - Smart block builder implementation
            - Builder funcition definition
            - Builder funcition implementation
        - Edit tool
            - Remove button
    */

    public abstract ModificationOrganizer Organizer { get; }

    public bool failed = false;

    public bool IsStructural { get; protected set; } = true;

    public float BlockSize
    {
        get
        {
            return LinkedFloor.LinkedBuildingController.BlockSize;
        }
    }

    public override void Setup(IBaseObject superObject)
    {
        base.Setup(superObject);

        if(!(superObject is FloorController))
        {
            Debug.LogWarning("Error, Super object of On floor object is not a floor controller");
            return;
        }

        LinkedFloor = superObject as FloorController;
        LinkedFloor.AddOnFloorObject(this);

        DeleteButtonFunction = new SingleButtonBaseEditFunction(buttonName: "Delete Object", buttonFunction: delegate { DestroyObject(); });
        AddEditButtonFunctionToBeginning(DeleteButtonFunction);

    }

    public SingleButtonBaseEditFunction DeleteButtonFunction { get; private set; }

    /*
    public abstract Vector2Int FirstBuildPosition { protected get; set; }
    public abstract Vector2Int SecondBuildPosition { protected get; set; }
    */

    public ModificationNode FirstPositionNode { get; protected set; }
    public ModificationNode SecondPositionNode { get; protected set; }

    public VirtualBlock.BlockTypes BlockTypeOfFirstNode
    {
        get
        {
            if(FirstPositionNode == null)
            {
                Debug.LogWarning("Error: First Position node not set");
                return VirtualBlock.BlockTypes.Empty;
            }

            if (FirstPositionNode is GridModificationNode node) 
            {
                Vector2Int checkLocation = MathHelper.ClampVector2Int(value: node.AbsoluteCoordinate, max: LinkedFloor.LinkedBuildingController.GridSize - Vector2Int.one, min: Vector2Int.zero); //Reduces position of Node location

                return LinkedFloor.BlockAtPosition(checkLocation).BlockType;
            }
            else
            {
                return LinkedFloor.GetBlockFromCoordinateAbsolute(FirstPositionNode.transform.position).BlockType;
            }
        }
    }

    /*
    public float FloorBaseHeightBasedOnFirstNode
    {
        get
        {
            if (FirstPositionNode == null)
            {
                Debug.LogWarning("Error: First Position node not set");
                return 0;
            }

            LinkedFloor.BaseHeightOfBlock(FirstPositionNode.)

            switch (BlockTypeOfFirstNode)
            {
                case VirtualBlock.BlockTypes.Empty:
                    return 0;

                case VirtualBlock.BlockTypes.FloorWithCeiling:
                    return LinkedFloor.BottomFloorHeight;

                case VirtualBlock.BlockTypes.FloorWithoutCeiling:
                    return LinkedFloor.BottomFloorHeight;

                case VirtualBlock.BlockTypes.CeilingWithoutFloor:
                    return 0;

                case VirtualBlock.BlockTypes.Wall:
                    break;

                default:
                    Debug.LogWarning("Error: Block type not defined");
                    break;
            }

            return LinkedFloor.BottomFloorHeight;
        }
    }

    public float FloorHeightBasedOnFirstNode
    {
        get
        {
            return LinkedFloor.CompleteFloorHeight - FloorBaseHeightBasedOnFirstNode;
        }
    }
    */

    public abstract void MoveOnGrid(Vector2Int offset);

    /*
    protected bool buildSuccessful = true;

    public bool BuildSuccessful
    {
        get
        {
            return buildSuccessful;
        }
    }
    */

    /*
    void DeleteObject()
    {
        //ToDo: Implement delete function
        DestroyObject();
    }
    */


}
