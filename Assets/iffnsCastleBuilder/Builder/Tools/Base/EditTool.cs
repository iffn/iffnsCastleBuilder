using iffnsStuff.iffnsBaseSystemForUnity;
using iffnsStuff.iffnsUnityResources;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class EditTool : MonoBehaviour
    {
        static EditTool MainEditTool;

        //Unity assignments
        //[SerializeField] UIResources UIResourcesHolder = null;
        [SerializeField] RTSController currentRTSController = null;
        [SerializeField] BuildingToolController currentBuildingToolController = null;
        [SerializeField] HumanBuilderController currentBuilderController = null;
        [SerializeField] ControlBox PropertyMenu;

        //Runtime parameters
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

        private void OnDisable()
        {
            //ToDo: Find better way to catch unload when exiting play mode for example

            //When exiting play mode: ModificationNode.Hide() causes error during gameObject.SetActive(false)
            try
            {
                if (gameObject.activeInHierarchy == false)
                {
                    DeactivateEdit();
                }
            }
            catch
            {
                Debug.Log("Prevented unload error");
            }
        }

        private void Start()
        {
            MainEditTool = this;
        }

        public static void DeactivateEditOnMain()
        {
            if (MainEditTool == null) return;

            MainEditTool.DeactivateEdit();
        }

        void DeactivateEdit()
        {
            PropertyMenu.Clear();
            HideCurrentModificationActions();
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
                    DeactivateEdit();
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

                    if (activeNode is NodeWallFlipNode activeNodeWallFlipNode)
                    {
                        activeNodeWallFlipNode.FlipNodeWall();
                        activeNode = null;
                        return true;
                    }

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
                        Vector2Int newCoordinate;

                        switch (activeGridNode.Type)
                        {
                            case GridModificationNode.ModificationNodeType.Block:
                                VirtualBlock currentBlock = SmartBlockBuilderTool.GetBlockFromClick(OnlyCheckCurrentFloor: true, currentBuilding: currentBuilding, currentRTSCamera: currentRTSController);
                                if (currentBlock == null) return true;
                                newCoordinate = currentBlock.Coordinate;
                                break;
                            case GridModificationNode.ModificationNodeType.Node:
                                newCoordinate = SmartBlockBuilderTool.GetNodeCoordinateFromClick(currentBuilding: currentBuilding, currentRTSController: currentRTSController);
                                break;
                            default:
                                Debug.LogWarning("Error: Enum case not defined");
                                return true;
                        }

                        if (newCoordinate == null) return true;

                        Vector2Int previousCoordinate = activeGridNode.AbsoluteCoordinate;
                        activeGridNode.AbsoluteCoordinate = newCoordinate;

                        if (activeGridNode.LinkedOnFloorObject.Failed)
                        {
                            activeGridNode.AbsoluteCoordinate = previousCoordinate;
                        }
                        else
                        {
                            UpdatePropertyMenu();
                        }

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
                        Vector2Int newCoordinate = SmartBlockBuilderTool.GetNodeCoordinateFromClick(currentBuilding: currentBuilding, currentRTSController: currentRTSController);

                        if (!activeNodeWallEditModNode.WouldBeSameAsOtherCoordinate(newCoordinate: newCoordinate))
                        {
                            activeNodeWallEditModNode.NodeCoordinate = newCoordinate;
                        }
                    }
                    else if (activeNode is NodeWallMultiModNode activeNodeWallMultiModNode)
                    {
                        Vector2Int newCoordinate = SmartBlockBuilderTool.GetNodeCoordinateFromClick(currentBuilding: currentBuilding, currentRTSController: currentRTSController);

                        if (!activeNodeWallMultiModNode.WouldBeSameAsOtherCoordinate(newCoordinate: newCoordinate))
                        {
                            activeNodeWallMultiModNode.NodeCoordinate = newCoordinate;
                        }
                    }
                }
                else //Mouse up
                {
                    DeactivateNodeDraging();

                    return true;
                }

                return true;
            }
        }

        void DeactivateNodeDraging()
        {
            if (activeObject != null)
            {
                if (activeNode == null)
                {
                    activeObject = null;
                    return;
                }

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
            if (activeObject == null) return; //Only show is something is selected -> Not so when object removed after pressing a button

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