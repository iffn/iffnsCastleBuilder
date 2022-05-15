using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class SmartBlockUI : MonoBehaviour
    {
        public SmartBlockBuilderTool CurrentSmartBlockBuilderTool;

        [SerializeField] VectorButton RoofRectangular;
        [SerializeField] VectorButton RoofCircular;
        [SerializeField] VectorButton StairRectangular;
        [SerializeField] VectorButton StairCircular;
        [SerializeField] VectorButton ArcHorizontal;
        [SerializeField] VectorButton Door;
        [SerializeField] VectorButton Window;
        [SerializeField] VectorButton ArcVertical;
        [SerializeField] VectorButton RailingLinear;
        [SerializeField] VectorButton RailingArc;
        [SerializeField] VectorButton NonCardinalWall;
        [SerializeField] VectorButton Table;
        [SerializeField] VectorButton Chair;
        [SerializeField] VectorButton Bed;
        [SerializeField] VectorButton Column;
        [SerializeField] VectorButton Counter;
        [SerializeField] VectorButton Ladder;
        [SerializeField] VectorButton Triangle;
        [SerializeField] VectorButton TriangularRoof;
        [SerializeField] VectorButton RoofWall;

        void DisableAllHighlights()
        {
            RoofRectangular.Highlight = false;
            RoofCircular.Highlight = false;
            StairRectangular.Highlight = false;
            ArcHorizontal.Highlight = false;
            Door.Highlight = false;
            Window.Highlight = false;
            ArcVertical.Highlight = false;
            RailingLinear.Highlight = false;
            RailingArc.Highlight = false;
            NonCardinalWall.Highlight = false;
            StairCircular.Highlight = false;
            Table.Highlight = false;
            Chair.Highlight = false;
            Bed.Highlight = false;
            Column.Highlight = false;
            Counter.Highlight = false;
            Ladder.Highlight = false;
            Triangle.Highlight = false;
            TriangularRoof.Highlight = false;
            RoofWall.Highlight = false;
        }

        //Function defined inside Uniti for each button
        public void SetSmartBlockToolType(VectorButton ClickedButton)
        {
            DisableAllHighlights();


            ClickedButton.Highlight = true;

            if (ClickedButton == RoofRectangular)
            {
                CurrentSmartBlockBuilderTool.CurrentSmartBlockToolType = SmartBlockBuilderTool.SmartBlockToolType.RectangularRoof;
            }
            else if (ClickedButton == RoofCircular)
            {
                CurrentSmartBlockBuilderTool.CurrentSmartBlockToolType = SmartBlockBuilderTool.SmartBlockToolType.RoofCircular;
            }
            else if (ClickedButton == StairRectangular)
            {
                CurrentSmartBlockBuilderTool.CurrentSmartBlockToolType = SmartBlockBuilderTool.SmartBlockToolType.RectangularStair;
            }
            else if (ClickedButton == StairCircular)
            {
                CurrentSmartBlockBuilderTool.CurrentSmartBlockToolType = SmartBlockBuilderTool.SmartBlockToolType.CircularStair;
            }
            else if (ClickedButton == ArcHorizontal)
            {
                CurrentSmartBlockBuilderTool.CurrentSmartBlockToolType = SmartBlockBuilderTool.SmartBlockToolType.ArcHorizontal;
            }
            else if (ClickedButton == Door)
            {
                CurrentSmartBlockBuilderTool.CurrentSmartBlockToolType = SmartBlockBuilderTool.SmartBlockToolType.Door;
            }
            else if (ClickedButton == Window)
            {
                CurrentSmartBlockBuilderTool.CurrentSmartBlockToolType = SmartBlockBuilderTool.SmartBlockToolType.Window;
            }
            else if (ClickedButton == ArcVertical)
            {
                CurrentSmartBlockBuilderTool.CurrentSmartBlockToolType = SmartBlockBuilderTool.SmartBlockToolType.VerticalArc;
            }
            else if (ClickedButton == RailingLinear)
            {
                CurrentSmartBlockBuilderTool.CurrentSmartBlockToolType = SmartBlockBuilderTool.SmartBlockToolType.RailingLinear;
            }
            else if (ClickedButton == RailingArc)
            {
                CurrentSmartBlockBuilderTool.CurrentSmartBlockToolType = SmartBlockBuilderTool.SmartBlockToolType.RailingArc;
            }
            else if (ClickedButton == NonCardinalWall)
            {
                CurrentSmartBlockBuilderTool.CurrentSmartBlockToolType = SmartBlockBuilderTool.SmartBlockToolType.NonCardinalWall;
            }
            else if (ClickedButton == Table)
            {
                CurrentSmartBlockBuilderTool.CurrentSmartBlockToolType = SmartBlockBuilderTool.SmartBlockToolType.Table;
            }
            else if (ClickedButton == Chair)
            {
                CurrentSmartBlockBuilderTool.CurrentSmartBlockToolType = SmartBlockBuilderTool.SmartBlockToolType.Chair;
            }
            else if (ClickedButton == Bed)
            {
                CurrentSmartBlockBuilderTool.CurrentSmartBlockToolType = SmartBlockBuilderTool.SmartBlockToolType.Bed;
            }
            else if (ClickedButton == Column)
            {
                CurrentSmartBlockBuilderTool.CurrentSmartBlockToolType = SmartBlockBuilderTool.SmartBlockToolType.Column;
            }
            else if (ClickedButton == Counter)
            {
                CurrentSmartBlockBuilderTool.CurrentSmartBlockToolType = SmartBlockBuilderTool.SmartBlockToolType.Counter;
            }
            else if (ClickedButton == Ladder)
            {
                CurrentSmartBlockBuilderTool.CurrentSmartBlockToolType = SmartBlockBuilderTool.SmartBlockToolType.Ladder;
            }
            else if (ClickedButton == Triangle)
            {
                CurrentSmartBlockBuilderTool.CurrentSmartBlockToolType = SmartBlockBuilderTool.SmartBlockToolType.Triangle;
            }
            else if (ClickedButton == TriangularRoof)
            {
                CurrentSmartBlockBuilderTool.CurrentSmartBlockToolType = SmartBlockBuilderTool.SmartBlockToolType.TriangularRoof;
            }
            else if (ClickedButton == RoofWall)
            {
                CurrentSmartBlockBuilderTool.CurrentSmartBlockToolType = SmartBlockBuilderTool.SmartBlockToolType.RoofWall;
            }
            else
            {
                Debug.Log("Errror in Smart Block builder UI: New smart block type button handling not defined");
            }
        }

        //Used for setting the state at the beginning
        public void SetBlockType(SmartBlockBuilderTool.SmartBlockToolType newSmartBlockToolType)
        {
            CurrentSmartBlockBuilderTool.CurrentSmartBlockToolType = newSmartBlockToolType;

            DisableAllHighlights();

            switch (newSmartBlockToolType)
            {
                case SmartBlockBuilderTool.SmartBlockToolType.RectangularRoof:
                    RoofRectangular.Highlight = true;
                    break;
                case SmartBlockBuilderTool.SmartBlockToolType.RoofCircular:
                    RoofCircular.Highlight = true;
                    break;
                case SmartBlockBuilderTool.SmartBlockToolType.RectangularStair:
                    StairRectangular.Highlight = true;
                    break;
                case SmartBlockBuilderTool.SmartBlockToolType.CircularStair:
                    StairCircular.Highlight = true;
                    break;
                case SmartBlockBuilderTool.SmartBlockToolType.ArcHorizontal:
                    ArcHorizontal.Highlight = true;
                    break;
                case SmartBlockBuilderTool.SmartBlockToolType.Door:
                    Door.Highlight = true;
                    break;
                case SmartBlockBuilderTool.SmartBlockToolType.Window:
                    Window.Highlight = true;
                    break;
                case SmartBlockBuilderTool.SmartBlockToolType.VerticalArc:
                    ArcVertical.Highlight = true;
                    break;
                case SmartBlockBuilderTool.SmartBlockToolType.RailingLinear:
                    RailingLinear.Highlight = true;
                    break;
                case SmartBlockBuilderTool.SmartBlockToolType.RailingArc:
                    RailingArc.Highlight = true;
                    break;
                case SmartBlockBuilderTool.SmartBlockToolType.NonCardinalWall:
                    NonCardinalWall.Highlight = true;
                    break;
                case SmartBlockBuilderTool.SmartBlockToolType.Table:
                    Table.Highlight = true;
                    break;
                case SmartBlockBuilderTool.SmartBlockToolType.Chair:
                    Chair.Highlight = true;
                    break;
                case SmartBlockBuilderTool.SmartBlockToolType.Bed:
                    Bed.Highlight = true;
                    break;
                case SmartBlockBuilderTool.SmartBlockToolType.Column:
                    Column.Highlight = true;
                    break;
                case SmartBlockBuilderTool.SmartBlockToolType.Counter:
                    Counter.Highlight = true;
                    break;
                case SmartBlockBuilderTool.SmartBlockToolType.Ladder:
                    Ladder.Highlight = true;
                    break;
                case SmartBlockBuilderTool.SmartBlockToolType.Triangle:
                    Triangle.Highlight = true;
                    break;
                case SmartBlockBuilderTool.SmartBlockToolType.TriangularRoof:
                    TriangularRoof.Highlight = true;
                    break;
                case SmartBlockBuilderTool.SmartBlockToolType.RoofWall:
                    RoofWall.Highlight = true;
                    break;
                default:
                    Debug.Log("Errror in Smart Block builder UI: New tool type enum handling not defined");
                    break;
            }
        }

        public void Setup()
        {

        }

        // Start is called before the first frame update
        void Start()
        {
            RoofRectangular.Highlight = true;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}