using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class FileSelectionLine : MonoBehaviour
    {
        [SerializeField] Button selectButton;
        [SerializeField] TMP_Text titleText;

        [SerializeField] VersionMismatchController mismatchIdentifier;

        SaveAndLoadUI.FileLineInfo info;

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

        public delegate void ButtonFunction(SaveAndLoadUI.FileLineInfo info);

        public void Setup(SaveAndLoadUI.FileLineInfo info, ButtonFunction buttonFunction)
        {
            this.info = info;
            Title = info.fileNameWithoutEnding;

            selectButton.onClick.AddListener(delegate { buttonFunction(info); });

            mismatchIdentifier.Setup(upgradeType: info.upgradeType);

            if(info.upgradeType == SaveAndLoadSystem.UpgradeType.notSupported)
            {
                selectButton.gameObject.SetActive(false);
            }
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