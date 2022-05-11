﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsUnityResources
{
    public class RTSController : MonoBehaviour
    {
        Vector3 cameraHomePosition;
        Quaternion cameraHomeDirectionOrientation;
        Quaternion cameraHomeTiltAngle;
        float cameraHomeZoomPosition;
        public Camera mainCamera;

        [SerializeField] GameObject clippingPlane;

        float maxDeltaTime = 1f / 45;
        float currentCameraOffset;
        float isoCameraOffset = -100;

        void SetHomePosition()
        {
            cameraHomePosition = transform.position;
            cameraHomeDirectionOrientation = transform.rotation;
            cameraHomeTiltAngle = CameraTilt.transform.localRotation;
            cameraHomeZoomPosition = CameraMover.transform.localPosition.z;
        }

        //Also assigned with UI home button
        public void RestoreHomePosition()
        {
            transform.position = cameraHomePosition;
            transform.rotation = cameraHomeDirectionOrientation;
            CameraTilt.transform.localRotation = cameraHomeTiltAngle;
            CameraMover.transform.localPosition = Vector3.forward * cameraHomeZoomPosition;
        }

        public void SetClippingHeightAbsolute(float clippingHeightAbsolute)
        {
            /*
            float nearClipOffset = 0.05f;

            GameObject something = null;

            int dot = System.Math.Sign(Vector3.Dot(clippingPlane.transform.forward, something.transform.position - mainCamera.transform.position));

            Vector3 camSpacePos = mainCamera.worldToCameraMatrix.MultiplyPoint(clippingPlane.transform.position);
            Vector3 camSpaceNormal = mainCamera.worldToCameraMatrix.MultiplyVector(clippingPlane.transform.forward) * dot;
            float camSpaceDst = -Vector3.Dot(camSpacePos, camSpaceNormal) + nearClipOffset;

            mainCamera.projectionMatrix = playerCam.CalculateObliqueMatrix(clipPlaneCameraSpace);
            */
        }

        public float movementSpeedWASD = 8f;
        public float movementSpeedMouse = 0.7f;
        public float rotationSpeedQE = 60f;
        public float rotationSpeedMouse = 800f;
        public float scrollSpeed = 8000f;

        public GameObject CameraTilt;
        public GameObject CameraMover;

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
            perpesctive,
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
                    case CameraPerspectiveType.perpesctive:
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
            Ray returnRay = new Ray();

            switch (cameraPerspective)
            {
                case CameraPerspectiveType.perpesctive:
                    returnRay = mainCamera.ScreenPointToRay(Input.mousePosition);
                    break;

                case CameraPerspectiveType.isometric:
                    float verticalOffset = mainCamera.orthographicSize;
                    float verticalScale = (Input.mousePosition.y / Screen.height - 0.5f) * 2;

                    float horizontalOffset = mainCamera.orthographicSize * mainCamera.aspect;
                    float horizontalScale = (Input.mousePosition.x / Screen.width - 0.5f) * 2;


                    Vector3 relativeCameraOffset = new Vector3(
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

            RaycastHit hit;

            Physics.Raycast(ray, out hit, Mathf.Infinity);

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
                gameObject.transform.Translate(Vector3.forward * movementSpeedWASD * deltaTime);
            }

            if (Input.GetKey(KeyCode.S))
            {
                gameObject.transform.Translate(Vector3.back * movementSpeedWASD * deltaTime);
            }

            if (Input.GetKey(KeyCode.A))
            {
                gameObject.transform.Translate(Vector3.left * movementSpeedWASD * deltaTime);
            }

            if (Input.GetKey(KeyCode.D))
            {
                gameObject.transform.Translate(Vector3.right * movementSpeedWASD * deltaTime);
            }

            if (Input.GetKey(KeyCode.Q))
            {
                gameObject.transform.Rotate(Vector3.up * rotationSpeedQE * deltaTime);
            }

            if (Input.GetKey(KeyCode.E))
            {
                gameObject.transform.Rotate(Vector3.down * rotationSpeedQE * deltaTime);
            }

            if (Input.GetMouseButton(2)) //Middle Mouse Button
            {
                gameObject.transform.Translate(new Vector3(Input.GetAxis("Mouse X") * movementSpeedMouse, 0, Input.GetAxis("Mouse Y") * movementSpeedMouse));
            }

            if (Input.GetMouseButton(1)) //Right Mouse Button
            {
                gameObject.transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * rotationSpeedMouse * deltaTime); // left / right rotation
            }

            if (Input.GetMouseButton(1)) //Right Mouse Button
            {
                CameraTilt.transform.Rotate(Vector3.right * Input.GetAxis("Mouse Y") * rotationSpeedMouse * deltaTime); //up / down rotation

                if (CameraTilt.transform.localRotation.eulerAngles.y > 90)
                {
                    if (CameraTilt.transform.localRotation.eulerAngles.x < 90) CameraTilt.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
                    else CameraTilt.transform.localRotation = Quaternion.Euler(new Vector3(270, 0, 0));
                }
            }

            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) //Only scroll zoom if not over UI
            {
                if (Input.GetAxis("Mouse ScrollWheel") != 0)
                {
                    //Ignore if over UI

                    float ScrollInput = Input.GetAxis("Mouse ScrollWheel");

                    float maxScrollMultiplier = 1.15f;
                    float perspectiveScrollMultiplier = 1f - Input.GetAxis("Mouse ScrollWheel") * scrollSpeed * deltaTime;
                    if (perspectiveScrollMultiplier > maxScrollMultiplier) perspectiveScrollMultiplier = maxScrollMultiplier;
                    else if (perspectiveScrollMultiplier < 1f / maxScrollMultiplier) perspectiveScrollMultiplier = 1f / maxScrollMultiplier;

                    currentCameraOffset *= perspectiveScrollMultiplier;

                    switch (cameraPerspective)
                    {
                        case CameraPerspectiveType.perpesctive:
                            CameraMover.transform.localPosition = new Vector3(0, 0, currentCameraOffset);
                            break;
                        case CameraPerspectiveType.isometric:
                            CameraMover.transform.localPosition = new Vector3(0, 0, isoCameraOffset);
                            break;
                        case CameraPerspectiveType.flying:
                            CameraMover.transform.localPosition = new Vector3(0, 0, currentCameraOffset);
                            break;
                        default:
                            break;
                    }


                    //CameraMover.transform.Translate(Vector3.forward * Input.GetAxis("Mouse ScrollWheel") * scrollIncrement * deltaTimer);

                    if (cameraPerspective == CameraPerspectiveType.isometric)
                    {
                        mainCamera.orthographicSize = mainCamera.orthographicSize * perspectiveScrollMultiplier;
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