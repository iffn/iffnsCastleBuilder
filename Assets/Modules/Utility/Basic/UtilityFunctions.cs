using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyStringComponents
{
    public static readonly char quote = '"';
    public static readonly char slash = '/';
    public static readonly char backslash = '\\';
    //public static readonly char tab = '\t';
    public static readonly string tab = "\t";
    public static readonly string newLine = System.Environment.NewLine;
}

public static class JsonLineHelper
{
    public abstract class JsonValue
    {
        public string name;
        public bool IsValid = true;

        protected JsonValue(string name)
        {
            this.name = name;
        }

        protected abstract string ValueString { get; set; }

        public static JsonValue FromJsonString(string jsonString)
        {
            string name;

            if (jsonString[0] != '"')
            {
                Debug.LogWarning("Error when assigning json string: String does not start with quote. JSon line = " + jsonString);
                return null;
            }

            if (jsonString[^1] == ',')
            {
                jsonString = jsonString.Remove(jsonString.Length - 1);
            }

            int colonLocation = jsonString.IndexOf("\":");

            /*
            if (colonLocation < 2)
            {
                Debug.LogWarning("Error when assigning json string: Colon arrangement not found in the correct location. JSon line = " + setValue);
                isValid = false;
                return;
            }
            */

            name = jsonString.Substring(startIndex: 1, length: colonLocation - 1);

            /*
            if (setValue.Length - colonLocation < 3)
            {
                Debug.LogWarning("Error when assigning json string: value string is too short = " + setValue);
                isValid = false;
                return;
            }
            */

            string valueString = jsonString.Substring(colonLocation + 3);

            if (valueString[^1] == '"')
            {
                if (valueString[0] == '"')
                {
                    valueString = valueString.Remove(0, 1);
                }

                if (valueString[^1] == '"')
                {
                    valueString = valueString.Remove(valueString.Length - 1);
                }

                return new JsonStringValue(name: name, value: valueString);
            }

            if (valueString.Contains('.'))
            {
                return new JsonFloatValue(name: name, value: valueString);
            }

            return new JsonIntValue(name: name, value: valueString);
        }

        public string JsonString
        {
            get
            {
                return MyStringComponents.quote + name + MyStringComponents.quote + ": " + ValueString;
            }
        }
    }

    public class JsonStringValue : JsonValue
    {
        public string value;

        public JsonStringValue(string name, string value) : base(name: name)
        {
            this.value = value;
        }

        protected override string ValueString
        {
            get
            {
                return MyStringComponents.quote + value + MyStringComponents.quote;
            }
            set
            {
                string valueString = value;

                if (valueString[0] == '"')
                {
                    valueString = valueString.Remove(0);
                }

                if (valueString[^1] == '"')
                {
                    valueString = valueString.Remove(-1);
                }

                this.value = valueString;
            }
        }
    }

    public class JsonIntValue : JsonValue
    {
        public int value;

        public JsonIntValue(string name, int value) : base(name: name)
        {
            this.value = value;
        }

        public JsonIntValue(string name, string value) : base(name: name)
        {
            this.ValueString = value;
        }

        protected override string ValueString
        {
            get
            {
                return value.ToString();
            }
            set
            {
                this.value = StringHelper.ConvertStringToInt(text: value, globalFormat: true, worked: out bool worked);

                if (!worked)
                {
                    Debug.LogWarning($"Error: {ValueString} could not be converted to an int");
                }
            }
        }
    }

    public class JsonFloatValue : JsonValue
    {
        public float value;

        public JsonFloatValue(string name, float value) : base(name: name)
        {
            this.value = value;
        }

        public JsonFloatValue(string name, string value) : base(name: name)
        {
            this.ValueString = value;
        }

        protected override string ValueString
        {
            get
            {
                return value.ToString();
            }
            set
            {
                string valueString = value;

                if (valueString[0] == '"')
                {
                    valueString = valueString.Remove(0);
                }

                if (valueString[^1] == '"')
                {
                    valueString = valueString.Remove(-1);
                }

                this.value = StringHelper.ConvertStringToFloat(text: ValueString, globalFormat: true, worked: out bool worked);

                if (!worked)
                {
                    Debug.LogWarning($"Error: {ValueString} could not be converted to an int");
                }
            }
        }
    }

    public static Vector3 ConvertJSONStringToVector3(string text, bool globalFormat, out bool worked)
    {
        string usedText = text.Replace("[", "");
        usedText = usedText.Replace("]", "");

        string[] numbers = usedText.Split(", ");

        if (numbers.Length != 3)
        {
            numbers = usedText.Split(",");

            if (numbers.Length != 3)
            {
                Debug.LogWarning($"Error: {text} could not be converted into a Vector3");
                worked = false;
                return Vector3Int.zero;
            }
        }

        float x = StringHelper.ConvertStringToFloat(text: numbers[0], globalFormat: globalFormat, worked: out bool xWorked);
        float y = StringHelper.ConvertStringToFloat(text: numbers[1], globalFormat: globalFormat, worked: out bool yWorked);
        float z = StringHelper.ConvertStringToFloat(text: numbers[2], globalFormat: globalFormat, worked: out bool zWorked);

        if (!xWorked || !yWorked || !zWorked)
        {
            Debug.LogWarning($"Error: {text} could not be converted into a Vector3");
            worked = false;
            return Vector3Int.zero;
        }

        worked = true;
        return new Vector3(x, y, z);
    }


