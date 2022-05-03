using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class ModificationNodeLibraryIntegrator : MonoBehaviour
    {
        [SerializeField] RadiusModificationNode radiusModificationNodeTemplate = null;
        [SerializeField] BlockGridPositionModificationNode blockGridPositionModificationNodeTemplate = null;
        [SerializeField] NodeGridPositionModificationNode nodeGridPositionModificationNodeTemplate = null;
        [SerializeField] GridRadiusModificationNode gridRadiusModificationNodeTemplate = null;
        [SerializeField] GridOrientationNode gridOrientationNodeTemplate = null;
        [SerializeField] NodeWallEditModNode nodeWallEditMododeTemplate = null;
        [SerializeField] NodeWallRemoveNode nodeWallRemoveNodeTemplate = null;
        [SerializeField] NodeWallMultiModNode nodeWallMultiModNodeTemplate = null;

        public RadiusModificationNode RadiusModificationNodeTemplate
        {
            get
            {
                return radiusModificationNodeTemplate;
            }
        }
        public BlockGridPositionModificationNode BlockGridPositionModificationNodeTemplate
        {
            get
            {
                return blockGridPositionModificationNodeTemplate;
            }
        }

        public NodeGridPositionModificationNode NodeGridPositionModificationNodeTemplate
        {
            get
            {
                return nodeGridPositionModificationNodeTemplate;
            }
        }

        public GridRadiusModificationNode GridRadiusModificationNodeTemplate
        {
            get
            {
                return gridRadiusModificationNodeTemplate;
            }
        }

        public GridOrientationNode GridOrientationNodeTemplate
        {
            get
            {
                return gridOrientationNodeTemplate;
            }
        }

        public NodeWallEditModNode NodeWallEditMododeTemplate
        {
            get
            {
                return nodeWallEditMododeTemplate;
            }
        }

        public NodeWallRemoveNode NodeWallRemoveNodeTemplate
        {
            get
            {
                return nodeWallRemoveNodeTemplate;
            }
        }

        public NodeWallMultiModNode NodeWallMultiModNodeTemplate
        {
            get
            {
                return nodeWallMultiModNodeTemplate;
            }
        }

        public void Setup()
        {
            ModificationNodeLibrary.Setup(integrator: this);
        }
    }

    public static class ModificationNodeLibrary
    {
        //Setup
        static ModificationNodeLibraryIntegrator integrator;

        public static void Setup(ModificationNodeLibraryIntegrator integrator)
        {
            ModificationNodeLibrary.integrator = integrator;
        }


        //RadiusModificationNode
        public static RadiusModificationNode RadiusModificationNodeTemplate
        {
            get
            {
                return integrator.RadiusModificationNodeTemplate;
            }
        }

        public static RadiusModificationNode NewRadiusModificationNode
        {
            get
            {
                return GameObject.Instantiate(RadiusModificationNodeTemplate).transform.GetComponent<RadiusModificationNode>();
            }
        }

        //BlockGridModificationNode
        public static BlockGridPositionModificationNode BlockGridPositionModificationNodeTemplate
        {
            get
            {
                return integrator.BlockGridPositionModificationNodeTemplate;
            }
        }

        public static BlockGridPositionModificationNode NewBlockGridPositionModificationNode
        {
            get
            {
                return GameObject.Instantiate(BlockGridPositionModificationNodeTemplate).transform.GetComponent<BlockGridPositionModificationNode>();
            }
        }

        //NodeGridModificationNode
        public static NodeGridPositionModificationNode NodeGridPositionModificationNodeTemplate
        {
            get
            {
                return integrator.NodeGridPositionModificationNodeTemplate;
            }
        }

        public static NodeGridPositionModificationNode NewNodeGridPositionModificationNode
        {
            get
            {
                return GameObject.Instantiate(NodeGridPositionModificationNodeTemplate).transform.GetComponent<NodeGridPositionModificationNode>();
            }
        }

        //GridRadiusModificationNode
        public static GridRadiusModificationNode GridRadiusModificationNodeTemplate
        {
            get
            {
                return integrator.GridRadiusModificationNodeTemplate;
            }
        }

        public static GridRadiusModificationNode NewGridRadiusModificationNode
        {
            get
            {
                return GameObject.Instantiate(GridRadiusModificationNodeTemplate).transform.GetComponent<GridRadiusModificationNode>();
            }
        }

        //GridOrientationNode
        public static GridOrientationNode GridOrientationNodeTemplate
        {
            get
            {
                return integrator.GridOrientationNodeTemplate;
            }
        }

        public static GridOrientationNode NewGridOrientationNode
        {
            get
            {
                return GameObject.Instantiate(GridOrientationNodeTemplate).transform.GetComponent<GridOrientationNode>();
            }
        }

        //NodeWallEditModNode
        public static NodeWallEditModNode NodeWallEditMododeTemplate
        {
            get
            {
                return integrator.NodeWallEditMododeTemplate;
            }
        }

        public static NodeWallEditModNode NewNodeWallEditModode
        {
            get
            {
                return GameObject.Instantiate(NodeWallEditMododeTemplate).transform.GetComponent<NodeWallEditModNode>();
            }
        }

        //NodeWallEditModNode
        public static NodeWallRemoveNode NodeWallRemoveNodeTemplate
        {
            get
            {
                return integrator.NodeWallRemoveNodeTemplate;
            }
        }

        public static NodeWallRemoveNode NewNodeWallRemoveNode
        {
            get
            {
                return GameObject.Instantiate(NodeWallRemoveNodeTemplate).transform.GetComponent<NodeWallRemoveNode>();
            }
        }

        //NodeWallEditModNode
        public static NodeWallMultiModNode NodeWallMultiModNodeTemplate
        {
            get
            {
                return integrator.NodeWallMultiModNodeTemplate;
            }
        }

        public static NodeWallMultiModNode NewNodeWallMultiModNode
        {
            get
            {
                return GameObject.Instantiate(NodeWallMultiModNodeTemplate).transform.GetComponent<NodeWallMultiModNode>();
            }
        }
    }
}