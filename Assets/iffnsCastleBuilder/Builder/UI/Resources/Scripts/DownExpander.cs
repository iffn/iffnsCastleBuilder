using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownExpander : MonoBehaviour
{
    //Unity assignments
    [SerializeField] float bottomHeight = 3;

    //Runtime parameters
    RectTransform rectTransform;


    // Start is called before the first frame update
    void Start()
    {
        rectTransform = transform.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        float newHeight = (transform.position.y - bottomHeight) / transform.lossyScale.y;
        
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, newHeight);
    }
}
