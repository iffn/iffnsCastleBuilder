using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class SaveAndLoadUI : MonoBehaviour
    {
        //Unity assignments
        [SerializeField] FileSelectionLine FileSelectionLineTemplate = null;
        [SerializeField] SaveAndLoadSystem CurrentSaveAndLoadSystem = null;

        [SerializeField] GameObject SaveCheckmark = null;
        [SerializeField] GameObject SaveUnavailable = null;
        [SerializeField] GameObject LoadCheckmark = null;
        [SerializeField] GameObject LoadUnavailable = null;
        [SerializeField] VectorButton NewButton = null;
        [SerializeField] TMP_InputField CastleTitle = null;
        [SerializeField] VersionMismatchController fileNameMismatch = null;

        [SerializeField] RectTransform ExpandArea = null;
        [SerializeField] RectTransform ExpandIcon;

        readonly float saveMarkTimeSeconds = 1.5f;

        public enum SaveAndLoadButtonStates
        {
            Normal,
            Done,
            Unable
        }

        SaveAndLoadButtonStates saveButtonState = SaveAndLoadButtonStates.Unable;
        public SaveAndLoadButtonStates SaveButtonState
        {
            get
            {
                return saveButtonState;
            }
            set
            {
                saveButtonState = value;

                switch (value)
                {
                    case SaveAndLoadButtonStates.Normal:
                        SaveCheckmark.SetActive(false);
                        SaveUnavailable.SetActive(false);
                        break;
                    case SaveAndLoadButtonStates.Done:
                        SaveCheckmark.SetActive(true);
                        SaveUnavailable.SetActive(false);
                        StartCoroutine(RestoreSaveButton(saveMarkTimeSeconds));
                        break;
                    case SaveAndLoadButtonStates.Unable:
                        SaveCheckmark.SetActive(false);
                        SaveUnavailable.SetActive(true);
                        break;
                    default:
                        Debug.LogWarning("Error: Save button not defined");
                        break;
                }
            }
        }

        public SaveAndLoadSystem.UpgradeType TitleMismatch
        {
            set
            {
                fileNameMismatch.UpdateData(upgradeType: value);
            }
        }

        IEnumerator RestoreSaveButton(float seconds)
        {
            yield return new WaitForSeconds(seconds);

            SaveCheckmark.SetActive(false);
        }


        SaveAndLoadButtonStates loadButtonState = SaveAndLoadButtonStates.Unable;
        public SaveAndLoadButtonStates LoadButtonState
        {
            get
            {
                return loadButtonState;
            }
            set
            {
                loadButtonState = value;

                switch (value)
                {
                    case SaveAndLoadButtonStates.Normal:
                        LoadCheckmark.SetActive(false);
                        LoadUnavailable.SetActive(false);
                        break;
                    case SaveAndLoadButtonStates.Done:
                        LoadCheckmark.SetActive(true);
                        LoadUnavailable.SetActive(false);
                        StartCoroutine(RestoreLoadButton(saveMarkTimeSeconds));
                        break;
                    case SaveAndLoadButtonStates.Unable:
                        LoadCheckmark.SetActive(false);
                        LoadUnavailable.SetActive(true);
                        break;
                    default:
                        Debug.LogWarning("Error: Save button not defined");
                        break;
                }
            }
        }

        IEnumerator RestoreLoadButton(float seconds)
        {
            yield return new WaitForSeconds(seconds);

            LoadButtonState = SaveAndLoadButtonStates.Normal;
        }

        public string CurrentTitle
        {
            get
            {
                return CastleTitle.text;
            }
            set
            {
                CastleTitle.text = value;
            }
        }

        //File list
        public bool FileListShown { get; private set; } = false;

        public class FileLineInfo
        {
            public string fileNameWithoutEnding;
            public SaveAndLoadSystem.UpgradeType upgradeType;

            public FileLineInfo(string fileNameWithoutEnding, SaveAndLoadSystem.UpgradeType upgradeType)
            {
                this.fileNameWithoutEnding = fileNameWithoutEnding;
                this.upgradeType = upgradeType;
            }
        }

        public void ShowFileList(List<FileLineInfo> fileInfos)
        {
            FileListShown = true;

            foreach(FileLineInfo info in fileInfos)
            {
                AddFileLine(info: info);
            }

            ExpandArea.sizeDelta = new Vector2(ExpandArea.sizeDelta.x, fileInfos.Count * FileSelectionLineTemplate.Height);

            ExpandIcon.localRotation = Quaternion.Euler(180 * Vector3.forward);
        }

        public void HideFileList()
        {
            FileListShown = false;

            ClearFileList();

            ExpandIcon.localRotation = Quaternion.identity;
        }

        //File selection stuff
        readonly List<FileSelectionLine> fileLines = new();

        void AddFileLine(FileLineInfo info)
        {
            FileSelectionLine fileLine = Instantiate(FileSelectionLineTemplate).transform.GetComponent<FileSelectionLine>();

            fileLines.Add(fileLine);

            fileLine.transform.SetParent(ExpandArea.transform);

            fileLine.transform.localScale = Vector3.one;

            fileLine.Setup(info: info, buttonFunction: delegate { CurrentSaveAndLoadSystem.SelectFileFromList(info); });
        }

        void ClearFileList()
        {
            foreach (FileSelectionLine fileLine in fileLines)
            {
                Destroy(fileLine.gameObject);
            }

            fileLines.Clear();
        }


        public void ClearCastle()
        {
            //ToDo: Implement function and add confirm function

            NewButton.gameObject.SetActive(true);
        }
    }
}