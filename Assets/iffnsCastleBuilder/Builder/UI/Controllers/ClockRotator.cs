using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class ClockRotator : MonoBehaviour
    {
        [SerializeField] Transform ClockFinger;
        [SerializeField] LightController LinkedLightController;

        // Start is called before the first frame update
        void Start()
        {
            ClockFingerAngleRad = LinkedLightController.LightRotationAngleRad;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void RotateClockToMousePosition()
        {
            Vector2 ClockHandlePosition = ClockFinger.transform.position; //Only takes the X and Y vectors

            Vector2 MousePositionPixel = Input.mousePosition;

            Vector2 offset = MousePositionPixel - ClockHandlePosition;

            float angle = Mathf.Atan2(offset.y, offset.x);

            LinkedLightController.LightRotationAngleRad = angle;
            ClockFingerAngleRad = angle;

        }

        public float ClockFingerAngleRad
        {
            set
            {
                ClockFinger.transform.localRotation = Quaternion.Euler(value * Mathf.Rad2Deg * Vector3.forward);
            }
        }
    }
}