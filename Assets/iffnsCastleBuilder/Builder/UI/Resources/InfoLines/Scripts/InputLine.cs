using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class InputLine : ControlLine
    {
        public InputField InputField;

        MailboxLineString stringLine;
        MailboxLineRanged rangedLine;
        MailboxLineDistinctUnnamed distinctUnnamedLine;
        IBaseObject lineOwner;

        //BuilderController controller;

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }

        public void SetUp(string text, UnityAction<string> ReturnFunctionScript, List<DelegateLibrary.VoidFunction> additionalCalls = null)
        {
            base.SetUp(text);

            InputField.onEndEdit.AddListener(ReturnFunctionScript);

            this.additionalCalls = additionalCalls;
        }

        //public void SetUp(MailboxLineString stringLine, BaseObject lineOwner, BuilderController controller)
        public void SetUp(MailboxLineString stringLine, IBaseObject lineOwner, List<DelegateLibrary.VoidFunction> additionalCalls = null)
        {
            this.stringLine = stringLine;
            this.lineOwner = lineOwner;
            //this.controller = controller;

            base.SetUp(stringLine.Name);

            InputField.text = stringLine.Val;
            InputField.onEndEdit.AddListener(ChangeStringLineValue);

            this.additionalCalls = additionalCalls;
        }

        //public void SetUp(MailboxLineRanged rangedLine, BaseObject lineOwner, BuilderController controller)
        public void SetUp(MailboxLineRanged rangedLine, IBaseObject lineOwner, List<DelegateLibrary.VoidFunction> additionalCalls = null)
        {
            this.rangedLine = rangedLine;
            this.lineOwner = lineOwner;
            //this.controller = controller;

            base.SetUp(rangedLine.Name);

            InputField.contentType = InputField.ContentType.DecimalNumber;
            InputField.text = rangedLine.Val.ToString();
            InputField.onEndEdit.AddListener(ChangeRangedLineValue);

            this.additionalCalls = additionalCalls;
        }

        public void SetUp(MailboxLineDistinctUnnamed distinctUnnamedLine, IBaseObject lineOwner, List<DelegateLibrary.VoidFunction> additionalCalls = null)
        {
            this.distinctUnnamedLine = distinctUnnamedLine;
            this.lineOwner = lineOwner;

            base.SetUp(distinctUnnamedLine.Name);

            InputField.contentType = InputField.ContentType.IntegerNumber;
            InputField.text = distinctUnnamedLine.Val.ToString();
            InputField.onEndEdit.AddListener(ChangeDistinctUnnamedLineValue);

            this.additionalCalls = additionalCalls;
        }


        public void ChangeStringLineValue(string newString)
        {
            stringLine.Val = newString;

            if (lineOwner != null) lineOwner.ApplyBuildParameters();

            RunAllAdditionalCalls();
        }

        public void ChangeRangedLineValue(string newString)
        {
            float previousValue = rangedLine.Val;

            rangedLine.Val = float.Parse(newString);

            if(lineOwner != null)
            {
                bool previouslyFalied = lineOwner.Failed;

                lineOwner.ApplyBuildParameters();

                if (!previouslyFalied && lineOwner.Failed)
                {
                    InputField.text = previousValue.ToString();
                    rangedLine.Val = previousValue;
                    lineOwner.ApplyBuildParameters();
                }
            }

            RunAllAdditionalCalls();
        }

        public void ChangeDistinctUnnamedLineValue(string newString)
        {
            distinctUnnamedLine.Val = int.Parse(newString);

            if (lineOwner != null) lineOwner.ApplyBuildParameters();

            RunAllAdditionalCalls();
        }
    }
}