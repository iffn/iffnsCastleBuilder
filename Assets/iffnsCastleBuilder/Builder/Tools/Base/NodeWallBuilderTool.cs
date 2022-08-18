using iffnsStuff.iffnsUnityResources;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class NodeWallBuilderTool : MonoBehaviour
    {
        //Setup in editor
        [SerializeField] CastleBuilderController CurrentBuilderController;
        [SerializeField] RTSController currentRTSController;
        [SerializeField] PreviewBlock PreviewBlockTemplate;

        //Runtime parameters
        PreviewBlock previewBlock;
        NodeWallNode originNode;
        NodeWallNode endNode;

        ToolType currentToolType;

        public ToolType CurrentToolType
        {
            set
            {
                currentToolType = value;
            }
        }

        CastleController CurrentBuilding
        {
            get
            {
                return CurrentBuilderController.CurrentBuilding;
            }
        }

        public enum ToolType
        {
            Line,
            Rectangle,
            Edit
        }

        public void SetWallPreviewBlock(NodeWallNode firstNode, NodeWallNode secondNode)
        {
            if (previewBlock == null) previewBlock = Instantiate(PreviewBlockTemplate.transform.gameObject).transform.GetComponent<PreviewBlock>();

            previewBlock.SetWallPreviewBlock(firstNode: firstNode, secondNode: secondNode);
        }

        NodeWallNode GetNodePositionFromClick()
        {
            if (EventSystem.current.IsPointerOverGameObject()) //If over UI
            {
                return null;
            }

            Ray ray = currentRTSController.GetRayFromCameraMouseLocation();
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //RaycastHit hit;

            Vector3 direction = ray.direction;
            Vector3 originAbsolute = ray.origin;

            float goalHeight = CurrentBuilding.CurrentFloorObject.transform.position.y + CurrentBuilding.CurrentFloorObject.BottomFloorHeight;

            float heightDifference = -originAbsolute.y + goalHeight;
            float scaler = heightDifference / direction.y;

            Vector3 offset = direction * scaler;
            Vector3 impactPoint = originAbsolute + offset;

            return CurrentBuilding.CurrentFloorObject.GetNodeFromCoordinateAbsolute(impactPoint);
        }

        void NodeWallLineUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    //Do nothing
                    return;
                }

                originNode = GetNodePositionFromClick();
            }

            if (originNode == null) return;

            Vector2Int offset;

            if (Input.GetMouseButton(0))
            {
                NodeWallNode potentialEndNode = GetNodePositionFromClick();

                if (potentialEndNode == null)
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        clear();
                    }

                    return;
                }

                endNode = potentialEndNode;

                offset = endNode.Coordinate - originNode.Coordinate;

                if (!(offset.x == 0 && offset.y == 0))
                {
                    SetWallPreviewBlock(firstNode: originNode, secondNode: endNode);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                offset = endNode.Coordinate - originNode.Coordinate;

                if (originNode != null && endNode != null)
                {
                    if (!(offset.x == 0 && offset.y == 0))
                    {
                        CurrentBuilding.CurrentFloorObject.CurrentNodeWallSystem.CreateNodeWall(startPosition: originNode.Coordinate, endPosition: endNode.Coordinate);
                    }
                }

                clear();
            }

            void clear()
            {
                originNode = null;
                endNode = null;
                if (previewBlock != null) previewBlock.Clear();
            }



            return;

            /*
            //Generally clear preview block whenever unlicking
            if (Input.GetMouseButtonUp(0))
            {
                if(previewBlock != null) previewBlock.Clear();
            }

            //If clicking
            if (Input.GetMouseButtonDown(0))
            {
                //Ignore if over UI
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    //Do nothing
                    return;
                }

                //Get node from clicking
                originNode = GetNodePositionFromClick();
            }

            //Return if nothing selected
            if (originNode == null) return;

            //Continously get end node
            endNode = GetNodePositionFromClick();

            Vector2Int offset = endNode.Coordinate - originNode.Coordinate;

            if (!(offset.x == 0 && offset.y == 0))
            {
                //Set preview block (Will also create it if it has not been created yet
                SetWallPreviewBlock(firstNode: originNode, secondNode: endNode);
            }
            else
            {
                if (Input.GetMouseButtonUp(0))
                {
                    CurrentBuilding.CurrentFloorObject.CurrentNodeWallSystem.CreateNodeWall(startPosition: originNode.Coordinate, endPosition: endNode.Coordinate);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                previewBlock.Clear();
                originNode = null;
                endNode = null;

            }
            */
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            switch (currentToolType)
            {
                case ToolType.Line:
                    NodeWallLineUpdate();
                    break;
                case ToolType.Rectangle:
                    break;
                case ToolType.Edit:
                    break;
                default:
                    Debug.LogWarning("Error: Node wall tool type not defined");
                    break;
            }
        }
    }
}