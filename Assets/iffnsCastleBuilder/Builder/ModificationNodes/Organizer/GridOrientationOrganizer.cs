using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class GridOrientationOrganizer : ModificationOrganizer
    {
        readonly BlockGridPositionModificationNode positionNode;
        readonly GridOrientationNode orientationNode;
        readonly OnFloorObject linkedObject;

        float BlockSize
        {
            get
            {
                return linkedObject.LinkedFloor.LinkedBuildingController.BlockSize;
            }
        }

        public override void SetLinkedObjectPositionAndOrientation(bool raiseToFloor)
        {
            bool firstOnGrid = linkedObject.LinkedFloor.LinkedBuildingController.BlockCoordinateIsOnGrid(positionNode.AbsoluteCoordinate);

            if (!(firstOnGrid))
            {
                linkedObject.Failed = true;
                return;
            }

            linkedObject.transform.localPosition = linkedObject.LinkedFloor.NodePositionFromBlockIndex(blockIndex: positionNode.AbsoluteCoordinate, orientation: orientationNode.CurrentOrientation);
            if (raiseToFloor) linkedObject.transform.localPosition += Vector3.up * linkedObject.LinkedFloor.BottomFloorHeight;

            switch (orientationNode.CurrentOrientation.ForwardOrientation)
            {
                case GridOrientation.GridForwardOrientations.ZPositive:
                    linkedObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    break;
                case GridOrientation.GridForwardOrientations.XPositive:
                    linkedObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
                    break;
                case GridOrientation.GridForwardOrientations.ZNegative:
                    linkedObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
                    break;
                case GridOrientation.GridForwardOrientations.XNegative:
                    linkedObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 270, 0));
                    break;
                default:
                    Debug.LogWarning("Error with Switch: Undefined orientation " + orientationNode.CurrentOrientation.ToString());
                    break;
            }
        }

        public override void MoveOnGrid(Vector2Int offset)
        {
            Vector2Int tempNodePosition = positionNode.AbsoluteCoordinate + offset;

            //Note: On grid error check is part of apply build parameters

            positionNode.AbsoluteCoordinate = tempNodePosition;
        }

        public GridOrientationOrganizer(OnFloorObject linkedObject, BlockGridPositionModificationNode positionNode, GridOrientationNode orientationNode)
        {
            this.linkedObject = linkedObject;
            this.positionNode = positionNode;
            this.orientationNode = orientationNode;

            linkedObject.HideModificationNodes();
        }

        public override Vector3 FirstBuildPositionAbsolute
        {
            set
            {
                positionNode.AbsoluteCoordinate = linkedObject.LinkedFloor.GetBlockCoordinateFromCoordinateAbsolute(value);
            }
        }

        public override Vector3 SecondBuildPositionAbsolute
        {
            set
            {
                Vector2Int orientationLocation = linkedObject.LinkedFloor.GetBlockCoordinateFromCoordinateAbsolute(value);

                Vector2Int offset = orientationLocation - positionNode.AbsoluteCoordinate;

                if (Mathf.Abs(offset.x) - Mathf.Abs(offset.y) > 0)
                {
                    if (offset.x >= 0)
                    {
                        orientationNode.CurrentOrientation = new GridOrientation(forwardOrientation: GridOrientation.GridForwardOrientations.XPositive);
                    }
                    else
                    {
                        orientationNode.CurrentOrientation = new GridOrientation(forwardOrientation: GridOrientation.GridForwardOrientations.XNegative);
                    }
                }
                else
                {
                    if (offset.y >= 0)
                    {
                        orientationNode.CurrentOrientation = new GridOrientation(forwardOrientation: GridOrientation.GridForwardOrientations.ZPositive);
                    }
                    else
                    {
                        orientationNode.CurrentOrientation = new GridOrientation(forwardOrientation: GridOrientation.GridForwardOrientations.ZNegative);
                    }
                }
            }
        }
    }
}