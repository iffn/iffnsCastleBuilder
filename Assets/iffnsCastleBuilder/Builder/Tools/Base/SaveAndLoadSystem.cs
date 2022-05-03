using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using iffnsStuff.iffnsBaseSystemForUnity;

namespace iffnsStuff.iffnsCastleBuilder
{
    //using System.Diagnostics;

    public class SaveAndLoadSystem : MonoBehaviour
    {
        //Set in Unity
        [SerializeField] HumanBuilderController CurrentBuilderController;
        [SerializeField] SaveAndLoadUI CurrentSaveAndLoadUI;
        [SerializeField] BuildingToolController ToolController;

        readonly string fileEnding = ".json";

        static readonly string buildingFileLication = StaticSaveAndLoadSystem.UserFileLocation + MyStringComponents.slash + "Buildings";

        string completeFileLocation
        {
            get
            {
                return buildingFileLication + MyStringComponents.slash + CurrentFileNameWithoutEnding + fileEnding;
            }
        }

        HumanBuildingController CurrentBuilding
        {
            get
            {
                return CurrentBuilderController.CurrentBuilding;
            }
        }

        public void ClearBuilding()
        {
            //Load default building -> Merge with HumanBuildingTestWorldController

            Debug.Log("Clear building to be added");
        }

        public void LoadBuilding()
        {
            if (!SelectedFileExists(updateList: true))
            {
                UpdateButtons(updateList: false);
                return;
            }

            int storedFloorNumber = CurrentBuilding.CurrentFloorNumber;
            CurrentBuilding.CurrentFloorNumber = 0;

            //System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            StaticSaveAndLoadSystem.LoadBaseObjectParametersToExistingObject(completeFileLocation: completeFileLocation, baseObject: CurrentBuilding);

            CurrentBuilding.ApplyBuildParameters();
            //Debug.Log("Time taken = " + watch.ElapsedMilliseconds + "ms");

            CurrentSaveAndLoadUI.SaveButtonState = SaveAndLoadUI.SaveButtonStates.Done;
            CurrentSaveAndLoadUI.LoadButtonState = SaveAndLoadUI.LoadButtonStates.Done;

            CurrentBuilding.CurrentFloorNumber = storedFloorNumber;

            ToolController.CurrentNavigationTools.UpdateUI();
        }

        bool SelectedFileExists(bool updateList)
        {
            if (currentFileListWithoutEnding.Contains(CurrentSaveAndLoadUI.CurrentTitle) == false)
            {
                if (updateList) UpdateFileList();
                else return false;

                if (currentFileListWithoutEnding.Contains(CurrentSaveAndLoadUI.CurrentTitle) == false)
                {
                    return false;
                }
            }

            return true;
        }

        public string CurrentFileNameWithoutEnding
        {
            get
            {
                return CurrentSaveAndLoadUI.CurrentTitle;
            }
        }

        public void SaveBuilding()
        {
            if (string.IsNullOrWhiteSpace(CurrentFileNameWithoutEnding)) return; //Don't save empty file, IsNullOrWhiteSpace also includes empty: https://docs.microsoft.com/en-us/dotnet/api/system.string.isnullorwhitespace

            StaticSaveAndLoadSystem.SaveObjectToFileLocation(saveObject: CurrentBuilding, completeFileLocation: completeFileLocation);

            CurrentSaveAndLoadUI.SaveButtonState = SaveAndLoadUI.SaveButtonStates.Done;
            CurrentSaveAndLoadUI.LoadButtonState = SaveAndLoadUI.LoadButtonStates.Done;
        }

        //Update buttons
        void UpdateButtons(bool updateList)
        {
            if (SelectedFileExists(updateList: updateList))
            {
                CurrentSaveAndLoadUI.SaveButtonState = SaveAndLoadUI.SaveButtonStates.Override;
                CurrentSaveAndLoadUI.LoadButtonState = SaveAndLoadUI.LoadButtonStates.Override;
            }
            else
            {
                CurrentSaveAndLoadUI.SaveButtonState = SaveAndLoadUI.SaveButtonStates.New;
                CurrentSaveAndLoadUI.LoadButtonState = SaveAndLoadUI.LoadButtonStates.Unknown;
            }
        }

        //File list
        List<string> currentFileListWithoutEnding = new List<string>();

        void UpdateFileList()
        {
            currentFileListWithoutEnding.Clear();

            List<string> tempFileList = StaticSaveAndLoadSystem.GetFileListFromLocation(Type: HumanBuildingController.constIdentifierString, completeFileLocation: buildingFileLication, fileEnding: fileEnding);

            foreach (string file in tempFileList)
            {
                currentFileListWithoutEnding.Add(file.Substring(0, file.Length - fileEnding.Length)); //remove file ending
            }
        }

        public void ToggleFileList()
        {
            if (CurrentSaveAndLoadUI.FileListShown)
            {
                //Hide file list
                CurrentSaveAndLoadUI.HideFileList();
            }
            else
            {
                //Show file list
                UpdateFileList();

                CurrentSaveAndLoadUI.ShowFileList(currentFileListWithoutEnding);
            }
        }

        public void SelectFileFromList(string fileName)
        {
            CurrentSaveAndLoadUI.CurrentTitle = fileName;

            CurrentSaveAndLoadUI.HideFileList();

            //Update buttons not needed because the file name change already updates the buttons
            //UpdateButtons(updateList: true);
        }

        //Name change
        public void FileNameChange()
        {
            UpdateButtons(updateList: true);
        }
    }
}