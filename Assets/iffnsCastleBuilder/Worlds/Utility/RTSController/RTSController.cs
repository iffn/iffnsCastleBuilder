using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class RTSController : MonoBehaviour
    {
        //Unity assignments
        [SerializeField] Camera mainCamera;
        [SerializeField] GameObject CameraTilt;
        [SerializeField] GameObject CameraMover;
        [SerializeField] NavigationTools LinkedNavigationTools;
        //[SerializeField] GameObject clippingPlane;

        //Public settings
        public float movementSpeedWASD = 8f;
        public float movementSpeedMouse = 0.7f;
        public float rotationSpeedQE = 60f;
        public float rotationSpeedMouse = 800f;
        public float scrollSpeed = 8000f;

        //Internal settings
        readonly float maxDeltaTime = 1f / 45;
        readonly float isoCameraOffset = -100;
        readonly float maxZoomIn = -0.011f;
        readonly float maxZoomOut = -2000;
        readonly float minOrthographicSize = 0.001f;
        readonly float maxOrthographicSize = 2000f;

        //Runtime variables
        float currentCameraOffset;
        Vector3 cameraHomePosition;
        Quaternion cameraHomeDirectionOrientation;
        Quaternion cameraHomeTiltAngle;
        float cameraHomeZoomPosition;
        float isoCameraSize;

        void SetHomePosition()
        {
            cameraHomePosition = transform.position;
            cameraHomeDirectionOrientation = transform.rotation;
            cameraHomeTiltAngle = CameraTilt.transform.localRotation;
            cameraHomeZoomPosition = CameraMover.transform.localPosition.z;
            isoCameraSize = mainCamera.orthographicSize;
        }

        //Also assigned with UI home button
        public void RestoreHomePosition()
        {
            transform.position = cameraHomePosition;
            transform.rotation = cameraHomeDirectionOrientation;
            CameraTilt.transform.localRotation = cameraHomeTiltAngle;

            CameraMover.transform.localPosition = Vector3.forward * cameraHomeZoomPosition;
            currentCameraOffset = cameraHomeZoomPosition;
            mainCamera.orthographicSize = isoCameraSize;
        }

        /*
        public void SetClippingHeightAbsolute(float clippingHeightAbsolute)
        {
            float nearClipOffset = 0.05f;

            GameObject something = null;

            int dot = System.Math.Sign(Vector3.Dot(clippingPlane.transform.forward, something.transform.position - mainCamera.transform.position));

            Vector3 camSpacePos = mainCamera.worldToCameraMatrix.MultiplyPoint(clippingPlane.transform.position);
            Vector3 camSpaceNormal = mainCamera.worldToCameraMatrix.MultiplyVector(clippingPlane.transform.forward) * dot;
            float camSpaceDst = -Vector3.Dot(camSpacePos, camSpaceNormal) + nearClipOffset;

            mainCamera.projectionMatrix = playerCam.CalculateObliqueMatrix(clipPlaneCameraSpace);
        }
        */

        public float CameraTiltAngle
        {
            get
            {
                return CameraTilt.transform.localRotation.eulerAngles.x;
            }
        }

        //Camera perspective
        public enum CameraPerspectiveType
        {
            walking,
            isometric,
            flying
        }

        CameraPerspectiveType cameraPerspective;

        public CameraPerspectiveType CameraPerspective
        {
            set
            {
                cameraPerspective = value;

                switch (cameraPerspective)
                {
                    case CameraPerspectiveType.walking:
                        mainCamera.orthographic = false;
                        CameraMover.transform.localPosition = new Vector3(0, 0, currentCameraOffset);
                        break;
                    case CameraPerspectiveType.isometric:
                        mainCamera.orthographic = true;
                        CameraMover.transform.localPosition = new Vector3(0, 0, isoCameraOffset);
                        break;
                    case CameraPerspectiveType.flying:
                        mainCamera.orthographic = false;
                        CameraMover.transform.localPosition = new Vector3(0, 0, currentCameraOffset);
                        break;
                    default:
                        break;
                }
            }
        }

        public Ray GetRayFromCameraMouseLocation()
        {
            Ray returnRay = new();

            switch (cameraPerspective)
            {
                case CameraPerspectiveType.walking:
                    returnRay = mainCamera.ScreenPointToRay(Input.mousePosition);
                    break;

                case CameraPerspectiveType.isometric:
                    float verticalOffset = mainCamera.orthographicSize;
                    float verticalScale = (Input.mousePosition.y / Screen.height - 0.5f) * 2;

                    float horizontalOffset = mainCamera.orthographicSize * mainCamera.aspect;
                    float horizontalScale = (Input.mousePosition.x / Screen.width - 0.5f) * 2;


                    Vector3 relativeCameraOffset = new(
                        x: horizontalOffset * horizontalScale,
                        y: verticalOffset * verticalScale,
                        z: 0);

                    returnRay.origin = mainCamera.transform.position + mainCamera.transform.rotation * relativeCameraOffset;

                    returnRay.direction = mainCamera.transform.rotation * Vector3.forward;
                    break;

                case CameraPerspectiveType.flying:
                    returnRay = mainCamera.ScreenPointToRay(Input.mousePosition);
                    break;

                default:
                    break;
            }

            return returnRay;
        }

        public Vector3 GetImpactPositionFromMouseLocation()
        {
            Ray ray = GetRayFromCameraMouseLocation();
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity);

            return hit.point;
        }

        // Start is called before the first frame update
        void Start()
        {
            SetHomePosition();
            currentCameraOffset = CameraMover.transform.localPosition.z;
        }

        // Update is called once per frame
        void Update()
        {
            float deltaTime = Time.deltaTime;
            if (deltaTime > maxDeltaTime) deltaTime = maxDeltaTime;


            if (Input.GetKey(KeyCode.W))
            {
                gameObject.transform.Translate(deltaTime * movementSpeedWASD * Vector3.forward);
            }

            if (Input.GetKey(KeyCode.S))
            {
                gameObject.transform.Translate(deltaTime * movementSpeedWASD * Vector3.back);
            }

            if (Input.GetKey(KeyCode.A))
            {
                gameObject.transform.Translate(deltaTime * movementSpeedWASD * Vector3.left);
            }

            if (Input.GetKey(KeyCode.D))
            {
                gameObject.transform.Translate(deltaTime * movementSpeedWASD * Vector3.right);
            }

            if (Input.GetKey(KeyCode.Q))
            {
                gameObject.transform.Rotate(deltaTime * rotationSpeedQE * Vector3.up);
            }

            if (Input.GetKey(KeyCode.E))
            {
                gameObject.transform.Rotate(deltaTime * rotationSpeedQE * Vector3.down);
            }

            if (Input.GetMouseButton(2)) //Middle Mouse Button
            {
                gameObject.transform.Translate(new Vector3(Input.GetAxis("Mouse X") * movementSpeedMouse, 0, Input.GetAxis("Mouse Y") * movementSpeedMouse));
            }

            if (Input.GetMouseButton(1)) //Right Mouse Button
            {
                gameObject.transform.Rotate(Input.GetAxis("Mouse X") * rotationSpeedMouse * deltaTime * Vector3.up); // left / right rotation
            }

            if (Input.GetMouseButton(1)) //Right Mouse Button
            {
                CameraTilt.transform.Rotate(Input.GetAxis("Mouse Y") * rotationSpeedMouse * deltaTime * Vector3.right); //up / down rotation

                if (CameraTilt.transform.localRotation.eulerAngles.y > 90)
                {
                    if (CameraTilt.transform.localRotation.eulerAngles.x < 90) CameraTilt.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
                    else CameraTilt.transform.localRotation = Quaternion.Euler(new Vector3(270, 0, 0));
                }

                if (CameraTilt.transform.eulerAngles.x < 90)
                {
                    LinkedNavigationTools.ViewDirection = HumanBuildingController.FloorViewDirectionType.topDown;
                }
                else if (CameraTilt.transform.eulerAngles.x > 270)
                {
                    LinkedNavigationTools.ViewDirection = HumanBuildingController.FloorViewDirectionType.bottomUp;
                }
            }

            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) //Only scroll zoom if not over UI
            {
                if (Input.GetAxis("Mouse ScrollWheel") != 0)
                {
                    //Ignore if over UI

                    float ScrollInput = Input.GetAxis("Mouse ScrollWheel");

                    float maxScrollMultiplier = 1.15f;
                    float perspectiveScrollMultiplier = 1f - ScrollInput * scrollSpeed * deltaTime;
                    if (perspectiveScrollMultiplier > maxScrollMultiplier) perspectiveScrollMultiplier = maxScrollMultiplier;
                    else if (perspectiveScrollMultiplier < 1f / maxScrollMultiplier) perspectiveScrollMultiplier = 1f / maxScrollMultiplier;

                    switch (cameraPerspective)
                    {
                        case CameraPerspectiveType.walking:
                            currentCameraOffset *= perspectiveScrollMultiplier;
                            currentCameraOffset = Mathf.Clamp(value: currentCameraOffset, min: maxZoomOut, max: maxZoomIn); //All values are negative
                            CameraMover.transform.localPosition = new Vector3(0, 0, currentCameraOffset);
                            break;
                        case CameraPerspectiveType.isometric:
                            CameraMover.transform.localPosition = new Vector3(0, 0, isoCameraOffset);
                            mainCamera.orthographicSize = Mathf.Clamp(value: mainCamera.orthographicSize * perspectiveScrollMultiplier, min: minOrthographicSize, max: maxOrthographicSize);
                            break;
                        case CameraPerspectiveType.flying:
                            currentCameraOffset *= perspectiveScrollMultiplier;
                            currentCameraOffset = Mathf.Clamp(value: currentCameraOffset, min: maxZoomOut, max: maxZoomIn); //All values are negative
                            CameraMover.transform.localPosition = new Vector3(0, 0, currentCameraOffset);
                            break;
                        default:
                            break;
                    }


                    //CameraMover.transform.Translate(Vector3.forward * Input.GetAxis("Mouse ScrollWheel") * scrollIncrement * deltaTimer);

                    if (cameraPerspective == CameraPerspectiveType.isometric)
                    {
                        
                    }

                    /*
                    public void ControlCamera()
                    {
                        float sizeChange = -Input.GetAxis("Mouse ScrollWheel") * Camera.main.orthographicSize * cameraZoomSpeed;

                        Camera.main.orthographicSize += sizeChange;

                        //Calculate zoom movement to keep the mouse position on the same spot
                        transform.Translate(new Vector3(
                            -sizeChange * 2 * (Input.mousePosition.x / Screen.width - 0.5f) * Screen.width / Screen.height,    //x
                            0,                                                                  //y
                            -sizeChange * 2 * (Input.mousePosition.y / Screen.height - 0.5f)    //z
                            ));

                        //Middle mouse button movement
                        if (Input.GetMouseButton(2))
                        {

                            transform.Translate(new Vector3(
                            -Input.GetAxis("Mouse X") * Camera.main.orthographicSize * 0.05f * Screen.width / Screen.height,   //x
                            0,                                                                  //y
                            -Input.GetAxis("Mouse Y") * Camera.main.orthographicSize * 0.05f    //z
                            ));
                        }

                        //float test = (float)Screen.width / (float)Screen.height;
                    }
                    */
                }
            }
            

            if (Input.GetKeyDown(KeyCode.Home))
            {
                RestoreHomePosition();
            }
        }
    }
}