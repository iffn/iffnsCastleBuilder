using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class GridScaleNode : ModificationNode
    {
        //HumanBuildingController linkedController;
        DirectionTypes directionType;
        GridScalerOrganizer linkedOrganizer;

        public bool modificationInProgress = false;
        int newTempGridPosition;

        public int TempGridPosition
        {
            get
            {
                return newTempGridPosition;
            }
        }

        public enum DirectionTypes
        {
            xPos,
            xNeg,
            zPos,
            zNeg
        }

        HumanBuildingController linkedBuilding;

        public void Setup(HumanBuildingController linkedBuilding, GridScalerOrganizer organizer, DirectionTypes directionType)
        {
            base.setup(linkedOrganizer: organizer);

            this.linkedBuilding = linkedBuilding;

            this.directionType = directionType;
            this.linkedOrganizer = organizer;
        }

        public override void UpdatePosition()
        {
            float length;
            float width = linkedBuilding.BlockSize / 3;
            float height;

            if (modificationInProgress)
            {
                height = linkedBuilding.CurrentFloorObject.CompleteFloorHeight + heightOvershoot;
            }
            else
            {
                height = linkedBuilding.CurrentFloorObject.BottomFloorHeight + heightOvershoot;
            }



            if (directionType == DirectionTypes.xPos || directionType == DirectionTypes.xNeg)
            {
                if (linkedOrganizer.zPosNode.TempGridPosition != 0)
                {
                    length = linkedBuilding.BlockSize * (linkedBuilding.GridSize.y + linkedOrganizer.zPosNode.TempGridPosition);
                }
                else
                {
                    length = linkedBuilding.BlockSize * (linkedBuilding.GridSize.y + linkedOrganizer.zNegNode.TempGridPosition);
                }
            }
            else
            {
                if (linkedOrganizer.xPosNode.TempGridPosition != 0)
                {
                    length = linkedBuilding.BlockSize * (linkedBuilding.GridSize.x + linkedOrganizer.xPosNode.TempGridPosition);
                }
                else
                {
                    length = linkedBuilding.BlockSize * (linkedBuilding.GridSize.x + linkedOrganizer.xNegNode.TempGridPosition);
                }
            }

            transform.localScale = new Vector3(length - width, height, width);
            //Debug.Log(directionType.ToString() + ": " + length + " with " + linkedController.GridSize);

            switch (directionType)
            {
                case DirectionTypes.xPos:
                    transform.localPosition = new Vector3((linkedBuilding.GridSize.x + newTempGridPosition) * linkedBuilding.BlockSize, height / 2, length / 2 - linkedOrganizer.zNegNode.TempGridPosition * linkedBuilding.BlockSize);
                    transform.localRotation = Quaternion.Euler(Vector3.up * 90);
                    break;
                case DirectionTypes.xNeg:
                    transform.localPosition = new Vector3(-newTempGridPosition * linkedBuilding.BlockSize, height / 2, length / 2 - linkedOrganizer.zNegNode.TempGridPosition * linkedBuilding.BlockSize);
                    transform.localRotation = Quaternion.Euler(Vector3.up * 270);
                    break;
                case DirectionTypes.zPos:
                    transform.localPosition = new Vector3(length / 2 - linkedOrganizer.xNegNode.TempGridPosition * linkedBuilding.BlockSize, height / 2, (linkedBuilding.GridSize.y + newTempGridPosition) * linkedBuilding.BlockSize);
                    transform.localRotation = Quaternion.Euler(Vector3.up * 0);
                    break;
                case DirectionTypes.zNeg:
                    transform.localPosition = new Vector3(length / 2 - linkedOrganizer.xNegNode.TempGridPosition * linkedBuilding.BlockSize, height / 2, -newTempGridPosition * linkedBuilding.BlockSize);
                    transform.localRotation = Quaternion.Euler(Vector3.up * 180);
                    break;
                default:
                    Debug.LogWarning("Error: Direction type not defined");
                    break;
            }

            //transform.localPosition += new Vector3(-linkedController.BlockSize / 2, 0, -linkedController.BlockSize /2);
        }


        public Vector2Int NewTempGridPosition
        {
            set
            {
                //Vector3 localPosition = linkedController.transform.InverseTransformDirection(value);
                //Vector2Int gridPosition = linkedController.CurrentFloorObject.GetBlockFromCoordinateRelative(localPosition).Position;
                Vector2Int gridPosition = value;

                //Debug.Log(gridPosition);

                switch (directionType)
                {
                    case DirectionTypes.xPos:
                        newTempGridPosition = gridPosition.x - linkedBuilding.GridSize.x;
                        break;
                    case DirectionTypes.xNeg:
                        newTempGridPosition = -gridPosition.x;
                        break;
                    case DirectionTypes.zPos:
                        newTempGridPosition = gridPosition.y - linkedBuilding.GridSize.y;
                        break;
                    case DirectionTypes.zNeg:
                        newTempGridPosition = -gridPosition.y;
                        break;
                    default:
                        Debug.LogWarning("Error: Direction type not defined");
                        break;
                }

                //Debug.Log(linkedController.GridSize);

                //Debug.Log(directionType.ToString() + "->" + newTempGridPosition);

                //linkedOrganizer.UpdatePosition();
            }
        }

        public void ApplyNewGridPosition()
        {
            modificationInProgress = false;

            switch (directionType)
            {
                case DirectionTypes.xPos:
                    linkedBuilding.ChangeGridXPos(newTempGridPosition);
                    break;
                case DirectionTypes.xNeg:
                    linkedBuilding.ChangeGridXNeg(newTempGridPosition);
                    break;
                case DirectionTypes.zPos:
                    linkedBuilding.ChangeGridZPos(newTempGridPosition);
                    break;
                case DirectionTypes.zNeg:
                    linkedBuilding.ChangeGridZNeg(newTempGridPosition);
                    break;
                default:
                    break;
            }

            newTempGridPosition = 0;

            //linkedOrganizer.UpdatePosition();

        }

        public override bool ColliderActivationState
        {
            set
            {
                transform.GetComponent<Collider>().enabled = value;
            }
        }
    }
}