using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour
{
    public GameObject StepTemplate;
    public float blockSize = 1f / 3f;
    public float stairHeight = 2.5f;

    //Build parameters
    public int width = 3;
    public int length = 6;
    public float targetStepHeight = 0.25f;

    List<GameObject> Steps = new List<GameObject>();

    public void UpdateDesign()
    {
        foreach(GameObject step in Steps)
        {
            GameObject.Destroy(step);
        }

        float stepCountFloat = stairHeight / targetStepHeight;

        int stepCount = Mathf.RoundToInt(stepCountFloat);

        for(int i = 0; i < stepCount; i++)
        {
            GameObject step = GameObject.Instantiate(StepTemplate, transform, false);

            Steps.Add(step);

            step.transform.localPosition = new Vector3(0.5f, 0.5f + i, i);

            step.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, 180));
        }

        transform.localScale = new Vector3(blockSize * width, stairHeight / stepCount, blockSize * length / stepCount);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDesign();
    }
}
