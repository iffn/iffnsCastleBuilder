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
        [SerializeField] RectTransform HiddenContent;
        [SerializeField] RectTransform DescriptionHolder;
        [SerializeField] TMPro.TMP_Text Description = null;

        [SerializeField] RectTransform ButtonHolder = null;
        [SerializeField] GameObject ExpandIcon = null;
        [SerializeField] MaterialButton MaterialButtonTemplate = null;
        [SerializeField] Shader UIShader = null;

        MaterialLibraryExtenderTemplate MaterialLibrary;
        TexturingUI LinkedTexturingUI = null;
        List<MaterialButton> MaterialButtons = new List<MaterialButton>();
        MaterialButton previousButton = null;

        RectTransform linkedRectTransform;

        public void ToggleExpand()
        {
            Expand = !Expand;
        }

        public bool Expand
        {
            set
            {
                HiddenContent.gameObject.SetActive(value);

                if (value)
                {
                    ExpandIcon.transform.rotation = Quaternion.Euler(Vector3.forward * 180);
                    linkedRectTransform.sizeDelta = new Vector2(300, 30 + HiddenContent.sizeDelta.y);
                }
                else
                {
                    ExpandIcon.transform.rotation = Quaternion.Euler(Vector3.zero);
                    linkedRectTransform.sizeDelta = new Vector2(300, 30);
                }
            }
            get
            {
                return HiddenContent.gameObject.activeSelf;
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
            LinkedTexturingUI.LinkedPainterTool.CurrentMaterial = clickedButton.MaterialReference;

            LinkedTexturingUI.UnhighlightPreviousMaterialButton(newLibrary: this);
            LinkedTexturingUI.UnhighlightIlluminationButtons();

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
            linkedRectTransform = transform.GetComponent<RectTransform>();

            Title.text = MaterialLibrary.name;

            float borderAddition = 10;
            float lineHeight = 16;

            //Description
            int descriptionLength = library.LibraryInfoText.Length;
            float descriptionHeight;

            if (descriptionLength > 0)
            {
                Description.text = library.LibraryInfoText;

                int symbolsPerLine = 50;

                int descriptionLines = descriptionLength / symbolsPerLine;
                if (descriptionLines < 1) descriptionLines = 1;
                else if (descriptionLength % symbolsPerLine > symbolsPerLine * 0.2f) descriptionLines += 1;

                descriptionHeight = descriptionLines * lineHeight + borderAddition;

                DescriptionHolder.gameObject.SetActive(true);

                DescriptionHolder.sizeDelta = new Vector2(300, descriptionHeight);
            }
            else
            {
                DescriptionHolder.gameObject.SetActive(false);
                descriptionHeight = 0;
            }

            //Buttons
            ButtonHolder.transform.localPosition = descriptionHeight * Vector3.down;

            List<MaterialManager> managers = MaterialLibrary.AllMaterialManagers;

            foreach (MaterialManager manager in managers)
            {
                MaterialButton newButton = Instantiate(original: MaterialButtonTemplate.gameObject, parent: ButtonHolder.transform).GetComponent<MaterialButton>();

                Material uiMaterial = new Material(manager.LinkedMaterial);
                uiMaterial.shader = UIShader;

                MaterialButtons.Add(newButton);

                newButton.SetMaterialReference(previewMaterial: uiMaterial, materialReference: manager);

                newButton.AddButtonFunction(delegate { SetMaterial(newButton); });
            }

            float GridSize = 100;

            int rowNumber = managers.Count / 3;
            if (managers.Count % 3 > 0) rowNumber++;

            ButtonHolder.sizeDelta = new Vector2(3 * GridSize, rowNumber * GridSize);

            HiddenContent.sizeDelta = new Vector3(300, descriptionHeight + ButtonHolder.sizeDelta.y);

            Expand = false;
        }
    }
}