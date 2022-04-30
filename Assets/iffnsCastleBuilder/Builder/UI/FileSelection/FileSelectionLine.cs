using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FileSelectionLine : MonoBehaviour
{
    public Button selectButton;

    public Text titleText;

    string fileName;
    public string FileName
    {
        get
        {
            return fileName;
        }
    }

    public string Title
    {
        get
        {
            return titleText.text;
        }
        set
        {
            titleText.text = value;
        }
    }

    public delegate void ButtonFunction(string fileName);

    public void Setup(string fileName, string title, ButtonFunction buttonFunction)
    {
        this.fileName = fileName;
        this.Title = title;

        selectButton.onClick.AddListener(delegate { buttonFunction(fileName); });
    }


}
