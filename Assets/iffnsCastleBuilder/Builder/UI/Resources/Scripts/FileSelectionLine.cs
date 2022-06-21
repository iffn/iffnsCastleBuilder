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

        [SerializeField] GameObject VersionIndicatorHolder;
        [SerializeField] GameObject NoUpgradeSymbol;
        [SerializeField] RectTransform VersionIndicatorTransform;
        [SerializeField] Image VersionIndicatorImage;
        [SerializeField] TMP_Text UpgradeInfo;

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

        public void Setup(string fileName, string title, ButtonFunction buttonFunction, SaveAndLoadSystem.UpgradeType upgradeType)
        {
            this.fileName = fileName;
            Title = title;

            selectButton.onClick.AddListener(delegate { buttonFunction(fileName); });

            switch (upgradeType)
            {
                case SaveAndLoadSystem.UpgradeType.sameVersion:
                    VersionIndicatorHolder.SetActive(false);
                    break;

                case SaveAndLoadSystem.UpgradeType.noIssueNewVersion:
                    VersionIndicatorHolder.SetActive(false);
                    break;

                case SaveAndLoadSystem.UpgradeType.someElementsNotSupported:
                    VersionIndicatorHolder.SetActive(true);
                    NoUpgradeSymbol.SetActive(false);
                    VersionIndicatorTransform.localEulerAngles = Vector3.zero;
                    VersionIndicatorImage.color = Color.yellow;
                    UpgradeInfo.text = "File created in a newer version. Some elements may not be supported";
                    break;

                case SaveAndLoadSystem.UpgradeType.someNotSupportedAndWrongPosition:
                    VersionIndicatorHolder.SetActive(true);
                    NoUpgradeSymbol.SetActive(false);
                    VersionIndicatorTransform.localEulerAngles = Vector3.zero;
                    VersionIndicatorImage.color = Color.red;
                    UpgradeInfo.text = "File created in a newer version. Some elements may not be supported while others may have wrong parameters";
                    break;

                case SaveAndLoadSystem.UpgradeType.upgrade:
                    VersionIndicatorTransform.localEulerAngles = 180 * Vector3.forward;
                    VersionIndicatorHolder.SetActive(true);
                    NoUpgradeSymbol.SetActive(false);
                    VersionIndicatorImage.color = Color.blue;
                    UpgradeInfo.text = "File created in an older version and will be upgraded when loading";
                    break;

                case SaveAndLoadSystem.UpgradeType.notSupported:
                    VersionIndicatorHolder.SetActive(true);
                    VersionIndicatorTransform.gameObject.SetActive(false);
                    NoUpgradeSymbol.SetActive(true);
                    UpgradeInfo.text = "File created in a newer version and cannot be read";
                    selectButton.gameObject.SetActive(false);
                    break;

                default:
                    break;
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