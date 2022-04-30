using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MeshManager : MonoBehaviour
{
    public BaseGameObject LinkedMainObject { get; protected set; }

    
    protected void setup(BaseGameObject mainObject)
    {
        LinkedMainObject = mainObject;
    }
    

    public abstract Material CurrentMaterial { get; }

    public abstract MailboxLineMaterial CurrentMaterialReference { get; }
}
