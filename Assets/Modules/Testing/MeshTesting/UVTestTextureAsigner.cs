using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVTestTextureAsigner : MonoBehaviour
{
    [SerializeField] Material TestMaterial;
    [SerializeField] GameObject AsignmentParent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(key: KeyCode.P))
        {
            Transform[] parentArray = AsignmentParent.transform.GetComponentsInChildren<Transform>();
            foreach (Transform child in parentArray)
            {
                MeshRenderer renderer = child.GetComponent<MeshRenderer>();

                if (renderer != null)
                {
                    renderer.material = TestMaterial;
                }
            }

        }
    }
}
