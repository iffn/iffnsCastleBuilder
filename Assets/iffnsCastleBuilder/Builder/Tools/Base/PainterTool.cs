using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using iffnsStuff.iffnsBaseSystemForUnity;
using iffnsStuff.iffnsUnityResources;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class PainterTool : MonoBehaviour
    {
        [SerializeField] TexturingUI LinkedTexturingUI;

        public MaterialManager currentMaterial;

        [SerializeField] CastleBuilderController CurrentBuilderController;
        [SerializeField] RTSController currentRTSController;
        [SerializeField] BuildingToolController LinkedBuildingToolController;

        VirtualBlock startBlock;
        VirtualBlock endBlock;

        bool found = false;

        ToolType previousToolType = ToolType.Painter;

        ToolType currentToolType = ToolType.Painter;
        public ToolType CurrentToolType
        {
            get
            {
                return currentToolType;
            }
            set
            {
                if (value == ToolType.Pipette)
                {
                    previousToolType = currentToolType;
                }
                else
                {
                    previousToolType = value;
                }

                currentToolType = value;
            }
        }

        public enum ToolType
        {
            Painter,
            PaintRectangle,
            Pipette
        }


        CastleController CurrentBuilding
        {
            get
            {
                return CurrentBuilderController.CurrentBuilding;
            }
        }

        // Start is called before the first frame update
        public void Setup()
        {
            LinkedTexturingUI.Setup();
        }

        MeshManager GetMeshManagerFromClick()
        {
            if (EventSystem.current.IsPointerOverGameObject()) //if over UI button
            {
                //Do nothing
                return null;
            }

            Ray ray = currentRTSController.GetRayFromCameraMouseLocation();

            RaycastHit hit;

            Vector3 direction = ray.direction;
            Vector3 originAbsolute = ray.origin;

            Physics.Raycast(ray, out hit, Mathf.Infinity);

            if (hit.collider == null) return null;

            GameObject clickedOnObject = hit.collider.transform.gameObject;

            if (clickedOnObject == null) return null;

            MeshManager currentManager = clickedOnObject.transform.GetComponent<MeshManager>();

            return currentManager;
        }

        RaycastHit GetHitFromClick()
        {
            Ray ray = currentRTSController.GetRayFromCameraMouseLocation();

            /*
            Vector3 direction = ray.direction;
            Vector3 originAbsolute = ray.origin;
            */

            Physics.Raycast(ray: ray, hitInfo: out RaycastHit hit, maxDistance: Mathf.Infinity, layerMask: WorldController.LayerManager.SelectionLayerMask);

            return hit;
        }

        MeshManager GetMeshManagerFromHit(RaycastHit hit)
        {
            if (hit.collider == null) return null;

            GameObject clickedOnObject = hit.collider.transform.gameObject;

            if (clickedOnObject == null) return null;

            MeshManager currentManager = clickedOnObject.transform.GetComponent<MeshManager>();

            return currentManager;
        }

        MailboxLineMaterial GetMaterialLineFromMeshManager(MeshManager currentManager, RaycastHit hit)
        {
            MailboxLineMaterial materialLine = currentManager.CurrentMaterialReference;

            FloorController currentFloorController = currentManager.LinkedMainObject as FloorController;

            if (materialLine == null)
            {
                if (currentFloorController != null)
                {
                    VirtualBlock block = currentFloorController.GetBlockFromImpact(impactPointAbsolute: hit.point, normal: hit.normal);

                    if (block == null) return null;

                    Vector3 hitDirection = hit.normal;

                    Quaternion floorRotation = currentManager.LinkedMainObject.transform.rotation;

                    if (MathHelper.FloatIsZero(Vector3.Angle(hitDirection, floorRotation * Vector3.up)))
                    {
                        materialLine = block.FloorMaterialParam;
                    }
                    else if (MathHelper.FloatIsZero(Vector3.Angle(hitDirection, floorRotation * Vector3.down)))
                    {
                        materialLine = block.CeilingMaterialParam;
                    }
                    else if (MathHelper.FloatIsZero(Vector3.Angle(hitDirection, floorRotation * Vector3.forward)))
                    {
                        materialLine = block.FrontWallMaterialParam;
                    }
                    else if (MathHelper.FloatIsZero(Vector3.Angle(hitDirection, floorRotation * Vector3.back)))
                    {
                        materialLine = block.BackWallMaterialParam;
                    }
                    else if (MathHelper.FloatIsZero(Vector3.Angle(hitDirection, floorRotation * Vector3.left)))
                    {
                        materialLine = block.LeftWallMaterialParam;
                    }
                    else if (MathHelper.FloatIsZero(Vector3.Angle(hitDirection, floorRotation * Vector3.right)))
                    {
                        materialLine = block.RightWallMaterialParam;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            return materialLine;
        }

        void PainterUpdate()
        {
            if (Input.GetMouseButton(0))
            {
                if (EventSystem.current.IsPointerOverGameObject()) //if over UI button
                {
                    //Do nothing
                    return;
                }

                RaycastHit hit = GetHitFromClick();

                MeshManager currentManager = GetMeshManagerFromHit(hit: hit);

                if (currentManager == null) return;

                MailboxLineMaterial materialLine = GetMaterialLineFromMeshManager(currentManager: currentManager, hit: hit);

                FloorController currentFloorController = currentManager.LinkedMainObject as FloorController;

                if (materialLine == null) return;

                if (materialLine.Val == currentMaterial) return;

                materialLine.Val = currentMaterial;

                if (currentFloorController != null)
                {
                    currentFloorController.RebuildBlockMeshes();
                }
                else
                {
                    currentManager.LinkedMainObject.ApplyBuildParameters();
                }
            }
        }

        void PaintRectangleUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject()) //if over UI button
                {
                    //Do nothing
                    return;
                }

                RaycastHit hit = GetHitFromClick();

                MeshManager currentManager = GetMeshManagerFromHit(hit: hit);

                if (currentManager == null) return;

                FloorController currentFloorController = currentManager.LinkedMainObject as FloorController;

                if (currentFloorController == null)
                {
                    return;
                }

                startBlock = currentFloorController.GetBlockFromImpact(impactPointAbsolute: hit.point, normal: hit.normal);
                endBlock = startBlock;
                LinkedBuildingToolController.CurrentWallBuilderTool.SetCardinalPreviewBlock(firstBlock: startBlock, secondBlock: endBlock);

            }
            else if (Input.GetMouseButton(0))
            {

                if (Input.GetKeyDown(key: KeyCode.Escape))
                {
                    Clear();
                    return;
                }

                if (startBlock == null) return;

                LinkedBuildingToolController.CurrentWallBuilderTool.SetCardinalPreviewBlock(firstBlock: startBlock, secondBlock: endBlock);

                if (EventSystem.current.IsPointerOverGameObject()) //if over UI button
                {
                    //Do nothing
                    return;
                }

                RaycastHit hit = GetHitFromClick();

                MeshManager currentManager = GetMeshManagerFromHit(hit: hit);

                if (currentManager == null) return;

                FloorController currentFloorController = currentManager.LinkedMainObject as FloorController;

                if (currentFloorController == null)
                {
                    return;
                }

                endBlock = currentFloorController.GetBlockFromImpact(impactPointAbsolute: hit.point, normal: hit.normal);

            }

            if (Input.GetMouseButtonUp(0))
            {
                if (startBlock == null) return;

                //Paint all blocks
                int xMin = Mathf.Min(startBlock.XCoordinate, endBlock.XCoordinate);
                int xMax = Mathf.Max(startBlock.XCoordinate, endBlock.XCoordinate);
                int zMin = Mathf.Min(startBlock.ZCoordinate, endBlock.ZCoordinate);
                int zMax = Mathf.Max(startBlock.ZCoordinate, endBlock.ZCoordinate);

                FloorController currentFloorController = startBlock.LinkedFloorController;

                for (int x = xMin; x <= xMax; x++)
                {
                    for (int z = zMin; z <= zMax; z++)
                    {
                        VirtualBlock currentBlock = currentFloorController.BlockAtPosition(xPos: x, zPos: z);
                        if (currentBlock.FloorMaterialParam != null)
                        {
                            switch (CurrentBuilderController.CurrentBuildingToolController.CurrentNavigationTools.ViewDirection)
                            {
                                case CastleController.FloorViewDirectionType.topDown:
                                    currentBlock.FloorMaterialParam.Val = currentMaterial;
                                    break;
                                case CastleController.FloorViewDirectionType.bottomUp:
                                    currentBlock.CeilingMaterialParam.Val = currentMaterial;
                                    break;
                                case CastleController.FloorViewDirectionType.complete:
                                    currentBlock.FloorMaterialParam.Val = currentMaterial;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }

                currentFloorController.RebuildBlockMeshes();

                Clear();
            }

            void Clear()
            {
                startBlock = null;
                endBlock = null;
                //ClearCardinalPreviewBlock
                LinkedBuildingToolController.CurrentWallBuilderTool.ClearCardinalPreviewBlock();
            }
        }

        void PipetteUpdate()
        {
            if (Input.GetMouseButton(0))
            {
                if (EventSystem.current.IsPointerOverGameObject()) //if over UI button
                {
                    //Do nothing
                    return;
                }

                RaycastHit hit = GetHitFromClick();

                MeshManager currentManager = GetMeshManagerFromHit(hit: hit);

                if (currentManager == null) return;

                MailboxLineMaterial materialLine = GetMaterialLineFromMeshManager(currentManager: currentManager, hit: hit);

                FloorController currentFloorController = currentManager.LinkedMainObject as FloorController;

                if (materialLine == null) return;

                LinkedTexturingUI.SetMaterial(materialLine.Val);

                found = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (found)
                {
                    found = false;
                    CurrentToolType = previousToolType;
                    
                    LinkedTexturingUI.SetToolType(previousToolType);
                }
            }
        }



        private void Update()
        {
            switch (CurrentToolType)
            {
                case ToolType.Painter:
                    PainterUpdate();
                    break;
                case ToolType.PaintRectangle:
                    PaintRectangleUpdate();
                    break;
                case ToolType.Pipette:
                    PipetteUpdate();
                    break;
                default:
                    break;
            }
        }



        VirtualBlock GetBlockFromClick(bool OnlyCheckCurrentFloor)
        {
            return SmartBlockBuilderTool.GetBlockFromClick(OnlyCheckCurrentFloor: OnlyCheckCurrentFloor, currentBuilding: CurrentBuilding, currentRTSCamera: currentRTSController);
        }
    }
}