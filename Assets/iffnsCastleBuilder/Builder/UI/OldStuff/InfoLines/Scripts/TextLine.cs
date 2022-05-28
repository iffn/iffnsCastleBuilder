using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class TextLine : ControlLine
    {
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

        public void SetUp(string text, bool bold)
        {
            base.SetUp(text);

            if (bold) TitleText.fontStyle = FontStyle.Bold;
            else TitleText.fontStyle = FontStyle.Normal;

        }
    }
}