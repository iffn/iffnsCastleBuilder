using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class LightController : MonoBehaviour
    {
        [SerializeField] Transform SunPlane;
        [SerializeField] Transform SunRotator;
        [SerializeField] Light SunLight;

        [SerializeField] Gradient ambientLightGradient;
        [SerializeField] AnimationCurve SunIntensityCurve;

        float lightRotationAngleRad = Mathf.PI * 0.5f;

        float dayCicleRatio = 0.5f;

        public float DayCicleRatio
        {
            set
            {
                dayCicleRatio = value;

                float sunHeight = (dayCicleRatio < 0.5f) ? dayCicleRatio * 2 : 2 - dayCicleRatio * 2f;
                float sunIntensity = SunIntensityCurve.Evaluate(sunHeight);

                sunIntensity = Mathf.Clamp(sunIntensity, 0.01f, 1f);

                SunRotator.transform.localRotation = Quaternion.Euler(0, 0, dayCicleRatio * 360);

                UnityEngine.RenderSettings.ambientLight = ambientLightGradient.Evaluate(sunHeight);

                SunLight.intensity = sunIntensity;
            }
            get
            {
                return dayCicleRatio;
            }
        }

        public float LightRotationAngleRad
        {
            set
            {
                SunRotator.transform.localRotation = Quaternion.Euler(value * Mathf.Rad2Deg * Vector3.forward);
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
                    SunLight.intensity = 0;
                }
                else
                {
                    RenderSettings.ambientIntensity = 1f;
                    RenderSettings.reflectionIntensity = 1f;
                    SunLight.intensity = 1;
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
            DayCicleRatio = dayCicleRatio;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnApplicationQuit()
        {
            UnityEngine.RenderSettings.ambientLight = Color.white;
        }
    }
}