using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ControlLine : MonoBehaviour {

    public Text TitleText;

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


    // Use this for initialization
    virtual protected void Start () {
		
	}

    // Update is called once per frame
    virtual protected void Update () {
		
	}

    virtual protected void SetUp(string title)
    {
        this.Title = title;
    }

    protected List<DelegateLibrary.VoidFunction> additionalCalls = new List<DelegateLibrary.VoidFunction>();

    protected void RunAllAdditionalCalls()
    {
        if (additionalCalls == null) return;
        
        foreach(DelegateLibrary.VoidFunction call in additionalCalls)
        {
            call();
        }
    }

    /*
    public void AddAdditionalCalls(List<DelegateLibrary.VoidFunction> additionalCalls)
    {

    }
    */
}
