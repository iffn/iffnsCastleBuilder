using iffnsStuff.iffnsBaseSystemForUnity;
using iffnsStuff.iffnsUnityResources;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class EditTool : MonoBehaviour
    {
        //Assignments
        //[SerializeField] UIResources UIResourcesHolder = null;
        [SerializeField] RTSController currentRTSController = null;
        [SerializeField] BuildingToolController currentBuildingToolController = null;
        [SerializeField] HumanBuilderController currentBuilderController = null;

        IBaseObject activeObject;
        ModificationNode activeNode;
        NodeWallSystem currentNodeWallSystem;

        HumanBuildingController currentBuilding
        {
            get
            {
                return currentBuilderController.CurrentBuilding;
            }
        }

        // Update is called once per frame
        void Update()
        {
            //Go through various updates
            if (NodeUpdate() == true) return; //Run this one first since the dragging needs to be completed
            if (SelectUpdate() == true) return;
        }

        void DeactivateEdit()
        {
            if (activeObject != null)
            {
                if (activeNode is GridScaleNode activeGridScaleNode)
                {
                    activeGridScaleNode.ApplyNewGridPosition();

                    UpdatePropertyMenu();
                    currentBuildingToolController.CurrentNavigationTools.UpdateBlockLines();
                }

                activeNode.ColliderActivationState = true;

                if (activeNode.LinkedObject != null) activeNode.LinkedObject.ColliderActivationState = true;

                activeNode = null;
            }
        }

        void HideCurrentModificationActions()
        {
            if (activeObject != null)
            {
                if (activeObject is BaseGameObject activeBaseGameObject)
                {
                    activeBaseGameObject.HideModificationNodes();
                }

                activeObject = null;
            }

            if (currentNodeWallSystem != null)
            {
                currentNodeWallSystem.HideModificationNodes();
                currentNodeWallSystem = null;
            }
        }

        bool SelectUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return true; //Ignore if over UI

                GameObject clickedOnObject = GetClickedGameObject();

                if (clickedOnObject == null)
                {
                    PropertyMenu.Clear();
                    HideCurrentModificationActions();
                    return false;
                }

                ClickForwarder forwarder = clickedOnObject.GetComponent<ClickForwarder>();

                if (forwarder != null)
                {
                    HideCurrentModificationActions();

                    activeObject = forwarder.MainObject;

                    if (activeObject == null)
                    {
                        PropertyMenu.Clear();

                        return false;
                    }

                    //Node wall edit
                    currentNodeWallSystem = forwarder.transform.parent.GetComponent<NodeWallSystem>();

                    if (currentNodeWallSystem != null)
                    {
                        currentNodeWallSystem.ShowModificationNodes(activateCollider: true);

                        return true;
                    }

                    ShowPropertyMenu(forwarder.MainObject);
                    forwarder.MainObject.ShowModificationNodes(activateCollider: true);
                    return true;
                }
                else
                {
                    NodeClickForwarder nodeForwarder = clickedOnObject.GetComponent<NodeClickForwarder>();

                    if (nodeForwarder != null)
                    {
                        nodeForwarder.ClickOnFunction.Invoke();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        bool NodeUpdate() //Return true if found something useful
        {
            if (activeNode == null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return true; //Ignore if over UI

                    GameObject clickedOnObject = GetClickedGameObject();

                    if (clickedOnObject == null) return false;

                    activeNode = clickedOnObject.GetComponent<ModificationNode>();

                    if (activeNode == null) return false;

                    if (activeNode is NodeWallRemoveNode activeNodeWallRemoveNode)
                    {
                        activeNodeWallRemoveNode.RemoveNodeWall();
                        activeNode = null;
                        return true;
                    }

                    activeNode.ColliderActivationState = false;
                    if (activeNode.LinkedObject != null) activeNode.LinkedObject.ColliderActivationState = false;


                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (Input.GetMouseButton(0))
                {
                    if (activeNode is GridModificationNode activeGridNode)
                    {
                        switch (activeGridNode.Type)
                        {
                            case GridModificationNode.ModificationNodeType.Block:
                                VirtualBlock currentBlock = SmartBlockBuilderTool.GetBlockFromClick(OnlyCheckCurrentFloor: true, currentBuilding: currentBuilding, currentRTSCamera: currentRTSController);
                                if (currentBlock == null) return true;
                                activeGridNode.AbsoluteCoordinate = currentBlock.Coordinate;
                                break;
                            case GridModificationNode.ModificationNodeType.Node:
                                Vector2Int currentNodePosition = SmartBlockBuilderTool.GetNodeCoordinateFromClick(currentBuilding: currentBuilding, currentRTSController: currentRTSController);
                                if (currentNodePosition == null) return true;
                                activeGridNode.AbsoluteCoordinate = currentNodePosition;
                                break;
                            default:
                                break;
                        }

                        UpdatePropertyMenu();

                        return true;
                    }
                    else if (activeNode is FloatingModificationNode activeMovingNode)
                    {
                        activeMovingNode.AbsolutePosition = SmartBlockBuilderTool.GetPositionFromClick(currentRTSController: currentRTSController);
                    }
                    else if (activeNode is GridScaleNode activeScaleNode)
                    {
                        activeScaleNode.NewTempGridPosition = SmartBlockBuilderTool.GetBlockCoordinateFromClick(OnlyCheckCurrentFloor: true, currentBuilding: currentBuilding, currentRTSController: currentRTSController);

                        activeScaleNode.modificationInProgress = true;
                    }
                    else if (activeNode is NodeWallEditModNode activeNodeWallEditModNode)
                    {
                        activeNodeWallEditModNode.NodeCoordinate = SmartBlockBuilderTool.GetNodeCoordinateFromClick(currentBuilding: currentBuilding, currentRTSController: currentRTSController);
                    }
                    else if (activeNode is NodeWallMultiModNode activeNodeWallMultiModNode)
                    {
                        activeNodeWallMultiModNode.NodeCoordinate = SmartBlockBuilderTool.GetNodeCoordinateFromClick(currentBuilding: currentBuilding, currentRTSController: currentRTSController);
                    }
                }
                else
                {
                    DeactivateEdit();

                    return true;
                }

                return true;
            }
        }

        public ControlBox PropertyMenu;

        void ShowPropertyMenu(IBaseObject currentObject)
        {
            List<IBaseObject> hierarchyList = new List<IBaseObject>();

            IBaseObject currentInvestigationObject = currentObject.SuperObject;

            hierarchyList.Add(currentObject);

            while (currentInvestigationObject != null)
            {
                if (currentInvestigationObject is HumanBuildingTestWorldController) break; //Ignore world controller

                hierarchyList.Insert(0, currentInvestigationObject);

                currentInvestigationObject = currentInvestigationObject.SuperObject;
            }

            PropertyMenu.Clear();

            foreach (IBaseObject hierarchyObject in hierarchyList)
            {
                PropertyMenu.AddTextLine(text: hierarchyObject.IdentifierString, bold: true);

                List<DelegateLibrary.VoidFunction> additionalCalls = new List<DelegateLibrary.VoidFunction>();

                if (hierarchyObject is HumanBuildingController tempBuilding)
                {
                    additionalCalls.Add(delegate { currentBuildingToolController.CurrentNavigationTools.UpdateBlockLines(); });
                }
                else if (hierarchyObject is FloorController tempFloor)
                {
                    additionalCalls.Add(delegate { currentBuildingToolController.CurrentNavigationTools.UpdateBlockLines(); });
                }

                PropertyMenu.AddMailboxLines(lines: hierarchyObject.SingleMailboxLines, lineOwner: hierarchyObject, additionalCalls: additionalCalls);
            }

            ShowEditButtons(currentObject);
        }

        void UpdatePropertyMenu()
        {
            ShowPropertyMenu(currentObject: activeObject);
        }

        void ClearPropertyMenu()
        {
            PropertyMenu.Clear();
        }

        void ShowEditButtons(IBaseObject currentObject)
        {

            foreach (BaseEditButtonFunction buttonFunction in currentObject.EditButtons)
            {
                //Add clear property menu to delete button
                if (currentObject is OnFloorObject currentOnFloorObject)
                {
                    if (buttonFunction == currentOnFloorObject.DeleteButtonFunction)
                    {
                        PropertyMenu.AddButtonLine(text: buttonFunction.ButtonName, call: delegate { buttonFunction.Function(); ClearPropertyMenu(); activeObject = null; });
                        continue;
                    }
                }

                PropertyMenu.AddButtonLine(text: buttonFunction.ButtonName, call: delegate { buttonFunction.Function(); UpdatePropertyMenu(); });
            }
        }

        void ShowModificationNodes(IBaseObject currentObject)
        {
            if (currentObject is BaseGameObject currentBaseGameObject)
            {
                currentBaseGameObject.ShowModificationNodes(activateCollider: true);
            }
        }

        GameObject GetClickedGameObject()
        {
            Ray ray = currentRTSController.GetRayFromCameraMouseLocation();
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                return hit.collider.transform.gameObject;
            }

            else return null;
        }
    }
}