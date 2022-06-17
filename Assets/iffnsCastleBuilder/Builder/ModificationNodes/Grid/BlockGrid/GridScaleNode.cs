using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class GridScaleNode : ModificationNode
    {
        HumanBuildingController linkedController;
        DirectionTypes directionType;
        GridScalerOrganizer organizer;

        public bool modificationInProgress = false;
        int newTempGridPosition;

        public int tempGridPosition
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

        public void Setup(HumanBuildingController linkedController, GridScalerOrganizer organizer, DirectionTypes directionType)
        {
            base.setup(linkedObject: linkedController);

            this.linkedController = linkedController;
            this.directionType = directionType;
            this.organizer = organizer;
        }

        public override void UpdatePosition()
        {
            float length;
            float width = linkedController.BlockSize / 3;
            float height;

            if (modificationInProgress)
            {
                height = linkedController.CurrentFloorObject.CompleteFloorHeight + heightOvershoot;
            }
            else
            {
                height = linkedController.CurrentFloorObject.BottomFloorHeight + heightOvershoot;
            }



            if (directionType == DirectionTypes.xPos || directionType == DirectionTypes.xNeg)
            {
                if (organizer.zPosNode.tempGridPosition != 0)
                {
                    length = linkedController.BlockSize * (linkedController.BlockGridSize.y + organizer.zPosNode.tempGridPosition);
                }
                else
                {
                    length = linkedController.BlockSize * (linkedController.BlockGridSize.y + organizer.zNegNode.tempGridPosition);
                }
            }
            else
            {
                if (organizer.xPosNode.tempGridPosition != 0)
                {
                    length = linkedController.BlockSize * (linkedController.BlockGridSize.x + organizer.xPosNode.tempGridPosition);
                }
                else
                {
                    length = linkedController.BlockSize * (linkedController.BlockGridSize.x + organizer.xNegNode.tempGridPosition);
                }
            }

            transform.localScale = new Vector3(length - width, height, width);
            //Debug.Log(directionType.ToString() + ": " + length + " with " + linkedController.GridSize);

            switch (directionType)
            {
                case DirectionTypes.xPos:
                    transform.localPosition = new Vector3((linkedController.BlockGridSize.x + newTempGridPosition) * linkedController.BlockSize, height / 2, length / 2 - organizer.zNegNode.tempGridPosition * linkedController.BlockSize);
                    transform.localRotation = Quaternion.Euler(Vector3.up * 90);
                    break;
                case DirectionTypes.xNeg:
                    transform.localPosition = new Vector3(-newTempGridPosition * linkedController.BlockSize, height / 2, length / 2 - organizer.zNegNode.tempGridPosition * linkedController.BlockSize);
                    transform.localRotation = Quaternion.Euler(Vector3.up * 270);
                    break;
                case DirectionTypes.zPos:
                    transform.localPosition = new Vector3(length / 2 - organizer.xNegNode.tempGridPosition * linkedController.BlockSize, height / 2, (linkedController.BlockGridSize.y + newTempGridPosition) * linkedController.BlockSize);
                    transform.localRotation = Quaternion.Euler(Vector3.up * 0);
                    break;
                case DirectionTypes.zNeg:
                    transform.localPosition = new Vector3(length / 2 - organizer.xNegNode.tempGridPosition * linkedController.BlockSize, height / 2, -newTempGridPosition * linkedController.BlockSize);
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
                        newTempGridPosition = gridPosition.x - linkedController.BlockGridSize.x;
                        break;
                    case DirectionTypes.xNeg:
                        newTempGridPosition = -gridPosition.x;
                        break;
                    case DirectionTypes.zPos:
                        newTempGridPosition = gridPosition.y - linkedController.BlockGridSize.y;
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

                organizer.UpdatePosition();
            }
        }

        public void ApplyNewGridPosition()
        {
            modificationInProgress = false;

            switch (directionType)
            {
                case DirectionTypes.xPos:
                    linkedController.ChangeGridXPos(newTempGridPosition);
                    break;
                case DirectionTypes.xNeg:
                    linkedController.ChangeGridXNeg(newTempGridPosition);
                    break;
                case DirectionTypes.zPos:
                    linkedController.ChangeGridZPos(newTempGridPosition);
                    break;
                case DirectionTypes.zNeg:
                    linkedController.ChangeGridZNeg(newTempGridPosition);
                    break;
                default:
                    break;
            }

            linkedController.DestroyFailedSubObjects();

            newTempGridPosition = 0;

            organizer.UpdatePosition();
        }

        protected override bool NodeColliderState
        {
            set
            {
                transform.GetComponent<Collider>().enabled = value;
            }
        }
    }
}