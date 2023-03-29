using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WebGLTest : MonoBehaviour
{
    [SerializeField] TextAsset testAsset;
    [SerializeField] TMP_Text output;
 
    // Start is called before the first frame update
    void Start()
    {
        output.text = testAsset.text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
