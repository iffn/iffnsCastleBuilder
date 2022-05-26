using iffnsStuff.iffnsBaseSystemForUnity;
using iffnsStuff.iffnsUnityResources;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class SmartBlockBuilderTool : MonoBehaviour
    {
        //Unity assignments
        [SerializeField] HumanBuilderController currentBuilderController = null;
        [SerializeField] RTSController currentRTSController = null;
        //[SerializeField] BuildingToolController currentBuildingToolController = null;

        //Runtime parameters
        Vector3 startPosition;
        OnFloorObject CurrentOnFloorObject;
        OnFloorObject currentTemplate;

        bool workedPreviously;
        Vector3 previousEndPosition;


        HumanBuildingController currentBuilding
        {
            get
            {
                return currentBuilderController.CurrentBuilding;
            }
        }

        SmartBlockToolType currentSmartBlockToolType = SmartBlockToolType.RectangularRoof;
        public SmartBlockToolType CurrentSmartBlockToolType
        {
            get
            {
                return currentSmartBlockToolType;
            }
            set
            {
                currentSmartBlockToolType = value;
                UpdateCurrentTemplate();
            }
        }

        public enum SmartBlockToolType
        {
            RectangularRoof,
            RoofCircular,
            ArcHorizontal,
            Door,
            Window,
            VerticalArc,
            RailingLinear,
            RailingArc,
            NonCardinalWall,
            RectangularStair,
            CircularStair,
            Table,
            Chair,
            Bed,
            Column,
            Counter,
            Ladder,
            Triangle,
            TriangularRoof,
            RoofWall
        }


        void GeneralEditUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject()) //If over UI
                {
                    return;
                }

                //startBlock = GetBlockFromClick(OnlyCheckCurrentFloor: true);
                startPosition = GetImpactPointFromClick();

                //if (startBlock != null)
                if (startPosition != null)
                {
                    //Debug.Log(startBlock.XPosition + "," + startBlock.ZPosition);

                    if (currentTemplate == null)
                    {
                        Debug.LogWarning("Error with edit tool: Current template not defined");
                        return;
                    }

                    CurrentOnFloorObject = Instantiate(original: currentTemplate);
                    CurrentOnFloorObject.Setup(superObject: currentBuilding.CurrentFloorObject);
                    CurrentOnFloorObject.ColliderActivationState = false;

                    workedPreviously = false;

                    if (CurrentOnFloorObject.Organizer != null)
                    {
                        CurrentOnFloorObject.Organizer.FirstBuildPositionAbsolute = startPosition;
                    }
                    else
                    {
                        //ToDo: Remove once all implemented

                        if (CurrentOnFloorObject.FirstPositionNode == null)
                        {
                            Debug.LogWarning("Error when creating OnFloorObject: Object does not have the First Position Node defined. Identifier = " + CurrentOnFloorObject.IdentifierString);
                            CurrentOnFloorObject.DestroyObject();
                            CurrentOnFloorObject = null;
                            return;
                        }

                        if (CurrentOnFloorObject.FirstPositionNode is GridModificationNode firstGridNode)
                        {
                            switch (firstGridNode.Type)
                            {
                                case GridModificationNode.ModificationNodeType.Block:
                                    firstGridNode.AbsoluteCoordinate = currentBuilding.CurrentFloorObject.GetBlockCoordinateFromCoordinateAbsolute(startPosition);
                                    break;
                                case GridModificationNode.ModificationNodeType.Node:
                                    firstGridNode.AbsoluteCoordinate = currentBuilding.CurrentFloorObject.GetNodeCoordinateFromPositionAbsolute(startPosition);
                                    break;
                                default:
                                    Debug.LogWarning("Error: Node type not defined");
                                    break;
                            }


                        }
                        else if (CurrentOnFloorObject.FirstPositionNode is FloatingModificationNode firstFloatNode)
                        {
                            firstFloatNode.AbsolutePosition = GetPositionFromClick(currentRTSController: currentRTSController);
                        }
                    }

                    CurrentOnFloorObject.FirstPositionNode.Show(activateCollider: false);
                    if(CurrentOnFloorObject.SecondPositionNode != null) CurrentOnFloorObject.SecondPositionNode.Show(activateCollider: false);
                    //CurrentOnFloorObject.SecondPositionNode.Position = new Vector2Int(startBlock.Position.x + 1, startBlock.Position.y + 1);

                    SetSecondPosition(startPosition);

                    CurrentOnFloorObject.ApplyBuildParameters();

                    if (!workedPreviously) workedPreviously = !CurrentOnFloorObject.failed;
                }
            }
            else if (Input.GetMouseButton(0))
            {
                if (CurrentOnFloorObject == null) return;

                Vector3 newEndPosition = GetImpactPointFromClick();

                SetSecondPosition(newEndPosition);

                //Revert if it worked previously and failed
                if (workedPreviously)
                {
                    if (CurrentOnFloorObject.failed)
                    {
                        SetSecondPosition(previousEndPosition);
                    }
                    else
                    {
                        previousEndPosition = newEndPosition;
                    }
                }
                else
                {
                    if (!CurrentOnFloorObject.failed)
                    {
                        workedPreviously = true;
                        previousEndPosition = newEndPosition;
                    }
                }

                //Cancel
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    CurrentOnFloorObject.DestroyObject();
                }
            }
            else //Mouse up
            {
                if (CurrentOnFloorObject != null)
                {
                    if (!CurrentOnFloorObject.failed)
                    {
                        CurrentOnFloorObject.HideModificationNodes();
                        CurrentOnFloorObject.ColliderActivationState = true;
                        CurrentOnFloorObject = null;
                    }
                    else
                    {
                        CurrentOnFloorObject.DestroyObject();
                    }
                }
            }
        }

        void SetSecondPosition(Vector3 newEndPosition)
        {
            if (CurrentOnFloorObject.Organizer != null)
            {
                CurrentOnFloorObject.Organizer.SecondBuildPositionAbsolute = newEndPosition;
            }
            else
            {
                //ToDo: Remove once all implemented

                if (CurrentOnFloorObject.SecondPositionNode == null)
                {
                    //Cannot set second position since it's not defined
                    return;
                }

                if (CurrentOnFloorObject.SecondPositionNode is GridModificationNode secondGridNode)
                {
                    //VirtualBlock endBlock = GetBlockFromClick(OnlyCheckCurrentFloor: true);


                    //if (endBlock != null)
                    if (newEndPosition != null)
                    {
                        //Debug.Log(endBlock.XPosition + "," + endBlock.ZPosition);

                        Vector2Int coordinate;

                        switch (secondGridNode.Type)
                        {
                            case GridModificationNode.ModificationNodeType.Block:
                                coordinate = currentBuilding.CurrentFloorObject.GetBlockCoordinateFromCoordinateAbsolute(newEndPosition);
                                if (currentBuilding.BlockCoordinateIsOnGrid(coordinate))
                                {
                                    secondGridNode.AbsoluteCoordinate = coordinate;
                                }
                                break;
                            case GridModificationNode.ModificationNodeType.Node:
                                coordinate = currentBuilding.CurrentFloorObject.GetNodeCoordinateFromPositionAbsolute(newEndPosition);
                                if (currentBuilding.NodeCoordinateIsOnGrid(coordinate))
                                {
                                    if (secondGridNode == null || secondGridNode.AbsoluteCoordinate == null || coordinate == null)
                                    {
                                        Debug.Log("hi");
                                    }

                                    secondGridNode.AbsoluteCoordinate = coordinate;
                                }
                                break;
                            default:
                                Debug.LogWarning("Error: Node type not defined");
                                break;
                        }

                        //secondNode.AbsoluteCoordinate = endBlock.Position;

                        //CurrentOnFloorObject.FirstPositionNode.AbsolutePosition = startBlock.Position;
                        CurrentOnFloorObject.SecondPositionNode.Show(activateCollider: false);
                    }
                }
                else if (CurrentOnFloorObject.SecondPositionNode is FloatingModificationNode secondFloatNode)
                {
                    secondFloatNode.AbsolutePosition = GetPositionFromClick(currentRTSController: currentRTSController);
                }
            }
        }

        //Update
        void UpdateCurrentTemplate()
        {
            string currentIdentiefierString = "";

            switch (currentSmartBlockToolType)
            {
                case SmartBlockToolType.RectangularRoof:
                    currentIdentiefierString = nameof(RectangularRoof);
                    break;
                case SmartBlockToolType.RoofCircular:
                    currentIdentiefierString = nameof(CircularRoof);
                    break;
                case SmartBlockToolType.RectangularStair:
                    currentIdentiefierString = nameof(RectangularStairs);
                    break;
                case SmartBlockToolType.ArcHorizontal:
                    currentIdentiefierString = nameof(HorizontalArc);
                    break;
                case SmartBlockToolType.Door:
                    currentIdentiefierString = nameof(Door);
                    break;
                case SmartBlockToolType.Window:
                    currentIdentiefierString = nameof(Window);
                    break;
                case SmartBlockToolType.VerticalArc:
                    currentIdentiefierString = nameof(VerticalArc);
                    break;
                case SmartBlockToolType.RailingLinear:
                    currentIdentiefierString = nameof(RailingLinear);
                    break;
                case SmartBlockToolType.RailingArc:
                    currentIdentiefierString = nameof(RailingArc);
                    break;
                case SmartBlockToolType.NonCardinalWall:
                    currentIdentiefierString = nameof(NonCardinalWall);
                    break;
                case SmartBlockToolType.CircularStair:
                    currentIdentiefierString = nameof(CircularStair);
                    break;
                case SmartBlockToolType.Table:
                    currentIdentiefierString = nameof(TableGrid);
                    break;
                case SmartBlockToolType.Chair:
                    currentIdentiefierString = nameof(ChairGrid);
                    break;
                case SmartBlockToolType.Bed:
                    currentIdentiefierString = nameof(BedGrid);
                    break;
                case SmartBlockToolType.Column:
                    currentIdentiefierString = nameof(Column);
                    break;
                case SmartBlockToolType.Counter:
                    currentIdentiefierString = nameof(Counter);
                    break;
                case SmartBlockToolType.Ladder:
                    currentIdentiefierString = nameof(Ladder);
                    break;
                case SmartBlockToolType.Triangle:
                    currentIdentiefierString = nameof(Triangle);
                    break;
                case SmartBlockToolType.TriangularRoof:
                    currentIdentiefierString = nameof(TriangularRoof);
                    break;
                case SmartBlockToolType.RoofWall:
                    currentIdentiefierString = nameof(RoofWall);
                    break;
                default:
                    Debug.LogWarning("Error: Identifier type not yet yet");
                    break;
            }

            if (currentIdentiefierString.Equals(""))
            {
                return;
            }

            currentTemplate = ResourceLibrary.TryGetTemplateFromStringIdentifier(identifier: currentIdentiefierString) as OnFloorObject;
        }

        private void Start()
        {
            UpdateCurrentTemplate();
        }

        void Update()
        {
            GeneralEditUpdate();
        }

        VirtualBlock GetBlockFromClick(bool OnlyCheckCurrentFloor)
        {
            return GetBlockFromClick(OnlyCheckCurrentFloor: OnlyCheckCurrentFloor, currentBuilding: currentBuilding, currentRTSCamera: currentRTSController);
        }

        public static Vector3 GetPositionFromClick(RTSController currentRTSController)
        {
            return currentRTSController.GetImpactPositionFromMouseLocation();
        }

        //public static Vector2Int GetNodeCoordinateFromClick(bool OnlyCheckCurrentFloor, HumanBuildingController currentBuilding, RTSController currentRTSController)
        public static Vector2Int GetNodeCoordinateFromClick(HumanBuildingController currentBuilding, RTSController currentRTSController)
        {
            Ray ray = currentRTSController.GetRayFromCameraMouseLocation();
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //RaycastHit hit;
            Vector3 direction = ray.direction;
            Vector3 originAbsolute = ray.origin;

            float goalHeight = currentBuilding.CurrentFloorObject.transform.position.y;

            float heightDifference = -originAbsolute.y + goalHeight;
            float scaler = heightDifference / direction.y;

            Vector3 offset = direction * scaler;
            Vector3 impactPoint = originAbsolute + offset;

            return currentBuilding.CurrentFloorObject.GetNodeFromCoordinateAbsolute(impactPoint).Coordinate;

        }

        public Vector3 GetImpactPointFromClick()
        {
            Ray ray = currentRTSController.GetRayFromCameraMouseLocation();
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //RaycastHit hit;
            Vector3 direction = ray.direction;
            Vector3 originAbsolute = ray.origin;

            float goalHeight = currentBuilding.CurrentFloorObject.transform.position.y;

            float heightDifference = -originAbsolute.y + goalHeight;
            float scaler = heightDifference / direction.y;

            Vector3 offset = direction * scaler;
            Vector3 impactPoint = originAbsolute + offset;

            return impactPoint;
        }

        public static Vector2Int GetBlockCoordinateFromClick(bool OnlyCheckCurrentFloor, HumanBuildingController currentBuilding, RTSController currentRTSController)
        {
            Ray ray = currentRTSController.GetRayFromCameraMouseLocation();
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            Vector3 direction = ray.direction;
            Vector3 originAbsolute = ray.origin;

            Vector2Int GetBlockCoordinateFromSurface()
            {
                float goalHeight = currentBuilding.CurrentFloorObject.transform.position.y + currentBuilding.CurrentFloorObject.BottomFloorHeight;

                float heightDifference = -originAbsolute.y + goalHeight;
                float scaler = heightDifference / direction.y;

                Vector3 offset = direction * scaler;
                Vector3 impactPoint = originAbsolute + offset;

                return currentBuilding.CurrentFloorObject.GetBlockCoordinateFromCoordinateAbsolute(impactPoint);
            }

            Physics.Raycast(ray, out hit, Mathf.Infinity);

            if (hit.collider == null)
            {
                return GetBlockCoordinateFromSurface();
            }

            GameObject clickedOnObject = hit.collider.transform.gameObject;

            if (clickedOnObject == null)
            {
                return GetBlockCoordinateFromSurface();
            }

            ClickForwarder forwarder = clickedOnObject.transform.GetComponent<ClickForwarder>();

            if (forwarder == null)
            {
                return GetBlockCoordinateFromSurface();
            }

            //VirtualBlock hitBlock;

            if (forwarder.MainObject is FloorController clickedFloor)
            {
                if (OnlyCheckCurrentFloor && !clickedFloor.IsCurrentFloor) return GetBlockCoordinateFromSurface();

                return clickedFloor.GetBlockCoordinateFromImpact(impactPointAbsolute: hit.point, normal: hit.normal);
            }

            if (forwarder.MainObject is OnFloorObject currentFloorObject)
            {
                if (OnlyCheckCurrentFloor && !currentFloorObject.LinkedFloor.IsCurrentFloor) return GetBlockCoordinateFromSurface();

                return currentFloorObject.LinkedFloor.GetBlockCoordinateFromImpact(impactPointAbsolute: hit.point, normal: hit.normal);
            }

            return GetBlockCoordinateFromSurface();
        }

        public static VirtualBlock GetBlockFromClick(bool OnlyCheckCurrentFloor, HumanBuildingController currentBuilding, RTSController currentRTSCamera)
        {
            if (EventSystem.current.IsPointerOverGameObject()) //If over UI
            {
                return null;
            }

            Ray ray = currentRTSCamera.GetRayFromCameraMouseLocation();
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            Vector3 direction = ray.direction;
            Vector3 originAbsolute = ray.origin;

            VirtualBlock GetBlockFromSurface()
            {
                float goalHeight = currentBuilding.CurrentFloorObject.transform.position.y + currentBuilding.CurrentFloorObject.BottomFloorHeight;

                float heightDifference = -originAbsolute.y + goalHeight;
                float scaler = heightDifference / direction.y;

                Vector3 offset = direction * scaler;
                Vector3 impactPoint = originAbsolute + offset;

                return currentBuilding.CurrentFloorObject.GetBlockFromCoordinateAbsolute(impactPoint);
            }

            Physics.Raycast(ray, out hit, Mathf.Infinity);

            if (hit.collider == null)
            {
                return GetBlockFromSurface();
            }

            GameObject clickedOnObject = hit.collider.transform.gameObject;

            if (clickedOnObject == null)
            {
                return GetBlockFromSurface();
            }

            ClickForwarder forwarder = clickedOnObject.transform.GetComponent<ClickForwarder>();

            if (forwarder == null)
            {
                return GetBlockFromSurface();
            }

            //VirtualBlock hitBlock;

            if (forwarder.MainObject is FloorController clickedFloor)
            {
                if (!(OnlyCheckCurrentFloor && clickedFloor == currentBuilding.CurrentFloorObject)) return GetBlockFromSurface();

                return clickedFloor.GetBlockFromImpact(impactPointAbsolute: hit.point, normal: hit.normal);
            }

            if (forwarder.MainObject is OnFloorObject currentFloorObject)
            {
                if (OnlyCheckCurrentFloor && !currentFloorObject.LinkedFloor.CurrentFloor) return GetBlockFromSurface();

                return currentFloorObject.LinkedFloor.GetBlockFromImpact(impactPointAbsolute: hit.point, normal: hit.normal);
            }

            return GetBlockFromSurface();
        }


    }
}