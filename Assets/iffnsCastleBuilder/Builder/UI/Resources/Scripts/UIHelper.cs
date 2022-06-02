using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIHelper : MonoBehaviour
{
    public UISizeController SizeController {private set; get; }

    public virtual void Setup(UISizeController linkedSizeController)
    {
        SizeController = linkedSizeController;
    }

    public abstract void UpdateSize();
}
