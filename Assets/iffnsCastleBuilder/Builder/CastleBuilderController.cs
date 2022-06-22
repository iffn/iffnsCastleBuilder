using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class CastleBuilderController : MonoBehaviour
    {
        public CastleTestWorldController WorldController;

        [HideInInspector] public CastleController CurrentBuilding;
        public BuildingToolController CurrentBuildingToolController;

        public void Setup(CastleTestWorldController worldController)
        {
            CurrentBuildingToolController.Setup();

            WorldController = worldController;
        }

        public void UpdateIU()
        {

        }
    }
}