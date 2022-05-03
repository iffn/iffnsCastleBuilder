using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class HumanBuilderController : MonoBehaviour
    {
        public HumanBuildingTestWorldController WorldController;

        [HideInInspector] public HumanBuildingController CurrentBuilding;
        public BuildingToolController CurrentBuildingToolController;

        public void Setup(HumanBuildingTestWorldController worldController)
        {
            CurrentBuildingToolController.Setup();

            WorldController = worldController;
        }

        public void UpdateIU()
        {

        }
    }
}