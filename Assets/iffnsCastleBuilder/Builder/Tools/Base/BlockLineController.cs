using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class BlockLineController : MonoBehaviour
    {
        //Unity assignments
        //[SerializeField] BlockLineType currentBlockLineType = BlockLineType.Complete;
        [SerializeField] LineRenderer BlockLineTemplate;
        [SerializeField] GameObject SizeInfo;
        [SerializeField] Transform BlockLineHolder;

        //Runtime variables
        readonly List<GameObject> blockLines = new List<GameObject>();
        //int BlockLineRadius = 3;
        //static bool createWallLines = false;
        CastleController linkedBuilding;
        Vector2Int lastBlockGridSize = Vector2Int.zero;

        public void Setup(CastleController linkedBuilding)
        {
            this.linkedBuilding = linkedBuilding;
        }


        public enum BlockLineType
        {
            Complete,
            AroundFocus
        }

        public bool SizeInfoActivation
        {
            get
            {
                return SizeInfo.activeSelf;
            }
            set
            {
                SizeInfo.SetActive(value);
            }
        }

        CastleController.FloorViewDirectionType viewDirection;
        public CastleController.FloorViewDirectionType ViewDirection
        {
            get
            {
                return viewDirection;
            }
            set
            {
                viewDirection = value;

                //ToDo: Update view direction

                UpdatePositionAndRotation();
            }
        }

        public void UpdateAll()
        {
            ViewDirection = linkedBuilding.FloorViewDirection;

            SetCompleteGrid();

            UpdatePositionAndRotation();
        }

        public void UpdatePositionAndRotation()
        {
            UpdateBlockLinePosition();

            UpdateDimensionInfoPosition();
        }

        void UpdateBlockLinePosition()
        {
            BlockLineHolder.transform.position = linkedBuilding.CurrentFloorObject.transform.position;
            BlockLineHolder.transform.rotation = linkedBuilding.CurrentFloorObject.transform.rotation;

            SizeInfo.transform.position = linkedBuilding.CurrentFloorObject.transform.position;

            switch (viewDirection)
            {
                case CastleController.FloorViewDirectionType.topDown:
                    BlockLineHolder.transform.position += Vector3.up * (linkedBuilding.CurrentFloorObject.BottomFloorHeight + MathHelper.SmallFloat);
                    BlockLineHolder.transform.localScale = Vector3.one;
                    /*
                    DimensionInfo.transform.rotation = Quaternion.Euler(Vector3.right * 90);
                    DimensionInfo.transform.position += Vector3.up * CurrentBuilding.CurrentFloorObject.BottomFloorHeight;
                    */
                    break;
                case CastleController.FloorViewDirectionType.bottomUp:
                    BlockLineHolder.transform.position += new Vector3(linkedBuilding.BlockGridSize.x * linkedBuilding.BlockSize, -MathHelper.SmallFloat, 0);
                    BlockLineHolder.transform.localScale = new Vector3(1, -1, 1);
                    BlockLineHolder.transform.Rotate(Vector3.forward * 180);
                    /*
                    DimensionInfo.transform.rotation = Quaternion.Euler(new Vector3(-90, -90, 0));
                    */
                    break;
                default:
                    break;
            }
        }

        void UpdateDimensionInfoPosition()
        {
            switch (viewDirection)
            {
                case CastleController.FloorViewDirectionType.topDown:
                    SizeInfo.transform.rotation = Quaternion.identity;
                    SizeInfo.transform.position += Vector3.up * linkedBuilding.CurrentFloorObject.BottomFloorHeight;
                    break;
                case CastleController.FloorViewDirectionType.bottomUp:
                    SizeInfo.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 180));
                    break;
                default:
                    break;
            }
        }

        public void SetCompleteGrid()
        {
            if (lastBlockGridSize.x == linkedBuilding.BlockGridSize.x && lastBlockGridSize.y == linkedBuilding.BlockGridSize.y) return;

            lastBlockGridSize = linkedBuilding.BlockGridSize;

            //Remove old block lines
            foreach (GameObject child in blockLines)
            {
                Destroy(child);
            }

            blockLines.Clear();

            //Add new lines
            for (int xPos = 0; xPos <= linkedBuilding.BlockGridSize.x; xPos++)
            {
                LineRenderer currentLine = Instantiate(BlockLineTemplate).transform.GetComponent<LineRenderer>();

                currentLine.transform.parent = BlockLineHolder;

                currentLine.transform.localPosition = Vector3.right * linkedBuilding.BlockSize * xPos;

                currentLine.SetPosition(1, Vector3.right * linkedBuilding.BlockSize * linkedBuilding.BlockGridSize.y);

                currentLine.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 90));

                blockLines.Add(currentLine.gameObject);
            }

            for (int zPos = 0; zPos <= linkedBuilding.BlockGridSize.y; zPos++)
            {
                LineRenderer currentLine = Instantiate(BlockLineTemplate).transform.GetComponent<LineRenderer>();

                currentLine.transform.parent = BlockLineHolder;

                currentLine.transform.localPosition = Vector3.forward * linkedBuilding.BlockSize * zPos;

                currentLine.SetPosition(1, Vector3.right * linkedBuilding.BlockSize * linkedBuilding.BlockGridSize.x);
                
                blockLines.Add(currentLine.gameObject);
            }
        }

        /*
        public void UpdateBlockLines(CastleController.FloorViewDirectionType viewDirection)
        {
            //Check if floor lines activated

            switch (currentBlockLineType)
            {
                case BlockLineType.Complete:
                    SetCompleteGrid();
                    SetBaseBlockLinePosition(viewDirection: viewDirection);
                    break;
                case BlockLineType.AroundFocus:
                    
                    VirtualBlock newFocusBlock = getNewFocusBlock();
                    if (newFocusBlock != null) ShowBlockLinesAroundFocus(focusBlock: newFocusBlock);
                    
                    break;
                default:
                    break;
            }
        }
        */

        /*
        void ShowBlockLinesAroundFocus(VirtualBlock focusBlock)
        {
            //Cleanup
            if (currentLineHolder != null)
            {
                Destroy(currentLineHolder);
            }

            blockLines.Clear();

            //Setup new line holder
            currentLineHolder = new GameObject();
            currentLineHolder.transform.position = CurrentBuilding.CurrentFloorObject.transform.position + Vector3.up * (CurrentBuilding.CurrentFloorObject.BottomFloorHeight + 0.01f);
            currentLineHolder.transform.rotation = CurrentBuilding.CurrentFloorObject.transform.rotation;

            //Check min and max values
            Vector2Int startingPosition = Vector2Int.zero;
            Vector2Int endPosition = Vector2Int.zero;

            startingPosition.x = focusBlock.XCoordinate - BlockLineRadius;
            endPosition.x = focusBlock.XCoordinate + BlockLineRadius;
            startingPosition.y = focusBlock.ZCoordinate - BlockLineRadius;
            endPosition.y = focusBlock.ZCoordinate + BlockLineRadius;

            if (startingPosition.x < 0) startingPosition.x = 0;
            if (startingPosition.y < 0) startingPosition.y = 0;
            if (endPosition.x > CurrentBuilding.BlockGridSize.x - 1) endPosition.x = CurrentBuilding.BlockGridSize.x - 1;
            if (endPosition.y > CurrentBuilding.BlockGridSize.y - 1) endPosition.y = CurrentBuilding.BlockGridSize.y - 1;

            //Create lines
            for (int xPos = startingPosition.x; xPos <= endPosition.x + 1; xPos++)
            {
                LineRenderer currentLine = Instantiate(BlockLineTemplate).transform.GetComponent<LineRenderer>();
                currentLine.transform.parent = currentLineHolder.transform;

                currentLine.transform.localPosition = CurrentBuilding.CurrentFloorObject.GetLocalNodePositionFromNodeIndex(new Vector2Int(xPos, startingPosition.y));
                currentLine.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 90));

                float lineLength = (endPosition.y - startingPosition.y + 1) * CurrentBuilding.BlockSize;

                currentLine.SetPosition(1, Vector3.right * lineLength);
            }

            for (int zPos = startingPosition.y; zPos <= endPosition.y + 1; zPos++)
            {
                LineRenderer currentLine = Instantiate(BlockLineTemplate).transform.GetComponent<LineRenderer>();
                currentLine.transform.parent = currentLineHolder.transform;

                currentLine.transform.localPosition = CurrentBuilding.CurrentFloorObject.GetLocalNodePositionFromNodeIndex(new Vector2Int(startingPosition.x, zPos));
                currentLine.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 90));

                float lineLength = (endPosition.x - startingPosition.x + 1) * CurrentBuilding.BlockSize;

                currentLine.SetPosition(1, Vector3.down * lineLength);
            }
        }
        */


        /*
        public void CreateBaseGridForEachFloor()
        {

            for (int floorNumber = -linkedBuilding.NegativeFloors; floorNumber <= linkedBuilding.PositiveFloors; floorNumber++)
            {
                FloorController currentFloor = linkedBuilding.Floor(floorNumber: floorNumber);

                GameObject currentLineHolder = new GameObject();

                currentLineHolder.SetActive(false);

                currentLineHolder.transform.parent = transform;

                currentLineHolder.transform.position = currentFloor.transform.position + Vector3.up * (currentFloor.BottomFloorHeight + 0.01f);

                currentLineHolder.transform.rotation = currentFloor.transform.rotation;

                blockLines.Add(currentLineHolder);

                for (int xPos = 0; xPos <= linkedBuilding.BlockGridSize.x; xPos++)
                {
                    LineRenderer currentLine = Instantiate(BlockLineTemplate).transform.GetComponent<LineRenderer>();

                    currentLine.transform.parent = currentLineHolder.transform;

                    currentLine.transform.localPosition = Vector3.right * linkedBuilding.BlockSize * (xPos - 0.5f) - Vector3.forward * linkedBuilding.BlockSize * 0.5f;

                    currentLine.SetPosition(1, Vector3.right * linkedBuilding.BlockSize * linkedBuilding.BlockGridSize.y);

                    currentLine.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 90));
                }

                for (int zPos = 0; zPos <= linkedBuilding.BlockGridSize.y; zPos++)
                {
                    LineRenderer currentLine = Instantiate(BlockLineTemplate).transform.GetComponent<LineRenderer>();

                    currentLine.transform.parent = currentLineHolder.transform;

                    currentLine.transform.localPosition = Vector3.forward * linkedBuilding.BlockSize * (zPos - 0.5f) - Vector3.right * linkedBuilding.BlockSize * 0.5f;

                    currentLine.SetPosition(1, Vector3.right * linkedBuilding.BlockSize * linkedBuilding.BlockGridSize.x);
                }
            }

            blockLines[linkedBuilding.CurrentFloorIndex].SetActive(true);
        }
        */

    }
}