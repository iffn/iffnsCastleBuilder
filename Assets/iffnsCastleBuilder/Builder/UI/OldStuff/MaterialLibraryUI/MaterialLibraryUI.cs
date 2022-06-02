using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using iffnsStuff.iffnsBaseSystemForUnity;
using UnityEngine.UI;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class MaterialLibraryUI : MonoBehaviour
    {
        [SerializeField] Text Title = null;
        [SerializeField] GameObject ContentHolder = null;
        [SerializeField] GameObject ExpandIcon = null;
        [SerializeField] MaterialButton MaterialButtonTemplate = null;
        [SerializeField] Shader UIShader = null;

        MaterialLibraryExtenderTemplate MaterialLibrary;
        TexturingUI LinkedTexturingUI = null;
        List<MaterialButton> MaterialButtons = new List<MaterialButton>();
        MaterialButton previousButton = null;

        public void ToggleExpand()
        {
            Expand = !Expand;
        }

        public bool Expand
        {
            set
            {
                ContentHolder.SetActive(value);

                if (value)
                {
                    ExpandIcon.transform.rotation = Quaternion.Euler(Vector3.forward * 180);
                }
                else
                {
                    ExpandIcon.transform.rotation = Quaternion.Euler(Vector3.zero);
                }
            }
            get
            {
                return ContentHolder.activeSelf;
            }
        }

        public void ExpandAndActivateFirstMaterial()
        {
            Expand = true;
            previousButton = MaterialButtons[0];
            SetMaterial(MaterialButtons[0]);
        }

        public void UnhighlightPrevious()
        {
            if (previousButton != null) previousButton.Highlight = false;
        }

        public void SetMaterial(MaterialButton clickedButton)
        {
            LinkedTexturingUI.LinkedPainterTool.currentMaterial = clickedButton.MaterialReference;

            LinkedTexturingUI.UnhighlightPrevious(newLibrary: this);

            if (previousButton != null) previousButton.Highlight = false;

            previousButton = clickedButton;

            clickedButton.Highlight = true;
        }

        public bool SetMaterialAndExpansion(MaterialManager manager)
        {
            foreach (MaterialButton button in MaterialButtons)
            {
                if (button.MaterialReference == manager)
                {
                    SetMaterial(button);
                    Expand = true;
                    return true;
                }
            }

            return false;
        }

        public void Setup(MaterialLibraryExtenderTemplate library, TexturingUI linkedTexturingUI)
        {
            MaterialLibrary = library;
            LinkedTexturingUI = linkedTexturingUI;

            Title.text = MaterialLibrary.name;
            
            foreach (MaterialManager manager in MaterialLibrary.AllMaterialManagers)
            {
                MaterialButton newButton = Instantiate(original: MaterialButtonTemplate.gameObject, parent: ContentHolder.transform).GetComponent<MaterialButton>();

                Material uiMaterial = new Material(manager.LinkedMaterial);
                uiMaterial.shader = UIShader;

                MaterialButtons.Add(newButton);

                newButton.SetMaterialReference(previewMaterial: uiMaterial, materialReference: manager);

                newButton.AddButtonFunction(delegate { SetMaterial(newButton); });
            }

            Expand = false;
        }
    }
}