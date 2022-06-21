using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class VersionMismatchController : MonoBehaviour
    {
        [SerializeField] GameObject NoUpgradeSymbol;
        [SerializeField] RectTransform VersionIndicatorTransform;
        [SerializeField] Image VersionIndicatorImage;
        [SerializeField] TMP_Text UpgradeInfo;

        public void UpdateData(SaveAndLoadSystem.UpgradeType upgradeType)
        {
            switch (upgradeType)
            {
                case SaveAndLoadSystem.UpgradeType.sameVersion:
                    gameObject.SetActive(false);
                    break;

                case SaveAndLoadSystem.UpgradeType.noIssueNewVersion:
                    gameObject.SetActive(false);
                    break;

                case SaveAndLoadSystem.UpgradeType.someElementsNotSupported:
                    gameObject.SetActive(true);
                    NoUpgradeSymbol.SetActive(false);
                    VersionIndicatorTransform.localEulerAngles = 180 * Vector3.forward;
                    VersionIndicatorImage.color = Color.yellow;
                    UpgradeInfo.text = "This file was created in a newer version. Some elements may not be supported.";
                    break;

                case SaveAndLoadSystem.UpgradeType.someNotSupportedAndWrongPosition:
                    gameObject.SetActive(true);
                    NoUpgradeSymbol.SetActive(false);
                    VersionIndicatorTransform.localEulerAngles = 180 * Vector3.forward;
                    VersionIndicatorImage.color = Color.red;
                    UpgradeInfo.text = "This file was created in a newer version. Some elements may not be supported while others may have wrong parameters when loading.";
                    break;

                case SaveAndLoadSystem.UpgradeType.upgrade:
                    VersionIndicatorTransform.localEulerAngles = Vector3.zero;
                    gameObject.SetActive(true);
                    NoUpgradeSymbol.SetActive(false);
                    VersionIndicatorImage.color = Color.blue;
                    UpgradeInfo.text = "This file was created in an older version and will be upgraded when loading.";
                    break;

                case SaveAndLoadSystem.UpgradeType.notSupported:
                    gameObject.SetActive(true);
                    VersionIndicatorTransform.gameObject.SetActive(false);
                    NoUpgradeSymbol.SetActive(true);
                    UpgradeInfo.text = "This file was created in a newer version and cannot be read.";
                    break;

                default:
                    break;
            }
        }

        public void Setup(SaveAndLoadSystem.UpgradeType upgradeType)
        {
            UpdateData(upgradeType: upgradeType);
        }
    }
}