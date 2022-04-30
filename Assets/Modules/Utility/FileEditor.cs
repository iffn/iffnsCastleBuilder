using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FileEditor
{
    /*
    public static List<string> SplitLineIntoTitleAndContent(string text)
    {
        string txt = text;

        List<string> returnList = new List<string>();

        txt.Replace(myStringComponents.tab, ""); //ToDo: Get Replace to work

        if(txt.Contains(":"))
        {
            int colonLocation = txt.IndexOf(":");

            returnList.Add(txt.Substring(0, colonLocation));

            if(txt.Length - txt.IndexOf(":") >= 2)
            {
                txt = txt.Substring(colonLocation + 2);
                returnList.Add(txt);
            }
        }
        else
        {
            returnList.Add("");
        }

        return returnList;
    }
    */
    public static List<string> SepparateLineAtPart(string text, string part)
    {
        string txt = "" + text;

        List<string> returnList = new List<string>();

        txt.Replace(MyStringComponents.tab, ""); //ToDo: Get Replace to work

        if (txt.Contains(part))
        {
            int partLocation = txt.IndexOf(part);

            returnList.Add(txt.Substring(0, partLocation));

            if (txt.Length - txt.IndexOf(part) >= part.Length)
            {
                txt = txt.Substring(partLocation + part.Length);
                returnList.Add(txt);
            }
        }
        else
        {
            returnList.Add(txt);
        }

        return returnList;
    }

    public static int ConvertStringToInt(string text)
    {

        int returnValue = 0;

        try
        {
            int.TryParse(text, out returnValue);
        }
        catch
        {
            //ToDo: Error detected
        }
        finally
        {

        }

        return returnValue;
    }

    public static float ConvertStringToFloat(string text)
    {

        float returnValue = 0;

        try
        {
            float.TryParse(text, out returnValue);
        }
        catch
        {
            //ToDo: Error detected
        }
        finally
        {

        }

        return returnValue;
    }

    

    public static Vector3 ConvertJSONStringToVector3(string text)
    {
        string txt = text;

        Vector3 returnVector = Vector3.zero;

        //X value
        if (txt.Contains(","))
        {
            float x = 0;

            try
            {
                float.TryParse(txt.Substring(1, txt.IndexOf(",") - 1), out x);
            }
            catch
            {
                //ToDo: Error detected
            }
            finally
            {

            }

            returnVector.x = x;

            txt = txt.Substring(txt.IndexOf(",") + 1);
        }
        else
        {
            //ToDo: Error detected
            return Vector3.zero;
        }

        //Y Value
        if (txt.Contains(","))
        {
            float y = 0;

            try
            {
                string temp = txt.Substring(0, txt.IndexOf(","));
                float.TryParse(temp, out y);
            }
            catch
            {
                //ToDo: Error detected
            }
            finally
            {

            }

            returnVector.y = y;
            txt = txt.Substring(txt.IndexOf(","));
            
        }
        else
        {
            //ToDo: Error detected
            return Vector3.zero;
        }

        //Z Value;
        if (!txt.Contains("]"))
        {
            txt = txt.Substring(1, txt.Length - 2);

            float z = 0;

            try
            {
                float.TryParse(txt, out z);
            }
            catch
            {
                //ToDo: Error detected
            }
            finally
            {

            }

            returnVector.z = z;
        }
        else
        {
            //ToDo: Error detected
            return Vector3.zero;
        }

        return returnVector;
    }


    public static Vector2Int ConvertJSONStringToVector2Int(string text)
    {
        string txt = text;

        Vector2Int returnVector = Vector2Int.zero;

        //X value
        if (txt.Contains(","))
        {
            int x = 0;

            try
            {

                int.TryParse(txt.Substring(1, txt.IndexOf(",") - 1), out x);
            }
            catch
            {
                //ToDo: Error detected
            }
            finally
            {

            }

            returnVector.x = x;

            txt = txt.Substring(txt.IndexOf(",") + 1);
        }
        else
        {
            //ToDo: Error detected
            return Vector2Int.zero;
        }

        txt = txt.Substring(0, txt.Length - 1);

        //Y Value;
        if (!txt.Contains(","))
        {
            int y = 0;

            try
            {
                int.TryParse(txt, out y);
            }
            catch
            {
                //ToDo: Error detected
            }
            finally
            {

            }

            returnVector.y = y;
        }
        else
        {
            //ToDo: Error detected
            return Vector2Int.zero;
        }

        return returnVector;
    }

    public static Vector2 ConvertJSONStringToVector2(string text)
    {
        string txt = text;

        Vector2 returnVector = Vector2.zero;

        //X value
        if (txt.Contains(","))
        {
            float x = 0;

            try
            {

                float.TryParse(txt.Substring(1, txt.IndexOf(",") - 1), out x);
            }
            catch
            {
                //ToDo: Error detected
            }
            finally
            {

            }

            returnVector.x = x;

            txt = txt.Substring(txt.IndexOf(",") + 1);
        }
        else
        {
            //ToDo: Error detected
            return Vector2Int.zero;
        }

        txt = txt.Substring(0, txt.Length - 1);

        //Y Value;
        if (!txt.Contains(","))
        {
            float y = 0;

            try
            {
                float.TryParse(txt, out y);
            }
            catch
            {
                //ToDo: Error detected
            }
            finally
            {

            }

            returnVector.y = y;
        }
        else
        {
            //ToDo: Error detected
            return Vector2.zero;
        }

        return returnVector;
    }

    public static Quaternion ConvertJSONStringToQuaternion(string text)
    {
        string txt = text;

        Quaternion returnRotation = Quaternion.identity;

        //X value
        if (txt.Contains(","))
        {
            float x = 0;

            try
            {

                float.TryParse(txt.Substring(1, txt.IndexOf(",") - 1), out x);
            }
            catch
            {
                //ToDo: Error detected
            }
            finally
            {

            }

            returnRotation.x = x;

            txt = txt.Substring(txt.IndexOf(",") + 1);
        }
        else
        {
            //ToDo: Error detected
            return Quaternion.identity;
        }

        //Y Value
        if (txt.Contains(","))
        {
            float y = 0;

            try
            {
                float.TryParse(txt.Substring(0, txt.IndexOf(",")), out y);
            }
            catch
            {
                //ToDo: Error detected
            }
            finally
            {

            }

            returnRotation.y = y;
            txt = txt.Substring(txt.IndexOf(",") + 1);
        }
        else
        {
            //ToDo: Error detected
            return Quaternion.identity;
        }

        //Z Value
        if (txt.Contains(","))
        {
            float z = 0;

            try
            {
                float.TryParse(txt.Substring(0, txt.IndexOf(",")), out z);
            }
            catch
            {
                //ToDo: Error detected
            }
            finally
            {

            }

            returnRotation.z = z;
            txt = txt.Substring(txt.IndexOf(",") + 1);
        }
        else
        {
            //ToDo: Error detected
            return Quaternion.identity;
        }

        txt = txt.Substring(0, txt.Length - 1);

        //W Value;
        if (!txt.Contains(","))
        {
            float w = 0;

            try
            {
                float.TryParse(txt, out w);
            }
            catch
            {
                //ToDo: Error detected
            }
            finally
            {

            }

            returnRotation.w = w;
        }
        else
        {
            //ToDo: Error detected
            return Quaternion.identity;
        }

        return returnRotation;
    }

    public static Color ConvertJSONStringToColor(string text)
    {
        Color returnColor = new Color();

        returnColor = Color.white;

        //ToDO: Define color

        return returnColor;
    }
}
