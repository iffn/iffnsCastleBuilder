using iffnsStuff.iffnsCastleBuilder;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanSelector : MonoBehaviour
{
    [SerializeField] List<GameObject> Plans;
    [SerializeField] CastleBuilderController linkedController;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int currentFloor = linkedController.CurrentBuilding.CurrentFloorNumber;

        currentFloor = MathHelper.ClampInt(value: currentFloor, max: Plans.Count - 1, min: 0);

        float currentHeight = 0.3f;
        for(int i = 0; i< Plans.Count; i++)
        {
            GameObject plan = Plans[i];

            plan.SetActive(false);
            plan.transform.localPosition = new Vector3(plan.transform.localPosition.x, currentHeight, plan.transform.localPosition.z);
            currentHeight += linkedController.CurrentBuilding.CurrentFloorObject.CompleteFloorHeight;
        }
        
        Plans[currentFloor].SetActive(true);

        /*
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            foreach (GameObject plan in Plans) plan.SetActive(false);
        }
        
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            foreach (GameObject plan in Plans) plan.SetActive(false);
            Plans[0].SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            foreach (GameObject plan in Plans) plan.SetActive(false);
            Plans[1].SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            foreach (GameObject plan in Plans) plan.SetActive(false);
            Plans[2].SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            foreach (GameObject plan in Plans) plan.SetActive(false);
            Plans[3].SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            foreach (GameObject plan in Plans) plan.SetActive(false);
            Plans[4].SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            foreach (GameObject plan in Plans) plan.SetActive(false);
            Plans[5].SetActive(true);
        }
        */
    }
}
