    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanBuildingTestWorldController : WorldController
{
    [SerializeField] Vector2Int BuildingGridSize;

    public override string IdentifierString
    {
        get
        {
            return "Test World Controller";
        }
    }

    MailboxLineSingleSubObject currentBuildingParam;

    //public List<HumanBuildingController> HumanBuildingControllers;

    public HumanBuilderController CurrentBuilderController;


    public override void Setup(IBaseObject superObject)
    {
        base.Setup(superObject);

        //LoadDefaultBuilding();
        SetupDefaultBuilding();

        currentBuildingParam = new MailboxLineSingleSubObject(valueName: "MainBuilding", subObject: CurrentBuilderController.CurrentBuilding, objectHolder: CurrentMailbox);

        CurrentBuilderController.Setup(worldController: this);
    }

    public override void ResetObject()
    {
        baseReset();
    }

    void SetupDefaultBuilding()
    {
        CurrentBuilderController.CurrentBuilding = GameObject.Instantiate(ResourceLibrary.TryGetTemplateFromStringIdentifier(HumanBuildingController.constIdentifierString) as HumanBuildingController);

        CurrentBuilderController.CurrentBuilding.CompleteSetUpWithBuildParameters(superObject: this, gridSize: BuildingGridSize, negativeFloors: 0, totalFloors: 1);

        CurrentBuilderController.CurrentBuilding.ApplyBuildParameters();
    }

    void LoadDefaultBuilding()
    {
        string fileLocation = Application.streamingAssetsPath + @"\Buildings";
        string fileNameWithEnding = "DefaultBuilding.json";

        string completeFilePath = fileLocation + @"\" + fileNameWithEnding;

        CurrentBuilderController.CurrentBuilding = StaticSaveAndLoadSystem.LoadBaseObjectIntoSuperObject(completeFileLocation: completeFilePath, superObject: CurrentBuilderController.WorldController) as HumanBuildingController;

        CurrentBuilderController.CurrentBuilding.ApplyBuildParameters();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        Setup(null);
    }


    // Update is called once per frame
    protected override void Update()
    {
        
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
