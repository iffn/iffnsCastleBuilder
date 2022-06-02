using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISizeController : MonoBehaviour
{
    //Unity assignments
    [SerializeField] Slider UIScaler;

    [SerializeField] RectTransform MenuLeft;
    [SerializeField] RectTransform BackgroundLeft;
    [SerializeField] RectTransform LeftEdgeButtons;

    [SerializeField] RectTransform MenuRightTop;
    [SerializeField] RectTransform MenuRightBottom;
    [SerializeField] RectTransform BackgroundRight;
    [SerializeField] RectTransform FileSelector;
    [SerializeField] RectTransform FileListArea;
    [SerializeField] RectTransform UIScaleButton;

    [SerializeField] List<UIHelper> UIHelpers;

    //Runtime parameters
    float buttonSize = 100;

    //Fixed parameters
    float minButtonSize = 20;
    float defaultButtonSize = 100;
    Vector2Int buttonCount = new Vector2Int(12, 6);
    Vector2Int minAddition = new Vector2Int(4, 4);

    int prevWidth = 0;
    int prevHeight = 0;

    float MinButtonSize
    {
        get
        {
            Vector2Int minSize = buttonCount + minAddition;

            float minWidth = Screen.width / minSize.x;
            float minHeight = Screen.height / minSize.y;

            if(minWidth < minHeight) return minWidth;
            else return minHeight;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach(UIHelper helper in UIHelpers)
        {
            helper.Setup(linkedSizeController: this);
        }
    }

    public void UpdateSize()
    {
        //Calculate size
        buttonSize = defaultButtonSize * UIScaler.value;

        buttonSize = Mathf.Clamp(value: buttonSize, min: minButtonSize, max: MinButtonSize);

        UIScaler.value = buttonSize / defaultButtonSize;

        float scaleFactor = buttonSize / defaultButtonSize;
        float invertedScaleFactor = 1 / scaleFactor;

        Vector3 scaleVector = scaleFactor * Vector3.one;

        //General
        UIScaleButton.localScale = scaleFactor * Vector3.one;

        //Left menu
        MenuLeft.localScale = scaleVector;
        BackgroundLeft.localScale = scaleVector;
        BackgroundLeft.sizeDelta = new Vector2(BackgroundLeft.sizeDelta.x, Screen.height * invertedScaleFactor);

        LeftEdgeButtons.anchoredPosition = 3 * defaultButtonSize * scaleFactor * Vector3.right;
        LeftEdgeButtons.localScale = scaleVector;

        //Right menu
        MenuRightTop.localScale = scaleVector;
        MenuRightBottom.localScale = scaleVector;

        //Top
        float fileNameWidth = Screen.width - (buttonCount.x + 0.5f) * buttonSize;
        FileSelector.sizeDelta = new Vector2(fileNameWidth * invertedScaleFactor, FileSelector.sizeDelta.y);

        BackgroundRight.localScale = new Vector3(scaleFactor, BackgroundRight.localScale.y, BackgroundRight.localScale.z);

        FileListArea.sizeDelta = new Vector2((fileNameWidth + buttonSize * 0.5f) * invertedScaleFactor, (Screen.height - buttonSize * 0.5f) * invertedScaleFactor);

        //Rest
        foreach (UIHelper helper in UIHelpers)
        {
            helper.UpdateSize();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (prevWidth != Screen.width || prevHeight != Screen.height)
        {
            prevHeight = Screen.height;
            prevWidth = Screen.width;

            UpdateSize();
        }
    }
}
