using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using iffnsStuff.iffnsBaseSystemForUnity;
using System.Linq;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class TexturingUI : MonoBehaviour
    {
        [SerializeField] PainterTool linkedPainterTool;
        [SerializeField] MaterialLibraryUI MaterialLibraryUITemplate = null;
        [SerializeField] VectorButton PainterButton = null;
        [SerializeField] VectorButton PaintRectangleButton = null;
        [SerializeField] VectorButton PipetteButton = null;
        [SerializeField] GameObject MaterialLibraryHolder = null;
        [SerializeField] VectorButton IlluminationDisabledButton = null;
        [SerializeField] VectorButton IlluminationEnabledButton = null;

        List<MaterialLibraryUI> libraryUIs = new List<MaterialLibraryUI>();
        MaterialLibraryUI previousLibrary;
        public PainterTool LinkedPainterTool
        {
            get { return linkedPainterTool; }
            set { linkedPainterTool = value; }
        }

        void UnhighlightAllBlockTypeButtons()
        {
            PainterButton.Highlight = false;
            PaintRectangleButton.Highlight = false;
            PipetteButton.Highlight = false;
        }

        public void SetToolType(VectorButton ClickedButton)
        {
            UnhighlightAllBlockTypeButtons();

            ClickedButton.Highlight = true;

            if (ClickedButton == PainterButton)
            {
                linkedPainterTool.CurrentToolType = PainterTool.ToolType.Painter;
            }
            else if (ClickedButton == PaintRectangleButton)
            {
                linkedPainterTool.CurrentToolType = PainterTool.ToolType.PaintRectangle;
            }
            else if (ClickedButton == PipetteButton)
            {
                linkedPainterTool.CurrentToolType = PainterTool.ToolType.Pipette;
                UnhighlightIlluminationButtons();
            }
        }

        public void SetToolType(PainterTool.ToolType tool)
        {
            UnhighlightAllBlockTypeButtons();

            switch (tool)
            {
                case PainterTool.ToolType.Painter:
                    PainterButton.Highlight = true;
                    break;
                case PainterTool.ToolType.PaintRectangle:
                    PaintRectangleButton.Highlight = true;
                    break;
                case PainterTool.ToolType.Pipette:
                    PipetteButton.Highlight = true;
                    UnhighlightIlluminationButtons();
                    break;
                default:
                    break;
            }
        }

        public void UnhighlightIlluminationButtons()
        {
            IlluminationDisabledButton.Highlight = false;
            IlluminationEnabledButton.Highlight = false;
        }

        public void SelectIlluminationType(VectorButton ClickedButton)
        {
            UnhighlightIlluminationButtons();
            UnhighlightPreviousMaterialButton(null);

            ClickedButton.Highlight = true;

            if (ClickedButton == IlluminationDisabledButton)
            {
                linkedPainterTool.IlluminationToolState = PainterTool.IlluminationToolStates.DisableIllumination;
            }
            else if(ClickedButton == IlluminationEnabledButton)
            {
                linkedPainterTool.IlluminationToolState = PainterTool.IlluminationToolStates.Illuminate;
            }
        }

        public void UnhighlightPreviousMaterialButton(MaterialLibraryUI newLibrary)
        {
            if (previousLibrary != null) previousLibrary.UnhighlightPrevious();
            previousLibrary = newLibrary;
        }

        public void SetMaterial(MaterialManager manager)
        {
            bool found = false;

            if (previousLibrary != null) previousLibrary.UnhighlightPrevious();

            foreach (MaterialLibraryUI library in libraryUIs)
            {
                if (library.SetMaterialAndExpansion(manager))
                {
                    found = true;
                    previousLibrary = library;
                    break;
                }
            }

            if (!found)
            {
                Debug.LogWarning("Error: Could not find material " + manager.Identifier + "in UI libraries");
            }
        }

        public void Setup()
        {
            SetToolType(PainterButton);
            
            List<MaterialLibraryExtenderTemplate> libraries = MaterialLibrary.AllLibraryExtendersForUser;

            /*
            for(int i = 0; i<managers.Count; i++)
            {
                MaterialManager manager = managers[i];

                if (ExcludedMaterialList.Materials.Contains(manager.LinkedMaterial))
                {
                    managers.RemoveAt(i);
                    i--;
                }
            }
            */

            foreach (MaterialLibraryExtenderTemplate library in libraries)
            {
                MaterialLibraryUI currentLibraryUI = Instantiate(original: MaterialLibraryUITemplate.gameObject, parent: MaterialLibraryHolder.transform).GetComponent<MaterialLibraryUI>();
                libraryUIs.Add(currentLibraryUI);

                currentLibraryUI.Setup(library: library, linkedTexturingUI: this);
            }

            libraryUIs[0].ExpandAndActivateFirstMaterial();
        }
    }
}