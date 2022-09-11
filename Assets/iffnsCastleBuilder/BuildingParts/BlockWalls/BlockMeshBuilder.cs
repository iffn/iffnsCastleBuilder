using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class BlockMeshBuilder
    {
        //public BlockMeshDirectionHolder Wall;
        //public BlockMeshDirectionHolder Floor;

        public FloorController LinkedFloor;

        public BlockMeshBuilder(FloorController linkedFloor)
        {
            LinkedFloor = linkedFloor;
        }

        public float BlockSize
        {
            get
            {
                return LinkedFloor.BlockSize;
            }
        }

        public void SetBlockInfo()
        {
            for (int xPos = 0; xPos < LinkedFloor.LinkedBuildingController.BlockGridSize.x; xPos++)
            {
                for (int zPos = 0; zPos < LinkedFloor.LinkedBuildingController.BlockGridSize.y; zPos++)
                {
                    VirtualBlock currentBlock = LinkedFloor.BlockAtPosition(xPos: xPos, zPos: zPos);

                    if (currentBlock.BlockType == VirtualBlock.BlockTypes.Empty)
                    {
                        currentBlock.CurrentShapeInfo.HasFloorAndCeiling = false;
                        continue;
                    }

                    currentBlock.CurrentShapeInfo.HasFloorAndCeiling = true;

                    //Left wall
                    if (xPos == 0)
                    {
                        switch (currentBlock.BlockType)
                        {
                            case VirtualBlock.BlockTypes.Floor:
                                currentBlock.CurrentShapeInfo.LeftWallType = VirtualBlock.ShapeInfo.WallTypes.Floor;
                                break;
                            case VirtualBlock.BlockTypes.Wall:
                                currentBlock.CurrentShapeInfo.LeftWallType = VirtualBlock.ShapeInfo.WallTypes.WallFull;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        VirtualBlock leftBlock = LinkedFloor.BlockAtPosition(xPos: xPos - 1, zPos: zPos);

                        AssignWallTypeBasedOnNeigbor(currentBlock: currentBlock, nextBlock: leftBlock, wallDirection: FloorController.BlockDirections.Left);
                    }

                    //Right wall
                    if (xPos == LinkedFloor.LinkedBuildingController.BlockGridSize.x - 1)
                    {
                        switch (currentBlock.BlockType)
                        {
                            case VirtualBlock.BlockTypes.Floor:
                                currentBlock.CurrentShapeInfo.RightWallType = VirtualBlock.ShapeInfo.WallTypes.Floor;
                                break;
                            case VirtualBlock.BlockTypes.Wall:
                                currentBlock.CurrentShapeInfo.RightWallType = VirtualBlock.ShapeInfo.WallTypes.WallFull;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        VirtualBlock rightBlock = LinkedFloor.BlockAtPosition(xPos: xPos + 1, zPos: zPos);

                        AssignWallTypeBasedOnNeigbor(currentBlock: currentBlock, nextBlock: rightBlock, wallDirection: FloorController.BlockDirections.Right);
                    }

                    //Back wall
                    if (zPos == 0)
                    {
                        switch (currentBlock.BlockType)
                        {
                            case VirtualBlock.BlockTypes.Floor:
                                currentBlock.CurrentShapeInfo.BackWallType = VirtualBlock.ShapeInfo.WallTypes.Floor;
                                break;
                            case VirtualBlock.BlockTypes.Wall:
                                currentBlock.CurrentShapeInfo.BackWallType = VirtualBlock.ShapeInfo.WallTypes.WallFull;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        VirtualBlock backBlock = LinkedFloor.BlockAtPosition(xPos: xPos, zPos: zPos - 1);

                        AssignWallTypeBasedOnNeigbor(currentBlock: currentBlock, nextBlock: backBlock, wallDirection: FloorController.BlockDirections.Back);
                    }

                    //Front wall
                    if (zPos == LinkedFloor.LinkedBuildingController.BlockGridSize.y - 1)
                    {
                        switch (currentBlock.BlockType)
                        {
                            case VirtualBlock.BlockTypes.Floor:
                                currentBlock.CurrentShapeInfo.FrontWallType = VirtualBlock.ShapeInfo.WallTypes.Floor;
                                break;
                            case VirtualBlock.BlockTypes.Wall:
                                currentBlock.CurrentShapeInfo.FrontWallType = VirtualBlock.ShapeInfo.WallTypes.WallFull;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        VirtualBlock frontBlock = LinkedFloor.BlockAtPosition(xPos: xPos, zPos: zPos + 1);

                        AssignWallTypeBasedOnNeigbor(currentBlock: currentBlock, nextBlock: frontBlock, wallDirection: FloorController.BlockDirections.Font);
                    }

                }
            }

            void AssignWallTypeBasedOnNeigbor(VirtualBlock currentBlock, VirtualBlock nextBlock, FloorController.BlockDirections wallDirection)
            {
                switch (currentBlock.BlockType)
                {
                    case VirtualBlock.BlockTypes.Floor:
                        switch (nextBlock.BlockType)
                        {
                            case VirtualBlock.BlockTypes.Empty:
                                AssignWallBasedOnDirection(currentBlock: currentBlock, wallDirection: wallDirection, newWallType: VirtualBlock.ShapeInfo.WallTypes.Floor);
                                break;
                            case VirtualBlock.BlockTypes.Floor:
                                AssignWallBasedOnDirection(currentBlock: currentBlock, wallDirection: wallDirection, newWallType: VirtualBlock.ShapeInfo.WallTypes.None);
                                break;
                            case VirtualBlock.BlockTypes.Wall:
                                AssignWallBasedOnDirection(currentBlock: currentBlock, wallDirection: wallDirection, newWallType: VirtualBlock.ShapeInfo.WallTypes.None);
                                break;
                            default:
                                break;
                        }
                        break;
                    case VirtualBlock.BlockTypes.Wall:
                        switch (nextBlock.BlockType)
                        {
                            case VirtualBlock.BlockTypes.Empty:
                                AssignWallBasedOnDirection(currentBlock: currentBlock, wallDirection: wallDirection, newWallType: VirtualBlock.ShapeInfo.WallTypes.WallFull);
                                break;
                            case VirtualBlock.BlockTypes.Floor:
                                AssignWallBasedOnDirection(currentBlock: currentBlock, wallDirection: wallDirection, newWallType: VirtualBlock.ShapeInfo.WallTypes.WallCutoff);
                                break;
                            case VirtualBlock.BlockTypes.Wall:
                                AssignWallBasedOnDirection(currentBlock: currentBlock, wallDirection: wallDirection, newWallType: VirtualBlock.ShapeInfo.WallTypes.None);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }

                void AssignWallBasedOnDirection(VirtualBlock currentBlock, FloorController.BlockDirections wallDirection, VirtualBlock.ShapeInfo.WallTypes newWallType)
                {
                    switch (wallDirection)
                    {
                        case FloorController.BlockDirections.Font:
                            currentBlock.CurrentShapeInfo.FrontWallType = newWallType;
                            break;
                        case FloorController.BlockDirections.Back:
                            currentBlock.CurrentShapeInfo.BackWallType = newWallType;
                            break;
                        case FloorController.BlockDirections.Left:
                            currentBlock.CurrentShapeInfo.LeftWallType = newWallType;
                            break;
                        case FloorController.BlockDirections.Right:
                            currentBlock.CurrentShapeInfo.RightWallType = newWallType;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public void GenerateMeshesBasedOnInfo(bool optimize)
        {
            if (optimize)
            {
                GenerateCapBasedOnInfoWithOptimization(CapType.ceiling);
                GenerateCapBasedOnInfoWithOptimization(CapType.topFloor);
                GenerateCapBasedOnInfoWithOptimization(CapType.topWall);

                GenerateWallsBasedOnInfoWithOptimization(WallDirections.Front);
                GenerateWallsBasedOnInfoWithOptimization(WallDirections.Back);
                GenerateWallsBasedOnInfoWithOptimization(WallDirections.Left);
                GenerateWallsBasedOnInfoWithOptimization(WallDirections.Right);
            }
            else
            {
                GenerateCapBasedOnInfoWithoutOptimization();
                GenerateWallsBasedOnInfoWithoutOptimization();
            }

            LinkedFloor.UpdateAllUVs();
        }

        enum CapType
        {
            ceiling,
            topFloor,
            topWall
        }

        void GenerateCapBasedOnInfoWithOptimization(CapType capType)
        {
            //Using greedy meshing: https://www.youtube.com/watch?v=L6P86i5O9iU

            Material GetMaterial(int xPos, int zPos)
            {
                switch (capType)
                {
                    case CapType.ceiling:
                        return LinkedFloor.BlockAtPosition(xPos: xPos, zPos: zPos).CeilingMaterial;
                    case CapType.topFloor:
                        return LinkedFloor.BlockAtPosition(xPos: xPos, zPos: zPos).FloorMaterial;
                    case CapType.topWall:
                        return LinkedFloor.BlockAtPosition(xPos: xPos, zPos: zPos).FloorMaterial;
                    default:
                        Debug.LogWarning($"Error: Enum state {capType} not defined in {nameof(CapType)} inside BlockMeshBuilder");
                        return LinkedFloor.BlockAtPosition(xPos: xPos, zPos: zPos).FloorMaterial;
                }
            }

            Vector2Int BlockGridSize = LinkedFloor.LinkedBuildingController.BlockGridSize;

            Vector2Int[,] blockSizes = new Vector2Int[BlockGridSize.x, BlockGridSize.y];

            //Fill out 1,1 table
            for (int xPos = 0; xPos < BlockGridSize.x; xPos++)
            {
                for (int zPos = 0; zPos < BlockGridSize.y; zPos++)
                {
                    VirtualBlock currentBlock = LinkedFloor.BlockAtPosition(xPos: xPos, zPos: zPos);

                    bool hasNoSurface = currentBlock.BlockType == VirtualBlock.BlockTypes.Empty
                    || capType == CapType.topFloor && currentBlock.BlockType == VirtualBlock.BlockTypes.Wall
                    || capType == CapType.topWall && currentBlock.BlockType == VirtualBlock.BlockTypes.Floor;

                    if (hasNoSurface)
                    {
                        blockSizes[xPos, zPos] = Vector2Int.zero;
                    }
                    else
                    {
                        blockSizes[xPos, zPos] = Vector2Int.one;
                    } 
                }
            }

            //Merge blocks into lines
            for (int xPos = 0; xPos < BlockGridSize.x; xPos++)
            {
                int previousZPos = -1;

                for (int zPos = 0; zPos < BlockGridSize.y; zPos++)
                {
                    //If empty:
                    if (blockSizes[xPos, zPos].x == 0)
                    {
                        previousZPos = -1;
                        continue;
                    }

                    //If previously empty
                    if (previousZPos == -1)
                    {
                        previousZPos = zPos;
                        continue;
                    }

                    //Merge if same material
                    if(GetMaterial(xPos, zPos) == GetMaterial(xPos, previousZPos))
                    {
                        blockSizes[xPos, previousZPos] = blockSizes[xPos, previousZPos] + new Vector2Int(0, 1);
                        blockSizes[xPos, zPos] = Vector2Int.zero;
                    }
                    else
                    {
                        previousZPos = zPos;
                    }
                    
                }
            }

            
            //Merge lines into rectangles
            for (int zPos = 0; zPos < BlockGridSize.y; zPos++)
            {
                int previousXPos = -1;

                for (int xPos = 0; xPos < BlockGridSize.x; xPos++)
                {
                    //If empty:
                    if (blockSizes[xPos, zPos].x == 0)
                    {
                        previousXPos = -1;
                        continue;
                    }

                    //If previously empty
                    if (previousXPos == -1)
                    {
                        previousXPos = xPos;
                        continue;
                    }

                    //If same length and material, merge
                    if(blockSizes[previousXPos, zPos].y == blockSizes[xPos, zPos].y
                        && GetMaterial(xPos, zPos) == GetMaterial(previousXPos, zPos))
                    {
                        blockSizes[previousXPos, zPos] = blockSizes[previousXPos, zPos] + new Vector2Int(1, 0);
                        blockSizes[xPos, zPos] = Vector2Int.zero;
                    }
                    else
                    {
                        previousXPos = xPos;
                    }
                }
            }

            //Build mesh
            for (int xPos = 0; xPos < BlockGridSize.x; xPos++)
            {
                for (int zPos = 0; zPos < BlockGridSize.y; zPos++)
                {
                    Vector2Int currentSize = blockSizes[xPos, zPos];

                    if (currentSize.x == 0 && currentSize.y == 0) continue;

                    float xSize = BlockSize * currentSize.x;
                    float zSize = BlockSize * currentSize.y;

                    TriangleMeshInfo rectangle = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.right * xSize, secondLine: Vector3.forward *zSize, uvOffset: new Vector2(xPos, zPos) * BlockSize);
                    rectangle.Move(LinkedFloor.GetLocalNodePositionFromNodeIndex(new Vector2Int(xPos, zPos)));

                    rectangle.AlternativeMaterial = GetMaterial(xPos: xPos, zPos: zPos);

                    switch (capType)
                    {
                        case CapType.ceiling:
                            rectangle.FlipTriangles();
                            break;
                        case CapType.topFloor:
                            rectangle.Move(Vector3.up * LinkedFloor.BottomFloorHeight);
                            break;
                        case CapType.topWall:
                            rectangle.Move(Vector3.up * LinkedFloor.WallHeightWithScaler);
                            break;
                        default:
                            break;
                    }

                    LinkedFloor.AddStaticMesh(rectangle);
                }
            }
        }

        void GenerateCapBasedOnInfoWithoutOptimization()
        {
            for (int xPos = 0; xPos < LinkedFloor.LinkedBuildingController.BlockGridSize.x; xPos++)
            {
                for (int zPos = 0; zPos < LinkedFloor.LinkedBuildingController.BlockGridSize.y; zPos++)
                {
                    VirtualBlock currentBlock = LinkedFloor.BlockAtPosition(xPos: xPos, zPos: zPos);

                    if (!currentBlock.CurrentShapeInfo.HasFloorAndCeiling) continue;

                    //Add Ceiling
                    TriangleMeshInfo floor = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.right * LinkedFloor.BlockSize, secondLine: Vector3.forward * LinkedFloor.BlockSize, uvOffset: new Vector2(xPos, zPos) * BlockSize);
                    floor.Move(LinkedFloor.GetLocalNodePositionFromNodeIndex(new Vector2Int(xPos, zPos)));
                    TriangleMeshInfo ceiling = floor.CloneFlipped;

                    if (currentBlock.BlockType == VirtualBlock.BlockTypes.Wall)
                    {
                        floor.Move(Vector3.up * LinkedFloor.WallHeightWithScaler);
                    }
                    else
                    {
                        floor.Move(Vector3.up * LinkedFloor.BottomFloorHeight);
                    }

                    floor.AlternativeMaterial = currentBlock.FloorMaterial;
                    ceiling.AlternativeMaterial = currentBlock.CeilingMaterial;

                    LinkedFloor.AddStaticMesh(floor);
                    LinkedFloor.AddStaticMesh(ceiling);

                }
            }
        }

        enum WallDirections
        {
            Front,
            Back,
            Left,
            Right
        }

        void GenerateWallsBasedOnInfoWithOptimization(WallDirections wallDirection)
        {
            void EvaluateBlock(VirtualBlock currentBlock, ref VirtualBlock.ShapeInfo.WallTypes previousWallType, ref int blocks, ref VirtualBlock startingBlock)
            {
                

                //Left wall
                VirtualBlock.ShapeInfo.WallTypes currentWallType;

                switch (wallDirection)
                {
                    case WallDirections.Front:
                        currentWallType = currentBlock.CurrentShapeInfo.FrontWallType;
                        break;
                    case WallDirections.Back:
                        currentWallType = currentBlock.CurrentShapeInfo.BackWallType;
                        break;
                    case WallDirections.Left:
                        currentWallType = currentBlock.CurrentShapeInfo.LeftWallType;
                        break;
                    case WallDirections.Right:
                        currentWallType = currentBlock.CurrentShapeInfo.RightWallType;
                        break;
                    default:
                        currentWallType = currentBlock.CurrentShapeInfo.FrontWallType;
                        break;
                }

                bool sameMaterial = false;

                if (startingBlock != null)
                {
                    switch (wallDirection)
                    {
                        case WallDirections.Front:
                            sameMaterial = currentWallType == previousWallType && currentBlock.FrontWallMaterial == startingBlock.FrontWallMaterial;
                            break;
                        case WallDirections.Back:
                            sameMaterial = currentWallType == previousWallType && currentBlock.BackWallMaterial == startingBlock.BackWallMaterial;
                            break;
                        case WallDirections.Left:
                            sameMaterial = currentWallType == previousWallType && currentBlock.LeftWallMaterial == startingBlock.LeftWallMaterial;
                            break;
                        case WallDirections.Right:
                            sameMaterial = currentWallType == previousWallType && currentBlock.RightWallMaterial == startingBlock.RightWallMaterial;
                            break;
                        default:
                            break;
                    }
                }

                if (sameMaterial)
                {
                    blocks++;
                }
                else
                {
                    //Build wall if needed
                    if (previousWallType != VirtualBlock.ShapeInfo.WallTypes.None)
                    {
                        BuildAndAddWall(startingBlock: startingBlock, blocks: blocks, wallDirection: wallDirection);
                    }

                    previousWallType = currentWallType;
                    startingBlock = currentBlock;

                    if (currentWallType == VirtualBlock.ShapeInfo.WallTypes.None)
                    {
                        blocks = 0;
                    }
                    else
                    {
                        blocks = 1;
                    }
                }
            }

            if(wallDirection == WallDirections.Left || wallDirection == WallDirections.Right)
            {
                //left right walls
                for (int xPos = 0; xPos < LinkedFloor.LinkedBuildingController.BlockGridSize.x; xPos++)
                {
                    VirtualBlock.ShapeInfo.WallTypes previousWallType = VirtualBlock.ShapeInfo.WallTypes.None;
                    int blocks = 0;
                    VirtualBlock startingBlock = null;

                    for (int zPos = 0; zPos < LinkedFloor.LinkedBuildingController.BlockGridSize.y; zPos++)
                    {
                        VirtualBlock currentBlock = LinkedFloor.BlockAtPosition(xPos: xPos, zPos: zPos);

                        EvaluateBlock(currentBlock: currentBlock, previousWallType: ref previousWallType, blocks: ref blocks, startingBlock: ref startingBlock);
                    }

                    //build final wall if needed
                    if (previousWallType != VirtualBlock.ShapeInfo.WallTypes.None)
                    {
                        BuildAndAddWall(startingBlock: startingBlock, blocks: blocks, wallDirection: wallDirection);
                    }
                }
            }
            else
            {
                //left front and back walls
                for (int zPos = 0; zPos < LinkedFloor.LinkedBuildingController.BlockGridSize.y; zPos++)
                {
                    VirtualBlock.ShapeInfo.WallTypes previousWallType = VirtualBlock.ShapeInfo.WallTypes.None;
                    int blocks = 0;
                    VirtualBlock startingBlock = null;

                    for (int xPos = 0; xPos < LinkedFloor.LinkedBuildingController.BlockGridSize.x; xPos++)
                    {
                        VirtualBlock currentBlock = LinkedFloor.BlockAtPosition(xPos: xPos, zPos: zPos);

                        EvaluateBlock(currentBlock: currentBlock, previousWallType: ref previousWallType, blocks: ref blocks, startingBlock: ref startingBlock);
                    }

                    //build final wall if needed
                    if (previousWallType != VirtualBlock.ShapeInfo.WallTypes.None)
                    {
                        BuildAndAddWall(startingBlock: startingBlock, blocks: blocks, wallDirection: wallDirection);
                    }
                }
            }

            

            void BuildAndAddWall(VirtualBlock startingBlock, int blocks, WallDirections wallDirection)
            {
                Vector2 HeightAndHeightOffset;

                switch (wallDirection)
                {
                    case WallDirections.Front:
                        HeightAndHeightOffset = GetHeightAndHeightOffset(wallType: startingBlock.CurrentShapeInfo.FrontWallType);
                        break;
                    case WallDirections.Back:
                        HeightAndHeightOffset = GetHeightAndHeightOffset(wallType: startingBlock.CurrentShapeInfo.BackWallType);
                        break;
                    case WallDirections.Left:
                        HeightAndHeightOffset = GetHeightAndHeightOffset(wallType: startingBlock.CurrentShapeInfo.LeftWallType);
                        break;
                    case WallDirections.Right:
                        HeightAndHeightOffset = GetHeightAndHeightOffset(wallType: startingBlock.CurrentShapeInfo.RightWallType);
                        break;
                    default:
                        Debug.Log("Error: Enum state missing");
                        HeightAndHeightOffset = Vector2.one;
                        break;
                }

                float height = HeightAndHeightOffset.x;
                float heightOffset = HeightAndHeightOffset.y;

                Vector3 wallVector;
                bool flipNormals = false;
                Material wallMaterial;

                switch (wallDirection)
                {
                    case WallDirections.Front:
                        wallVector = Vector3.right;
                        wallMaterial = startingBlock.FrontWallMaterial;
                        flipNormals = true;
                        break;
                    case WallDirections.Back:
                        flipNormals = false;
                        wallVector = Vector3.right;
                        wallMaterial = startingBlock.BackWallMaterial;
                        break;
                    case WallDirections.Left:
                        wallVector = Vector3.forward;
                        flipNormals = true;
                        wallMaterial = startingBlock.LeftWallMaterial;
                        break;
                    case WallDirections.Right:
                        wallVector = Vector3.forward;
                        wallMaterial = startingBlock.RightWallMaterial;
                        break;
                    default:
                        wallVector = Vector3.forward;
                        wallMaterial = null;
                        break;
                }

                TriangleMeshInfo currentInfo = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: wallVector * LinkedFloor.BlockSize * blocks, secondLine: Vector3.up * height, uvOffset: new Vector2(startingBlock.ZCoordinate, 0) * BlockSize);
                currentInfo.Move(Vector3.up * heightOffset);
                if(flipNormals) currentInfo.FlipTriangles();

                GridOrientation.GridQuarterOrientations orientation;

                switch (wallDirection)
                {
                    case WallDirections.Front:
                        orientation = GridOrientation.GridQuarterOrientations.XPosZNeg;
                        break;
                    case WallDirections.Back:
                        orientation = GridOrientation.GridQuarterOrientations.XPosZPos;
                        break;
                    case WallDirections.Left:
                        orientation = GridOrientation.GridQuarterOrientations.XPosZPos;
                        break;
                    case WallDirections.Right:
                        orientation = GridOrientation.GridQuarterOrientations.XNegZPos;
                        break;
                    default:
                        orientation = GridOrientation.GridQuarterOrientations.XPosZNeg;
                        break;
                }

                currentInfo.Move(LinkedFloor.NodePositionFromBlockIndex(blockIndex: new Vector2Int(startingBlock.XCoordinate, startingBlock.ZCoordinate), orientation: new GridOrientation(orientation)));
                currentInfo.AlternativeMaterial = wallMaterial;
                LinkedFloor.AddStaticMesh(currentInfo);
            }

            Vector2 GetHeightAndHeightOffset(VirtualBlock.ShapeInfo.WallTypes wallType)
            {
                float height = 0;
                float heightOffset = 0;

                switch (wallType)
                {
                    case VirtualBlock.ShapeInfo.WallTypes.Floor:
                        height = LinkedFloor.BottomFloorHeight;
                        heightOffset = 0;
                        break;
                    case VirtualBlock.ShapeInfo.WallTypes.WallFull:
                        height = LinkedFloor.WallHeightWithScaler;
                        heightOffset = 0;
                        break;
                    case VirtualBlock.ShapeInfo.WallTypes.WallCutoff:
                        height = LinkedFloor.WallBetweenHeightWithScaler;
                        heightOffset = LinkedFloor.BottomFloorHeight;
                        break;
                    default:
                        break;
                }

                return new Vector2(height, heightOffset);
            }
        }

        void GenerateWallsBasedOnInfoWithoutOptimization()
        {
            //left right walls
            for (int xPos = 0; xPos < LinkedFloor.LinkedBuildingController.BlockGridSize.x; xPos++)
            {
                for (int zPos = 0; zPos < LinkedFloor.LinkedBuildingController.BlockGridSize.y; zPos++)
                {
                    VirtualBlock currentBlock = LinkedFloor.BlockAtPosition(xPos: xPos, zPos: zPos);

                    float heightOffset;
                    float height;

                    //Left wall
                    if (currentBlock.CurrentShapeInfo.LeftWallType != VirtualBlock.ShapeInfo.WallTypes.None)
                    {
                        Vector2 HeightAndHeightOffset = GetHeightAndHeightOffset(wallType: currentBlock.CurrentShapeInfo.LeftWallType);

                        height = HeightAndHeightOffset.x;
                        heightOffset = HeightAndHeightOffset.y;

                        TriangleMeshInfo currentInfo = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.forward * LinkedFloor.BlockSize, secondLine: Vector3.up * height, uvOffset: new Vector2(zPos, 0) * BlockSize);
                        currentInfo.Move(Vector3.up * heightOffset);
                        currentInfo.FlipTriangles();
                        currentInfo.Move(LinkedFloor.NodePositionFromBlockIndex(blockIndex: new Vector2Int(xPos, zPos), orientation: new GridOrientation(GridOrientation.GridQuarterOrientations.XPosZPos)));
                        currentInfo.AlternativeMaterial = currentBlock.LeftWallMaterial;
                        LinkedFloor.AddStaticMesh(currentInfo);
                    }

                    //Right wall
                    if (currentBlock.CurrentShapeInfo.RightWallType != VirtualBlock.ShapeInfo.WallTypes.None)
                    {
                        Vector2 HeightAndHeightOffset = GetHeightAndHeightOffset(wallType: currentBlock.CurrentShapeInfo.RightWallType);

                        height = HeightAndHeightOffset.x;
                        heightOffset = HeightAndHeightOffset.y;

                        TriangleMeshInfo currentInfo = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.forward * LinkedFloor.BlockSize, secondLine: Vector3.up * height, uvOffset: new Vector2(zPos, 0) * BlockSize);
                        currentInfo.Move(Vector3.up * heightOffset);
                        currentInfo.Move(LinkedFloor.NodePositionFromBlockIndex(blockIndex: new Vector2Int(xPos, zPos), orientation: new GridOrientation(GridOrientation.GridQuarterOrientations.XNegZPos)));
                        currentInfo.AlternativeMaterial = currentBlock.RightWallMaterial;
                        LinkedFloor.AddStaticMesh(currentInfo);
                    }
                }
            }

            //front back walls
            for (int zPos = 0; zPos < LinkedFloor.LinkedBuildingController.BlockGridSize.y; zPos++)
            {
                for (int xPos = 0; xPos < LinkedFloor.LinkedBuildingController.BlockGridSize.x; xPos++)
                {
                    VirtualBlock currentBlock = LinkedFloor.BlockAtPosition(xPos: xPos, zPos: zPos);

                    float heightOffset;
                    float height;

                    //Front wall
                    if (currentBlock.CurrentShapeInfo.BackWallType != VirtualBlock.ShapeInfo.WallTypes.None)
                    {
                        Vector2 HeightAndHeightOffset = GetHeightAndHeightOffset(wallType: currentBlock.CurrentShapeInfo.BackWallType);

                        height = HeightAndHeightOffset.x;
                        heightOffset = HeightAndHeightOffset.y;

                        TriangleMeshInfo currentInfo = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.right * LinkedFloor.BlockSize, secondLine: Vector3.up * height, uvOffset: new Vector2(zPos, 0) * BlockSize);
                        currentInfo.Move(Vector3.up * heightOffset);
                        currentInfo.Move(LinkedFloor.NodePositionFromBlockIndex(blockIndex: new Vector2Int(xPos, zPos), orientation: new GridOrientation(GridOrientation.GridQuarterOrientations.XPosZPos)));
                        currentInfo.AlternativeMaterial = currentBlock.BackWallMaterial;
                        LinkedFloor.AddStaticMesh(currentInfo);
                    }

                    //Back wall
                    if (currentBlock.CurrentShapeInfo.FrontWallType != VirtualBlock.ShapeInfo.WallTypes.None)
                    {
                        Vector2 HeightAndHeightOffset = GetHeightAndHeightOffset(wallType: currentBlock.CurrentShapeInfo.FrontWallType);

                        height = HeightAndHeightOffset.x;
                        heightOffset = HeightAndHeightOffset.y;

                        TriangleMeshInfo currentInfo = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.right * LinkedFloor.BlockSize, secondLine: Vector3.up * height, uvOffset: new Vector2(zPos, 0) * BlockSize);
                        currentInfo.Move(Vector3.up * heightOffset);
                        currentInfo.FlipTriangles();
                        currentInfo.Move(LinkedFloor.NodePositionFromBlockIndex(blockIndex: new Vector2Int(xPos, zPos), orientation: new GridOrientation(GridOrientation.GridQuarterOrientations.XPosZNeg)));
                        currentInfo.AlternativeMaterial = currentBlock.FrontWallMaterial;
                        LinkedFloor.AddStaticMesh(currentInfo);
                    }
                }
            }

            Vector2 GetHeightAndHeightOffset(VirtualBlock.ShapeInfo.WallTypes wallType)
            {
                float height = 0;
                float heightOffset = 0;

                switch (wallType)
                {
                    case VirtualBlock.ShapeInfo.WallTypes.Floor:
                        height = LinkedFloor.BottomFloorHeight;
                        heightOffset = 0;
                        break;
                    case VirtualBlock.ShapeInfo.WallTypes.WallFull:
                        height = LinkedFloor.WallHeightWithScaler;
                        heightOffset = 0;
                        break;
                    case VirtualBlock.ShapeInfo.WallTypes.WallCutoff:
                        height = LinkedFloor.WallBetweenHeightWithScaler;
                        heightOffset = LinkedFloor.BottomFloorHeight;
                        break;
                    default:
                        break;
                }

                return new Vector2(height, heightOffset);
            }
        }
    }
}