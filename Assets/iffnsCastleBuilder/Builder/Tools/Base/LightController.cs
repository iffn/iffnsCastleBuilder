using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class LightController : MonoBehaviour
    {
        [SerializeField] Transform SunPlane;
        [SerializeField] Transform SunRotation;
        [SerializeField] Light LinkedLight;

        float lightRotationAngleRad = Mathf.PI * 0.5f;
        public float LightRotationAngleRad
        {
            set
            {
                SunRotation.transform.localRotation = Quaternion.Euler(value * Mathf.Rad2Deg * Vector3.up);
                lightRotationAngleRad = value;

                float angle = value * Mathf.Rad2Deg;    
                // 0 = morning
                // 90 = noon
                // -90 = midnight
                // 180 = -180 = evening

                if(angle < -20 && angle > -160)
                {
                    RenderSettings.ambientIntensity = 0.2f;
                    RenderSettings.reflectionIntensity = 0.2f;
                    LinkedLight.intensity = 0;
                }
                else
                {
                    RenderSettings.ambientIntensity = 1f;
                    RenderSettings.reflectionIntensity = 1f;
                    LinkedLight.intensity = 1;
                }

                //
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