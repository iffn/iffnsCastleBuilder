using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class NodeWallBuilderUI : MonoBehaviour
    {
        [SerializeField] NodeWallBuilderTool CurrentNodeWallBuilderTool;

        [SerializeField] VectorButton LineButton;
        [SerializeField] VectorButton RectangleButton;
        [SerializeField] VectorButton EditButton;

        public void SetToolType(VectorButton ClickedButton)
        {
            LineButton.Highlight = false;
            RectangleButton.Highlight = false;
            EditButton.Highlight = false;
            ClickedButton.Highlight = true;

            if (ClickedButton == LineButton)
            {
                CurrentNodeWallBuilderTool.CurrentToolType = NodeWallBuilderTool.ToolType.Line;
            }
            else if (ClickedButton == RectangleButton)
            {
                CurrentNodeWallBuilderTool.CurrentToolType = NodeWallBuilderTool.ToolType.Rectangle;
            }
            else if (ClickedButton == EditButton)
            {
                CurrentNodeWallBuilderTool.CurrentToolType = NodeWallBuilderTool.ToolType.Edit;
            }
            else
            {
                Debug.LogWarning("Error: Node wall button not known, name = " + ClickedButton.name);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            SetToolType(LineButton);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}