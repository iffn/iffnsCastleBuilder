using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISizeController : MonoBehaviour
{
    //Unity assignments
    [SerializeField] Slider testSlider;

    [SerializeField] RectTransform MenuLeft;
    [SerializeField] RectTransform BackgroundLeft;
    [SerializeField] RectTransform LeftEdgeButtons;

    //Runtime parameters
    float buttonSize = 100;

    //Fixed parameters
    float defaultButtonSize = 100;
    Vector2Int buttonCount = new Vector2Int(12, 6);
    Vector2Int minAddition = new Vector2Int(4, 4);


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
        
    }

    // Update is called once per frame
    void Update()
    {
        buttonSize = defaultButtonSize * testSlider.value;

        buttonSize = Mathf.Clamp(value: buttonSize, min: 1, max: MinButtonSize);

        float scaleFactor = buttonSize / defaultButtonSize;
        float invertedScaleFactor = 1 / scaleFactor;

        Vector3 scaleVector = scaleFactor * Vector3.one;

        MenuLeft.localScale = scaleVector;
        BackgroundLeft.localScale = scaleVector;
        BackgroundLeft.sizeDelta = new Vector2(BackgroundLeft.sizeDelta.x, Screen.height * invertedScaleFactor);

        LeftEdgeButtons.anchoredPosition = 3 * defaultButtonSize * scaleFactor * Vector3.right;
        LeftEdgeButtons.localScale = scaleVector;

    }
}
