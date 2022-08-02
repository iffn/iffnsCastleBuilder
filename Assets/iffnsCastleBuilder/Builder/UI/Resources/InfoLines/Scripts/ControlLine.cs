using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace iffnsStuff.iffnsCastleBuilder
{
    public abstract class ControlLine : MonoBehaviour
    {
        public TMP_Text TitleText;

        public string Title
        {
            get
            {
                return TitleText.text;
            }

            set
            {
                TitleText.text = value;
            }
        }

        public float Height
        {
            get
            {
                return transform.GetComponent<RectTransform>().sizeDelta.y;
            }
        }

        // Use this for initialization
        virtual protected void Start()
        {

        }

        // Update is called once per frame
        virtual protected void Update()
        {

        }

        virtual protected void SetUp(string title)
        {
            Title = title;
        }

        protected List<DelegateLibrary.VoidFunction> additionalCalls = new List<DelegateLibrary.VoidFunction>();

        protected void RunAllAdditionalCalls()
        {
            if (additionalCalls == null) return;

            foreach (DelegateLibrary.VoidFunction call in additionalCalls)
            {
                call();
            }
        }
    }
}