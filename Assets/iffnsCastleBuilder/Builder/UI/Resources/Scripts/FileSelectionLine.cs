using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class FileSelectionLine : MonoBehaviour
    {
        public Button selectButton;

        public TMP_Text titleText;

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
            Title = title;

            selectButton.onClick.AddListener(delegate { buttonFunction(fileName); });
        }

        public float Height
        {
            get
            {
                return transform.GetComponent<RectTransform>().sizeDelta.y;
            }
        }
    }
}