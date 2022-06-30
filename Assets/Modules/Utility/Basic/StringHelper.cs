using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

public class StringHelper : MonoBehaviour
{
    public static int ConvertStringToInt(string text, bool globalFormat, out bool worked)
    {
        int output;

        if (globalFormat)
        {
            worked = int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out output);
        }
        else
        {
            worked = int.TryParse(text, out output);
        }

        if (!worked)
        {
            Debug.LogWarning($"Error: {text} could not be converted into an int");
        }

        return output;
    }

    public static float ConvertStringToFloat(string text, bool globalFormat, out bool worked)
    {
        float output;

        if (globalFormat)
        {
            worked = float.TryParse(text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out output);
        }
        else
        {
            worked = float.TryParse(text, out output);
        }

        if (!worked)
        {
            Debug.LogWarning($"Error: {text} could not be converted into a float");
        }

        return output;
    }
}
