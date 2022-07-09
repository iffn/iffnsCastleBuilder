using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using iffnsStuff.iffnsBaseSystemForUnity;
using System.Text.RegularExpressions;
using System.Globalization;

namespace iffnsStuff.iffnsCastleBuilder
{
    //using System.Diagnostics;

    public class SaveAndLoadSystem : MonoBehaviour
    {
        //Set in Unity
        [SerializeField] CastleBuilderController CurrentBuilderController;
        [SerializeField] SaveAndLoadUI CurrentSaveAndLoadUI;
        [SerializeField] BuildingToolController ToolController;

        //Settings
        readonly static string fileIdentifier = "CastleBuilder by iffn";
        readonly static string mainBuildingIdentifier = "MainBuilding";
        readonly string fileEnding = ".castle";
        static readonly string buildingFileLocation = StaticSaveAndLoadSystem.UserFileLocation + MyStringComponents.slash + "Buildings";
        readonly static Vector3Int currentVersion = new Vector3Int(1, 0, 0);
            /*
                Version types:
                x = Main version: File cannot be read when opening with an older version
                y = Secondary version: Existing parts may change when opening with an older version
                z = Minor version: Some new parts not supported when opening with an older version
            */
        readonly float TimeUntilLoadOrClearConfirmationSeconds = 20;

        //Runtime parameters
        List<StaticSaveAndLoadSystem.BaseLoadFileInfo> loadInfos;
        readonly List<SaveAndLoadUI.FileLineInfo> fileInfos = new();
        TextAsset DefaultBuildingFile;
        bool confirmOverride = true;
        float LastSaveLoadOrClearTime = 0;


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
            return $"{version.x}.{version.y}.{version.z}";
        }

