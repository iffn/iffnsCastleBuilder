using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallLineSelectorButton : VectorButton
{
    public NavigationTools currentNavigationTools;

    bool currentState = false;

    public void ToggleButtonState()
    {
        currentState = !currentState;

        currentNavigationTools.SetBlockLineVisibility(currentState);

        Highlight = currentState;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
