using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using iffnsStuff.iffnsBaseSystemForUnity;
using TMPro;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class Vector2IntLine : ControlLine
    {
        public TMP_InputField InputFieldX;
        public TMP_InputField InputFieldY;

        MailboxLineVector2Int currentVector2IntLine;

        IBaseObject lineOwner;

        //BuilderController controller;

        //public void SetUp(MailboxLineVector2Int vector2IntLine, BaseObject partToUpdateBuildParameters, BuilderController controller)
        public void SetUp(MailboxLineVector2Int vector2IntLine, IBaseObject partToUpdateBuildParameters, List<DelegateLibrary.VoidFunction> additionalCalls = null)
        {
            currentVector2IntLine = vector2IntLine;
            lineOwner = partToUpdateBuildParameters;
            //this.controller = controller;

            base.SetUp(vector2IntLine.Name);

            InputFieldX.text = vector2IntLine.Val.x.ToString();
            InputFieldX.onEndEdit.AddListener(ChangeXValue);

            InputFieldY.text = vector2IntLine.Val.y.ToString();
            InputFieldY.onEndEdit.AddListener(ChangeYValue);

            InputFieldX.contentType = TMP_InputField.ContentType.IntegerNumber;
            InputFieldY.contentType = TMP_InputField.ContentType.IntegerNumber;

            this.additionalCalls = additionalCalls;
        }

        public void ChangeXValue(string value)
        {
            int newValue = StringHelper.ConvertStringToInt(text: value, globalFormat: false, worked: out bool worked);

            if(worked) ChangeValueIfItWorks(axis: MathHelper.Vector2Axis.x, newValue: newValue);
        }

        public void ChangeYValue(string value)
        {
            int newValue = StringHelper.ConvertStringToInt(text: value, globalFormat: false, worked: out bool worked);

            if (worked) ChangeValueIfItWorks(axis: MathHelper.Vector2Axis.y, newValue: newValue);
        }

        void ChangeValueIfItWorks(MathHelper.Vector2Axis axis, int newValue)
        {
            Vector2Int previousValue = currentVector2IntLine.Val;

            currentVector2IntLine.Val = MathHelper.ChangeVectorValue(vector: currentVector2IntLine.Val, axis: axis, newValue: newValue);

            if (lineOwner != null)
            {
                bool previouslyFalied = lineOwner.Failed;

                lineOwner.ApplyBuildParameters();

                if (!previouslyFalied && lineOwner.Failed)
                {
                    switch (axis)
                    {
                        case MathHelper.Vector2Axis.x:
                            InputFieldX.text = previousValue.x.ToString();
                            break;
                        case MathHelper.Vector2Axis.y:
                            InputFieldY.text = previousValue.y.ToString();
                            break;
                        default:
                            Debug.LogWarning("Error: Unknown axis type");
                            break;
                    }

                    currentVector2IntLine.Val = previousValue;
                    lineOwner.ApplyBuildParameters();
                }
            }

            RunAllAdditionalCalls();
        }
    }
}