using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuildingController
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public abstract void ShowInterior(int floorNumber);

    public abstract void ShowRoof(int floorNumber);
}
