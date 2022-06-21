using System.Collections;
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
        //readonly static string currentVersion = "1.0.0";
        readonly static string mainBuildingIdentifier = "MainBuilding";
        
        readonly static Vector3Int currentVersion = new Vector3Int(2, 1, 1);
        /*
            Version types:
            x = Main version: File cannot be read when opening with an older version
            y = Secondary version: Existing parts may change when opening with an older version
            z = Minor version: Some new parts not supported when opening with an older version
        */

        public enum UpgradeType
        {
            sameVersion,
            noIssueNewVersion,
            someElementsNotSupported,
            someNotSupportedAndWrongPosition,
            upgrade,
            notSupported
        }

        public string GetVersionString(Vector3Int version)
        {
            return version.x + "." + version.y + "." + version.z;
        }

        public Vector3Int GetVersionVector(string version)
        {
            string[] parts = version.Split('.');

            if (parts.Length != 3)
            {
                Debug.Log("Error");
            }

            int x = int.Parse(parts[0]);
            int y = int.Parse(parts[1]);
            int z = int.Parse(parts[2]);

            return new Vector3Int(x, y, z);
        }

        UpgradeType VersionType(Vector3Int fileVersion)
        {
            if(fileVersion.x > currentVersion.x)
            {
                return UpgradeType.notSupported;
            }
            if(fileVersion.x < currentVersion.x)
            {
                return UpgradeType.upgrade;
            }

            if (fileVersion.y > currentVersion.y)
            {
                return UpgradeType.someNotSupportedAndWrongPosition;
            }
            if (fileVersion.y < currentVersion.y)
            {
                return UpgradeType.upgrade;
            }

            if (fileVersion.z > currentVersion.z)
            {
                return UpgradeType.someElementsNotSupported;
            }
            if (fileVersion.z < currentVersion.z)
            {
                return UpgradeType.noIssueNewVersion;
            }

            return UpgradeType.sameVersion;
        }

        TextAsset DefaultBuildingFile;

        readonly string fileEnding = ".json";

        static readonly string buildingFileLocation = StaticSaveAndLoadSystem.UserFileLocation + MyStringComponents.slash + "Buildings";

        string completeFileLocation
        {
            get
            {
                return buildingFileLocation + MyStringComponents.slash + CurrentFileNameWithoutEnding + fileEnding;
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

            if (loadInfo.version != GetVersionString(currentVersion))
            {
                Debug.Log(loadInfo.FileNameWithoutEnding + " needs an upgrade");
            }

            return true;
        }

        public bool SelectedFileExists(bool updateList)
        {
            if (updateList)
            {
                List<StaticSaveAndLoadSystem.BaseLoadFileInfo> loadInfos = StaticSaveAndLoadSystem.GetBaseFileInfosFromFolderLocation(completeFolderLocation: buildingFileLocation, fileEnding: fileEnding);

                foreach (StaticSaveAndLoadSystem.BaseLoadFileInfo info in loadInfos)
                {
                    if (info.FileNameWithoutEnding == CurrentSaveAndLoadUI.CurrentTitle) return true;
                }
                
                return false;
            }
            else
            {
                foreach(SaveAndLoadUI.FileLineInfo info in fileInfos)
                {
                    if(info.fileNameWithoutEnding == CurrentSaveAndLoadUI.CurrentTitle) return true;
                }
                return false;
            }
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

            StaticSaveAndLoadSystem.SaveFileInfo fileInfo = new(type: fileIdentifier, version: GetVersionString(currentVersion), saveObject: saveObject);

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
        List<SaveAndLoadUI.FileLineInfo> fileInfos = new();

        public List<SaveAndLoadUI.FileLineInfo> CurrentFileInfos
        {
            get
            {
                UpdateFileList();

                return fileInfos;
            }
        }

        void UpdateFileList()
        {
            fileInfos.Clear();

            List<StaticSaveAndLoadSystem.BaseLoadFileInfo> loadInfos = StaticSaveAndLoadSystem.GetBaseFileInfosFromFolderLocation(completeFolderLocation: buildingFileLocation, fileEnding: fileEnding);

            foreach (StaticSaveAndLoadSystem.BaseLoadFileInfo loadInfo in loadInfos)
            {
                if (!LoadInfoIsValid(loadInfo)) continue;

                fileInfos.Add(new SaveAndLoadUI.FileLineInfo(fileNameWithoutEnding: loadInfo.FileNameWithoutEnding, upgradeType: VersionType(GetVersionVector(loadInfo.version))));
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

                CurrentSaveAndLoadUI.ShowFileList(fileInfos);
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