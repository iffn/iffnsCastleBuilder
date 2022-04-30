using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialButton : MonoBehaviour
{
    [SerializeField] Image PreivewImage;
    [SerializeField] GameObject MainHighlightObject;

    public MaterialManager MaterialReference { get; private set; }

    public void SetMaterialReference(Material previewMaterial, MaterialManager materialReference)
    {
        PreivewImage.material = previewMaterial;
        this.MaterialReference = materialReference;
    }

    public bool Highlight
    {
        set
        {
            MainHighlightObject.SetActive(value);
        }
    }
}
