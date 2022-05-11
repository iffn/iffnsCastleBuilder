using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class LightController : MonoBehaviour
    {
        [SerializeField] Transform SunPlane;
        [SerializeField] Transform SunRotation;

        float lightRotationAngleRad = Mathf.PI * 0.5f;
        public float LightRotationAngleRad
        {
            set
            {
                SunRotation.transform.localRotation = Quaternion.Euler(value * Mathf.Rad2Deg * Vector3.up);
                lightRotationAngleRad = value;
            }

            get
            {
                return lightRotationAngleRad;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            LightRotationAngleRad = lightRotationAngleRad;
        }

        // Update is called once per frame
        void Update()
        {

        }

    }
}