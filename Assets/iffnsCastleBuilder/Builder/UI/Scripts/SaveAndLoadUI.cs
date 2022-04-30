using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveAndLoadUI : MonoBehaviour
{
    //Unity assignments
    [SerializeField] FileSelectionLine FileSelectionLineTemplate = null;
    [SerializeField] SaveAndLoadSystem CurrentSaveAndLoadSystem = null;

    [SerializeField] VectorButton SaveNewButton = null;
    [SerializeField] VectorButton SaveDoneButton = null;
    [SerializeField] VectorButton SaveOverrideButton = null;
    [SerializeField] VectorButton LoadNewButton = null;
    [SerializeField] VectorButton LoadDoneButton = null;
    [SerializeField] VectorButton LoadOverrideButton = null;
    [SerializeField] VectorButton LoadUnknownButton = null;
    [SerializeField] VectorButton NewButton = null;
    [SerializeField] InputField CastleTitle = null;

    [SerializeField] GameObject ExpandArea = null;
    VectorButton activeSaveButton;

    public enum SaveButtonStates
    {
        New,
        Done,
        Override
    }


    VectorButton ActiveSaveButton
    {
        get
        {
            if (activeSaveButton == null) activeSaveButton = SaveNewButton;

            return activeSaveButton;
        }
        set
        {
            if (activeSaveButton == null) activeSaveButton = SaveNewButton;

            activeSaveButton.gameObject.SetActive(false);

            activeSaveButton = value;

            activeSaveButton.gameObject.SetActive(true);
        }
    }

    SaveButtonStates saveButtonState = SaveButtonStates.Done;

    public SaveButtonStates SaveButtonState
    {
        get
        {
            return saveButtonState;
        }
        set
        {
            switch (value)
            {
                case SaveButtonStates.New:
                    ActiveSaveButton = SaveNewButton;
                    break;
                case SaveButtonStates.Done:
                    ActiveSaveButton = SaveDoneButton;
                    break;
                case SaveButtonStates.Override:
                    ActiveSaveButton = SaveOverrideButton;
                    break;
                default:
                    Debug.LogWarning("Error: Save button not defined");
                    break;
            }
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

    VectorButton activeLoadButton;

    VectorButton ActiveLoadButton
    {
        get
        {
            if (activeLoadButton == null) activeLoadButton = LoadUnknownButton;

            return activeLoadButton;
        }
        set
        {
            if (activeLoadButton == null) activeLoadButton = LoadUnknownButton;

            activeLoadButton.gameObject.SetActive(false);

            activeLoadButton = value;

            activeLoadButton.gameObject.SetActive(true);
        }
    }

    public LoadButtonStates LoadButtonState
    {
        get
        {
            return loadButtonState;
        }
        set
        {
            switch (value)
            {
                case LoadButtonStates.New:
                    activeLoadButton = LoadNewButton;
                    break;
                case LoadButtonStates.Done:
                    activeLoadButton = LoadDoneButton;
                    break;
                case LoadButtonStates.Override:
                    activeLoadButton = LoadOverrideButton;
                    break;
                case LoadButtonStates.Unknown:
                    activeLoadButton = LoadUnknownButton;
                    break;
                default:
                    Debug.LogWarning("Error: Load button not defined");
                    break;
            }
        }
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

        foreach(string fileName in fileList)
        {
            AddFileLine(fileName: fileName, title: fileName);
        }
    }

    public void HideFileList()
    {
        FileListShown = false;

        ClearFileList();
    }

    //File selection stuff
    List<FileSelectionLine> fileLines = new List<FileSelectionLine>();

    void AddFileLine(string fileName, string title)
    {
        FileSelectionLine fileLine = GameObject.Instantiate(FileSelectionLineTemplate).transform.GetComponent<FileSelectionLine>();

        fileLines.Add(fileLine);

        fileLine.transform.SetParent(ExpandArea.transform);

        fileLine.transform.localScale = Vector3.one;

        fileLine.Setup(fileName: fileName, title: title, buttonFunction: delegate { CurrentSaveAndLoadSystem.SelectFileFromList(title); });
    }

    void ClearFileList()
    {
        foreach (FileSelectionLine fileLine in fileLines)
        {
            GameObject.Destroy(fileLine.gameObject);
        }

        fileLines.Clear();
    }


    public void ClearCastle()
    {
        //ToDo: Implement function and add confirm function

        NewButton.gameObject.SetActive(true);
    }
    /*
    public void SaveCastle()
    {
        //Activate correct buttons
        SaveNewButton.gameObject.SetActive(false);
        SaveDoneButton.gameObject.SetActive(true);
        SaveOverrideButton.gameObject.SetActive(false);

        LoadNewButton.gameObject.SetActive(false);
        LoadDoneButton.gameObject.SetActive(true);
        LoadOverrideButton.gameObject.SetActive(false);
    }

    public void LoadCastle()
    {
        //Activate correct buttons
        SaveNewButton.gameObject.SetActive(false);
        SaveDoneButton.gameObject.SetActive(true);
        SaveOverrideButton.gameObject.SetActive(false);

        LoadNewButton.gameObject.SetActive(false);
        LoadDoneButton.gameObject.SetActive(true);
        LoadOverrideButton.gameObject.SetActive(false);

        //Load castle from file with CurrentSaveAndLoadSystem
    }

    public void NewCastle()
    {
        //Activate correct buttons
        SaveNewButton.gameObject.SetActive(false);
        SaveDoneButton.gameObject.SetActive(true);
        SaveOverrideButton.gameObject.SetActive(false);

        LoadNewButton.gameObject.SetActive(false);
        LoadDoneButton.gameObject.SetActive(true);
        LoadOverrideButton.gameObject.SetActive(false);

        CastleTitle.SetTextWithoutNotify("");

        //CastleTitle.SetAllDirty();
    }

    public void SomethingHasBeenChanged()
    {

    }

    public void ToggleWorldSelector()
    {

    }

    public void UpdatedTitle()
    {
        //Check with CurrentSaveAndLoadSystem if file already exists

        //Update button types
    }

    bool filesAreExpanded = false;
    public GameObject ExpandFilesIcon;

    public void ToggleExpandFiles()
    {
        //Flip button
        ExpandFilesIcon.transform.localScale = new Vector3(ExpandFilesIcon.transform.localScale.x, -ExpandFilesIcon.transform.localScale.y, ExpandFilesIcon.transform.localScale.z);
        
        //Flip boolean
        filesAreExpanded = !filesAreExpanded;
        
        //Set based on new state
        if (filesAreExpanded)
        {
            //Get file list

            //Update file list

            //Show file list

        }
        else
        {
            //Hide file list
        }
    }

    public void SetTitle(Text SelectedText)
    {
        CastleTitle.SetTextWithoutNotify(SelectedText.text);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    */
}
