using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonLine : ControlLine
{
    public Button button;

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

    public void SetUp(string text, UnityEngine.Events.UnityAction call)
    {
        base.SetUp(text);

        button.onClick.AddListener(call);
    }
}
