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
        [SerializeField] CastleBuilderController CurrentBuilderController;
        [SerializeField] RTSController CurrentRTSCamera;
        [SerializeField] Slider FloorSelector;
        [SerializeField] GameObject FloorNumberHolder;
        [SerializeField] GameObject FloorNumberTemplate;
        [SerializeField] Slider WallHeightScaler;
        [SerializeField] VectorButton CameraWalkingIcon;
        [SerializeField] VectorButton CameraPerspectiveIcon;
        [SerializeField] VectorButton CameraIsometricIcon;
        [SerializeField] VectorButton CameraFlyingIcon;
        [SerializeField] DummyHumanPlayerController HumanPlayerController;
        [SerializeField] BuildingToolController LinkedBuildingToolController;
        [SerializeField] VectorButton PlayerButton;
        [SerializeField] VectorButton BlockLineButton;
        [SerializeField] OrientationCubeController linkedOrientationCubeController;
        public BlockLineController LinkedBlockLineController;

        //Variables
        bool showBlockLinesOnCurrentFloor = false;
        VirtualBlock previousBlockLineFocus;
        Vector3 previousCursorPosition = Vector3.zero;

        bool playerPositioningActive = false;
        int currentFloorNumber;

        public void UpdateViewIdentifier(float headingAngleDeg, float tiltAngleDeg)
        {
            linkedOrientationCubeController.SetViewAngles(headingAngleDeg: headingAngleDeg, tiltAngleDeg: tiltAngleDeg);
        }

        public void RestoreHomePosition()
        {
            CurrentRTSCamera.SetStandardView(RTSController.StandardViews.Home);
        }

        public void SetStandardView(ViewDirectionPasser directionPasser)
        {
            CurrentRTSCamera.SetStandardView(directionPasser.ViewDirection);
        }

        public CastleController CurrentBuilding
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

            //Set new floor number
            CurrentBuilding.CurrentFloorNumber = Mathf.RoundToInt(floor);

            UpdateCameraPositionRelativeToCurrentFloor();

            LinkedBlockLineController.UpdatePositionAndRotation();
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

        public CastleController.FloorViewDirectionType ViewDirection
        {
            get
            {
                return CurrentBuilding.FloorViewDirection;
            }
            set
            {
                if (CurrentBuilding.FloorViewDirection == value) return;

                CurrentBuilding.FloorViewDirection = value;

                LinkedBlockLineController.ViewDirection = value;
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
                if (showBlockLinesOnCurrentFloor == value) return;
                
                ToggleBlockLines();
            }
        }

        public void ToggleBlockLines()
        {
            showBlockLinesOnCurrentFloor = !showBlockLinesOnCurrentFloor;

            LinkedBlockLineController.gameObject.SetActive(showBlockLinesOnCurrentFloor);
            BlockLineButton.Highlight = showBlockLinesOnCurrentFloor;
            if (showBlockLinesOnCurrentFloor) LinkedBlockLineController.SetCompleteGrid();
        }


        public void UpdateUI()
        {
            UpdateFloorNumbers();

            LinkedBlockLineController.UpdateAll();
        }

        /*
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
        */

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
            //if (createWallLines) CreateBaseGridForEachFloor();

            ShowBlockLinesOnCurrentFloor = true;

            LinkedBlockLineController.UpdateAll();

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
            LinkedBlockLineController.Setup(linkedBuilding: CurrentBuilding);

            SetDefaultVisuals();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (playerPositioningActive)
            {
                PositionPlayer();
            }

            if (Input.GetKeyDown(KeyCode.F1))
            {
                LinkedBlockLineController.SizeInfoActivation = !LinkedBlockLineController.SizeInfoActivation;
            }
        }
    }
}
