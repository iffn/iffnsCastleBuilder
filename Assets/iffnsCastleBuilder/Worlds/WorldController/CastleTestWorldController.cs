using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class CastleTestWorldController : WorldController
    {
        [SerializeField] Vector2Int BuildingGridSize;
        [SerializeField] ModificationNodeLibraryIntegrator CurrentModificationNodeLibraryIntegrator = null; //ToDo: Move to Builder library
        [SerializeField] ResourceLibraryIntegrator CurrentResourceLibraryIntegrator = null; //ToDo: Rename to Human building Resource Library
        [SerializeField] MaterialLibraryIntegrator CurrentMaterialLibraryIntegrator = null;
        [SerializeField] TextAsset DefaultBuildingFile;

        MailboxLineSingleSubObject currentBuildingParam;

        //public List<CastleController> CastleControllers;

        public CastleBuilderController CurrentBuilderController;

        protected override void Start()
        {
            Setup(null);
        }

        public override void Setup(IBaseObject superObject)
        {
            base.Setup(superObject);

            //LoadDefaultBuilding();
            CurrentResourceLibraryIntegrator.Setup();
            CurrentModificationNodeLibraryIntegrator.Setup();
            CurrentMaterialLibraryIntegrator.Setup();
            
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
            CurrentBuilderController.CurrentBuilding = Instantiate(ResourceLibrary.TryGetTemplateFromStringIdentifier(nameof(CastleController)) as CastleController);

            CurrentBuilderController.CurrentBuilding.Setup(superObject: this);

            CurrentBuilderController.CurrentBuildingToolController.CurrentSaveAndLoadSystem.SetupAndLoadDefaultBuilding(DefaultBuildingFile: DefaultBuildingFile);

            /*
            CurrentBuilderController.CurrentBuilding.CompleteSetUpWithBuildParameters(superObject: this, gridSize: BuildingGridSize, negativeFloors: 0, totalFloors: 1);

            CurrentBuilderController.CurrentBuilding.ApplyBuildParameters();
            */
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
}