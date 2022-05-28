using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridlLayoutOrganizer : MonoBehaviour
{
    RectTransform rectTransform;
    float rowHeight = 100;

    public void Setup()
    {
        rectTransform = transform.GetComponent<RectTransform>();
    }

    private void Start()
    {
        Setup();

        SetHeight();
    }

    void SetHeight()
    {
        //GridLayoutGroup gridLayout = transform.GetComponent<GridLayoutGroup>();

        int childCount = 0;

        foreach (Transform child in transform)
        {
            if (!child.gameObject.activeSelf) continue;

            /*
            RectTransform rect = child.GetComponent<RectTransform>();

            if (rect != null) return;
            */

            childCount++;
        }

        int rowCount = childCount / 3;
        if (childCount % 3 != 0) rowCount++;

        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rowCount * rowHeight);
    }
}
