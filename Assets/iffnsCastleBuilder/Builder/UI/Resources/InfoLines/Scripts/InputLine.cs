using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class InputLine : ControlLine
    {
        public TMP_InputField InputField;
        public InputField InputFieldLegacy;

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

            if(ReturnFunctionScript != null)
            {
                if(InputField) InputField.onEndEdit.AddListener(ReturnFunctionScript);
                if(InputFieldLegacy) InputFieldLegacy.onEndEdit.AddListener(ReturnFunctionScript);
            }

            this.additionalCalls = additionalCalls;
        }

        //public void SetUp(MailboxLineString stringLine, BaseObject lineOwner, BuilderController controller)
        public void SetUp(MailboxLineString stringLine, IBaseObject lineOwner, List<DelegateLibrary.VoidFunction> additionalCalls = null)
        {
            this.stringLine = stringLine;
            this.lineOwner = lineOwner;
            //this.controller = controller;

            base.SetUp(stringLine.Name);

            if (InputField)
            {
                InputField.text = stringLine.Val;
                InputField.onEndEdit.AddListener(ChangeStringLineValue);
            }

            if (InputFieldLegacy)
            {
                InputFieldLegacy.text = stringLine.Val;
                InputFieldLegacy.onEndEdit.AddListener(ChangeStringLineValue);
            }


            this.additionalCalls = additionalCalls;
        }

        //public void SetUp(MailboxLineRanged rangedLine, BaseObject lineOwner, BuilderController controller)
        public void SetUp(MailboxLineRanged rangedLine, IBaseObject lineOwner, List<DelegateLibrary.VoidFunction> additionalCalls = null)
        {
            this.rangedLine = rangedLine;
            this.lineOwner = lineOwner;
            //this.controller = controller;

            base.SetUp(rangedLine.Name);

            if (InputField)
            {
                InputField.contentType = TMP_InputField.ContentType.DecimalNumber;
                InputField.text = StringHelper.ConvertFloatToString(value: rangedLine.Val, globalFormat: false); ;
                InputField.onEndEdit.AddListener(ChangeRangedLineValue);
            }

            if (InputFieldLegacy)
            {
                InputFieldLegacy.contentType = UnityEngine.UI.InputField.ContentType.DecimalNumber;
                InputFieldLegacy.text = StringHelper.ConvertFloatToString(value: rangedLine.Val, globalFormat: false); ;
                InputFieldLegacy.onEndEdit.AddListener(ChangeRangedLineValue);
            }

            this.additionalCalls = additionalCalls;
        }

        public void SetUp(MailboxLineDistinctUnnamed distinctUnnamedLine, IBaseObject lineOwner, List<DelegateLibrary.VoidFunction> additionalCalls = null)
        {
            this.distinctUnnamedLine = distinctUnnamedLine;
            this.lineOwner = lineOwner;

            base.SetUp(distinctUnnamedLine.Name);

            if (InputField)
            {
                InputField.contentType = TMP_InputField.ContentType.IntegerNumber;
                InputField.text = StringHelper.ConvertIntToString(value: distinctUnnamedLine.Val, globalFormat: false);
                InputField.onEndEdit.AddListener(ChangeDistinctUnnamedLineValue);
            }

            if (InputFieldLegacy)
            {
                InputFieldLegacy.contentType = UnityEngine.UI.InputField.ContentType.IntegerNumber;
                InputFieldLegacy.text = StringHelper.ConvertIntToString(value: distinctUnnamedLine.Val, globalFormat: false);
                InputFieldLegacy.onEndEdit.AddListener(ChangeDistinctUnnamedLineValue);
            }

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

            float newValue = StringHelper.ConvertStringToFloat(text: newString, globalFormat: false, worked: out bool worked);

            if (worked) rangedLine.Val = newValue;
            else return;

            if(lineOwner != null)
            {
                bool previouslyFalied = lineOwner.Failed;

                lineOwner.ApplyBuildParameters();

                if (!previouslyFalied && lineOwner.Failed)
                {
                    if(InputField) InputField.text = StringHelper.ConvertFloatToString(value: previousValue, globalFormat: false);
                    if(InputFieldLegacy) InputFieldLegacy.text = StringHelper.ConvertFloatToString(value: previousValue, globalFormat: false);

                    rangedLine.Val = previousValue;
                    lineOwner.ApplyBuildParameters();
                }
            }

            RunAllAdditionalCalls();
        }

        public void ChangeDistinctUnnamedLineValue(string newString)
        {
            int previousValue = distinctUnnamedLine.Val;

            int newValue = StringHelper.ConvertStringToInt(text: newString, globalFormat: false, worked: out bool worked);

            if (worked) distinctUnnamedLine.Val = newValue;
            else return;

            distinctUnnamedLine.Val = newValue;

            if (lineOwner != null)
            {
                bool previouslyFalied = lineOwner.Failed;

                lineOwner.ApplyBuildParameters();

                if (!previouslyFalied && lineOwner.Failed)
                {
                    if (InputField) InputField.text = StringHelper.ConvertFloatToString(value: previousValue, globalFormat: false);
                    if (InputFieldLegacy) InputFieldLegacy.text = StringHelper.ConvertFloatToString(value: previousValue, globalFormat: false);
                    distinctUnnamedLine.Val = previousValue;
                    lineOwner.ApplyBuildParameters();
                }
            }

            RunAllAdditionalCalls();
        }
    }
}