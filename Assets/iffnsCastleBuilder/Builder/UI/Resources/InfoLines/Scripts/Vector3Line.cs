using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class Vector3Line : ControlLine
    {

        public InputField InputFieldX;
        public InputField InputFieldY;
        public InputField InputFieldZ;

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

            InputFieldX.text = vector3Line.Val.x.ToString();
            InputFieldX.onEndEdit.AddListener(ChangeXValue);

            InputFieldY.text = vector3Line.Val.y.ToString();
            InputFieldY.onEndEdit.AddListener(ChangeYValue);

            InputFieldZ.text = vector3Line.Val.z.ToString();
            InputFieldZ.onEndEdit.AddListener(ChangeZValue);

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
                            InputFieldX.text = previousValue.x.ToString();
                            break;
                        case MathHelper.Vector3Axis.y:
                            InputFieldY.text = previousValue.y.ToString();
                            break;
                        case MathHelper.Vector3Axis.z:
                            InputFieldZ.text = previousValue.z.ToString();
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