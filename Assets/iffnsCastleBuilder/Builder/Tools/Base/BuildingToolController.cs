using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class BuildingToolController : MonoBehaviour
    {
        public NavigationTools CurrentNavigationTools;
        public BlockWallBuilderTool CurrentWallBuilderTool;
        public PainterTool CurrentPainterTool;
        public SmartBlockBuilderTool CurrentSmartBlockBuilderTool;
        public CopyPasteTool CurrentCopyPasteTool;
        public PipetteTool CurrentPipetteTool;
        public SaveAndLoadSystem CurrentSaveAndLoadSystem;


        //bool NavigationToolActivationState;
        bool BlockWallBuilderToolActivationState;
        bool PainterToolActivationState;
        bool SmartBlockBuilderToolActivationState;
        bool CopyPasteToolActivationState;
        bool PipetteToolActivationState;
        //bool SaveAndLoadSystemActivationState;

        public bool ToolActivationState
        {
            set
            {
                if (!value)
                {
                    BlockWallBuilderToolActivationState = CurrentWallBuilderTool.gameObject.activeSelf;
                    PainterToolActivationState = CurrentPainterTool.gameObject.activeSelf;
                    SmartBlockBuilderToolActivationState = CurrentSmartBlockBuilderTool.gameObject.activeSelf;
                    CopyPasteToolActivationState = CurrentCopyPasteTool.gameObject.activeSelf;
                    PipetteToolActivationState = CurrentPipetteTool.gameObject.activeSelf;

                    CurrentWallBuilderTool.gameObject.SetActive(false);
                    CurrentPainterTool.gameObject.SetActive(false);
                    CurrentSmartBlockBuilderTool.gameObject.SetActive(false);
                    CurrentCopyPasteTool.gameObject.SetActive(false);
                    CurrentPipetteTool.gameObject.SetActive(false);
                }
                else
                {
                    CurrentWallBuilderTool.gameObject.SetActive(BlockWallBuilderToolActivationState);
                    CurrentPainterTool.gameObject.SetActive(PainterToolActivationState);
                    CurrentSmartBlockBuilderTool.gameObject.SetActive(SmartBlockBuilderToolActivationState);
                    CurrentCopyPasteTool.gameObject.SetActive(CopyPasteToolActivationState);
                    CurrentPipetteTool.gameObject.SetActive(PipetteToolActivationState);
                }
            }
        }

        public void Setup()
        {
            CurrentNavigationTools.Setup();
            CurrentPainterTool.Setup();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}