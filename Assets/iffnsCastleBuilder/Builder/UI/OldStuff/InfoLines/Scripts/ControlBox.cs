using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using iffnsStuff.iffnsBaseSystemForUnity;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class ControlBox : MonoBehaviour
    {
        public UIResources uiResources;

        Transform SpawnLine(GameObject template)
        {
            Transform newObject = Instantiate(template).transform;
            newObject.SetParent(transform);

            RectTransform rectTransform = newObject.GetComponent<RectTransform>();
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localScale = Vector3.one;

            return newObject;
        }

        public void AddTextLine(string text, bool bold)
        {
            Transform newObject = SpawnLine(uiResources.TextLineTemplate.gameObject);

            newObject.GetComponent<TextLine>().SetUp(text, bold);
        }

        public void AddButtonLine(string text, UnityEngine.Events.UnityAction call)
        {
            Transform newObject = SpawnLine(uiResources.ButtonLineTemplate.gameObject);

            newObject.GetComponent<ButtonLine>().SetUp(text, call);
        }

        Transform getInputLineTemplate(bool longLine)
        {
            Transform newObject;

            if (longLine) newObject = SpawnLine(uiResources.InputLineLargeTemplate.gameObject);
            else newObject = SpawnLine(uiResources.InputLineSmallTemplate.gameObject);

            return newObject;
        }

        public void AddInputLine(string title, UnityEngine.Events.UnityAction<string> ReturnFunctionScript, bool longLine = false, List<DelegateLibrary.VoidFunction> additionalCalls = null)
        {
            Transform newObject = getInputLineTemplate(longLine: longLine);

            if (longLine) newObject = SpawnLine(uiResources.InputLineLargeTemplate.gameObject);
            else newObject = SpawnLine(uiResources.InputLineSmallTemplate.gameObject);

            newObject.GetComponent<InputLine>().SetUp(text: title, ReturnFunctionScript: ReturnFunctionScript, additionalCalls: additionalCalls);
        }

        public void AddInputLine(MailboxLineString stringLine, IBaseObject lineOwner, bool longLine = false, List<DelegateLibrary.VoidFunction> additionalCalls = null)
        {
            Transform newObject = getInputLineTemplate(longLine: longLine);

            newObject.GetComponent<InputLine>().SetUp(stringLine, lineOwner, additionalCalls);
        }

        public void AddInputLine(MailboxLineRanged rangedLine, IBaseObject lineOwner, bool longLine = false, List<DelegateLibrary.VoidFunction> additionalCalls = null)
        {
            Transform newObject = getInputLineTemplate(longLine: longLine);

            newObject.GetComponent<InputLine>().SetUp(rangedLine, lineOwner, additionalCalls);
        }

        public void AddInputLine(MailboxLineDistinctUnnamed distinctUnnamedLine, IBaseObject lineOwner, bool longLine = false, List<DelegateLibrary.VoidFunction> additionalCalls = null)
        {
            Transform newObject = getInputLineTemplate(longLine: longLine);

            newObject.GetComponent<InputLine>().SetUp(distinctUnnamedLine, lineOwner, additionalCalls);
        }

        public void AddInputLineBool(MailboxLineBool boolLine, IBaseObject lineOwner, List<DelegateLibrary.VoidFunction> additionalCalls = null)
        {
            Transform newObject = SpawnLine(uiResources.InputLineBool.gameObject).transform;

            newObject.GetComponent<InputLineBool>().SetUp(boolLine: boolLine, lineOwner: lineOwner, additionalCalls: additionalCalls);
        }

        public void AddVector3Line(MailboxLineVector3 vector3Line, IBaseObject lineOwner, List<DelegateLibrary.VoidFunction> additionalCalls = null)
        {
            Transform newObject = SpawnLine(uiResources.Vector3LineTemplate.gameObject);

            newObject.GetComponent<Vector3Line>().SetUp(vector3Line, lineOwner, additionalCalls: additionalCalls);
        }

        public void AddVector2IntLine(MailboxLineVector2Int vector2IntLine, IBaseObject lineOwner, List<DelegateLibrary.VoidFunction> additionalCalls = null)
        {
            Transform newObject = SpawnLine(uiResources.Vector2IntLineTemplate.gameObject);

            newObject.GetComponent<Vector2IntLine>().SetUp(vector2IntLine, lineOwner, additionalCalls: additionalCalls);
        }

        public void AddMailboxLineDistinctNamed(MailboxLineDistinctNamed distinctNamedLine, IBaseObject lineOwner, bool longLine = false, List<DelegateLibrary.VoidFunction> additionalCalls = null)
        {
            Transform newObject;

            if (longLine)
            {
                newObject = SpawnLine(uiResources.SelectLineLargeTemplate.gameObject);
            }
            else
            {
                newObject = SpawnLine(uiResources.SelectLineSmallTemplate.gameObject);
            }

            newObject.GetComponent<SelectLine>().SetUp(distinctNamedLine, lineOwner, additionalCalls: additionalCalls);
        }

        public void AddMailboxLine(MailboxLineSingle line, IBaseObject lineOwner, List<DelegateLibrary.VoidFunction> additionalCalls = null)
        {
            if (line is MailboxLineString)
            {
                if (line.Name.Length < 10)
                {
                    AddInputLine(line as MailboxLineString, lineOwner: lineOwner, longLine: false, additionalCalls: additionalCalls);
                }
                else
                {
                    AddInputLine(line as MailboxLineString, lineOwner: lineOwner, longLine: true, additionalCalls: additionalCalls);
                }
            }
            else if (line is MailboxLineRanged)
            {
                AddInputLine(line as MailboxLineRanged, lineOwner: lineOwner, additionalCalls: additionalCalls);
            }
            else if (line is MailboxLineDistinctUnnamed)
            {
                AddInputLine(line as MailboxLineDistinctUnnamed, lineOwner: lineOwner, additionalCalls: additionalCalls);
            }
            else if (line is MailboxLineBool)
            {
                AddInputLineBool(line as MailboxLineBool, lineOwner: lineOwner, additionalCalls: additionalCalls);
            }
            else if (line is MailboxLineVector3)
            {
                AddVector3Line(line as MailboxLineVector3, lineOwner: lineOwner, additionalCalls: additionalCalls);
            }
            else if (line is MailboxLineVector2Int)
            {
                AddVector2IntLine(line as MailboxLineVector2Int, lineOwner: lineOwner, additionalCalls: additionalCalls);
            }
            else if (line is MailboxLineDistinctNamed)
            {
                if (line.Name.Length < 10)
                {
                    AddMailboxLineDistinctNamed(line as MailboxLineDistinctNamed, lineOwner: lineOwner, longLine: false, additionalCalls: additionalCalls);
                }
                else
                {
                    AddMailboxLineDistinctNamed(line as MailboxLineDistinctNamed, lineOwner: lineOwner, longLine: true, additionalCalls: additionalCalls);
                }
            }
        }

        public void AddMailboxLines(List<MailboxLineSingle> lines, IBaseObject lineOwner, List<DelegateLibrary.VoidFunction> additionalCalls = null)
        {
            foreach (MailboxLineSingle line in lines)
            {
                AddMailboxLine(line: line, lineOwner: lineOwner, additionalCalls: additionalCalls);
            }
        }

        public void Clear()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}