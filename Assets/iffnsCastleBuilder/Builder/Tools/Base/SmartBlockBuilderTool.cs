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

        //VirtualBlock startBlock;
        Vector3 startPosition;
        OnFloorObject CurrentOnFloorObject;
        OnFloorObject currentTemplate;

        HumanBuildingController currentBuilding
        {
            get
            {
                return currentBuilderController.CurrentBuilding;
            }
        }

        SmartBlockToolType currentSmartBlockToolType = SmartBlockToolType.RoofRectangular;
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
            RoofRectangular,
            RoofCircular,
            ArcHorizontal,
            Door,
            Window,
            ArcVertical,
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
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0) && CurrentOnFloorObject == null)
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
                    //CurrentOnFloorObject.SecondPositionNode.Position = new Vector2Int(startBlock.Position.x + 1, startBlock.Position.y + 1);

                    CurrentOnFloorObject.ApplyBuildParameters();
                }
            }
            else if (Input.GetMouseButton(0))
            {
                Vector3 endPosition = GetImpactPointFromClick();

                if (CurrentOnFloorObject.Organizer != null)
                {
                    CurrentOnFloorObject.Organizer.SecondBuildPositionAbsolute = endPosition;
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
                        if (endPosition != null)
                        {
                            //Debug.Log(endBlock.XPosition + "," + endBlock.ZPosition);

                            Vector2Int coordinate;

                            switch (secondGridNode.Type)
                            {
                                case GridModificationNode.ModificationNodeType.Block:
                                    coordinate = currentBuilding.CurrentFloorObject.GetBlockCoordinateFromCoordinateAbsolute(endPosition);
                                    if (currentBuilding.BlockCoordinateIsOnGrid(coordinate))
                                    {
                                        secondGridNode.AbsoluteCoordinate = coordinate;
                                    }
                                    break;
                                case GridModificationNode.ModificationNodeType.Node:
                                    coordinate = currentBuilding.CurrentFloorObject.GetNodeCoordinateFromPositionAbsolute(endPosition);
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

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    CurrentOnFloorObject.DestroyObject();
                }
            }
            else
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

        //Update
        void UpdateCurrentTemplate()
        {
            string currentIdentiefierString = "";

            switch (currentSmartBlockToolType)
            {
                case SmartBlockToolType.RoofRectangular:
                    currentIdentiefierString = RectangularRoof.constIdentifierString;
                    break;
                case SmartBlockToolType.RoofCircular:
                    currentIdentiefierString = CircularRoof.constIdentifierString;
                    break;
                case SmartBlockToolType.RectangularStair:
                    currentIdentiefierString = RectangularStairs.constIdentifierString;
                    break;
                case SmartBlockToolType.ArcHorizontal:
                    currentIdentiefierString = HorizontalArc.constIdentifierString;
                    break;
                case SmartBlockToolType.Door:
                    currentIdentiefierString = Door.constIdentifierString;
                    break;
                case SmartBlockToolType.Window:
                    currentIdentiefierString = Window.constIdentifierString;
                    break;
                case SmartBlockToolType.ArcVertical:
                    currentIdentiefierString = VerticalArc.constIdentifierString;
                    break;
                case SmartBlockToolType.RailingLinear:
                    currentIdentiefierString = RailingLinear.constIdentifierString;
                    break;
                case SmartBlockToolType.RailingArc:
                    currentIdentiefierString = RailingArc.constIdentifierString;
                    break;
                case SmartBlockToolType.NonCardinalWall:
                    currentIdentiefierString = NonCardinalWall.constIdentifierString;
                    break;
                case SmartBlockToolType.CircularStair:
                    currentIdentiefierString = CircularStair.constIdentifierString;
                    break;
                case SmartBlockToolType.Table:
                    currentIdentiefierString = TableGrid.constIdentifierString;
                    break;
                case SmartBlockToolType.Chair:
                    currentIdentiefierString = ChairGrid.constIdentifierString;
                    break;
                case SmartBlockToolType.Bed:
                    currentIdentiefierString = BedGrid.constIdentifierString;
                    break;
                case SmartBlockToolType.Column:
                    currentIdentiefierString = Column.constIdentifierString;
                    break;
                case SmartBlockToolType.Counter:
                    currentIdentiefierString = Counter.constIdentifierString;
                    break;
                case SmartBlockToolType.Ladder:
                    currentIdentiefierString = Ladder.constIdentifierString;
                    break;
                case SmartBlockToolType.Triangle:
                    currentIdentiefierString = Triangle.constIdentifierString;
                    break;
                case SmartBlockToolType.TriangularRoof:
                    currentIdentiefierString = TriangularRoof.constIdentifierString;
                    break;
                case SmartBlockToolType.RoofWall:
                    currentIdentiefierString = RoofWall.constIdentifierString;
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