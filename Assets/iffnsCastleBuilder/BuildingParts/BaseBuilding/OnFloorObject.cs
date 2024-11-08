﻿using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
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

        public abstract bool RaiseToFloor { get; }

        public abstract ModificationOrganizer Organizer { get; }

        public abstract bool IsStructural { get; }

        public float BlockSize
        {
            get
            {
                return LinkedFloor.LinkedBuildingController.BlockSize;
            }
        }

        public virtual float ModificationNodeHeight
        {
            get
            {
                return LinkedFloor.CompleteFloorHeight;
            }
        }

        public override void Setup(IBaseObject superObject)
        {
            base.Setup(superObject);

            if (!(superObject is FloorController))
            {
                Debug.LogWarning("Error, Super object of On floor object is not a floor controller");
                return;
            }

            LinkedFloor = superObject as FloorController;
            LinkedFloor.AddOnFloorObject(this);

            SingleButtonBaseEditFunction deleteButtonFunction = new SingleButtonBaseEditFunction(buttonName: "Delete Object", buttonFunction: delegate { DestroyObject(); EditTool.DeactivateEditOnMain(); });

            AddEditButtonFunctionToBeginning(deleteButtonFunction);
            AddCopyButtons();
        }

        public override void ApplyBuildParameters()
        {
            ResetAllMeshes();

            Failed = false;

            Organizer.SetLinkedObjectPositionAndOrientation(raiseToFloor: RaiseToFloor);
            if (Failed) return;

            UpdateModificationNodePositions();
        }

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
                if (FirstPositionNode == null)
                {
                    Debug.LogWarning("Error: First Position node not set");
                    return VirtualBlock.BlockTypes.Empty;
                }

                if (FirstPositionNode is GridModificationNode node)
                {
                    Vector2Int checkLocation = MathHelper.ClampVector2Int(value: node.AbsoluteCoordinate, max: LinkedFloor.LinkedBuildingController.BlockGridSize - Vector2Int.one, min: Vector2Int.zero); //Reduces position of Node location

                    return LinkedFloor.BlockAtPosition(checkLocation).BlockType;
                }
                else
                {
                    return LinkedFloor.GetBlockFromCoordinateAbsolute(FirstPositionNode.transform.position).BlockType;
                }
            }
        }

        public bool CanCopyUp
        {
            get
            {
                return !LinkedFloor.IsTopFloor;
            }
        }

        public bool CanCopyDown
        {
            get
            {
                return !LinkedFloor.IsBottomFloor;
            }
        }

        protected void AddCopyButtons()
        {
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Move down", delegate { MoveDown(); }));
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Move up", delegate { MoveUp(); }));
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Copy down", delegate { CopyDown(); }));
            AddEditButtonFunctionToBeginning(function: new SingleButtonBaseEditFunction(buttonName: "Copy up", delegate { CopyUp(); }));
        }

        public void CopyUp()
        {
            if (!CanCopyUp) return;

            OnFloorObject newObject = Instantiate(ResourceLibrary.TryGetTemplateFromStringIdentifier(IdentifierString) as OnFloorObject);

            FloorController targetFloor = LinkedFloor.LinkedBuildingController.Floor(LinkedFloor.FloorNumber + 1);

            newObject.Setup(superObject: targetFloor);

            newObject.JSONBuildParameters = JSONBuildParameters;

            newObject.ApplyBuildParameters();
        }

        public void CopyDown()
        {
            if (!CanCopyDown) return;

            OnFloorObject newObject = Instantiate(ResourceLibrary.TryGetTemplateFromStringIdentifier(IdentifierString) as OnFloorObject);

            FloorController targetFloor = LinkedFloor.LinkedBuildingController.Floor(LinkedFloor.FloorNumber - 1);

            newObject.Setup(superObject: targetFloor);

            newObject.JSONBuildParameters = JSONBuildParameters;

            newObject.ApplyBuildParameters();
        }

        public void MoveUp()
        {
            if (!CanCopyUp) return;

            CopyUp();

            DestroyObject();

            EditTool.DeactivateEditOnMain();
        }

        public void MoveDown()
        {
            if (!CanCopyDown) return;

            CopyDown();

            DestroyObject();

            EditTool.DeactivateEditOnMain();
        }

        protected override void BuildAllMeshes()
        {
            GeneratePlanarUVMaps(meshTransform: transform, refernceTransform: LinkedFloor.LinkedBuildingController.transform);

            base.BuildAllMeshes();
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
}