using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class InfoLine : ControlLine
    {
        public TMP_Text ValueText;

        public string Value
        {
            get
            {
                return ValueText.text;
            }

            set
            {
                ValueText.text = value;
            }
        }

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

        new public void SetUp(string text)
        {
            base.SetUp(text);

        }
    }
}