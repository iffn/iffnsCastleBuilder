using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class InputLineBool : ControlLine
    {
        public Toggle InputToggle;

        MailboxLineBool boolLine;
        IBaseObject lineOwner;

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

        public void SetUp(MailboxLineBool boolLine, IBaseObject lineOwner, List<DelegateLibrary.VoidFunction> additionalCalls = null)
        {
            this.boolLine = boolLine;
            this.lineOwner = lineOwner;
            //this.controller = controller;

            base.SetUp(boolLine.Name);

            InputToggle.isOn = boolLine.Val;
            InputToggle.onValueChanged.AddListener(ChangeBoolLineValue);

            this.additionalCalls = additionalCalls;
        }

        public void ChangeBoolLineValue(bool newValue)
        {
            bool previousValue = boolLine.Val;

            boolLine.Val = newValue;

            if (lineOwner != null)
            {
                bool previouslyFalied = lineOwner.Failed;

                lineOwner.ApplyBuildParameters();

                if (!previouslyFalied && lineOwner.Failed)
                {
                    InputToggle.isOn = previousValue;
                    
                    boolLine.Val = previousValue;
                    lineOwner.ApplyBuildParameters();
                }
            }

            RunAllAdditionalCalls();
        }
    }
}