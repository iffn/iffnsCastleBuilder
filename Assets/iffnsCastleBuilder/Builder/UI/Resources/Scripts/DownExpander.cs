using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownExpander : UIHelper
{
    //Unity assignments
    [SerializeField] float bottomHeight = 3;

    //Runtime parameters
    RectTransform rectTransform;


    // Start is called before the first frame update
    public override void Setup(UISizeController linkedSizeController)
    {
        base.Setup(linkedSizeController);

        rectTransform = transform.GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (rectTransform == null) return;

        UpdateSize();
    }

    // Update is called once per frame
    public override void UpdateSize()
    {
        float newHeight = (transform.position.y - bottomHeight) / transform.lossyScale.y;
        
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, newHeight);
    }
}
