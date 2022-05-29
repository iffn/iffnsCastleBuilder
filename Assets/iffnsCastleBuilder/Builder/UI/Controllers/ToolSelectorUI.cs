using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class ToolSelectorUI : MonoBehaviour
    {
        [SerializeField] List<VectorButton> Buttons;
        [SerializeField] List<GameObject> Menus;
        [SerializeField] List<GameObject> Tools;

        public void SetMenuType(VectorButton ClickedButton)
        {
            if (!setupCorrectlyInEditor)
            {
                return;
            }

            foreach (VectorButton button in Buttons)
            {
                button.Highlight = false;
            }

            ClickedButton.Highlight = true;

            foreach (GameObject menu in Menus)
            {
                menu.SetActive(false);
            }

            foreach (GameObject tool in Tools)
            {
                tool.SetActive(false);
            }

            bool found = false;

            for (int i = 0; i < Buttons.Count; i++)
            {
                if (Buttons[i] == ClickedButton)
                {
                    Menus[i].SetActive(true);
                    Tools[i].SetActive(true);
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                Debug.LogWarning("Error: Button" + ClickedButton.name + " not found in menu " + transform.name);
            }
        }

        bool setupCorrectlyInEditor;
        public bool SetupCorrectly
        {
            get
            {
                return Buttons.Count == Menus.Count && Buttons.Count == Tools.Count;
            }
        }



        // Start is called before the first frame update
        void Start()
        {
            setupCorrectlyInEditor = SetupCorrectly;

            if (!setupCorrectlyInEditor)
            {
                Debug.LogWarning("Error with Menu " + transform.name + " : Buttons, Menus and Tools should have the same number of fields");
            }
            else if (Buttons.Count != 0)
            {
                SetMenuType(Buttons[0]);
            }

            foreach(VectorButton button in Buttons)
            {
                button.AddDeletate(new UnityEngine.Events.UnityAction(delegate { SetMenuType(ClickedButton: button); }));
            }
        }

        public void Setup()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}