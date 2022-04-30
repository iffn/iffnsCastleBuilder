using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FloorSelectorDisplay : MonoBehaviour
{
    Slider linkedSlider;

    public float max;
    public float min;
    public float val;


    // Start is called before the first frame update
    void Start()
    {
        linkedSlider = transform.GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        max = linkedSlider.maxValue;
        min = linkedSlider.minValue;
        val = linkedSlider.value;
    }
}
