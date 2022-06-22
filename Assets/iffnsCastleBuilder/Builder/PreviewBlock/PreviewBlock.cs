using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class PreviewBlock : MonoBehaviour
    {
        public void SetCardinalPreviewBlock(VirtualBlock firstBlock, VirtualBlock secondBlock, CastleController currentBuilding)
        {
            /*
            float xSize = (Mathf.Abs(firstBlock.XPosition - secondBlock.XPosition) + 1) * currentBuilding.BlockSize + 0.05f;
            float zSize = (Mathf.Abs(firstBlock.ZPosition - secondBlock.ZPosition) + 1) * currentBuilding.BlockSize + 0.05f;
            float ySize = currentBuilding.CurrentFloorObject.CompleteFloorHeight + 0.05f;

            float xPos = (0f + firstBlock.XPosition + secondBlock.XPosition) / 2 * currentBuilding.BlockSize;
            float zPos = (0f + firstBlock.ZPosition + secondBlock.ZPosition) / 2 * currentBuilding.BlockSize;
            float yPos = currentBuilding.CurrentFloorObject.transform.position.y + ySize / 2;

            transform.position = transform.rotation * new Vector3(xPos, yPos, zPos);
            transform.localScale = new Vector3(xSize, ySize, zSize);
            */

            Vector3 firstCenterPosition = currentBuilding.CurrentFloorObject.CenterPositionFromBlockIndex(firstBlock.Coordinate);
            Vector3 secondCenterPosition = currentBuilding.CurrentFloorObject.CenterPositionFromBlockIndex(secondBlock.Coordinate);

            Vector3 localPosition = (firstCenterPosition + secondCenterPosition) * 0.5f + Vector3.up * currentBuilding.CurrentFloorObject.CompleteFloorHeight * 0.5f;

            transform.rotation = currentBuilding.transform.rotation;
            transform.position = transform.rotation * localPosition + currentBuilding.CurrentFloorObject.transform.position;

            transform.localScale = new Vector3(
                Mathf.Abs(secondCenterPosition.x - firstCenterPosition.x) + currentBuilding.BlockSize,
                currentBuilding.CurrentFloorObject.CompleteFloorHeight,
                Mathf.Abs(secondCenterPosition.z - firstCenterPosition.z) + currentBuilding.BlockSize
                );
        }

        public void SetWallPreviewBlock(NodeWallNode firstNode, NodeWallNode secondNode)
        {
            transform.parent = firstNode.LinkedSystem.transform;

            transform.localPosition = firstNode.LocalPosition3D;

            transform.LookAt(transform.parent.position + transform.parent.rotation * secondNode.LocalPosition3D); /*firstNode.LinkedNodeWallSystem.transform.rotation*/

            transform.Translate(Vector3.forward * (secondNode.LocalPosition3D - firstNode.LocalPosition3D).magnitude / 2);
            //transform.localPosition = (firstNode.LocalPosition3D + secondNode.LocalPosition3D) / 2;

            transform.localScale = new Vector3(firstNode.LinkedSystem.WallThickness, firstNode.LinkedSystem.LinkedFloor.CompleteFloorHeight, (secondNode.LocalPosition3D - firstNode.LocalPosition3D).magnitude);

            transform.Translate(Vector3.up * transform.localScale.y / 2);
        }

        public void Clear()
        {
            Destroy(transform.gameObject);
        }
    }
}