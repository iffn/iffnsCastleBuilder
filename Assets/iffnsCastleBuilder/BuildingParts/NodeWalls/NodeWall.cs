using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    
    public interface NodeWallReference
    {
        public Vector2Int StartPosition { get; set; }
        public Vector2Int EndPosition { get; set; }
        public Vector2Int Offset { get;}
        public MailboxLineMaterial CornerMaterial { get;}
    }

    public class NodeWall : BaseVirtualObject, NodeWallReference
    {
        MailboxLineVector2Int startPositionParam;
        MailboxLineVector2Int endPositionParam;
        public MailboxLineMaterial RightMaterialParam { get; private set; }
        public MailboxLineMaterial LeftMaterialParam { get; private set; }
        public List<NodeWallNode> NodesFromStartToEnd;

        NodeWallSystem linkedSystem;
        public NodeWallSystem LinkedSystem
        {
            get
            {
                return linkedSystem;
            }
        }

        public Vector2Int StartPosition
        {
            get
            {
                return startPositionParam.Val;
            }
            set
            {
                startPositionParam.Val = value;
                EvaluateFailureState();
                linkedSystem.ApplyBuildParameters();
            }
        }

        public Vector2Int EndPosition
        {
            get
            {
                return endPositionParam.Val;
            }
            set
            {
                endPositionParam.Val = value;
                EvaluateFailureState();
                linkedSystem.ApplyBuildParameters();
            }
        }

        public Vector2Int Offset
        {
            get
            {
                return EndPosition - StartPosition;
            }
        }

        bool FailureState
        {
            get
            {
                if (!linkedSystem.LinkedFloor.LinkedBuildingController.NodeCoordinateIsOnGrid(StartPosition)) 
                    return true;
                if (!linkedSystem.LinkedFloor.LinkedBuildingController.NodeCoordinateIsOnGrid(EndPosition)) 
                    return true;

                if(StartPosition.x == EndPosition.x && StartPosition.y == EndPosition.y) 
                    return true;

                return false;
            }
        }
        
        public void EvaluateFailureState()
        {
            Failed = FailureState;
        }

        public void Move(Vector2Int offset)
        {
            startPositionParam.Val += offset;
            endPositionParam.Val += offset;
            
            EvaluateFailureState();
            if (Failed) return;

            linkedSystem.ApplyBuildParameters();

            //Reduce if nodes in between
            /*
            while (true)
            {


                if (newStartPosition.x > linkedSystem.NodeGridSize.x || newStartPosition.y > linkedSystem.NodeGridSize.y)
                {
                    if(NodesFromStartToEnd.Count > 0)
                    {
                        StartPosition = NodesFromStartToEnd[0].Coordinate;
                        NodesFromStartToEnd.RemoveAt(0);
                    }
                    else
                    {
                        DestroyObject();
                    }
                }
                //if (newEndPosition.x > linkedSystem.NodeGridSize.x || newEndPosition.y > linkedSystem.NodeGridSize.y)
            }
            */
        }

        void SetMaterialLines()
        {
            RightMaterialParam = new MailboxLineMaterial(name: "Right material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultPlaster);
            LeftMaterialParam = new MailboxLineMaterial(name: "Left material", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: DefaultCastleMaterials.DefaultPlaster);
        }

        public MailboxLineMaterial CornerMaterial
        {
            get
            {
                return RightMaterialParam;
            }
        }

        public NodeWall(IBaseObject superObject) : base(superObject: superObject)
        {
            startPositionParam = new MailboxLineVector2Int(name: "Start position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            endPositionParam = new MailboxLineVector2Int(name: "End position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter);
            SetMaterialLines();

            NodesFromStartToEnd = new List<NodeWallNode>();

            if (superObject is NodeWallSystem)
            {
                NodeWallSystem system = superObject as NodeWallSystem;

                system.AddNodeWall(this);
                linkedSystem = system;
            }
            else
            {
                Debug.LogWarning("Error: Super object of node wall is not a node wall system");
            }

        }

        public NodeWall(NodeWallSystem linkedSystem, Vector2Int startPosition, Vector2Int endPosition) : base(superObject: linkedSystem)
        {
            startPositionParam = new MailboxLineVector2Int(name: "Start position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: startPosition);
            endPositionParam = new MailboxLineVector2Int(name: "End position", objectHolder: CurrentMailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: endPosition);
            SetMaterialLines();

            NodesFromStartToEnd = new List<NodeWallNode>();

            this.linkedSystem = linkedSystem;
            linkedSystem.AddNodeWall(this);
        }

        public override void ResetObject()
        {
            baseReset();

            NodesFromStartToEnd.Clear();
        }

        public override void DestroyObject()
        {
            base.DestroyObject();

            //linkedSystem.RemoveNodeWall(this);
            linkedSystem.ApplyBuildParameters();
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

    public class DummyNodeWall : NodeWallReference
    {
        public OnFloorObject LinkedObject { get; set; }
        public Vector2Int StartPosition { get; set; }
        public Vector2Int EndPosition { get; set; }
        public MailboxLineMaterial CornerMaterial { get; set; }

        public Vector2Int Offset
        {
            get
            {
                return EndPosition - StartPosition;
            }
        }

        public DummyNodeWall(Vector2Int startPosition, Vector2Int endPosition, MailboxLineMaterial cornerMaterial, OnFloorObject linkedObject)
        {
            this.StartPosition = startPosition;
            this.EndPosition = endPosition;
            this.CornerMaterial = cornerMaterial;
            this.LinkedObject = linkedObject;
        }
    }
}