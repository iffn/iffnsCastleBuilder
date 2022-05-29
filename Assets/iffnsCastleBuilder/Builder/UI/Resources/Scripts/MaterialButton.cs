using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using iffnsStuff.iffnsBaseSystemForUnity;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class MaterialButton : MonoBehaviour
    {
        [SerializeField] Image PreivewImage;
        [SerializeField] GameObject MainHighlightObject;
        [SerializeField] GameObject DarkHoverOverHighlightObject;
        [SerializeField] Button LinkedButton;

        public void AddButtonFunction(UnityEngine.Events.UnityAction call)
        {
            LinkedButton.onClick.AddListener(call);
        }

        public MaterialManager MaterialReference { get; private set; }

        public void SetMaterialReference(Material previewMaterial, MaterialManager materialReference)
        {
            PreivewImage.material = previewMaterial;
            MaterialReference = materialReference;
        }

        public bool Highlight
        {
            set
            {
                MainHighlightObject.SetActive(value);
            }
        }
    }
}