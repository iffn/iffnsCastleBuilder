using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Vector2IntLine : ControlLine
{
    public InputField InputFieldX;
    public InputField InputFieldY;

    MailboxLineVector2Int currentVector2IntLine;

    IBaseObject lineOwner;

    //BuilderController controller;

    //public void SetUp(MailboxLineVector2Int vector2IntLine, BaseObject partToUpdateBuildParameters, BuilderController controller)
    public void SetUp(MailboxLineVector2Int vector2IntLine, IBaseObject partToUpdateBuildParameters, List<DelegateLibrary.VoidFunction> additionalCalls = null)
    {
        this.currentVector2IntLine = vector2IntLine;
        this.lineOwner = partToUpdateBuildParameters;
        //this.controller = controller;

        base.SetUp(vector2IntLine.Name);

        InputFieldX.text = vector2IntLine.Val.x.ToString();
        InputFieldX.onEndEdit.AddListener(ChangeXValue);

        InputFieldY.text = vector2IntLine.Val.y.ToString();
        InputFieldY.onEndEdit.AddListener(ChangeYValue);

        this.additionalCalls = additionalCalls;
    }

    public void ChangeXValue(string value)
    {
        currentVector2IntLine.Val = new Vector2Int(int.Parse(value), currentVector2IntLine.Val.y);

        if(lineOwner != null) lineOwner.ApplyBuildParameters();

        RunAllAdditionalCalls();
    }

    public void ChangeYValue(string value)
    {
        currentVector2IntLine.Val = new Vector2Int(currentVector2IntLine.Val.x, int.Parse(value));

        if (lineOwner != null) lineOwner.ApplyBuildParameters();

        RunAllAdditionalCalls();
    }
}

