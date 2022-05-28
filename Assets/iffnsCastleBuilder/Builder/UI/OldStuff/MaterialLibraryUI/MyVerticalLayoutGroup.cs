using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class MyVerticalLayoutGroup : MonoBehaviour
    {
        RectTransform thisRectTransform;

        // Start is called before the first frame update
        void Start()
        {
            thisRectTransform = transform.GetComponent<RectTransform>();
        }

        // Update is called once per frame
        void Update()
        {
            float currentSize = 0;

            foreach (Transform child in transform)
            {
                RectTransform element = child.GetComponent<RectTransform>();

                if (!child.gameObject.activeSelf) continue;

                element.localPosition = new Vector3(0, -currentSize, 0);
                //element.localPosition = new Vector3(element.position.x, -currentSize, element.position.z);

                currentSize += element.sizeDelta.y;
            }

            thisRectTransform.sizeDelta = new Vector2(thisRectTransform.sizeDelta.x, currentSize);
        }
    }
}