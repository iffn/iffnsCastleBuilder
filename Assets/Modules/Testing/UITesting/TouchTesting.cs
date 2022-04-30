using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchTesting : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        //Note: No touch controls in editor. On PC only when built

        Debug.Log("Touch supported = " + Input.touchSupported);
        Debug.Log("Touches = " + Input.touchCount);
    }
}
