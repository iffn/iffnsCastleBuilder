using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class VectorButton : MonoBehaviour
    {
        public GameObject DarkBackgroundObject;
        public GameObject LightBackgroundObject;
        public GameObject DarkHighlightObject;
        public GameObject LightHighlightObject;
        public GameObject DarkIconObject;
        public GameObject LightIconObject;

        UIBrightnessTypes uiBrightnessType = UIBrightnessTypes.Dark;
        public UIBrightnessTypes UiBrightnessType
        {
            get
            {
                return uiBrightnessType;
            }
            set
            {
                if (uiBrightnessType != value)
                {
                    switch (value)
                    {
                        case UIBrightnessTypes.Dark:
                            DarkBackgroundObject.SetActive(true);
                            DarkIconObject.SetActive(true);
                            LightBackgroundObject.SetActive(false);
                            LightIconObject.SetActive(false);
                            LightHighlightObject.SetActive(false);
                            break;
                        case UIBrightnessTypes.Light:
                            DarkBackgroundObject.SetActive(false);
                            DarkIconObject.SetActive(false);
                            LightBackgroundObject.SetActive(true);
                            LightIconObject.SetActive(true);
                            DarkHighlightObject.SetActive(false);
                            break;
                        default:
                            break;
                    }
                }

                uiBrightnessType = value;
            }
        }

        public enum UIBrightnessTypes
        {
            Dark,
            Light
        }


        bool highlight = false;

        public bool Highlight
        {
            set
            {
                highlight = value;

                switch (uiBrightnessType)
                {
                    case UIBrightnessTypes.Dark:
                        DarkHighlightObject.SetActive(highlight);
                        break;
                    case UIBrightnessTypes.Light:
                        LightHighlightObject.SetActive(highlight);
                        break;
                    default:
                        break;
                }
            }
        }

        public void ToggleHighLight()
        {
            Highlight = !highlight;
        }

    }
}