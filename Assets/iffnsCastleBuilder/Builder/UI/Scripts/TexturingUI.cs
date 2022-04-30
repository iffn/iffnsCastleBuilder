using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TexturingUI : MonoBehaviour
{
    [SerializeField] PainterTool LinkedPainterTool;
    [SerializeField] Shader UIShader = null;
    [SerializeField] MaterialButton MaterialButtonTemplate = null;
    [SerializeField] VectorButton PainterButton = null;
    [SerializeField] VectorButton PaintSqureButton = null;
    [SerializeField] VectorButton PipetteButton = null;
    [SerializeField] GameObject MaterialButtonHolder = null;

    List<MaterialButton> MaterialButtons = new List<MaterialButton>();

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

        if(ClickedButton == PainterButton)
        {
            LinkedPainterTool.CurrentToolType = PainterTool.ToolType.Painter;
        }
        else if (ClickedButton == PaintSqureButton)
        {
            LinkedPainterTool.CurrentToolType = PainterTool.ToolType.PaintRectangle;
        }
        else if(ClickedButton == PipetteButton)
        {
            LinkedPainterTool.CurrentToolType = PainterTool.ToolType.Pipette;
        }
    }

    public void SetMaterial(MaterialButton clickedButton)
    {
        LinkedPainterTool.currentMaterial = clickedButton.MaterialReference;

        foreach(MaterialButton button in MaterialButtons)
        {
            button.Highlight = false;
        }

        clickedButton.Highlight = true;
    }

    public void SetMaterial(MaterialManager manager)
    {
        foreach (MaterialButton button in MaterialButtons)
        {
            if (button.MaterialReference == manager)
            {
                SetMaterial(button);
                break;
            }
        }
    }

    public void Setup()
    {
        SetToolType(PainterButton);

        foreach (MaterialManager manager in MaterialLibrary.AllMaterialManagers)
        {
            Material uiMaterial = new Material(manager.LinkedMaterial);
            uiMaterial.shader = UIShader;

            MaterialButton newButton = GameObject.Instantiate(original: MaterialButtonTemplate.gameObject, parent: MaterialButtonHolder.transform).GetComponent<MaterialButton>();

            MaterialButtons.Add(newButton);

            newButton.SetMaterialReference(previewMaterial: uiMaterial, materialReference: manager);

            newButton.transform.GetComponent<Button>().onClick.AddListener(delegate { SetMaterial(newButton); });
        }

        //Remove invisible material
        GameObject.Destroy(MaterialButtons[0].gameObject);
        MaterialButtons.RemoveAt(0);

        SetMaterial(MaterialButtons[0]);
    }
}
