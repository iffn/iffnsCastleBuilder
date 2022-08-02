using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class Vector3Line : ControlLine
    {

        public TMP_InputField InputFieldX;
        public TMP_InputField InputFieldY;
        public TMP_InputField InputFieldZ;

        MailboxLineVector3 currentVector3Line;

        IBaseObject lineOwner;

        //BuilderController controller;


        // Use this for initialization
        public void SetUp(MailboxLineVector3 vector3Line, IBaseObject partToUpdateBuildParameters, List<DelegateLibrary.VoidFunction> additionalCalls = null)
        //public void SetUp(MailboxLineVector3 vector3Line, BaseObject partToUpdateBuildParameters, BuilderController controller)
        {
            currentVector3Line = vector3Line;
            lineOwner = partToUpdateBuildParameters;
            //this.controller = controller;

            base.SetUp(vector3Line.Name);

            InputFieldX.text = StringHelper.ConvertFloatToString(value: vector3Line.Val.x, globalFormat: false);
            InputFieldX.onEndEdit.AddListener(ChangeXValue);

            InputFieldX.text = StringHelper.ConvertFloatToString(value: vector3Line.Val.y, globalFormat: false);
            InputFieldY.onEndEdit.AddListener(ChangeYValue);

            InputFieldX.text = StringHelper.ConvertFloatToString(value: vector3Line.Val.z, globalFormat: false);
            InputFieldZ.onEndEdit.AddListener(ChangeZValue);

            InputFieldX.contentType = TMP_InputField.ContentType.DecimalNumber;
            InputFieldY.contentType = TMP_InputField.ContentType.DecimalNumber;
            InputFieldZ.contentType = TMP_InputField.ContentType.DecimalNumber;

            this.additionalCalls = additionalCalls;
        }

        public void ChangeXValue(string value)
        {
            float newValue = StringHelper.ConvertStringToFloat(text: value, globalFormat: false, worked: out bool worked);

            if (worked) ChangeValueIfItWorks(axis: MathHelper.Vector3Axis.x, newValue: newValue);
        }

        public void ChangeYValue(string value)
        {
            float newValue = StringHelper.ConvertStringToFloat(text: value, globalFormat: false, worked: out bool worked);

            if (worked) ChangeValueIfItWorks(axis: MathHelper.Vector3Axis.y, newValue: newValue);
        }

        public void ChangeZValue(string value)
        {
            float newValue = StringHelper.ConvertStringToFloat(text: value, globalFormat: false, worked: out bool worked);

            if (worked) ChangeValueIfItWorks(axis: MathHelper.Vector3Axis.z, newValue: newValue);
        }

        void ChangeValueIfItWorks(MathHelper.Vector3Axis axis, float newValue)
        {
            Vector3 previousValue = currentVector3Line.Val;

            currentVector3Line.Val = MathHelper.ChangeVectorValue(vector: currentVector3Line.Val, axis: axis, newValue: newValue);

            if (lineOwner != null)
            {
                bool previouslyFalied = lineOwner.Failed;

                lineOwner.ApplyBuildParameters();

                if (!previouslyFalied && lineOwner.Failed)
                {
                    switch (axis)
                    {
                        case MathHelper.Vector3Axis.x:
                            InputFieldX.text = StringHelper.ConvertFloatToString(value: previousValue.x, globalFormat: false);
                            break;
                        case MathHelper.Vector3Axis.y:
                            InputFieldX.text = StringHelper.ConvertFloatToString(value: previousValue.y, globalFormat: false);
                            break;
                        case MathHelper.Vector3Axis.z:
                            InputFieldX.text = StringHelper.ConvertFloatToString(value: previousValue.z, globalFormat: false);
                            break;
                        default:
                            Debug.LogWarning("Error: Unknown axis type");
                            break;
                    }

                    currentVector3Line.Val = previousValue;
                    lineOwner.ApplyBuildParameters();
                }
            }

            RunAllAdditionalCalls();
        }
    }
}