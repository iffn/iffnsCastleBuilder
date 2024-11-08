﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class BlockWallBuilderUI : MonoBehaviour
    {
        [SerializeField] BlockWallBuilderTool CurrentWallBuilderTool;

        [SerializeField] VectorButton WallButton;
        [SerializeField] VectorButton EmptyBlockButton;
        [SerializeField] VectorButton FloorButton;
        [SerializeField] VectorButton CopyUpWithFloor;
        [SerializeField] VectorButton CopyUpWithoutFloor;

        [SerializeField] VectorButton SingleWallButton;
        [SerializeField] VectorButton CardinalWallButton;
        [SerializeField] VectorButton FullBlockButton;
        [SerializeField] VectorButton HollowBlockButton;

        void UnhighlightAllBlockTypeButtons()
        {
            WallButton.Highlight = false;
            EmptyBlockButton.Highlight = false;
            FloorButton.Highlight = false;
            CopyUpWithFloor.Highlight = false;
            CopyUpWithoutFloor.Highlight = false;
        }

        public void SetBlockToolType(VectorButton ClickedButton)
        {
            UnhighlightAllBlockTypeButtons();

            ClickedButton.Highlight = true;

            if (ClickedButton == WallButton)
            {
                CurrentWallBuilderTool.CurrentBlockToolType = BlockWallBuilderTool.BlockToolType.Wall;
            }
            else if (ClickedButton == EmptyBlockButton)
            {
                CurrentWallBuilderTool.CurrentBlockToolType = BlockWallBuilderTool.BlockToolType.Empty;
            }
            else if (ClickedButton == FloorButton)
            {
                CurrentWallBuilderTool.CurrentBlockToolType = BlockWallBuilderTool.BlockToolType.Floor;
            }
            else if (ClickedButton == CopyUpWithFloor)
            {
                CurrentWallBuilderTool.CurrentBlockToolType = BlockWallBuilderTool.BlockToolType.CopyUpWithFloor;
            }
            else if (ClickedButton == CopyUpWithoutFloor)
            {
                CurrentWallBuilderTool.CurrentBlockToolType = BlockWallBuilderTool.BlockToolType.CopyUpWithoutFloor;
            }
            else
            {
                Debug.Log("Errror in wall builder UI: New block type button handling not defined");
            }
        }

        public void SetToolType(VectorButton ClickedButton)
        {
            SingleWallButton.Highlight = false;
            CardinalWallButton.Highlight = false;
            FullBlockButton.Highlight = false;
            HollowBlockButton.Highlight = false;

            ClickedButton.Highlight = true;

            if (ClickedButton == SingleWallButton)
            {
                CurrentWallBuilderTool.CurrentDrawToolType = BlockWallBuilderTool.DrawToolType.Dot;
            }
            else if (ClickedButton == CardinalWallButton)
            {
                CurrentWallBuilderTool.CurrentDrawToolType = BlockWallBuilderTool.DrawToolType.CardinalWall;
            }
            else if (ClickedButton == FullBlockButton)
            {
                CurrentWallBuilderTool.CurrentDrawToolType = BlockWallBuilderTool.DrawToolType.FullBlock;
            }
            else if (ClickedButton == HollowBlockButton)
            {
                CurrentWallBuilderTool.CurrentDrawToolType = BlockWallBuilderTool.DrawToolType.HollowBlock;
            }
        }

        public void SetBlockType(BlockWallBuilderTool.BlockToolType newBlockType)
        {
            CurrentWallBuilderTool.CurrentBlockToolType = newBlockType;

            UnhighlightAllBlockTypeButtons();

            switch (newBlockType)
            {
                case BlockWallBuilderTool.BlockToolType.Wall:
                    WallButton.Highlight = true;
                    break;
                case BlockWallBuilderTool.BlockToolType.Empty:
                    EmptyBlockButton.Highlight = true;
                    break;
                case BlockWallBuilderTool.BlockToolType.Floor:
                    FloorButton.Highlight = true;
                    break;
                case BlockWallBuilderTool.BlockToolType.CopyUpWithFloor:
                    CopyUpWithFloor.Highlight = true;
                    break;
                case BlockWallBuilderTool.BlockToolType.CopyUpWithoutFloor:
                    CopyUpWithoutFloor.Highlight = true;
                    break;
                default:
                    Debug.Log("Errror in Wall Builder UI: New block type enum handling not defined");
                    break;
            }
        }

        public void SetToolType(BlockWallBuilderTool.DrawToolType newToolType)
        {
            CurrentWallBuilderTool.CurrentDrawToolType = newToolType;

            SingleWallButton.Highlight = false;
            CardinalWallButton.Highlight = false;
            FullBlockButton.Highlight = false;
            HollowBlockButton.Highlight = false;

            switch (newToolType)
            {
                case BlockWallBuilderTool.DrawToolType.Dot:
                    SingleWallButton.Highlight = true;
                    break;
                case BlockWallBuilderTool.DrawToolType.CardinalWall:
                    CardinalWallButton.Highlight = true;
                    break;
                case BlockWallBuilderTool.DrawToolType.FullBlock:
                    FullBlockButton.Highlight = true;
                    break;
                case BlockWallBuilderTool.DrawToolType.HollowBlock:
                    HollowBlockButton.Highlight = true;
                    break;
                default:
                    Debug.Log("Errror in Wall Builder UI: New tool type enum handling not defined");
                    break;
            }
        }

        void Start()
        {
            SetBlockToolType(WallButton);
            SetToolType(SingleWallButton);
        }
    }
}