﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using iffnsStuff.iffnsBaseSystemForUnity;
using System.Text.RegularExpressions;

namespace iffnsStuff.iffnsCastleBuilder
{
    //using System.Diagnostics;

    public class SaveAndLoadSystem : MonoBehaviour
    {
        //Set in Unity
        [SerializeField] HumanBuilderController CurrentBuilderController;
        [SerializeField] SaveAndLoadUI CurrentSaveAndLoadUI;
        [SerializeField] BuildingToolController ToolController;

        readonly static string fileIdentifier = "CastleBuilder by iffn";
        readonly static string currentVersion = "1.0.0";
        readonly static string mainBuildingIdentifier = "MainBuilding";

        TextAsset DefaultBuildingFile;

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

        public void SetupAndLoadDefaultBuilding(TextAsset DefaultBuildingFile)
        {
            this.DefaultBuildingFile = DefaultBuildingFile;

            LoadDefaultBuilding();
        }
        
        void LoadDefaultBuilding()
        {
            string text = DefaultBuildingFile.text;

            List<string> RawJSONString = new List<string>(Regex.Split(text, System.Environment.NewLine));

            StaticSaveAndLoadSystem.FullLoadFileInfo fileInfo = StaticSaveAndLoadSystem.GetFileInfoFromJson(fileNameWithoutEnding: DefaultBuildingFile.name, jsonString: RawJSONString);

            //StaticSaveAndLoadSystem.LoadBaseObjectParametersToExistingObject(completeFileLocation: completeFileLocation, baseObject: CurrentBuilding);
            StaticSaveAndLoadSystem.LoadBaseObjectParametersToExistingObject(RawJSONString: fileInfo.LoadObjectString, baseObject: CurrentBuilding);

            CurrentBuilding.ApplyBuildParameters();
        }

        public void ClearBuilding()
        {
            //Load default building -> Merge with HumanBuildingTestWorldController

            EditTool.DeactivateEditOnMain();

            CurrentBuilding.CurrentFloorNumber = 0;

            LoadDefaultBuilding();

            ToolController.CurrentNavigationTools.UpdateUI();

            CurrentSaveAndLoadUI.CurrentTitle = "";
        }

        public void LoadBuilding()
        {
            /*
            System.Diagnostics.Stopwatch stopwatch = new();
            stopwatch.Start();
            System.Diagnostics.Stopwatch stepwatch = new();
            */

            if (!SelectedFileExists(updateList: true))
            {
                UpdateButtons(updateList: false);
                return;
            }

            EditTool.DeactivateEditOnMain();

            //Save floor number:
            int storedFloorNumber = CurrentBuilding.CurrentFloorNumber;
            CurrentBuilding.CurrentFloorNumber = 0;
            // -> Chateau time: 9.1E-06 s

            //Load parameters from file:
            //stepwatch.Restart();

            StaticSaveAndLoadSystem.FullLoadFileInfo fileInfo = StaticSaveAndLoadSystem.GetFileInfoFromFileLocation(completeFileLocation: completeFileLocation, fileEnding: fileEnding);

            if (!LoadInfoIsValid(fileInfo)) return;

            StaticSaveAndLoadSystem.LoadBaseObjectParametersToExistingObject(RawJSONString: fileInfo.LoadObjectString, baseObject: CurrentBuilding);
            //stepwatch.Stop();
            //Debug.Log("File load time = " + stepwatch.Elapsed.TotalSeconds);
            // -> Chateau time: 3.3s

            //Apply parameters:
            //stepwatch.Restart();
            CurrentBuilding.ApplyBuildParameters();
            //stepwatch.Stop();
            //Debug.Log("Apply build parameter time = " + stepwatch.Elapsed.TotalSeconds);
            // -> Chateau time: 1.7s

            //Destroy failed objects:
            CurrentBuilding.DestroyFailedSubObjects();
            // -> Chateau time: 0.025s

            //Restore floor number:
            CurrentBuilding.CurrentFloorNumber = storedFloorNumber;
            // -> Chateau time: 8.9E-06 s

            //Update UI:
            CurrentSaveAndLoadUI.SaveButtonState = SaveAndLoadUI.SaveButtonStates.Done;
            CurrentSaveAndLoadUI.LoadButtonState = SaveAndLoadUI.LoadButtonStates.Done;
            ToolController.CurrentNavigationTools.UpdateUI();
            // -> Chateau time: 0.005 s

            //stopwatch.Stop();
            //Debug.Log("Total load time = " + stopwatch.Elapsed.TotalSeconds);
            // -> Chateau time: 5.037s
        }

        bool LoadInfoIsValid(StaticSaveAndLoadSystem.LoadFileInfo loadInfo)
        {
            if (!loadInfo.IsValid) return false;
            if (loadInfo.identifier != fileIdentifier) return false;

            if (loadInfo.version != currentVersion)
            {
                Debug.Log(loadInfo.FileNameWithoutEnding + " needs an upgrade");
            }

            return true;
        }

        public bool SelectedFileExists(bool updateList)
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

            StaticSaveAndLoadSystem.SaveFileInfo.SaveObjectInfo saveObject = new(name: mainBuildingIdentifier, saveObject: CurrentBuilding);

            StaticSaveAndLoadSystem.SaveFileInfo fileInfo = new(type: fileIdentifier, version: currentVersion, saveObject: saveObject);

            StaticSaveAndLoadSystem.SaveFileToFileLocation(fileInfo: fileInfo, completeFileLocation: completeFileLocation);

            CurrentSaveAndLoadUI.SaveButtonState = SaveAndLoadUI.SaveButtonStates.Done;
            CurrentSaveAndLoadUI.LoadButtonState = SaveAndLoadUI.LoadButtonStates.Done;
        }

        //Update buttons
        public void UpdateButtons(bool updateList)
        {
            if (CurrentSaveAndLoadUI.CurrentTitle.Length == 0)
            {
                CurrentSaveAndLoadUI.SaveButtonState = SaveAndLoadUI.SaveButtonStates.Unknown;
                CurrentSaveAndLoadUI.LoadButtonState = SaveAndLoadUI.LoadButtonStates.Unknown;
            }
            else if (SelectedFileExists(updateList: updateList))
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

        public List<string> CurrentFileListWithoutEnding
        {
            get
            {
                UpdateFileList();

                return currentFileListWithoutEnding;
            }
        }

        void UpdateFileList()
        {
            currentFileListWithoutEnding.Clear();

            List<StaticSaveAndLoadSystem.BaseLoadFileInfo> loadInfos = StaticSaveAndLoadSystem.GetBaseFileInfosFromFolderLocation(completeFolderLocation: buildingFileLication, fileEnding: fileEnding);

            foreach (StaticSaveAndLoadSystem.BaseLoadFileInfo loadInfo in loadInfos)
            {
                if (!LoadInfoIsValid(loadInfo)) continue;

                currentFileListWithoutEnding.Add(loadInfo.FileNameWithoutEnding);
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