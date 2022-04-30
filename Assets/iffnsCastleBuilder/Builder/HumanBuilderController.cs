using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanBuilderController : MonoBehaviour
{
    public HumanBuildingTestWorldController WorldController;

    public HumanBuildingController CurrentBuilding;
    public BuildingToolController CurrentBuildingToolController;

    public void Setup(HumanBuildingTestWorldController worldController)
    {
        CurrentBuildingToolController.Setup();

        this.WorldController = worldController;
    }

    public void UpdateIU()
    {

    }
}
