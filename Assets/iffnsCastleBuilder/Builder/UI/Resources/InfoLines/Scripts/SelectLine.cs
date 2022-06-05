using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class SelectLine : ControlLine
    {
        public Dropdown DropdownMenu;

        MailboxLineDistinctNamed distinctLine;
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

        public void SetUp(string text, UnityAction<int> ReturnFunctionScript, List<string> lines, List<DelegateLibrary.VoidFunction> additionalCalls = null)
        {
            base.SetUp(text);

            DropdownMenu.onValueChanged.AddListener(ReturnFunctionScript);

            ClearAndAddOptions(lines);

            this.additionalCalls = additionalCalls;
        }

        public void SetUp(MailboxLineDistinctNamed stringLine, IBaseObject lineOwner, List<DelegateLibrary.VoidFunction> additionalCalls = null)
        {
            this.distinctLine = stringLine;
            this.lineOwner = lineOwner;
            //this.controller = controller;

            base.SetUp(stringLine.Name);

            DropdownMenu.onValueChanged.AddListener(ChangeDistinctNamedLineValue);

            ClearAndAddOptions(stringLine.Entries);

            DropdownMenu.value = stringLine.Val;

            this.additionalCalls = additionalCalls;
        }


        public void ChangeDistinctNamedLineValue(int newValue)
        {
            int previousValue = distinctLine.Val;

            distinctLine.Val = newValue;

            if (lineOwner != null)
            {
                bool previouslyFalied = lineOwner.Failed;

                lineOwner.ApplyBuildParameters();

                if (!previouslyFalied && lineOwner.Failed)
                {
                    DropdownMenu.value = previousValue;

                    distinctLine.Val = previousValue;
                    lineOwner.ApplyBuildParameters();
                }
            }

            RunAllAdditionalCalls();
        }

        void ClearAndAddOptions(List<string> lines)
        {
            DropdownMenu.ClearOptions();
            /*
            List<Dropdown.OptionData> data = DropdownMenu.options = new List<Dropdown.OptionData>();

            foreach (string line in lines)
            {
                data.Add(new Dropdown.OptionData(text: line));
            }
            */
            //DropdownMenu.AddOptions(data);
            DropdownMenu.AddOptions(lines);
        }
    }
}