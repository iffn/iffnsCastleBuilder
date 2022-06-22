using iffnsStuff.iffnsUnityResources;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class BlockWallBuilderTool : MonoBehaviour
    {
        //Setup in editor
        [SerializeField] CastleBuilderController CurrentBuilderController;
        [SerializeField] RTSController currentRTSController;
        [SerializeField] PreviewBlock PreviewBlockTemplate;

        //Runtime parameters
        PreviewBlock previewBlock;
        public BlockToolType CurrentBlockToolType = BlockToolType.Wall;
        public DrawToolType CurrentDrawToolType = DrawToolType.Dot;
        VirtualBlock originBlock;
        VirtualBlock endBlock;

        CastleController CurrentBuilding
        {
            get
            {
                return CurrentBuilderController.CurrentBuilding;
            }
        }

        public enum BlockToolType
        {
            Wall,
            Empty,
            Floor,
        }

        void ChangeBlockBasedOnCurrentTool(VirtualBlock block)
        {
            switch (CurrentBlockToolType)
            {
                case BlockToolType.Wall:
                    block.BlockType = VirtualBlock.BlockTypes.Wall;
                    break;
                case BlockToolType.Empty:
                    block.BlockType = VirtualBlock.BlockTypes.Empty;
                    break;
                case BlockToolType.Floor:
                    block.BlockType = VirtualBlock.BlockTypes.Floor;
                    break;
                default:
                    Debug.LogWarning("Error: Enum type not defined");
                    break;
            }
        }

        //Tool setting

        public enum DrawToolType
        {
            Dot,
            CardinalWall,
            FullBlock,
            HollowBlock
        }

        public void SetParameters(BlockToolType newBlockType, DrawToolType newDrawToolType)
        {
            CurrentBlockToolType = newBlockType;
            CurrentDrawToolType = newDrawToolType;
        }

        void DotToolUpdate()
        {
            if (Input.GetMouseButton(0))
            {
                if (EventSystem.current.IsPointerOverGameObject()) //if over UI button
                {
                    //Do nothing
                    return;
                }

                //ToDebug: Reference not set error causes crash?

                VirtualBlock block = GetBlockFromClick(OnlyCheckCurrentFloor: true);

                if (block == null) return;

                ChangeBlockBasedOnCurrentTool(block);

                block.LinkedFloorController.ApplyBuildParameters();
            }
        }

        void StraightLineUpdate()
        {
            //Get initial object if button down
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject()) //if over UI button
                {
                    return;
                }

                originBlock = GetBlockFromClick(OnlyCheckCurrentFloor: true);
            }

            //Return if nothing selected
            if (originBlock == null) return;

            //Preview line
            endBlock = GetBlockFromClick(OnlyCheckCurrentFloor: true);
            if (endBlock != null)
            {
                int xPosOrigin = originBlock.XCoordinate;
                int zPosOrigin = originBlock.ZCoordinate;
                int xPosEnd = endBlock.XCoordinate;
                int zPosEnd = endBlock.ZCoordinate;

                int xDifference = xPosEnd - xPosOrigin;
                int zDifference = zPosEnd - zPosOrigin;

                if (Mathf.Abs(xDifference) > Mathf.Abs(zDifference))
                {
                    SetCardinalPreviewBlock(firstBlock: originBlock, secondBlock: originBlock.LinkedFloorController.BlockAtPosition(xPos: xPosEnd, zPos: zPosOrigin));
                }
                else
                {
                    SetCardinalPreviewBlock(firstBlock: originBlock, secondBlock: originBlock.LinkedFloorController.BlockAtPosition(xPos: xPosOrigin, zPos: zPosEnd));
                }
            }
            else
            {
                if (previewBlock != null) previewBlock.Clear();
            }

            //Quit if button up
            if (Input.GetMouseButtonUp(0))
            {
                if (endBlock != null)
                {

                    if (originBlock == endBlock)
                    {
                        ChangeBlockBasedOnCurrentTool(originBlock);
                    }
                    else
                    {
                        //Create block line
                        int xPosOrigin = originBlock.XCoordinate;
                        int zPosOrigin = originBlock.ZCoordinate;
                        int xPosEnd = endBlock.XCoordinate;
                        int zPosEnd = endBlock.ZCoordinate;

                        int xDifference = xPosEnd - xPosOrigin;
                        int zDifference = zPosEnd - zPosOrigin;

                        if (Mathf.Abs(xDifference) > Mathf.Abs(zDifference))
                        {
                            int min = Mathf.Min(xPosOrigin, xPosEnd);
                            int max = Mathf.Max(xPosOrigin, xPosEnd);

                            for (int i = min; i <= max; i++)
                            {
                                VirtualBlock currentBlock = originBlock.LinkedFloorController.BlockAtPosition(xPos: i, zPos: zPosOrigin);

                                ChangeBlockBasedOnCurrentTool(currentBlock);
                            }
                        }
                        else
                        {
                            int min = Mathf.Min(zPosOrigin, zPosEnd);
                            int max = Mathf.Max(zPosOrigin, zPosEnd);

                            for (int i = min; i <= max; i++)
                            {
                                VirtualBlock currentBlock = originBlock.LinkedFloorController.BlockAtPosition(xPos: xPosOrigin, zPos: i);

                                ChangeBlockBasedOnCurrentTool(currentBlock);
                            }
                        }
                    }

                    originBlock.LinkedFloorController.GenerateFloorMesh();
                }

                originBlock = null;
                if (previewBlock != null) previewBlock.Clear();
            }
        }

        void FilledSquareUpdate()
        {
            //Get initial object if button down
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject()) //if over UI button
                {
                    //Do nothing
                    return;
                }

                originBlock = GetBlockFromClick(OnlyCheckCurrentFloor: true);
            }

            //Return if nothing selected
            if (originBlock == null) return;

            //Preview line
            endBlock = GetBlockFromClick(OnlyCheckCurrentFloor: true);
            if (endBlock != null)
            {
                SetCardinalPreviewBlock(firstBlock: originBlock, secondBlock: endBlock);
            }
            else
            {
                if (previewBlock != null) previewBlock.Clear();
            }

            //Quit if button up
            if (Input.GetMouseButtonUp(0))
            {
                if (endBlock != null)
                {

                    if (originBlock == endBlock)
                    {
                        ChangeBlockBasedOnCurrentTool(originBlock);
                    }
                    else
                    {
                        int xMin = Mathf.Min(originBlock.XCoordinate, endBlock.XCoordinate);
                        int xMax = Mathf.Max(originBlock.XCoordinate, endBlock.XCoordinate);
                        int zMin = Mathf.Min(originBlock.ZCoordinate, endBlock.ZCoordinate);
                        int zMax = Mathf.Max(originBlock.ZCoordinate, endBlock.ZCoordinate);

                        for (int x = xMin; x <= xMax; x++)
                        {
                            for (int z = zMin; z <= zMax; z++)
                            {
                                VirtualBlock currentBlock = originBlock.LinkedFloorController.BlockAtPosition(xPos: x, zPos: z);

                                ChangeBlockBasedOnCurrentTool(currentBlock);
                            }
                        }


                    }

                    originBlock.LinkedFloorController.GenerateFloorMesh();
                }

                originBlock = null;
                ClearCardinalPreviewBlock();
            }
        }

        void EmptySquareUpdate()
        {
            //Get initial object if button down
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject()) //if over UI button
                {
                    //Do nothing
                    return;
                }

                originBlock = GetBlockFromClick(OnlyCheckCurrentFloor: true);
            }

            //Return if nothing selected
            if (originBlock == null) return;

            //Preview line
            endBlock = GetBlockFromClick(OnlyCheckCurrentFloor: true);
            if (endBlock != null)
            {
                SetCardinalPreviewBlock(firstBlock: originBlock, secondBlock: endBlock);
            }
            else
            {
                if (previewBlock != null) previewBlock.Clear();
            }

            //Quit if button up
            if (Input.GetMouseButtonUp(0))
            {
                if (endBlock != null)
                {

                    if (originBlock == endBlock)
                    {
                        ChangeBlockBasedOnCurrentTool(originBlock);
                    }
                    else
                    {
                        int xMin = Mathf.Min(originBlock.XCoordinate, endBlock.XCoordinate);
                        int xMax = Mathf.Max(originBlock.XCoordinate, endBlock.XCoordinate);
                        int zMin = Mathf.Min(originBlock.ZCoordinate, endBlock.ZCoordinate);
                        int zMax = Mathf.Max(originBlock.ZCoordinate, endBlock.ZCoordinate);

                        for (int x = xMin; x <= xMax; x++)
                        {
                            VirtualBlock currentBlock = originBlock.LinkedFloorController.BlockAtPosition(xPos: x, zPos: originBlock.ZCoordinate);
                            ChangeBlockBasedOnCurrentTool(currentBlock);

                            currentBlock = originBlock.LinkedFloorController.BlockAtPosition(xPos: x, zPos: endBlock.ZCoordinate);
                            ChangeBlockBasedOnCurrentTool(currentBlock);
                        }

                        for (int z = zMin; z <= zMax; z++)
                        {
                            VirtualBlock currentBlock = originBlock.LinkedFloorController.BlockAtPosition(xPos: originBlock.XCoordinate, zPos: z);
                            ChangeBlockBasedOnCurrentTool(currentBlock);

                            currentBlock = originBlock.LinkedFloorController.BlockAtPosition(xPos: endBlock.XCoordinate, zPos: z);
                            ChangeBlockBasedOnCurrentTool(currentBlock);
                        }
                    }

                    originBlock.LinkedFloorController.GenerateFloorMesh();
                }

                originBlock = null;
                if (previewBlock != null) previewBlock.Clear();
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            switch (CurrentDrawToolType)
            {
                case DrawToolType.Dot:
                    DotToolUpdate();
                    break;
                case DrawToolType.CardinalWall:
                    StraightLineUpdate();
                    break;
                case DrawToolType.FullBlock:
                    FilledSquareUpdate();
                    break;
                case DrawToolType.HollowBlock:
                    EmptySquareUpdate();
                    break;
                default:
                    break;
            }
        }

        public void SetCardinalPreviewBlock(VirtualBlock firstBlock, VirtualBlock secondBlock)
        {
            if (previewBlock == null) previewBlock = Instantiate(PreviewBlockTemplate);

            previewBlock.SetCardinalPreviewBlock(firstBlock: firstBlock, secondBlock: secondBlock, currentBuilding: CurrentBuilding);


            /*
            previewBlock.transform.rotation = CurrentBuilding.transform.rotation;

            float xSize = (Mathf.Abs(firstBlock.XPosition - secondBlock.XPosition) + 1) * CurrentBuilding.blockSize + 0.05f;
            float zSize = (Mathf.Abs(firstBlock.ZPosition - secondBlock.ZPosition) + 1) * CurrentBuilding.blockSize + 0.05f;
            float ySize = CurrentBuilding.CurrentFloorObject.completeFloorHeight + 0.05f;

            float xPos = (0f + firstBlock.XPosition + secondBlock.XPosition) / 2 * CurrentBuilding.blockSize;
            float zPos = (0f + firstBlock.ZPosition + secondBlock.ZPosition) / 2 * CurrentBuilding.blockSize;
            float yPos = CurrentBuilding.CurrentFloorObject.transform.position.y + ySize / 2;

            previewBlock.transform.position = transform.rotation * new Vector3(xPos, yPos, zPos) + transform.position;
            previewBlock.transform.localScale = new Vector3(xSize, ySize, zSize);
            */
        }

        public void ClearCardinalPreviewBlock()
        {
            if (previewBlock != null)
            {
                previewBlock.Clear();
                previewBlock = null;
            }
        }


        VirtualBlock GetBlockFromClick(bool OnlyCheckCurrentFloor)
        {
            return SmartBlockBuilderTool.GetBlockFromClick(OnlyCheckCurrentFloor: OnlyCheckCurrentFloor, currentBuilding: CurrentBuilding, currentRTSCamera: currentRTSController);
        }
    }
}