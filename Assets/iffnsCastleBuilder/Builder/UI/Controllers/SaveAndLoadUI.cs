﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class SaveAndLoadUI : MonoBehaviour
    {
        //Unity assignments
        [SerializeField] FileSelectionLine FileSelectionLineTemplate = null;
        [SerializeField] SaveAndLoadSystem CurrentSaveAndLoadSystem = null;

        [SerializeField] VectorButton SaveNewButton = null;
        [SerializeField] VectorButton SaveDoneButton = null;
        [SerializeField] VectorButton SaveOverrideButton = null;
        [SerializeField] VectorButton SaveUnknownButton = null;
        [SerializeField] VectorButton LoadNewButton = null;
        [SerializeField] VectorButton LoadDoneButton = null;
        [SerializeField] VectorButton LoadOverrideButton = null;
        [SerializeField] VectorButton LoadUnknownButton = null;
        [SerializeField] VectorButton NewButton = null;
        [SerializeField] InputField CastleTitle = null;

        [SerializeField] RectTransform ExpandArea = null;
        [SerializeField] RectTransform ExpandIcon;

        readonly float saveMarkTimeSeconds = 1.5f;

        public enum SaveButtonStates
        {
            New,
            Done,
            Override,
            Unknown
        }

        SaveButtonStates saveButtonState = SaveButtonStates.Unknown;
        public SaveButtonStates SaveButtonState
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
                    case SaveButtonStates.New:
                        ActiveSaveButton = SaveNewButton;
                        break;
                    case SaveButtonStates.Done:
                        ActiveSaveButton = SaveDoneButton;
                        StartCoroutine(RestoreSaveButton(saveMarkTimeSeconds));
                        break;
                    case SaveButtonStates.Override:
                        ActiveSaveButton = SaveOverrideButton;
                        break;
                    case SaveButtonStates.Unknown:
                        ActiveSaveButton = SaveUnknownButton;
                        break;
                    default:
                        Debug.LogWarning("Error: Save button not defined");
                        break;
                }
            }
        }

        VectorButton activeSaveButton;
        VectorButton ActiveSaveButton
        {
            /*
            get
            {
                if (activeSaveButton == null) activeSaveButton = SaveNewButton;

                return activeSaveButton;
            }
            */
            set
            {
                if (activeSaveButton == null) activeSaveButton = SaveUnknownButton;

                activeSaveButton.gameObject.SetActive(false);

                activeSaveButton = value;

                activeSaveButton.gameObject.SetActive(true);
            }
        }

        IEnumerator RestoreSaveButton(float seconds)
        {
            yield return new WaitForSeconds(seconds);

            if(CurrentSaveAndLoadSystem.SelectedFileExists(updateList: true))
            {
                SaveButtonState = SaveButtonStates.Override;
            }
            else
            {
                SaveButtonState = SaveButtonStates.New;
            }
        }

        public enum LoadButtonStates
        {
            New,
            Done,
            Override,
            Unknown
        }

        LoadButtonStates loadButtonState = LoadButtonStates.Unknown;
        public LoadButtonStates LoadButtonState
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
                    case LoadButtonStates.New:
                        ActiveLoadButton = LoadNewButton;
                        break;
                    case LoadButtonStates.Done:
                        ActiveLoadButton = LoadDoneButton;
                        StartCoroutine(RestoreLoadButton(saveMarkTimeSeconds));
                        break;
                    case LoadButtonStates.Override:
                        ActiveLoadButton = LoadOverrideButton;
                        break;
                    case LoadButtonStates.Unknown:
                        ActiveLoadButton = LoadUnknownButton;
                        break;
                    default:
                        Debug.LogWarning("Error: Load button not defined");
                        break;
                }
            }
        }

        VectorButton activeLoadButton;
        VectorButton ActiveLoadButton
        {
            /*
            get
            {
                if (activeLoadButton == null) activeLoadButton = LoadUnknownButton;

                return activeLoadButton;
            }
            */
            set
            {
                if (activeLoadButton == null) activeLoadButton = LoadUnknownButton;

                activeLoadButton.gameObject.SetActive(false);

                activeLoadButton = value;

                activeLoadButton.gameObject.SetActive(true);
            }
        }

        IEnumerator RestoreLoadButton(float seconds)
        {
            yield return new WaitForSeconds(seconds);

            LoadButtonState = LoadButtonStates.Override;
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

        public void ShowFileList(List<string> fileList)
        {
            FileListShown = true;

            foreach (string fileName in fileList)
            {
                AddFileLine(fileName: fileName, title: fileName);
            }

            ExpandArea.sizeDelta = new Vector2(ExpandArea.sizeDelta.x, fileList.Count * FileSelectionLineTemplate.Height);

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

        void AddFileLine(string fileName, string title)
        {
            FileSelectionLine fileLine = Instantiate(FileSelectionLineTemplate).transform.GetComponent<FileSelectionLine>();

            fileLines.Add(fileLine);

            fileLine.transform.SetParent(ExpandArea.transform);

            fileLine.transform.localScale = Vector3.one;

            fileLine.Setup(fileName: fileName, title: title, buttonFunction: delegate { CurrentSaveAndLoadSystem.SelectFileFromList(title); });
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