    public static Vector2Int ConvertJSONStringToVector2Int(string text, out bool worked)
    {
        string usedText = text.Replace("[", "");
        usedText = usedText.Replace("]", "");

        string[] numbers = usedText.Split(", ");

        if (numbers.Length != 2)
        {
            numbers = usedText.Split(",");

            if (numbers.Length != 2)
            {
                Debug.LogWarning($"Error: {text} could not be converted into a Vector2Int");
                worked = false;
                return Vector2Int.zero;
            }
        }

        int x = StringHelper.ConvertStringToInt(text: numbers[0], globalFormat: true, worked: out bool xWorked);
        int y = StringHelper.ConvertStringToInt(text: numbers[1], globalFormat: true, worked: out bool yWorked);

        if (!xWorked || !yWorked)
        {
            Debug.LogWarning($"Error: {text} could not be converted into a Vector3");
            worked = false;
            return Vector2Int.zero;
        }

        worked = true;
        return new Vector2Int(x, y);
    }

    public static Vector2 ConvertJSONStringToVector2(string text, out bool worked)
    {
        string usedText = text.Replace("[", "");
        usedText = usedText.Replace("]", "");

        string[] numbers = usedText.Split(", ");

        if (numbers.Length != 2)
        {
            numbers = usedText.Split(",");

            if (numbers.Length != 2)
            {
                Debug.LogWarning($"Error: {text} could not be converted into a Vector2");
                worked = false;
                return Vector2.zero;
            }
        }

        float x = StringHelper.ConvertStringToFloat(text: numbers[0], globalFormat: true, worked: out bool xWorked);
        float y = StringHelper.ConvertStringToFloat(text: numbers[1], globalFormat: true, worked: out bool yWorked);

        if (!xWorked || !yWorked)
        {
            Debug.LogWarning($"Error: {text} could not be converted into a Vector2");
            worked = false;
            return Vector2.zero;
        }

        worked = true;
        return new Vector2(x, y);
    }

    public static Quaternion ConvertJSONStringToQuaternion(string text, out bool worked)
    {
        string usedText = text.Replace("[", "");
        usedText = usedText.Replace("]", "");

        string[] numbers = usedText.Split(", ");

        if (numbers.Length != 4)
        {
            numbers = usedText.Split(",");

            if (numbers.Length != 4)
            {
                Debug.LogWarning($"Error: {text} could not be converted into a Quaternion");
                worked = false;
                return Quaternion.identity;
            }
        }

        int x = StringHelper.ConvertStringToInt(text: numbers[0], globalFormat: true, worked: out bool xWorked);
        int y = StringHelper.ConvertStringToInt(text: numbers[1], globalFormat: true, worked: out bool yWorked);
        int z = StringHelper.ConvertStringToInt(text: numbers[2], globalFormat: true, worked: out bool zWorked);
        int w = StringHelper.ConvertStringToInt(text: numbers[3], globalFormat: true, worked: out bool wWorked);

        if (!xWorked || !yWorked || !zWorked || !wWorked)
        {
            Debug.LogWarning($"Error: {text} could not be converted into a Quaternion");
            worked = false;
            return Quaternion.identity;
        }

        worked = true;
        return new Quaternion(x, y, z, w);
    }

    public static Color ConvertJSONStringToColor(string text, out bool worked)
    {
        string usedText = text.Replace("[", "");
        usedText = usedText.Replace("]", "");

        string[] numbers = usedText.Split(", ");

        if (numbers.Length != 3 && numbers.Length != 4)
        {
            numbers = usedText.Split(",");

            if (numbers.Length != 3 && numbers.Length != 4)
            {
                Debug.LogWarning($"Error: {text} could not be converted into a Color");
                worked = false;
                return Color.magenta;
            }
        }

        float r, g, b, a = 1;

        bool aWorked = true;

        r = StringHelper.ConvertStringToFloat(text: numbers[0], globalFormat: true, worked: out bool rWorked);
        g = StringHelper.ConvertStringToFloat(text: numbers[1], globalFormat: true, worked: out bool gWorked);
        b = StringHelper.ConvertStringToFloat(text: numbers[2], globalFormat: true, worked: out bool bWorked);

        if (numbers.Length == 4)
        {
            a = StringHelper.ConvertStringToFloat(text: numbers[3], globalFormat: true, worked: out aWorked);
        }

        if(!rWorked || !gWorked || !bWorked || !aWorked)
        {
            worked = false;
            Debug.LogWarning($"Error: {text} could not be converted into a Color");
            return Color.magenta;
        }

        worked = true;
        return new Color(r, g, b, a);
    }
}

public static class UtilityFunctions
{
    public static void LogHierarchyLocationOfGameObject(GameObject currentObject)
    {
        List<string> hierarchyString = new List<string>();

        Transform investigationObject = currentObject.transform;

        while(investigationObject.transform.parent != null)
        {
            investigationObject = investigationObject.transform.parent;

            hierarchyString.Add(investigationObject.name);
        }

        if(hierarchyString.Count == 0)
        {
            Debug.Log("Hierarchy position of " + currentObject.name + " is at the top");
            return;
        }

        string outputString = "";

        for(int i = hierarchyString.Count - 1; i == 0; i--)
        {
            outputString += hierarchyString[i] + " - ";
        }

        outputString = outputString.Remove(outputString.Length - 3);

        Debug.Log("Hierarchy position of " + currentObject.name + " = " + outputString);
    }
}
