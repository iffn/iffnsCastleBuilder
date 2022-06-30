using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class UnityHelper
{
    public static void ResetLocalTransform(Transform resetTransform)
    {
        resetTransform.localPosition = Vector3.zero;
        resetTransform.localRotation = Quaternion.identity;
        resetTransform.localScale = Vector3.one;
    }

    
    public static void OutputValue(ref float outputValue)
    {
        Debug.Log(nameof(outputValue) + ": " + outputValue);
    }
    
    public static bool CursorIsOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject(); //if over UI button
    }

    /*
    public static void OutputValue(ref object outputValue)
    {
        Debug.Log(nameof(outputValue) + ": " + outputValue);
    }

    public static void OutputValue(object outputValue)
    {
        Debug.Log(nameof(outputValue) + ": " + outputValue);
    }
    */
}
