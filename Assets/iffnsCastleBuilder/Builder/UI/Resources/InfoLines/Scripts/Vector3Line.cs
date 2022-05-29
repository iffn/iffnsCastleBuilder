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
            currentVector3Line.Val = new Vector3(float.Parse(value), currentVector3Line.Val.y, currentVector3Line.Val.z);

            if (lineOwner != null) lineOwner.ApplyBuildParameters();

            RunAllAdditionalCalls();
        }

        public void ChangeYValue(string value)
        {
            currentVector3Line.Val = new Vector3(currentVector3Line.Val.x, float.Parse(value), currentVector3Line.Val.z); ;

            if (lineOwner != null) lineOwner.ApplyBuildParameters();

            RunAllAdditionalCalls();
        }

        public void ChangeZValue(string value)
        {
            currentVector3Line.Val = new Vector3(currentVector3Line.Val.x, currentVector3Line.Val.y, float.Parse(value));

            if (lineOwner != null) lineOwner.ApplyBuildParameters();

            RunAllAdditionalCalls();
        }
    }
}