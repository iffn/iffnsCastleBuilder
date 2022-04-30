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
