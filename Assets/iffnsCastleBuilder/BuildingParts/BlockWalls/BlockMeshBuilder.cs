using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMeshBuilder
{
    //public BlockMeshDirectionHolder Wall;
    //public BlockMeshDirectionHolder Floor;

    public FloorController LinkedFloor;

    public BlockMeshBuilder(FloorController linkedFloor)
    {
        this.LinkedFloor = linkedFloor;
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
        for (int xPos = 0; xPos < LinkedFloor.LinkedBuildingController.GridSize.x; xPos++)
        {
            for (int zPos = 0; zPos < LinkedFloor.LinkedBuildingController.GridSize.y; zPos++)
            {
                VirtualBlock currentBlock = LinkedFloor.BlockAtPosition(xPos: xPos, zPos: zPos);

                if(currentBlock.BlockType == VirtualBlock.BlockTypes.Empty)
                {
                    currentBlock.CurrentShapeInfo.HasFloorAndCeiling = false;
                    continue;
                }

                currentBlock.CurrentShapeInfo.HasFloorAndCeiling = true;

                //Left wall
                if(xPos == 0)
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
                if (xPos == LinkedFloor.LinkedBuildingController.GridSize.x - 1)
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
                if (zPos == LinkedFloor.LinkedBuildingController.GridSize.y - 1)
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


    public void GenerateMeshesBasedOnInfo()
    {
        GenerateCapBasedOnInfo();
        GenerateWallsBasedOnInfo();

        LinkedFloor.UpdateAllCradinalUVs();
    }

    void GenerateCapBasedOnInfo()
    {
        for (int xPos = 0; xPos < LinkedFloor.LinkedBuildingController.GridSize.x; xPos++)
        {
            for (int zPos = 0; zPos < LinkedFloor.LinkedBuildingController.GridSize.y; zPos++)
            {
                VirtualBlock currentBlock = LinkedFloor.BlockAtPosition(xPos: xPos, zPos: zPos);

                if (!currentBlock.CurrentShapeInfo.HasFloorAndCeiling) continue;

                //Add Ceiling
                TriangleMeshInfo floor = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.right * LinkedFloor.BlockSize, secondLine: Vector3.forward * LinkedFloor.BlockSize, UVOffset: new Vector2(xPos, zPos) * BlockSize);
                floor.Move(LinkedFloor.GetLocalNodePositionFromNodeIndex(new Vector2Int(xPos, zPos)));
                TriangleMeshInfo ceiling = floor.CloneFlipped;

                if(currentBlock.BlockType == VirtualBlock.BlockTypes.Wall)
                {
                    floor.Move(Vector3.up * LinkedFloor.CompleteFloorHeight);
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

    void GenerateWallsBasedOnInfo()
    {
        //left right walls
        for (int xPos = 0; xPos < LinkedFloor.LinkedBuildingController.GridSize.x; xPos++)
        {
            for (int zPos = 0; zPos < LinkedFloor.LinkedBuildingController.GridSize.y; zPos++)
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

                    TriangleMeshInfo currentInfo = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.forward * LinkedFloor.BlockSize, secondLine: Vector3.up * height, UVOffset: new Vector2(zPos, 0) * BlockSize);
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

                    TriangleMeshInfo currentInfo = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.forward * LinkedFloor.BlockSize, secondLine: Vector3.up * height, UVOffset: new Vector2(zPos, 0) * BlockSize);
                    currentInfo.Move(Vector3.up * heightOffset);
                    currentInfo.Move(LinkedFloor.NodePositionFromBlockIndex(blockIndex: new Vector2Int(xPos, zPos), orientation: new GridOrientation(GridOrientation.GridQuarterOrientations.XNegZPos)));
                    currentInfo.AlternativeMaterial = currentBlock.RightWallMaterial;
                    LinkedFloor.AddStaticMesh(currentInfo);
                }
            }
        }

        //front back walls
        for (int zPos = 0; zPos < LinkedFloor.LinkedBuildingController.GridSize.y; zPos++)
        {
            for (int xPos = 0; xPos < LinkedFloor.LinkedBuildingController.GridSize.x; xPos++)
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

                    TriangleMeshInfo currentInfo = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.right * LinkedFloor.BlockSize, secondLine: Vector3.up * height, UVOffset: new Vector2(zPos, 0) * BlockSize);
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

                    TriangleMeshInfo currentInfo = MeshGenerator.FilledShapes.RectangleAtCorner(baseLine: Vector3.right * LinkedFloor.BlockSize, secondLine: Vector3.up * height, UVOffset: new Vector2(zPos, 0) * BlockSize);
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
                    height = LinkedFloor.CompleteFloorHeight;
                    heightOffset = 0;
                    break;
                case VirtualBlock.ShapeInfo.WallTypes.WallCutoff:
                    height = LinkedFloor.WallBetweenHeight;
                    heightOffset = LinkedFloor.BottomFloorHeight;
                    break;
                default:
                    break;
            }

            return new Vector2(height, heightOffset);
        }
    }
}
