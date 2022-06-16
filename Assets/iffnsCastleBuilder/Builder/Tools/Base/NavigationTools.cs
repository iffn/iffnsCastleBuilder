using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using iffnsStuff.iffnsUnityResources;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class NavigationTools : MonoBehaviour
    {
        //Unity assignments
        [SerializeField] HumanBuilderController CurrentBuilderController;
        [SerializeField] RTSController CurrentRTSCamera;
        [SerializeField] Slider FloorSelector;
        [SerializeField] GameObject FloorNumberHolder;
        [SerializeField] int BlockLineRadius = 3;
        [SerializeField] GameObject BlockLineHolder;
        [SerializeField] GameObject DimensionInfo;
        [SerializeField] LineRenderer BlockLineTemplate;
        [SerializeField] GameObject FloorNumberTemplate;
        [SerializeField] Slider WallHeightScaler;
        [SerializeField] VectorButton CameraWalkingIcon;
        [SerializeField] VectorButton CameraPerspectiveIcon;
        [SerializeField] VectorButton CameraIsometricIcon;
        [SerializeField] VectorButton CameraFlyingIcon;
        [SerializeField] BlockLineType currentBlockLineType = BlockLineType.Complete;
        [SerializeField] DummyHumanPlayerController HumanPlayerController;
        [SerializeField] BuildingToolController LinkedBuildingToolController;
        [SerializeField] VectorButton PlayerButton;
        [SerializeField] VectorButton BlockLineButton;
        [SerializeField] OrientationCubeController linkedOrientationCubeController;

        //Variables
        List<GameObject> blockLines = new List<GameObject>();
        GameObject currentLineHolder;
        static bool createWallLines = false;
        bool showBlockLinesOnCurrentFloor = false;
        VirtualBlock previousBlockLineFocus;
        Vector3 previousCursorPosition = Vector3.zero;
        bool playerPositioningActive = false;
        int currentFloorNumber;

        public void UpdateViewIdentifier(float headingAngleDeg, float tiltAngleDeg)
        {
            linkedOrientationCubeController.SetViewAngles(headingAngleDeg: headingAngleDeg, tiltAngleDeg: tiltAngleDeg);
        }

        public void SetStandardView(ViewDirectionPasser directionPasser)
        {
            CurrentRTSCamera.SetStandardView(directionPasser.ViewDirection);
        }

        public enum BlockLineType
        {
            Complete,
            AroundFocus
        }

        public HumanBuildingController CurrentBuilding
        {
            get
            {
                return CurrentBuilderController.CurrentBuilding;
            }
        }

        public void SelectCurrentFloor(float floor)
        {
            if (CurrentBuilding == null)
            {
                return;
            }

            //Deactivate old floor line object
            //blockLines[CurrentBuilding.CurrentFloorIndex].SetActive(false);

            //Set new floor number
            CurrentBuilding.CurrentFloorNumber = Mathf.RoundToInt(floor);

            //Activate new floor line object
            //if (createWallLines) blockLines[CurrentBuilding.CurrentFloorIndex].SetActive(true);


            //CurrentBuilding.CurrentFloorObject.FoorHeihgtScaler = wallHeightScale;
            //Debug.Log("" + MainBuilding.CurrentFloorObject.transform.position.y);


            //Update camera

            UpdateCameraPositionRelativeToCurrentFloor();

            UpdateBlockLines();
        }


        void UpdateCameraPositionRelativeToCurrentFloor()
        {
            CurrentRTSCamera.transform.position = new Vector3(CurrentRTSCamera.transform.position.x, CurrentBuilding.CurrentFloorObject.transform.position.y + CurrentBuilding.CurrentFloorObject.CompleteFloorHeight / 2, CurrentRTSCamera.transform.position.z);
        }


        //Edit floor
        public void AddFloorAboveCurrentFloor()
        {
            CurrentBuilding.AddFloorAboveCurrentFloor();

            UpdateFloorNumbers();
            //SelectCurrentFloor(CurrentBuilding.CurrentFloorNumber + 1);

            //CurrentBuilding.CurrentFloorNumber += 1;

            FloorSelector.value = CurrentBuilding.CurrentFloorNumber + 1;
        }

        public void AddFloorBelowCurrentFloor()
        {
            CurrentBuilding.AddFloorBelowCurrentFloor();

            UpdateFloorNumbers();

            FloorSelector.value = CurrentBuilding.CurrentFloorNumber - 1;
        }

        public void RemoveCurrentFloor()
        {
            EditTool.DeactivateEditOnMain();

            CurrentBuilding.RemoveCurrentFloor();

            UpdateFloorNumbers();

            FloorSelector.value = CurrentBuilding.CurrentFloorNumber;
        }


        //Wall height scale

        public void UpdateWallHeightSlider()
        {
            if (CurrentBuilding == null)
            {
                return;
            }

            if (WallHeightScaler.value > 1f)
            {
                WallHeightScaler.value = 1f;
            }

            CurrentBuilding.WallDisplayHeightScaler = WallHeightScaler.value;


            //CurrentBuilderController.CurrentBuilding.CurrentFloorObject.UpdateFloorDisplayComplete();
        }

        public HumanBuildingController.FloorViewDirectionType ViewDirection
        {
            get
            {
                return CurrentBuilding.FloorViewDirection;
            }
            set
            {
                CurrentBuilding.FloorViewDirection = value;

                SetBaseBlockLinePosition(viewDirection: value);
            }
        }

        void UpdateFloorNumbers()
        {

            FloorSelector.minValue = -CurrentBuilding.NegativeFloors;
            FloorSelector.maxValue = CurrentBuilding.PositiveFloors;

            foreach (Transform child in FloorNumberHolder.transform)
            {
                Destroy(child.gameObject);
            }

            if (CurrentBuilding.NumberOfFloors <= 1) return;

            float FloorIndexHeight = 1200;

            float distanceBetweenFloorIndexes = FloorIndexHeight / (CurrentBuilding.NumberOfFloors - 1);

            for (int i = 0; i < CurrentBuilding.NumberOfFloors; i++)
            {
                GameObject currentNumber = Instantiate(FloorNumberTemplate);
                currentNumber.transform.SetParent(parent: FloorNumberHolder.transform, worldPositionStays: false);
                currentNumber.transform.localScale = Vector3.one;
                currentNumber.transform.localPosition = Vector3.up * distanceBetweenFloorIndexes * i;
                string positiveSign = "";
                if (-CurrentBuilding.NegativeFloors + i > 0)
                {
                    positiveSign = "+";
                }
                currentNumber.transform.GetComponent<Text>().text = positiveSign + (-CurrentBuilding.NegativeFloors + i).ToString() + " >";
            }
        }

        //Block lines
        public bool ShowBlockLinesOnCurrentFloor
        {
            get
            {
                return showBlockLinesOnCurrentFloor;
            }
            set 
            {
                showBlockLinesOnCurrentFloor = value;
                BlockLineHolder.SetActive(value);
                DimensionInfo.SetActive(value);
                BlockLineButton.Highlight = value;
                UpdateBlockLines();
            }
        }

        public void ToggleBlockLines()
        {
            ShowBlockLinesOnCurrentFloor = !ShowBlockLinesOnCurrentFloor;
        }

        public void UpdateBlockLines()
        {
            //Check if floor lines activated
            if (!showBlockLinesOnCurrentFloor)
            {
                return;
            }

            switch (currentBlockLineType)
            {
                case BlockLineType.Complete:
                    SetBaseBlockLines();
                    SetBaseBlockLinePosition(viewDirection: ViewDirection);
                    break;
                case BlockLineType.AroundFocus:
                    VirtualBlock newFocusBlock = getNewFocusBlock();
                    if (newFocusBlock != null) ShowBlockLinesAroundFocus(focusBlock: newFocusBlock);
                    break;
                default:
                    break;
            }
        }

        public void UpdateUI()
        {
            UpdateFloorNumbers();

            UpdateBlockLines();
        }

        VirtualBlock getNewFocusBlock()
        {
            //Check if cursor moved
            if (Input.mousePosition.x == previousCursorPosition.x
                && Input.mousePosition.y == previousCursorPosition.y)
            {
                //Cursor not moved
                return null;
            }
            else
            {
                previousCursorPosition = Input.mousePosition;
            }

            //Check if over UI
            if (EventSystem.current.IsPointerOverGameObject()) //if over UI button
            {
                //Do nothing
                return null;
            }

            //Check if new block selected
            VirtualBlock newFocusBlock = SmartBlockBuilderTool.GetBlockFromClick(OnlyCheckCurrentFloor: true, currentBuilding: CurrentBuilding, currentRTSCamera: CurrentRTSCamera);

            return newFocusBlock;
        }

        void SetBaseBlockLinePosition(HumanBuildingController.FloorViewDirectionType viewDirection)
        {
            BlockLineHolder.transform.position = CurrentBuilding.CurrentFloorObject.transform.position;
            BlockLineHolder.transform.rotation = CurrentBuilding.CurrentFloorObject.transform.rotation;

            DimensionInfo.transform.position = CurrentBuilding.CurrentFloorObject.transform.position;

            switch (viewDirection)
            {
                case HumanBuildingController.FloorViewDirectionType.topDown:
                    BlockLineHolder.transform.position += Vector3.up * (CurrentBuilding.CurrentFloorObject.BottomFloorHeight + MathHelper.SmallFloat);
                    BlockLineHolder.transform.localScale = Vector3.one;
                    DimensionInfo.transform.rotation = Quaternion.Euler(Vector3.right * 90);
                    DimensionInfo.transform.position += Vector3.up * CurrentBuilding.CurrentFloorObject.BottomFloorHeight;
                    break;
                case HumanBuildingController.FloorViewDirectionType.bottomUp:
                    BlockLineHolder.transform.position += new Vector3(CurrentBuilding.BlockGridSize.x * CurrentBuilding.BlockSize, -MathHelper.SmallFloat, 0);
                    BlockLineHolder.transform.localScale = new Vector3(1, -1, 1);
                    BlockLineHolder.transform.Rotate(Vector3.forward * 180);
                    DimensionInfo.transform.rotation = Quaternion.Euler(new Vector3(-90, -90, 0));
                    break;
                default:
                    break;
            }

            
        }

        void ShowBlockLinesAroundFocus(VirtualBlock focusBlock)
        {
            //Cleanup
            if (currentLineHolder != null)
            {
                Destroy(currentLineHolder);
            }

            /*
            foreach (GameObject line in blockLines)
            {
                GameObject.Destroy(line);
            }
            */

            blockLines.Clear();

            //Setup new line holder
            currentLineHolder = new GameObject();
            currentLineHolder.transform.position = CurrentBuilding.CurrentFloorObject.transform.position + Vector3.up * (CurrentBuilding.CurrentFloorObject.BottomFloorHeight + 0.01f);
            currentLineHolder.transform.rotation = CurrentBuilding.CurrentFloorObject.transform.rotation;

            //Check min and max values
            Vector2Int startingPosition = Vector2Int.zero;
            Vector2Int endPosition = Vector2Int.zero;

            startingPosition.x = focusBlock.XCoordinate - BlockLineRadius;
            endPosition.x = focusBlock.XCoordinate + BlockLineRadius;
            startingPosition.y = focusBlock.ZCoordinate - BlockLineRadius;
            endPosition.y = focusBlock.ZCoordinate + BlockLineRadius;

            if (startingPosition.x < 0) startingPosition.x = 0;
            if (startingPosition.y < 0) startingPosition.y = 0;
            if (endPosition.x > CurrentBuilding.BlockGridSize.x - 1) endPosition.x = CurrentBuilding.BlockGridSize.x - 1;
            if (endPosition.y > CurrentBuilding.BlockGridSize.y - 1) endPosition.y = CurrentBuilding.BlockGridSize.y - 1;

            //Create lines
            for (int xPos = startingPosition.x; xPos <= endPosition.x + 1; xPos++)
            {
                LineRenderer currentLine = Instantiate(BlockLineTemplate).transform.GetComponent<LineRenderer>();
                currentLine.transform.parent = currentLineHolder.transform;

                currentLine.transform.localPosition = CurrentBuilding.CurrentFloorObject.GetLocalNodePositionFromNodeIndex(new Vector2Int(xPos, startingPosition.y));
                currentLine.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 90));

                float lineLength = (endPosition.y - startingPosition.y + 1) * CurrentBuilding.BlockSize;

                currentLine.SetPosition(1, Vector3.right * lineLength);
            }

            for (int zPos = startingPosition.y; zPos <= endPosition.y + 1; zPos++)
            {
                LineRenderer currentLine = Instantiate(BlockLineTemplate).transform.GetComponent<LineRenderer>();
                currentLine.transform.parent = currentLineHolder.transform;

                currentLine.transform.localPosition = CurrentBuilding.CurrentFloorObject.GetLocalNodePositionFromNodeIndex(new Vector2Int(startingPosition.x, zPos));
                currentLine.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 90));

                float lineLength = (endPosition.x - startingPosition.x + 1) * CurrentBuilding.BlockSize;

                currentLine.SetPosition(1, Vector3.down * lineLength);
            }
        }

        void SetBaseBlockLines()
        {
            //Remove old block lines
            foreach (Transform child in BlockLineHolder.transform)
            {
                Destroy(child.gameObject);
            }

            //Add new lines
            for (int xPos = 0; xPos <= CurrentBuilding.BlockGridSize.x; xPos++)
            {
                LineRenderer currentLine = Instantiate(BlockLineTemplate).transform.GetComponent<LineRenderer>();

                currentLine.transform.parent = BlockLineHolder.transform;

                currentLine.transform.localPosition = Vector3.right * CurrentBuilding.BlockSize * xPos;

                currentLine.SetPosition(1, Vector3.right * CurrentBuilding.BlockSize * CurrentBuilding.BlockGridSize.y);

                currentLine.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 90));
            }

            for (int zPos = 0; zPos <= CurrentBuilding.BlockGridSize.y; zPos++)
            {
                LineRenderer currentLine = Instantiate(BlockLineTemplate).transform.GetComponent<LineRenderer>();

                currentLine.transform.parent = BlockLineHolder.transform;

                currentLine.transform.localPosition = Vector3.forward * CurrentBuilding.BlockSize * zPos;

                currentLine.SetPosition(1, Vector3.right * CurrentBuilding.BlockSize * CurrentBuilding.BlockGridSize.x);
            }
        }

        public void CreateBaseGridForEachFloor()
        {

            for (int floorNumber = -CurrentBuilding.NegativeFloors; floorNumber <= CurrentBuilding.PositiveFloors; floorNumber++)
            {
                FloorController currentFloor = CurrentBuilding.Floor(floorNumber: floorNumber);

                GameObject currentLineHolder = new GameObject();

                currentLineHolder.SetActive(false);

                currentLineHolder.transform.parent = BlockLineHolder.transform;

                currentLineHolder.transform.position = currentFloor.transform.position + Vector3.up * (currentFloor.BottomFloorHeight + 0.01f);

                currentLineHolder.transform.rotation = currentFloor.transform.rotation;

                blockLines.Add(currentLineHolder);

                for (int xPos = 0; xPos <= CurrentBuilding.BlockGridSize.x; xPos++)
                {
                    LineRenderer currentLine = Instantiate(BlockLineTemplate).transform.GetComponent<LineRenderer>();

                    currentLine.transform.parent = currentLineHolder.transform;

                    currentLine.transform.localPosition = Vector3.right * CurrentBuilding.BlockSize * (xPos - 0.5f) - Vector3.forward * CurrentBuilding.BlockSize * 0.5f;

                    currentLine.SetPosition(1, Vector3.right * CurrentBuilding.BlockSize * CurrentBuilding.BlockGridSize.y);

                    currentLine.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 90));
                }

                for (int zPos = 0; zPos <= CurrentBuilding.BlockGridSize.y; zPos++)
                {
                    LineRenderer currentLine = Instantiate(BlockLineTemplate).transform.GetComponent<LineRenderer>();

                    currentLine.transform.parent = currentLineHolder.transform;

                    currentLine.transform.localPosition = Vector3.forward * CurrentBuilding.BlockSize * (zPos - 0.5f) - Vector3.right * CurrentBuilding.BlockSize * 0.5f;

                    currentLine.SetPosition(1, Vector3.right * CurrentBuilding.BlockSize * CurrentBuilding.BlockGridSize.x);
                }
            }

            blockLines[CurrentBuilding.CurrentFloorIndex].SetActive(true);
        }


        public void SwitchCameraType(VectorButton clickedCameraIcon)
        {
            clickedCameraIcon.gameObject.SetActive(false);

            if (clickedCameraIcon == CameraPerspectiveIcon)
            {
                CameraIsometricIcon.gameObject.SetActive(true);
                CurrentRTSCamera.CameraPerspective = RTSController.CameraPerspectiveType.isometric;
            }
            else if (clickedCameraIcon == CameraIsometricIcon)
            {
                CameraPerspectiveIcon.gameObject.SetActive(true);
                CurrentRTSCamera.CameraPerspective = RTSController.CameraPerspectiveType.walking;
            }

            //ToDo: Add flying camera
            /*
            if (clickedCameraIcon == CameraWalkingIcon)
            {
                CameraIsometricIcon.gameObject.SetActive(true);
                CurrentRTSCamera.CameraPerspective = RTSController.CameraPerspectiveType.isometric;
            }
            else if (clickedCameraIcon == CameraIsometricIcon)
            {
                CameraFlyingIcon.gameObject.SetActive(true);
                CurrentRTSCamera.CameraPerspective = RTSController.CameraPerspectiveType.flying;
            }
            else if (clickedCameraIcon == CameraFlyingIcon)
            {
                CameraWalkingIcon.gameObject.SetActive(true);
                CurrentRTSCamera.CameraPerspective = RTSController.CameraPerspectiveType.perpesctive;
            }
            */
        }

        //Setup
        void SetDefaultVisuals()
        {
            //Block line visibility
            //Run twice for setting booleand to default setting
            if (createWallLines) CreateBaseGridForEachFloor();

            ShowBlockLinesOnCurrentFloor = true;

            /*
            ToogleBlockLines();
            ToogleBlockLines();
            */

            //Current floor
            UpdateFloorNumbers();


        }

        //Player positioning
        public bool PlayerPositioningActive
        {
            get
            {
                return playerPositioningActive;
            }

            set
            {
                playerPositioningActive = value;
                PlayerButton.Highlight = value;
            }
        }

        public void TogglePlayerPositioning()
        {
            PlayerPositioningActive = !PlayerPositioningActive;

            EditTool.DeactivateEditOnMain();
        }

        void PositionPlayer()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (!EventSystem.current.IsPointerOverGameObject()) //if not over UI button
                {
                    Ray ray = CurrentRTSCamera.GetRayFromCameraMouseLocation();

                    RaycastHit hit;

                    Vector3 direction = ray.direction;
                    Vector3 originAbsolute = ray.origin;

                    Physics.Raycast(ray, out hit, Mathf.Infinity);

                    if (hit.collider != null)
                    {
                        HumanPlayerController.transform.position = hit.point;

                        HumanPlayerController.transform.rotation = Quaternion.identity;

                        PlayerModeActive = true;

                        playerPositioningActive = false;
                        PlayerButton.Highlight = false;
                    }
                }
            }
        }

        bool PlayerModeActive
        {
            set
            {
                HumanPlayerController.gameObject.SetActive(value);
                HumanPlayerController.MouseUnlockAndVisibilityType = !value;
                CurrentRTSCamera.gameObject.SetActive(!value);
                LinkedBuildingToolController.ToolActivationState = !value;

                if (value)
                {
                    currentFloorNumber = CurrentBuilding.CurrentFloorNumber;
                    CurrentBuilding.CurrentFloorNumber = CurrentBuilding.PositiveFloors;
                }
                else
                {
                    CurrentBuilding.CurrentFloorNumber = currentFloorNumber;
                }
            }
        }

        public void ReturnFromPlayerToRTSControls()
        {
            PlayerModeActive = false;
        }

        public void Setup()
        {
            SetDefaultVisuals();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (currentBlockLineType == BlockLineType.AroundFocus)
            {
                UpdateBlockLines();
            }

            if (playerPositioningActive)
            {
                PositionPlayer();
            }

            if (Input.GetKeyDown(KeyCode.F1))
            {
                DimensionInfo.SetActive(!DimensionInfo.activeSelf);
            }
        }


    }
}
