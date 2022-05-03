using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class VirtualBlock : BaseVirtualObject
    {
        public const string staticIdentifierString = "Virtual block";

        public override string IdentifierString
        {
            get
            {
                return staticIdentifierString;
            }
        }

        //Build parameters
        public MailboxLineDistinctNamed BlockTypeParam;
        public MailboxLineMaterial CeilingMaterialParam;
        public MailboxLineMaterial FloorMaterialParam;
        public MailboxLineMaterial LeftWallMaterialParam;
        public MailboxLineMaterial RightWallMaterialParam;
        public MailboxLineMaterial FrontWallMaterialParam;
        public MailboxLineMaterial BackWallMaterialParam;

        public Material CeilingMaterial
        {
            get
            {
                if (CeilingMaterialParam == null) return null;
                return CeilingMaterialParam.Val.LinkedMaterial;
            }
        }

        public Material FloorMaterial
        {
            get
            {
                if (FloorMaterialParam == null) return null;
                return FloorMaterialParam.Val.LinkedMaterial;
            }
        }

        public Material LeftWallMaterial
        {
            get
            {
                if (LeftWallMaterialParam == null) return null;
                return LeftWallMaterialParam.Val.LinkedMaterial;
            }
        }

        public Material RightWallMaterial
        {
            get
            {
                if (RightWallMaterialParam == null) return null;
                return RightWallMaterialParam.Val.LinkedMaterial;
            }
        }

        public Material FrontWallMaterial
        {
            get
            {
                if (FrontWallMaterialParam == null) return null;
                return FrontWallMaterialParam.Val.LinkedMaterial;
            }
        }

        public Material BackWallMaterial
        {
            get
            {
                if (BackWallMaterialParam == null) return null;
                return BackWallMaterialParam.Val.LinkedMaterial;
            }
        }

        public void GenerateMailboxLinesBasedOnShapeInfo()
        {
            if (!CurrentShapeInfo.HasFloorAndCeiling)
            {
                if (CeilingMaterialParam != null)
                {
                    CeilingMaterialParam.Destroy();
                    CeilingMaterialParam = null;
                }

                if (FloorMaterialParam != null)
                {
                    FloorMaterialParam.Destroy();
                    FloorMaterialParam = null;
                }

                if (LeftWallMaterialParam != null)
                {
                    LeftWallMaterialParam.Destroy();
                    LeftWallMaterialParam = null;
                }

                if (RightWallMaterialParam != null)
                {
                    RightWallMaterialParam.Destroy();
                    RightWallMaterialParam = null;
                }

                if (FrontWallMaterialParam != null)
                {
                    FrontWallMaterialParam.Destroy();
                    FrontWallMaterialParam = null;
                }

                if (BackWallMaterialParam != null)
                {
                    BackWallMaterialParam.Destroy();
                    BackWallMaterialParam = null;
                }
            }
            else
            {
                if (CeilingMaterialParam == null)
                {
                    CeilingMaterialParam = new MailboxLineMaterial(name: "Ceiling material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultCeiling);

                }

                if (FloorMaterialParam == null)
                {
                    switch (BlockType)
                    {
                        case BlockTypes.Floor:
                            FloorMaterialParam = new MailboxLineMaterial(name: "Top cap material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultWoodPlanks);
                            break;
                        case BlockTypes.Wall:
                            FloorMaterialParam = new MailboxLineMaterial(name: "Top cap material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultStoneBricks);
                            break;
                    }
                }

                if (CurrentShapeInfo.LeftWallType == ShapeInfo.WallTypes.None)
                {
                    if (LeftWallMaterialParam != null)
                    {
                        LeftWallMaterialParam.Destroy();
                        LeftWallMaterialParam = null;
                    }
                }
                else
                {
                    if (LeftWallMaterialParam == null)
                    {
                        switch (BlockType)
                        {
                            case BlockTypes.Floor:
                                LeftWallMaterialParam = new MailboxLineMaterial(name: "Left wall material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultWoodSolid);
                                break;
                            case BlockTypes.Wall:
                                LeftWallMaterialParam = new MailboxLineMaterial(name: "Left wall material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultStoneBricks);
                                break;
                        }
                    }
                }

                if (CurrentShapeInfo.RightWallType == ShapeInfo.WallTypes.None)
                {
                    if (RightWallMaterialParam != null)
                    {
                        RightWallMaterialParam.Destroy();
                        RightWallMaterialParam = null;
                    }
                }
                else
                {
                    if (RightWallMaterialParam == null)
                    {
                        switch (BlockType)
                        {
                            case BlockTypes.Floor:
                                RightWallMaterialParam = new MailboxLineMaterial(name: "Right wall material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultWoodSolid);
                                break;
                            case BlockTypes.Wall:
                                RightWallMaterialParam = new MailboxLineMaterial(name: "Right wall material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultStoneBricks);
                                break;
                        }
                    }
                }

                if (CurrentShapeInfo.FrontWallType == ShapeInfo.WallTypes.None)
                {
                    if (FrontWallMaterialParam != null)
                    {
                        FrontWallMaterialParam.Destroy();
                        FrontWallMaterialParam = null;
                    }
                }
                else
                {
                    if (FrontWallMaterialParam == null)
                    {
                        switch (BlockType)
                        {
                            case BlockTypes.Floor:
                                FrontWallMaterialParam = new MailboxLineMaterial(name: "Front wall material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultWoodSolid);
                                break;
                            case BlockTypes.Wall:
                                FrontWallMaterialParam = new MailboxLineMaterial(name: "Front wall material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultStoneBricks);
                                break;
                        }
                    }
                }

                if (CurrentShapeInfo.BackWallType == ShapeInfo.WallTypes.None)
                {
                    if (BackWallMaterialParam != null)
                    {
                        BackWallMaterialParam.Destroy();
                        BackWallMaterialParam = null;
                    }
                }
                else
                {
                    if (BackWallMaterialParam == null)
                    {
                        switch (BlockType)
                        {
                            case BlockTypes.Floor:
                                BackWallMaterialParam = new MailboxLineMaterial(name: "Back wall material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultWoodSolid);
                                break;
                            case BlockTypes.Wall:
                                BackWallMaterialParam = new MailboxLineMaterial(name: "Back wall material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultStoneBricks);
                                break;
                        }
                    }
                }
            }
        }

        public ShapeInfo CurrentShapeInfo { get; private set; }

        public class ShapeInfo
        {
            public enum WallTypes
            {
                None,
                Floor,
                WallFull,
                WallCutoff
            }

            public VirtualBlock LinkedBlock { get; private set; }

            public WallTypes RightWallType = WallTypes.None;
            public WallTypes LeftWallType = WallTypes.None;
            public WallTypes FrontWallType = WallTypes.None;
            public WallTypes BackWallType = WallTypes.None;

            bool hasFloorAndCeiling = false;

            public ShapeInfo(VirtualBlock linkedBlock)
            {
                LinkedBlock = linkedBlock;
            }

            public bool HasFloorAndCeiling
            {
                set
                {
                    if (!value)
                    {
                        RightWallType = WallTypes.None;
                        LeftWallType = WallTypes.None;
                        FrontWallType = WallTypes.None;
                        BackWallType = WallTypes.None;
                    }

                    hasFloorAndCeiling = value;
                }
                get
                {
                    return hasFloorAndCeiling;
                }
            }
        }

        public VirtualBlock(IBaseObject superObject) : base(superObject: superObject)
        {
            linkedFloorController = superObject as FloorController;
            SetupBuildParameters();

            CurrentShapeInfo = new ShapeInfo(linkedBlock: this);
        }

        public VirtualBlock(int xPosition, int zPosition, FloorController linkedFloorController, BlockTypes blockType) : base(superObject: linkedFloorController)
        {
            position = new Vector2Int(xPosition, zPosition);

            this.linkedFloorController = linkedFloorController;

            SetupBuildParameters();

            BlockType = blockType;

            CurrentShapeInfo = new ShapeInfo(linkedBlock: this);
        }

        public void DefinePositionValue(Vector2Int position)
        {
            this.position = position;
        }

        public override void ResetObject()
        {
            baseReset();
        }

        Vector2Int position;

        public Vector2Int Coordinate
        {
            get
            {
                return new Vector2Int(position.x, position.y);
            }
        }

        //Fixed parameters
        //int xPosition;

        public int XCoordinate
        {
            get
            {
                return position.x;
            }
        }

        //int zPosition;
        public int ZCoordinate
        {
            get
            {
                return position.y;
            }
        }

        public Vector3 BottomLeftNodePosition
        {
            get
            {
                return linkedFloorController.GetLocalNodePositionFromNodeIndex(position);
            }
        }

        FloorController linkedFloorController;
        public FloorController LinkedFloorController
        {
            get
            {
                return linkedFloorController;
            }
        }

        /*
        public void SetupFixedParameters(int xPosition, int zPosition, FloorController linkedFloorController)
        {
            this.xPosition = xPosition;
            this.zPosition = zPosition;
            this.linkedFloorController = linkedFloorController;
        }
        */


        public enum BlockTypes
        {
            Empty,
            Floor,
            Wall
        }

        public BlockTypes BlockType
        {
            get
            {
                BlockTypes returnValue = (BlockTypes)BlockTypeParam.Val;

                return returnValue;
            }
            set
            {
                BlockTypes previousBlock = BlockType;

                BlockTypeParam.Val = (int)value;

                //Repaint
                if (previousBlock == BlockTypes.Floor && value == BlockTypes.Wall)
                {
                    AssignValueToAllSides(newTop: DefaultCastleMaterials.DefaultStoneBricks, newSide: DefaultCastleMaterials.DefaultStoneBricks);
                }
                else if (previousBlock == BlockTypes.Wall && value == BlockTypes.Floor)
                {
                    AssignValueToAllSides(newTop: DefaultCastleMaterials.DefaultWoodPlanks, newSide: DefaultCastleMaterials.DefaultWoodSolid);
                }
            }
        }

        void AssignValueToAllSides(MaterialManager newTop, MaterialManager newSide)
        {
            FloorMaterialParam.Val = newTop;
            if (LeftWallMaterialParam != null) LeftWallMaterialParam.Val = newSide;
            if (RightWallMaterialParam != null) RightWallMaterialParam.Val = newSide;
            if (FrontWallMaterialParam != null) FrontWallMaterialParam.Val = newSide;
            if (BackWallMaterialParam != null) BackWallMaterialParam.Val = newSide;
        }

        void SetupBlockTypeParam()
        {
            List<string> enumString = new List<string>();

            int enumValues = System.Enum.GetValues(typeof(BlockTypes)).Length;

            for (int i = 0; i < enumValues; i++) //Note, 
            {
                BlockTypes type = (BlockTypes)i;

                enumString.Add(type.ToString());
            }

            BlockTypeParam = new MailboxLineDistinctNamed(
                "Block type",
                CurrentMailbox,
                Mailbox.ValueType.buildParameter,
                enumString,
                0);
        }

        void SetupBuildParameters()
        {
            //FloorOrWallTextureParam = new MailboxLineString("Floor or wall texture", CurrentMailbox, Mailbox.ValueType.buildParameter, "");
            //CeilingTextureParam = new MailboxLineString("Ceiling texture", CurrentMailbox, Mailbox.ValueType.buildParameter, "");

            SetupBlockTypeParam();

            CeilingMaterialParam = new MailboxLineMaterial(name: "Ceiling material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultCeiling);
            FloorMaterialParam = new MailboxLineMaterial(name: "Top cap material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultWoodPlanks);
            FrontWallMaterialParam = new MailboxLineMaterial(name: "Front wall material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultStoneBricks);
            BackWallMaterialParam = new MailboxLineMaterial(name: "Back wall material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultStoneBricks);
            LeftWallMaterialParam = new MailboxLineMaterial(name: "Left wall material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultStoneBricks);
            RightWallMaterialParam = new MailboxLineMaterial(name: "Right wall material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultStoneBricks);
        }


        public void SetBuildParameters(BlockTypes blockType)
        {
            BlockType = blockType;
        }

        public override void ApplyBuildParameters()
        {
            NonOrderedApplyBuildParameters();
        }

        public override void InternalUpdate()
        {
            NonOrderedInternalUpdate();
        }

        public override void PlaytimeUpdate()
        {
            NonOrderedPlaytimeUpdate();
        }
    }
}