        public Vector3Int GetVersionVector(string version)
        {
            string[] parts = version.Split('.');

            if (parts.Length < 3)
            {
                return Vector3Int.zero;
            }

            int x = StringHelper.ConvertStringToInt(text: parts[0], globalFormat: true, out bool xWorked);
            int y = StringHelper.ConvertStringToInt(text: parts[1], globalFormat: true, out bool yWorked);
            int z = StringHelper.ConvertStringToInt(text: parts[2], globalFormat: true, out bool zWorked);

            if (!xWorked || !yWorked || !zWorked) return Vector3Int.zero;

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

        string CompleteFileLocation
        {
            get
            {
                return buildingFileLocation + MyStringComponents.slash + CurrentFileNameWithoutEnding + fileEnding;
            }
        }

        CastleController CurrentBuilding
        {
            get
            {
                return CurrentBuilderController.CurrentBuilding;
            }
        }

        public void SetupAndLoadDefaultBuilding(TextAsset DefaultBuildingFile)
        {
            this.DefaultBuildingFile = DefaultBuildingFile;

            UpdateFileList();

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

        bool ConfirmBeforeLoadOrClear
        {
            get
            {
                return Time.time > LastSaveLoadOrClearTime + TimeUntilLoadOrClearConfirmationSeconds;
            }
        }

        void SetLastSaveLoadOrClearTime()
        {
            LastSaveLoadOrClearTime = Time.time;
        }

        void HideAllConfirmations()
        {
            CurrentSaveAndLoadUI.ShowOverrideConfirmation = false;
            CurrentSaveAndLoadUI.ShowLoadConfirmation = false;
            CurrentSaveAndLoadUI.ShowClearConfirmation = false;
        }

        public void RequestSave()
        {
            HideAllConfirmations();

            if (!confirmOverride || !SelectedFileExists(updateList: true))
            {
                SaveBuilding();
            }
            else
            {
                CurrentSaveAndLoadUI.ShowOverrideConfirmation = true;
            }
        }

        public void RequestLoad()
        {
            HideAllConfirmations();

            if (ConfirmBeforeLoadOrClear)
            {
                CurrentSaveAndLoadUI.ShowLoadConfirmation = true;
            }
            else
            {
                LoadBuilding();
            }
        }

        public void RequestClear()
        {
            HideAllConfirmations();

            if (ConfirmBeforeLoadOrClear)
            {
                CurrentSaveAndLoadUI.ShowClearConfirmation = true;
            }
            else
            {
                ClearBuilding();
            }
        }

        public void ConfirmSave()
        {
            SaveBuilding();
            CurrentSaveAndLoadUI.ShowOverrideConfirmation = false;
        }

        public void ConfirmLoad()
        {
            LoadBuilding();
            CurrentSaveAndLoadUI.ShowLoadConfirmation = false;
        }
        

        public void ConfirmClear()
        {
            ClearBuilding();
            CurrentSaveAndLoadUI.ShowClearConfirmation = false;
        }

        public void CancelSave()
        {
            CurrentSaveAndLoadUI.ShowOverrideConfirmation = false;
        }

        public void CancelLoad()
        {
            CurrentSaveAndLoadUI.ShowLoadConfirmation = false;
        }

        public void CancelClear()
        {
            CurrentSaveAndLoadUI.ShowClearConfirmation = false;
        }

        public void ClearBuilding()
        {
            SetLastSaveLoadOrClearTime();

            //Load default building -> Merge with CastleTestWorldController

            EditTool.DeactivateEditOnMain();

            CurrentBuilding.CurrentFloorNumber = 0;

            LoadDefaultBuilding();

            ToolController.CurrentNavigationTools.UpdateUI();

            CurrentSaveAndLoadUI.CurrentTitle = "";
        }

        public void LoadBuilding()
        {
            SetLastSaveLoadOrClearTime();
            confirmOverride = false;

            /*
            System.Diagnostics.Stopwatch stopwatch = new();
            stopwatch.Start();
            System.Diagnostics.Stopwatch stepwatch = new();
            */

            StaticSaveAndLoadSystem.FullLoadFileInfo fileInfo = StaticSaveAndLoadSystem.GetFileInfoFromFileLocation(completeFileLocation: CompleteFileLocation, fileEnding: fileEnding);
            if (!LoadInfoIsValid(fileInfo)) return;
            if (VersionType(GetVersionVector(fileInfo.version)) == UpgradeType.notSupported) return;

            if (GetSelectedFile(updateList: true) == null)
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

            StaticSaveAndLoadSystem.LoadBaseObjectParametersToExistingObject(RawJSONString: fileInfo.LoadObjectString, baseObject: CurrentBuilding);
            //stepwatch.Stop();
            //Debug.Log("File load time = " + stepwatch.Elapsed.TotalSeconds);
            // -> Chateau time: 3.3s

            //Apply parameters:
            //stepwatch.Restart();
            if (CurrentBuilding.Failed) LoadDefaultBuilding();

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
            CurrentSaveAndLoadUI.LoadButtonState = SaveAndLoadUI.SaveAndLoadButtonStates.Done;
            ToolController.CurrentNavigationTools.UpdateUI();
            // -> Chateau time: 0.005 s

            //stopwatch.Stop();
            //Debug.Log("Total load time = " + stopwatch.Elapsed.TotalSeconds);
            // -> Chateau time: 5.037s
        }

        bool LoadInfoIsValid(StaticSaveAndLoadSystem.LoadFileInfo loadInfo)
        {
            if (loadInfo == null) return false;

            if (!loadInfo.IsValid) return false;
            if (loadInfo.identifier != fileIdentifier) return false;

            Vector3Int version = GetVersionVector(version: loadInfo.version);

            if (version.x < 1 || version.y < 0 || version.z <0) return false;

            return true;
        }

        public StaticSaveAndLoadSystem.BaseLoadFileInfo GetSelectedFile(bool updateList)
        {
            if (updateList) UpdateFileList();

            foreach (StaticSaveAndLoadSystem.BaseLoadFileInfo info in loadInfos)
            {
                if (info.FileNameWithoutEnding == CurrentSaveAndLoadUI.CurrentTitle) return info;
            }

            return null;
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
            SetLastSaveLoadOrClearTime();
            confirmOverride = false;

            if (string.IsNullOrWhiteSpace(CurrentFileNameWithoutEnding)) return; //Don't save empty file, IsNullOrWhiteSpace also includes empty: https://docs.microsoft.com/en-us/dotnet/api/system.string.isnullorwhitespace

            StaticSaveAndLoadSystem.SaveFileInfo.SaveObjectInfo saveObject = new(name: mainBuildingIdentifier, saveObject: CurrentBuilding);

            StaticSaveAndLoadSystem.SaveFileInfo fileInfo = new(type: fileIdentifier, version: GetVersionString(currentVersion), saveObject: saveObject);

            StaticSaveAndLoadSystem.SaveFileToFileLocation(fileInfo: fileInfo, completeFileLocation: CompleteFileLocation);

            CurrentSaveAndLoadUI.SaveButtonState = SaveAndLoadUI.SaveAndLoadButtonStates.Done;
        }

        bool SelectedFileExists(bool updateList)
        {
            StaticSaveAndLoadSystem.BaseLoadFileInfo file = GetSelectedFile(updateList: updateList);

            return file != null;
        }

        //Update buttons
        public void UpdateButtons(bool updateList)
        {
            if (CurrentSaveAndLoadUI.CurrentTitle.Length == 0)
            {
                CurrentSaveAndLoadUI.SaveButtonState = SaveAndLoadUI.SaveAndLoadButtonStates.Unable;
                CurrentSaveAndLoadUI.LoadButtonState = SaveAndLoadUI.SaveAndLoadButtonStates.Unable;
                CurrentSaveAndLoadUI.TitleMismatch = UpgradeType.sameVersion;
            }
            else
            {
                StaticSaveAndLoadSystem.BaseLoadFileInfo file = GetSelectedFile(updateList: updateList);

                if(file == null)
                {
                    CurrentSaveAndLoadUI.SaveButtonState = SaveAndLoadUI.SaveAndLoadButtonStates.Normal;
                    CurrentSaveAndLoadUI.LoadButtonState = SaveAndLoadUI.SaveAndLoadButtonStates.Unable;
                    CurrentSaveAndLoadUI.TitleMismatch = UpgradeType.sameVersion;
                }
                else
                {
                    CurrentSaveAndLoadUI.SaveButtonState = SaveAndLoadUI.SaveAndLoadButtonStates.Normal;
                    CurrentSaveAndLoadUI.LoadButtonState = SaveAndLoadUI.SaveAndLoadButtonStates.Normal;
                    CurrentSaveAndLoadUI.TitleMismatch = VersionType(GetVersionVector(version: file.version));
                }
            }
        }

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

            loadInfos = StaticSaveAndLoadSystem.GetBaseFileInfosFromFolderLocation(completeFolderLocation: buildingFileLocation, fileEnding: fileEnding);

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

        public void SelectFileFromList(SaveAndLoadUI.FileLineInfo info)
        {
            CurrentSaveAndLoadUI.CurrentTitle = info.fileNameWithoutEnding;

            CurrentSaveAndLoadUI.HideFileList();

            confirmOverride = true;

            //Update buttons not needed because the file name change already updates the buttons
            //UpdateButtons(updateList: true);
        }

        //Name change
        public void FileNameChange()
        {
            confirmOverride = true;
            UpdateButtons(updateList: true);
        }
    }
}