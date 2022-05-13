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
        [SerializeField] VectorButton PaintSqureButton = null;
        [SerializeField] VectorButton PipetteButton = null;
        [SerializeField] GameObject MaterialLibraryHolder = null;

        List<MaterialLibraryUI> libraryUIs = new List<MaterialLibraryUI>();

        public PainterTool LinkedPainterTool
        {
            get { return linkedPainterTool; }
            set { linkedPainterTool = value; }
        }

        void UnhighlightAllBlockTypeButtons()
        {
            PainterButton.Highlight = false;
            PaintSqureButton.Highlight = false;
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
            else if (ClickedButton == PaintSqureButton)
            {
                linkedPainterTool.CurrentToolType = PainterTool.ToolType.PaintRectangle;
            }
            else if (ClickedButton == PipetteButton)
            {
                linkedPainterTool.CurrentToolType = PainterTool.ToolType.Pipette;
            }
        }


        public void SetMaterial(MaterialManager manager)
        {
            bool found = false;

            foreach (MaterialLibraryUI library in libraryUIs)
            {
                if (library.SetMaterialAndExpansion(manager))
                {
                    found = true;
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