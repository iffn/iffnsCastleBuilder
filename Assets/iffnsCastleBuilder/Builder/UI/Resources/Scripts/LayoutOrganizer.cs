using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayoutOrganizer : MonoBehaviour
{
    [SerializeField] List<LayoutOrganizer> ChildLayoutOrganizers;

    RectTransform rectTransform;


    public void Setup()
    {
        rectTransform = transform.GetComponent<RectTransform>();
    }

    public float SetAndReturnHeight()
    {
        if (ChildLayoutOrganizers.Count == 0) return rectTransform.sizeDelta.y;

        float shouldBeHeight = 0;

        foreach (LayoutOrganizer child in ChildLayoutOrganizers)
        {
            shouldBeHeight += child.SetAndReturnHeight();
        }

        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, shouldBeHeight);

        return shouldBeHeight;
    }

    public void UpdateLayout()
    {

    }
}
