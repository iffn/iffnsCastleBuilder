using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class FloorController : BaseGameObject
    {
        //Unity assignments
        [SerializeField] GameObject smartBlockHolder = null;
        [SerializeField] GridScalerOrganizer GridScaleOrganizer;

        BlockMeshBuilder floorMeshBuilder;
        public void AddStaticMesh(TriangleMeshInfo staticMesh)
        {
            if (staticMesh.AllVerticesDirectly.Count == 0) return;

            StaticMeshManager.AddTriangleInfoIfValid(staticMesh);
        }

        public void UpdateAllCradinalUVs()
        {
            StaticMeshManager.UpdateCardinalUVMapsForAllUnusedTriangleInfos(originObjectForUV: LinkedBuildingController.transform);
        }

        public BlockMeshBuilder FloorMeshBuilder
        {
            get
            {
                return floorMeshBuilder;
            }
        }
        public GameObject SmartBlockHolder
        {
            get
            {
                return smartBlockHolder;
            }
        }

        public NodeWallSystem CurrentNodeWallSystem
        {
            get
            {
                return nodeWallSystemParam.SubObject as NodeWallSystem;
            }
        }

        //[SerializeField] CastleResources CastleResources;

        //Build parameters
        MailboxLineRanged bottomFloorHeightParam;
        MailboxLineRanged wallBetweenHeightParam;
        MailboxLineMultipleSubObject virtualBlocksParam;
        MailboxLineMultipleSubObject onFloorObjectsParam;
        MailboxLineSingleSubObject nodeWallSystemParam;

        CastleController buildingController;

        public enum BlockDirections
        {
            Font,
            Back,
            Left,
            Right
        }

        public CastleController LinkedBuildingController
        {
            get
            {
                return buildingController;
            }
        }

        public bool CurrentFloor
        {
            get
            {
                return buildingController.CurrentFloorObject == this;
            }
        }

        public int FloorNumber
        {
            get
            {
                return LinkedBuildingController.FloorNumberFromFloor(this);
            }
        }


        public float BottomFloorHeight
        {
            get
            {
                return bottomFloorHeightParam.Val;
            }
        }

        public float WallBetweenHeight
        {
            get
            {
                return wallBetweenHeightParam.Val;
            }
        }

        public float CompleteFloorHeight
        {
            get
            {
                return BottomFloorHeight + WallBetweenHeight;
            }
        }

        public float WallBetweenHeightWithScaler
        {
            get
            {
                if (CurrentFloor)
                {
                    return CompleteFloorHeight * LinkedBuildingController.WallDisplayHeightScaler - BottomFloorHeight;
                }
                else
                {
                    return CompleteFloorHeight - BottomFloorHeight;
                }
            }
        }

        public float WallHeightWithScaler
        {
            get
            {
                if (CurrentFloor)
                {
                    return CompleteFloorHeight * LinkedBuildingController.WallDisplayHeightScaler;
                }
                else
                {
                    return CompleteFloorHeight;
                }
            }
        }

        public float BlockSize
        {
            get
            {
                return LinkedBuildingController.BlockSize;
            }
        }

        public int FloorsAbove
        {
            get
            {
                return LinkedBuildingController.PositiveFloors - FloorNumber;
            }
        }

        public bool IsTopFloor
        {
            get
            {
                return FloorsAbove == 0;
            }
        }

        public int FloorsBelow
        {
            get
            {
                return FloorNumber + LinkedBuildingController.NegativeFloors;
            }
        }

        public bool IsBottomFloor
        {
            get
            {
                return FloorsBelow == 0;
            }
        }

        public FloorController FloorAbove
        {
            get
            {
                if (IsTopFloor) return null;
                return LinkedBuildingController.Floor(floorNumber: FloorNumber + 1);
            }
        }

        public bool IsCurrentFloor
        {
            get
            {
                return this == LinkedBuildingController.CurrentFloorObject;
            }
        }

        public float BaseHeightOfBlock(Vector2Int coordinate)
        {
            switch (BlockAtPosition(coordinate).BlockType)
            {
                case VirtualBlock.BlockTypes.Empty:
                    return 0;
                case VirtualBlock.BlockTypes.Floor:
                    return BottomFloorHeight;
                case VirtualBlock.BlockTypes.Wall:
                    return 0;
                default:
                    Debug.LogWarning("Error: Block type not defined");
                    return 0;
            }
        }

        public float BetweenFloorHeightOfBlock(Vector2Int coordinate)
        {
            switch (BlockAtPosition(coordinate).BlockType)
            {
                case VirtualBlock.BlockTypes.Empty:
                    return CompleteFloorHeight;
                case VirtualBlock.BlockTypes.Floor:
                    return WallBetweenHeight;
                case VirtualBlock.BlockTypes.Wall:
                    return CompleteFloorHeight;
                default:
                    Debug.LogWarning("Error: Block type not defined");
                    return CompleteFloorHeight;
            }
        }

        void SetupBuildParameters()
        {
            bottomFloorHeightParam = new MailboxLineRanged("Bottom floor height", CurrentMailbox, Mailbox.ValueType.buildParameter, 1f, 0.05f, 0.25f);
            wallBetweenHeightParam = new MailboxLineRanged("Wall height", CurrentMailbox, Mailbox.ValueType.buildParameter, 10f, 0.2f, 2.75f);
            virtualBlocksParam = new MailboxLineMultipleSubObject(valueName: "Floor matrix", CurrentMailbox);
            onFloorObjectsParam = new MailboxLineMultipleSubObject("On floor objects", CurrentMailbox);
            nodeWallSystemParam = new MailboxLineSingleSubObject("Node wall system", CurrentMailbox);
        }

        public override void ResetObject()
        {
            baseReset();

            FloorMatrix.Clear();

            CurrentNodeWallSystem.ResetObject();
        }

        //Block elements
        readonly List<List<VirtualBlock>> FloorMatrix = new List<List<VirtualBlock>>();

        public VirtualBlock BlockAtPosition(int xPos, int zPos)
        {
            if (xPos < 0 || xPos > buildingController.BlockGridSize.x - 1
                || zPos < 0 || zPos > buildingController.BlockGridSize.y - 1)
            {
                return null;
            }
            else
            {
                return FloorMatrix[xPos][zPos];
            }
        }

        public VirtualBlock BlockAtPosition(Vector2Int position)
        {
            int x = MathHelper.ClampInt(position.x, buildingController.BlockGridSize.x - 1, 0);
            int y = MathHelper.ClampInt(position.y, buildingController.BlockGridSize.y - 1, 0);

            return FloorMatrix[x][y];
        }

        List<VirtualBlock> AllBlocks
        {
            get
            {
                List<VirtualBlock> returnList = new List<VirtualBlock>();

                foreach (IBaseObject block in virtualBlocksParam.SubObjects)
                {
                    if (!(block is VirtualBlock currentBlock))
                    {
                        Debug.LogWarning("Error, Sub object has the wrong type. List = " + virtualBlocksParam.ValueName);
                        return returnList;
                    }

                    returnList.Add(currentBlock);
                }

                return returnList;
            }
        }


        //Modify stuff
        public void MoveStuffOnGrid(Vector2Int offset)
        {
            //Move blocks
            if (offset.x > 0)
            {
                //Move right
                for (int i = 0; i < virtualBlocksParam.NumberOfObjects; i++)
                {
                    int a = i;
                    int b = LinkedBuildingController.BlockGridSize.y;
                    int x = a / b;

                    int z = i % LinkedBuildingController.BlockGridSize.y;

                    if (x < offset.x)
                    {
                        virtualBlocksParam.SubObjects.Insert(0, new VirtualBlock(xPosition: x, zPosition: z, linkedFloorController: this, blockType: VirtualBlock.BlockTypes.Empty));
                    }
                    else if (x < LinkedBuildingController.BlockGridSize.x)
                    {
                        VirtualBlock block = virtualBlocksParam.SubObjects[i] as VirtualBlock;
                        block.DefinePositionValue(new Vector2Int(x, z));
                    }
                    else
                    {
                        int objectsToRemove = virtualBlocksParam.NumberOfObjects - i;

                        virtualBlocksParam.SubObjects.RemoveRange(i, objectsToRemove);
                        break;
                    }
                }
            }
            else if (offset.x < 0)
            {
                //Move left
                int absOffsetX = -offset.x;

                int initialNumberOfObjects = virtualBlocksParam.NumberOfObjects;

                int objectsToRemove = LinkedBuildingController.BlockGridSize.y * absOffsetX;
                virtualBlocksParam.SubObjects.RemoveRange(0, objectsToRemove);

                int numberOfObjectsToChange = initialNumberOfObjects - objectsToRemove;

                for (int i = 0; i < initialNumberOfObjects; i++)
                {

                    int x = i / LinkedBuildingController.BlockGridSize.x;
                    int z = i % LinkedBuildingController.BlockGridSize.y;

                    if (i < numberOfObjectsToChange)
                    {
                        VirtualBlock block = virtualBlocksParam.SubObjects[i] as VirtualBlock;
                        block.DefinePositionValue(new Vector2Int(x, z));
                    }
                    else
                    {
                        virtualBlocksParam.SubObjects.Add(new VirtualBlock(xPosition: x, zPosition: z, linkedFloorController: this, blockType: VirtualBlock.BlockTypes.Empty));
                    }
                }
            }

            if (offset.y > 0)
            {
                //Move up
                int index = 0;

                for (int x = 0; x < LinkedBuildingController.BlockGridSize.x; x++)
                {
                    for (int z = 0; z < LinkedBuildingController.BlockGridSize.y; z++)
                    {
                        if (z < offset.y)
                        {
                            //Add blocks
                            virtualBlocksParam.SubObjects.Insert(x * LinkedBuildingController.BlockGridSize.y, new VirtualBlock(xPosition: x, zPosition: z, linkedFloorController: this, blockType: VirtualBlock.BlockTypes.Empty));
                        }
                        else
                        {
                            //Modifiy position
                            VirtualBlock block = virtualBlocksParam.SubObjects[index] as VirtualBlock;
                            block.DefinePositionValue(new Vector2Int(x, z));
                        }

                        index++;
                    }

                    //Remove blocks
                    virtualBlocksParam.SubObjects.RemoveRange(index, offset.y);
                }
            }
            else if (offset.y < 0)
            {
                //Move down
                int offsetYAbs = -offset.y;
                int index = 0;

                for (int x = 0; x < LinkedBuildingController.BlockGridSize.x; x++)
                {
                    for (int z = 0; z < LinkedBuildingController.BlockGridSize.y; z++)
                    {
                        if (z < LinkedBuildingController.BlockGridSize.y - offsetYAbs)
                        {
                            //Modifiy position
                            /*
                            VirtualBlock block = virtualBlocksParam.SubObjects[index] as VirtualBlock;
                            block.DefinePositionValue(new Vector2Int(x, z));
                            Debug.Log("Defining block " + index + " at " + x + ", " + z);
                            */
                        }
                        else
                        {
                            //Add blocks
                            //int index = x * LinkedBuildingController.GridSize.y + z + offsetYAbs;
                            //Debug.Log("Inserting block " + x + ", " + z + " at index " + index);

                            virtualBlocksParam.SubObjects.Insert(x * LinkedBuildingController.BlockGridSize.y + z + offsetYAbs, new VirtualBlock(xPosition: x, zPosition: z, linkedFloorController: this, blockType: VirtualBlock.BlockTypes.Empty)); //X and Z positions not correct, reassigning below
                        }

                        index++;
                    }

                    //Remove blocks
                    virtualBlocksParam.SubObjects.RemoveRange(x * LinkedBuildingController.BlockGridSize.y, offsetYAbs);
                }

                UpdateMatrix(); //Resetting X Z references
            }

            //Move node walls
            CurrentNodeWallSystem.MoveAllNodeWalls(offset);

            //Move on floor objects
            for (int i = 0; i < onFloorObjectsParam.SubObjects.Count; i++) //Foreach does not work since it breaks when objects are removed
            {
                OnFloorObject currentOnFloorObject = onFloorObjectsParam.SubObjects[i] as OnFloorObject;

                if (currentOnFloorObject == null) continue;

                currentOnFloorObject.MoveOnGrid(offset: offset);

                if (currentOnFloorObject.Failed)
                {
                    currentOnFloorObject.DestroyObject();
                }
            }
        }

        public void ChangeBlockGrid(Vector2Int offset, Vector2Int oldGridSize)
        {
            //int currentGridSizeX = oldGridSize.x;

            //Remove X
            if (offset.x < 0)
            {
                //Remove blocks right
                int index = (oldGridSize.x + offset.x) * oldGridSize.y;
                int count = -oldGridSize.y * offset.x;

                virtualBlocksParam.SubObjects.RemoveRange(index: index, count: count);

                //currentGridSizeX += offset.x;
            }

            //Change Y
            if (offset.y > 0)
            {
                //Add blocks up
                for (int x = 0; x < oldGridSize.x; x++)
                {
                    for (int y = oldGridSize.y; y < LinkedBuildingController.BlockGridSize.y; y++)
                    {
                        int insertPosition = x * LinkedBuildingController.BlockGridSize.y + y;

                        if (x != oldGridSize.x - 1)
                        {
                            virtualBlocksParam.SubObjects.Insert(index: insertPosition, item: new VirtualBlock(xPosition: x, zPosition: y, linkedFloorController: this, blockType: VirtualBlock.BlockTypes.Empty));
                        }
                        else
                        {
                            virtualBlocksParam.SubObjects.Add(new VirtualBlock(xPosition: x, zPosition: y, linkedFloorController: this, blockType: VirtualBlock.BlockTypes.Empty));
                        }
                    }
                }
            }
            else if (offset.y < 0)
            {
                //Remove blocks up
                for (int i = 0; i < virtualBlocksParam.SubObjects.Count; i++)
                {
                    /*
                    int x = i / LinkedBuildingController.GridSize.x;
                    int z = i % LinkedBuildingController.GridSize.y;
                    */

                    VirtualBlock currentBlock = virtualBlocksParam.SubObjects[i] as VirtualBlock;

                    if (currentBlock.ZCoordinate >= LinkedBuildingController.BlockGridSize.y)
                    {
                        virtualBlocksParam.SubObjects.RemoveAt(i);
                        i--; //Check the same index again
                        continue;
                    }
                }
            }

            //Add blocks right
            if (offset.x > 0)
            {
                for (int x = oldGridSize.x; x < LinkedBuildingController.BlockGridSize.x; x++)
                {
                    for (int z = 0; z < LinkedBuildingController.BlockGridSize.y; z++)
                    {
                        virtualBlocksParam.SubObjects.Add(new VirtualBlock(xPosition: x, zPosition: z, linkedFloorController: this, blockType: VirtualBlock.BlockTypes.Empty));
                    }
                }

                //currentGridSizeX += offset.x;
            }

            UpdateMatrix();
        }

        //Visibility
        float wallHeightDisplayScaler = 1;

        //Update complete display

        //Above or below visibility
        public enum FloorVisibilityTypes
        {
            topDown,
            bottomUp,
        }

        FloorVisibilityTypes floorVisibilityType;

        public FloorVisibilityTypes FloorVisibilityType
        {
            set
            {
                //ToDo
                if (value == floorVisibilityType) return;

                floorVisibilityType = value;
                ApplyBuildParameters();
            }
            get
            {
                return floorVisibilityType;
            }
        }

        public override void Setup(IBaseObject superObject)
        {
            base.Setup(superObject);

            floorMeshBuilder = new BlockMeshBuilder(linkedFloor: this);

            buildingController = superObject as CastleController;

            SetupBuildParameters();

            //CurrentNodeWallSystem.Setup(linkedFloor: this);

            GridScaleOrganizer.Setup(linkedController: LinkedBuildingController);
            AddModificationNode(GridScaleOrganizer);
        }

        public void CompleteSetUpWithBuildParameters(CastleController buildingControler, VirtualBlock.BlockTypes blockType)
        {
            Setup(buildingControler);

            for (int xPos = 0; xPos < LinkedBuildingController.BlockGridSize.x; xPos++)
            {
                for (int zPos = 0; zPos < LinkedBuildingController.BlockGridSize.y; zPos++)
                {
                    VirtualBlock block = new VirtualBlock(xPosition: xPos, zPosition: zPos, linkedFloorController: this, blockType: blockType);
                    virtualBlocksParam.AddObject(block);
                }
            }

            NodeWallSystem nodeWallSystem = ResourceLibrary.TryGetBaseGameObjectFromStringIdentifier(nameof(NodeWallSystem), superObject: this) as NodeWallSystem;

            nodeWallSystemParam.SubObject = nodeWallSystem;

            //currentNodeWallSystem.Setup(superObject: this);

            onFloorObjectsParam.ClearAndDestroySubObjects();

            //CurrentNodeWallSystem.ResetSystem(); //Not needed, already done in setup
        }

        void UpdateMatrix()
        {
            int currentBlockIndex = 0;
            List<IBaseObject> blocks = virtualBlocksParam.SubObjects;

            FloorMatrix.Clear();

            for (int x = 0; x < buildingController.BlockGridSize.x; x++)
            {
                FloorMatrix.Add(new List<VirtualBlock>());
                for (int z = 0; z < buildingController.BlockGridSize.y; z++)
                {
                    VirtualBlock currentBlock = blocks[currentBlockIndex] as VirtualBlock;

                    FloorMatrix[FloorMatrix.Count - 1].Add(currentBlock);

                    currentBlock.DefinePositionValue(new Vector2Int(x, z));

                    currentBlockIndex++;
                }
            }
        }

        public void GenerateFloorMesh() //Public for faster Block wall builder
        {
            floorMeshBuilder.SetBlockInfo();

            List<VirtualBlock> allBlocks = AllBlocks;

            foreach (VirtualBlock block in allBlocks)
            {
                block.GenerateMailboxLinesBasedOnShapeInfo();
            }

            floorMeshBuilder.GenerateMeshesBasedOnInfo(optimize: LinkedBuildingController.optimizeMeshes);

            BuildAllMeshes();
        }

        public void RebuildBlockMeshes()
        {
            //System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            //watch.Start();
            //Debug.Log("Time taken for rebuild = " + watch.Elapsed.TotalSeconds * 1000 + "ms");

            floorMeshBuilder.GenerateMeshesBasedOnInfo(optimize: LinkedBuildingController.optimizeMeshes);
            //Debug.Log("Time taken for generating info = " + watch.ElapsedMilliseconds + "ms");

            //watch.Restart();

            BuildAllMeshes();

            //Debug.Log("Time taken for Building = " + watch.ElapsedMilliseconds + "ms");
        }

        float previousCompleteFloorHeight = 0;
        public override void ApplyBuildParameters()
        {
            if (CompleteFloorHeight != previousCompleteFloorHeight)
            {
                LinkedBuildingController.UpdateFloorPositions();
                previousCompleteFloorHeight = CompleteFloorHeight;
            }

            UpdateMatrix();

            switch (floorVisibilityType)
            {
                case FloorVisibilityTypes.topDown:
                    wallHeightDisplayScaler = buildingController.WallDisplayHeightScaler;
                    break;
                case FloorVisibilityTypes.bottomUp:
                    wallHeightDisplayScaler = buildingController.WallDisplayHeightScaler;
                    break;
                default:
                    Debug.LogWarning("Error when updating the floor visibility: FloorVisibilityType type not defined");
                    break;
            }

            //Generate floor mesh
            GenerateFloorMesh();

            //Ignore ApplyBuildParameters on NodeWallSystem since some SmartBlocks try to apply the build parameters to the NodeWallSystem
            CurrentNodeWallSystem.IgnoreApplyBuildParameters = true;

            //Update all smart blocks
            for (int i = 0; i < onFloorObjectsParam.SubObjects.Count; i++)
            {
                onFloorObjectsParam.SubObjects[i].ApplyBuildParameters();
            }

            CurrentNodeWallSystem.IgnoreApplyBuildParameters = false;

            CurrentNodeWallSystem.ApplyBuildParameters();
        }

        public override void InternalUpdate()
        {
            NonOrderedInternalUpdate();
        }

        public override void PlaytimeUpdate()
        {
            NonOrderedPlaytimeUpdate();
        }

        //Block selection
        public Vector2Int GetBlockCoordinateFromImpact(Vector3 impactPointAbsolute, Vector3 normal)
        {
            Vector3 BlockPointAbsolute = impactPointAbsolute - 0.5f * buildingController.BlockSize * normal.normalized;

            return GetBlockCoordinateFromCoordinateAbsolute(CoordinateAbsolute: BlockPointAbsolute);
        }

        public VirtualBlock GetBlockFromImpact(Vector3 impactPointAbsolute, Vector3 normal)
        {
            Vector3 BlockPointAbsolute = impactPointAbsolute - 0.5f * buildingController.BlockSize * normal.normalized;

            return GetBlockFromCoordinateAbsolute(CoordinateAbsolute: BlockPointAbsolute);
        }

        public VirtualBlock GetBlockFromCoordinateAbsolute(Vector3 CoordinateAbsolute)
        {
            Vector3 CoordinateRelative = transform.InverseTransformPoint(CoordinateAbsolute);

            return GetBlockFromCoordinateRelative(CoordinateRelative: CoordinateRelative);
        }

        public Vector2Int GetNodeCoordinateFromPositionAbsolute(Vector3 PositionAbsolute)
        {
            Vector3 CoordinateRelative = transform.InverseTransformPoint(PositionAbsolute);

            return GetNodeCoordinateFromPositionRelative(PositionRelative: CoordinateRelative);
        }

        public Vector2Int GetBlockCoordinateFromCoordinateAbsolute(Vector3 CoordinateAbsolute)
        {
            Vector3 CoordinateRelative = transform.InverseTransformPoint(CoordinateAbsolute);



            return GetBlockCoordinateFromCoordinateRelative(CoordinateRelative: CoordinateRelative);
        }

        public NodeWallNode GetNodeFromCoordinateAbsolute(Vector3 CoordinateAbsolute)
        {
            Vector3 CoordinateRelative = transform.InverseTransformPoint(CoordinateAbsolute);

            return GetNodeFromCoordinateRelative(CoordinateRelative);
        }

        public NodeWallNode GetNodeFromCoordinateRelative(Vector3 CoordinateRelative)
        {
            int xPos = Mathf.RoundToInt(CoordinateRelative.x / buildingController.BlockSize);
            int zPos = Mathf.RoundToInt(CoordinateRelative.z / buildingController.BlockSize);

            if(xPos > LinkedBuildingController.NodeGridSize.x - 1) xPos = LinkedBuildingController.NodeGridSize.x - 1;
            if(zPos > LinkedBuildingController.NodeGridSize.y - 1) zPos = LinkedBuildingController.NodeGridSize.y - 1;

            /*
            if (xPos > buildingController.GridSize.x || xPos < 0) return null;
            if (zPos > buildingController.GridSize.y || zPos < 0) return null;
            */

            return CurrentNodeWallSystem.NodeFromCoordinate(new Vector2Int(xPos, zPos));
        }

        public Vector3 GetLocalNodePositionFromNodeIndex(Vector2Int nodeIndex)
        {
            Vector3 returnValue = new Vector3(nodeIndex.x, 0, nodeIndex.y) * LinkedBuildingController.BlockSize;
            //Vector3 returnValue = new Vector3(-0.5f + blockIndex.x, 0, -0.5f + blockIndex.y) * LinkedBuildingController.BlockSize;

            return returnValue;
        }

        public Vector3 NodePositionFromBlockIndex(Vector2Int blockIndex)
        {
            Vector3 returnValue = new Vector3(blockIndex.x, 0, blockIndex.y) * LinkedBuildingController.BlockSize;

            return returnValue;
        }

        public Vector3 NodePositionFromBlockIndex(Vector2Int blockIndex, GridOrientation orientation)
        {
            switch (orientation.QuarterOrientation)
            {
                case GridOrientation.GridQuarterOrientations.XPosZPos:
                    return new Vector3(blockIndex.x, 0, blockIndex.y) * LinkedBuildingController.BlockSize;

                case GridOrientation.GridQuarterOrientations.XPosZNeg:
                    return new Vector3(blockIndex.x, 0, 1f + blockIndex.y) * LinkedBuildingController.BlockSize;

                case GridOrientation.GridQuarterOrientations.XNegZNeg:
                    return new Vector3(1f + blockIndex.x, 0, 1f + blockIndex.y) * LinkedBuildingController.BlockSize;

                case GridOrientation.GridQuarterOrientations.XNegZPos:
                    return new Vector3(1f + blockIndex.x, 0, blockIndex.y) * LinkedBuildingController.BlockSize;

                default:
                    Debug.LogWarning("Error: Orientation not defined");
                    return new Vector3(1f + blockIndex.x, 0, 1f + blockIndex.y) * LinkedBuildingController.BlockSize;
            }
        }

        public Vector3 CenterPositionFromBlockIndex(Vector2Int blockIndex)
        {
            Vector3 returnValue = new Vector3(blockIndex.x + 0.5f, 0, blockIndex.y + 0.5f) * LinkedBuildingController.BlockSize;

            return returnValue;
        }

        public Vector2Int GetBlockCoordinateFromCoordinateRelative(Vector3 CoordinateRelative)
        {
            int xPos = Mathf.RoundToInt(CoordinateRelative.x / buildingController.BlockSize - 0.5f);
            int zPos = Mathf.RoundToInt(CoordinateRelative.z / buildingController.BlockSize - 0.5f);

            return new Vector2Int(xPos, zPos);
        }

        public Vector2Int GetNodeCoordinateFromPositionRelative(Vector3 PositionRelative)
        {
            int xPos = Mathf.RoundToInt(PositionRelative.x / BlockSize);
            int zPos = Mathf.RoundToInt(PositionRelative.z / BlockSize);

            return new Vector2Int(xPos, zPos);
        }

        public VirtualBlock GetBlockFromCoordinateRelative(Vector3 CoordinateRelative)
        {
            int xPos = Mathf.RoundToInt(CoordinateRelative.x / buildingController.BlockSize - 0.5f);
            int zPos = Mathf.RoundToInt(CoordinateRelative.z / buildingController.BlockSize - 0.5f);

            /*
            if (xPos > buildingController.GridSize.x || xPos < 0) return null;
            if (zPos > buildingController.GridSize.y || zPos < 0) return null;
            */

            return BlockAtPosition(xPos: xPos, zPos: zPos);
        }

        public void AddOnFloorObject(OnFloorObject newObject)
        {
            onFloorObjectsParam.AddObject(newObject);
        }
    }
}