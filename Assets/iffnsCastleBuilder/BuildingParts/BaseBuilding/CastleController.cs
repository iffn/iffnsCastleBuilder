﻿using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class CastleController : BaseGameObject
    {
        //Settings
        public bool optimizeMeshes = false;

        bool allowGridScaleModification = true;
        public bool AllowGridScaleModification
        {
            get
            {
                return allowGridScaleModification;
            }
        }

        //Build parameters
        MailboxLineDistinctUnnamed negativeFloorParam;
        MailboxLineVector2Int gridSizeParam;
        Vector2Int previousGridSize;
        MailboxLineMultipleSubObject floorsParam;
        readonly List<FloorController> Floors = new List<FloorController>();

        public List<FloorController> CurrentListOfFloors
        {
            get
            {
                return new List<FloorController>(Floors);
            }
        }

        int currentFloorNumber = 0;
        public int PositiveFloors
        {
            get
            {
                return MathHelper.ClampIntMin(value: NumberOfFloors - NegativeFloors - 1, min: 0); //ToDo: During loading, the NumberOfFloors is smaller than NegativeFloors since the last floor is loaded first. This can return a negative number
            }
        }

        public int NegativeFloors
        {
            get
            {
                if (negativeFloorParam == null)
                {
                    Debug.LogWarning("Error, negative floor numbers is not yet set up");
                    return 0;
                }

                return negativeFloorParam.Val;
            }
        }

        public int NumberOfFloors
        {
            get
            {
                return floorsParam.NumberOfObjects;
            }
        }

        public Vector2Int NodeGridSize
        {
            get
            {
                return BlockGridSize + Vector2Int.one;
            }
            set
            {
                BlockGridSize = value - Vector2Int.one;
            }
        }

        public Vector2Int BlockGridSize
        {
            get
            {
                return new Vector2Int(gridSizeParam.Val.x, gridSizeParam.Val.y);
            }
            set
            {
                gridSizeParam.Val = previousGridSize;

                if (value.x != previousGridSize.x)
                {
                    //previousGridSize.x = value.x;
                    ChangeGridXPos(offset: value.x - gridSizeParam.Val.x);

                }

                if (value.y != previousGridSize.y)
                {
                    //previousGridSize.y = value.y;
                    ChangeGridZPos(offset: value.y - gridSizeParam.Val.y);
                }

                DestroyFailedSubObjects();
            }
        }

        public bool BlockCoordinateIsOnGrid(Vector2Int coordinate)
        {
            bool x = coordinate.x >= 0 && coordinate.x < BlockGridSize.x;
            bool y = coordinate.y >= 0 && coordinate.y < BlockGridSize.y;

            return x && y;
        }

        public bool NodeCoordinateIsOnGrid(Vector2Int coordinate)
        {
            bool x = coordinate.x >= 0 && coordinate.x < NodeGridSize.x;
            bool y = coordinate.y >= 0 && coordinate.y < NodeGridSize.y;

            return x && y;
        }

        //Grid size editing
        public void ChangeGridXPos(int offset)
        {
            if (offset > 0)
            {
                //Expand right
                ChangeBlockGridOnly(new Vector2Int(offset, 0));
            }
            else if (offset < 0)
            {
                //Retract right
                int fixedOffset = offset;
                if (BlockGridSize.x + offset < 1) fixedOffset = -BlockGridSize.x + 1; //Set minimum grid size to 1
                ChangeBlockGridOnly(new Vector2Int(fixedOffset, 0));
            }

            ApplyBuildParameters();
        }

        public void ChangeGridXNeg(int offset)
        {
            //Move stuff
            if (offset > 0)
            {
                //Expand left
                ChangeBlockGridOnly(new Vector2Int(offset, 0));

                MoveStuffOnGrid(new Vector2Int(offset, 0));
            }
            else if (offset < 0)
            {
                //Retract left
                if (BlockGridSize.x + offset < 1) offset = -BlockGridSize.x + 1; //Set minimum grid size to 1

                MoveStuffOnGrid(new Vector2Int(offset, 0));

                ChangeBlockGridOnly(new Vector2Int(offset, 0));
            }

            MoveBuilding(new Vector2(-offset * BlockSize, 0));

            ApplyBuildParameters();
        }

        public void ChangeGridZPos(int offset)
        {
            if (offset > 0)
            {
                //Expand up
                ChangeBlockGridOnly(new Vector2Int(0, offset));
            }
            else if (offset < 0)
            {
                //Retract up
                if (BlockGridSize.y + offset < 1) offset = -BlockGridSize.y + 1; //Set minimum grid size to 1

                ChangeBlockGridOnly(new Vector2Int(0, offset));
            }

            ApplyBuildParameters();
        }

        public void ChangeGridZNeg(int offset)
        {
            //Move stuff
            if (offset > 0)
            {
                //Expand down
                ChangeBlockGridOnly(new Vector2Int(0, offset));

                MoveStuffOnGrid(new Vector2Int(0, offset));
            }
            else if (offset < 0)
            {
                //Retract down
                if (BlockGridSize.y + offset < 1) offset = -BlockGridSize.y + 1; //Set minimum grid size to 1

                MoveStuffOnGrid(new Vector2Int(0, offset));

                ChangeBlockGridOnly(new Vector2Int(0, offset));
            }

            MoveBuilding(new Vector2(0, -offset * BlockSize));

            ApplyBuildParameters();
        }

        void MoveStuffOnGrid(Vector2Int offset)
        {
            foreach (FloorController floor in Floors)
            {
                floor.MoveStuffOnGrid(offset);
            }
        }

        void ChangeBlockGridOnly(Vector2Int offset)
        {

            Vector2Int oldGridSize = gridSizeParam.Val;

            gridSizeParam.Val = new Vector2Int(gridSizeParam.Val.x + offset.x, gridSizeParam.Val.y + offset.y);
            previousGridSize = new Vector2Int(gridSizeParam.Val.x, gridSizeParam.Val.y);

            //Change block grid
            foreach (FloorController floor in Floors)
            {
                floor.ChangeBlockGrid(offset: offset, oldGridSize: oldGridSize);
                floor.CurrentNodeWallSystem.MoveAllNodeWalls(Vector2Int.zero); //Remove node walls that are outside of the grid
            }

        }


        void MoveBuilding(Vector2 offset)
        {
            transform.Translate(translation: new Vector3(offset.x, 0, offset.y), relativeTo: Space.Self);
        }

        //Fixed parameters
        static readonly float blockSize = 1f / 3f;
        public float BlockSize
        {
            get
            {
                return blockSize;
            }
        }

        public override void Setup(IBaseObject superObject)
        {
            base.Setup(superObject);

            negativeFloorParam = new MailboxLineDistinctUnnamed("Negative floors", CurrentMailbox, Mailbox.ValueType.buildParameter, 10, 0, 0);
            gridSizeParam = new MailboxLineVector2Int(name: "Grid size", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            floorsParam = new MailboxLineMultipleSubObject("Floors", objectHolder: CurrentMailbox);
        }

        public void CompleteSetUpWithBuildParameters(IBaseObject superObject, Vector2Int gridSize, int negativeFloors, int totalFloors)
        {
            if (totalFloors < 0) totalFloors = 1;
            negativeFloors = Mathf.Abs(negativeFloors);
            if (negativeFloors > totalFloors) negativeFloors = 0;

            Setup(superObject: superObject);

            negativeFloorParam.Val = negativeFloors;
            gridSizeParam.Val = gridSize;
            previousGridSize = gridSize;

            for (int i = 0; i < totalFloors; i++)
            {
                FloorController floorTemplate = ResourceLibrary.TryGetTemplateFromStringIdentifier(identifier: nameof(FloorController)) as FloorController;

                if (floorTemplate == null) Debug.LogWarning("Error");

                FloorController floor = Instantiate(floorTemplate);

                if (floor == null) Debug.LogWarning("Error");

                VirtualBlock.BlockTypes blockType;

                if (i - negativeFloors == 0)
                {
                    blockType = VirtualBlock.BlockTypes.Floor;
                }
                else if (i - negativeFloors < 0)
                {
                    blockType = VirtualBlock.BlockTypes.Wall;
                }
                else
                {
                    blockType = VirtualBlock.BlockTypes.Empty;
                }

                floor.CompleteSetUpWithBuildParameters(buildingControler: this, blockType: blockType);

                floorsParam.AddObject(floor);
            }
        }

        public override void ApplyBuildParameters()
        {
            //Parameter change
            if (previousGridSize.magnitude != 0) //Avoid activation during load
            {
                if (gridSizeParam.Val.x != previousGridSize.x || gridSizeParam.Val.y != previousGridSize.y)
                {
                    BlockGridSize = gridSizeParam.Val;
                }
            }
            else
            {
                previousGridSize = BlockGridSize;
            }


            Floors.Clear();

            for (int i = 0; i < NumberOfFloors; i++)
            {
                FloorController floor = floorsParam.SubObjects[i] as FloorController;

                if (floor == null)
                {
                    Debug.LogWarning("Error, Sub object has the wrong type. List = " + floorsParam.ValueName);
                    break;
                }

                Floors.Add(floor);
            }

            float currentHeight = 0;

            for (int i = 0; i < Floors.Count; i++) //ToDo: Add negative floor
            {
                Floors[i].transform.position = Vector3.up * currentHeight;
                currentHeight += Floors[i].CompleteFloorHeight;
            }

            UpdateFloorPositions();

            UpdateFloorDisplay();

            NonOrderedApplyBuildParameters();
        }

        public override void ResetObject()
        {
            baseReset();

            Floors.Clear();

            previousGridSize = Vector2Int.zero; //Block grid size adjustment during load
        }

        public override void InternalUpdate()
        {
            NonOrderedInternalUpdate();
        }

        public override void PlaytimeUpdate()
        {
            NonOrderedPlaytimeUpdate();
        }

        public FloorController CurrentFloorObject
        {
            get
            {
                FloorController returnValue = Floor(currentFloorNumber);

                return returnValue;
            }
        }

        public FloorController Floor(int floorNumber)
        {
            if(floorNumber < -NegativeFloors)
            {
                return Floors[0];
            }

            if(floorNumber > PositiveFloors)
            {
                return Floors[^1];
            }

            int floorIndex = NegativeFloors + floorNumber;

            return Floors[floorIndex];
        }

        public int FloorNumberFromFloor(FloorController floor)
        {
            if (!Floors.Contains(floor)) return 0;

            return Floors.IndexOf(floor) - NegativeFloors;
        }

        public void UpdateFloorPositions()
        {
            float currentFloorHeight = 0;

            for (int floor = 0; floor < NumberOfFloors; floor++)
            {
                Floors[floor].transform.localPosition = Vector3.up * currentFloorHeight;

                currentFloorHeight += Floors[floor].CompleteFloorHeight;
            }

            float downMovement = 0;

            for (int i = -NegativeFloors; i < 0; i++)
            {
                downMovement += Floor(i).CompleteFloorHeight;
            }

            foreach (FloorController floor in Floors)
            {
                floor.transform.position += Vector3.down * downMovement;
            }
        }

        //Editing
        enum AddingDirection
        {
            above,
            below
        }

        void AddFloor(AddingDirection direciton)
        {
            FloorController floorTemplate = ResourceLibrary.TryGetTemplateFromStringIdentifier(identifier: nameof(FloorController)) as FloorController;

            if (floorTemplate == null) Debug.LogWarning("Error");

            FloorController floor = Instantiate(floorTemplate);

            if (floor == null) Debug.LogWarning("Error");

            VirtualBlock.BlockTypes blockType = VirtualBlock.BlockTypes.Empty;

            floor.CompleteSetUpWithBuildParameters(buildingControler: this, blockType: blockType);

            switch (direciton)
            {
                case AddingDirection.above:
                    floorsParam.InsertAfterObject(newObject: floor, oldObject: CurrentFloorObject);
                    if (currentFloorNumber <= -1) negativeFloorParam.Val = negativeFloorParam.Val + 1;
                    break;
                case AddingDirection.below:
                    floorsParam.InsertBeforeObject(newObject: floor, oldObject: CurrentFloorObject);
                    if (currentFloorNumber <= 0) negativeFloorParam.Val = negativeFloorParam.Val + 1;
                    break;
                default:
                    break;
            }

            ApplyBuildParameters();
        }

        public void AddFloorAboveCurrentFloor()
        {
            AddFloor(AddingDirection.above);
        }

        public void AddFloorBelowCurrentFloor()
        {
            AddFloor(AddingDirection.below);
        }

        public void RemoveCurrentFloor()
        {
            if (NumberOfFloors <= 1) return; //Keep at least 1 floor

            FloorController FloorToBeDestroyed = CurrentFloorObject;

            /*
            Logic:
            if(floor >= 0) floor above falls down except you're on the top floor
            if(floor <  0) floor below moves up
            */

            if(currentFloorNumber > 0)
            {
                if(FloorToBeDestroyed.IsTopFloor)
                {
                    CurrentFloorNumber--;
                }
            }
            else if(currentFloorNumber == 0)
            {
                if (FloorToBeDestroyed.IsTopFloor)
                {
                    negativeFloorParam.Val--;
                }
            }
            else
            {
                if (FloorToBeDestroyed.IsBottomFloor)
                {
                    CurrentFloorNumber++;
                }

                negativeFloorParam.Val--;
            }

            FloorToBeDestroyed.DestroyObject();

            ApplyBuildParameters();
        }


        //Update floor visibility
        void UpdateFloorVisibility()
        {
            switch (floorViewDirection)
            {
                case FloorViewDirectionType.topDown:
                    foreach(FloorController floor in Floors)
                    {
                        if (floor.FloorNumber <= CurrentFloorNumber)
                        {
                            floor.gameObject.SetActive(true);
                        }
                        else
                        {
                            floor.gameObject.SetActive(false);
                        }
                    }
                    break;
                case FloorViewDirectionType.bottomUp:
                    foreach (FloorController floor in Floors)
                    {
                        if (floor.FloorNumber >= CurrentFloorNumber)
                        {
                            floor.gameObject.SetActive(true);
                        }
                        else
                        {
                            floor.gameObject.SetActive(false);
                        }
                    }
                    /*
                    for (int floor = -NegativeFloors; floor <= PositiveFloors; floor++)
                    {
                        if (floor > CurrentFloorNumber)
                        {
                            Floor(floor).gameObject.SetActive(false);
                        }
                        else
                        {
                            Floor(floor).gameObject.SetActive(true);
                        }
                    }
                    */
                    break;
                case FloorViewDirectionType.complete:
                    foreach (FloorController floor in Floors)
                    {
                        floor.gameObject.SetActive(true);
                    }
                    break;
                default:
                    break;
            }
        }

        void UpdateFloorDisplay()
        {
            UpdateFloorVisibility();

            switch (floorViewDirection)
            {
                case FloorViewDirectionType.topDown:
                    for (int floor = -NegativeFloors; floor <= PositiveFloors; floor++)
                    {
                        if (floor <= CurrentFloorNumber)
                        {
                            Floor(floor).FloorVisibilityType = FloorController.FloorVisibilityTypes.topDown;
                        }
                    }

                    break;
                case FloorViewDirectionType.bottomUp:
                    for (int floor = -NegativeFloors; floor <= PositiveFloors; floor++)
                    {
                        if (floor == CurrentFloorNumber)
                        {
                            Floor(floor).FloorVisibilityType = FloorController.FloorVisibilityTypes.bottomUp;
                        }
                        else if (floor > CurrentFloorNumber)
                        {
                            Floor(floor).FloorVisibilityType = FloorController.FloorVisibilityTypes.topDown;
                        }
                    }

                    break;
                default:
                    break;
            }
        }

        int backupFloorNumber;
        float backupWallDisplayHeightScaler;

        public void BackupVisibilityAndShowAll()
        {
            backupFloorNumber = CurrentFloorNumber;
            CurrentFloorNumber = PositiveFloors;

            backupWallDisplayHeightScaler = WallDisplayHeightScaler;
            WallDisplayHeightScaler = 1f;
        }

        public void RestoreVisibility()
        {
            CurrentFloorNumber = backupFloorNumber;
            WallDisplayHeightScaler = backupWallDisplayHeightScaler;
        }

        //Above or below visibility
        public enum FloorViewDirectionType
        {
            topDown,
            bottomUp,
            complete
        }

        FloorViewDirectionType floorViewDirection = FloorViewDirectionType.topDown;
        public FloorViewDirectionType FloorViewDirection
        {
            set
            {
                if (FloorViewDirection != value)
                {
                    floorViewDirection = value;
                    UpdateFloorVisibility();
                }

            }
            get
            {
                return floorViewDirection;
            }
        }

        //Current floor
        public int CurrentFloorNumber
        {
            set
            {
                if (value > PositiveFloors)
                {
                    currentFloorNumber = PositiveFloors;
                }
                else if (value < -NegativeFloors)
                {
                    currentFloorNumber = -NegativeFloors;
                }
                else
                {
                    currentFloorNumber = value;
                }

                UpdateFloorVisibility();
            }

            get
            {
                return currentFloorNumber;
            }
        }

        public int CurrentFloorIndex
        {
            get
            {
                return currentFloorNumber + NegativeFloors;
            }
        }

        //Wall height scaler
        float wallHeightScaler = 1;

        public float WallDisplayHeightScaler
        {
            set
            {
                wallHeightScaler = value;
                CurrentFloorObject.ApplyBuildParameters();
            }

            get
            {
                return wallHeightScaler;
            }
        }

        public void ShowInterior(int floorNumber)
        {

        }

        public void ShowRoof(int floorNumber)
        {

        }

    }
}