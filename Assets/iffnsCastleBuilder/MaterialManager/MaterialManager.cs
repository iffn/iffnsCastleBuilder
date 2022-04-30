using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager
{
    public Material LinkedMaterial { get; private set; }
    public string Identifier { get; private set; }

    public MaterialManager(string identifier, Material linkedMaterial)
    {
        this.LinkedMaterial = linkedMaterial;
        this.Identifier = identifier;
    }
}
