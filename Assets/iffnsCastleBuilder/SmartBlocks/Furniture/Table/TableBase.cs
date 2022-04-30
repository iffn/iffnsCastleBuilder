using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableBase : MonoBehaviour
{
    //Unity references
    [SerializeField] GameObject TableTop;
    [SerializeField] GameObject Leg1;
    [SerializeField] GameObject Leg2;
    [SerializeField] GameObject Leg3;
    [SerializeField] GameObject Leg4;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyBuildParameters(Vector2 size)
    {
        //Top
        TableTop.transform.localScale = new Vector3(size.x, TableTop.transform.localScale.y, size.y);
        TableTop.transform.localPosition = new Vector3(TableTop.transform.localScale.x * 0.5f, TableTop.transform.localPosition.y, TableTop.transform.localScale.z * 0.5f);

        //Legs
        Leg1.transform.localPosition = Leg1.transform.localScale * 0.5f;

        Leg2.transform.localPosition = new Vector3(
            size.x - Leg2.transform.localScale.x * 0.5f,
            Leg2.transform.localScale.y * 0.5f,
            Leg2.transform.localScale.z * 0.5f);
        
        Leg3.transform.localPosition = new Vector3(
            size.x - Leg3.transform.localScale.x * 0.5f,
            Leg3.transform.localScale.y * 0.5f,
            size.y - Leg3.transform.localScale.z * 0.5f);
        
        Leg4.transform.localPosition = new Vector3(
            Leg4.transform.localScale.x * 0.5f,
            Leg4.transform.localScale.y * 0.5f,
            size.y - Leg4.transform.localScale.z * 0.5f);
    }
}
