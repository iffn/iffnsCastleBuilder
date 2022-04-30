using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseLadder : MonoBehaviour
{
    public float Width;
    public float Height;
    public float DistanceBetweenSteps = 0.5f;
    public float SideThickness = 0.1f;
    public float StepThickness = 0.1f;


    //Unity assignments
    [SerializeField] GameObject LeftEdge;
    [SerializeField] GameObject RightEdge;
    [SerializeField] UnityMeshManager StepTemplate;

    [SerializeField] List<UnityMeshManager> Edges;


    //Runtime parameters
    readonly List<UnityMeshManager> steps = new List<UnityMeshManager>();
    BaseGameObject mainObject;
    MailboxLineMaterial stepMaterial;

    public List<UnityMeshManager> UnmanagedStaticMeshes
    {
        get
        {
            List<UnityMeshManager> returnList = new List<UnityMeshManager>();

            returnList.AddRange(steps);
            returnList.AddRange(Edges);

            return returnList;
        }
    }

    public void Setup(BaseGameObject mainObject, MailboxLineMaterial edgeMaterial, MailboxLineMaterial stepMaterial)
    {
        this.mainObject = mainObject;
        this.stepMaterial = stepMaterial;

        foreach (UnityMeshManager edge in Edges)
        {
            edge.Setup(mainObject: mainObject, currentMaterialReference: edgeMaterial);
        }
    }

    public void SetMainParameters(float width, float height)
    {
        this.Width = width;
        this.Height = height;

        ApplyBuildParameters();
    }

    public void ApplyBuildParameters()
    {
        LeftEdge.transform.localScale = new Vector3(SideThickness, Height, SideThickness);
        RightEdge.transform.localScale = LeftEdge.transform.localScale;

        LeftEdge.transform.localPosition = new Vector3((-Width + SideThickness) * 0.5f, Height / 2, 0);
        RightEdge.transform.localPosition = new Vector3((Width - SideThickness) *0.5f, Height / 2, 0);
        
        foreach(UnityMeshManager edge in Edges)
        {
            edge.UpdateMaterial();
        }

        int numberOfSteps = (int)(Height / DistanceBetweenSteps);

        foreach(UnityMeshManager Step in steps)
        {
            GameObject.Destroy(Step.gameObject);
        }

        steps.Clear();

        for(int i = 0; i < numberOfSteps; i++)
        {
            GameObject step = GameObject.Instantiate(original: StepTemplate.gameObject, parent: transform);

            step.transform.parent = transform;
            step.transform.localPosition = Vector3.up * (DistanceBetweenSteps) * (i + 0.5f); 
            step.transform.localScale = new Vector3(StepThickness, (Width - SideThickness) * 0.5f, StepThickness);

            UnityMeshManager stepManager = step.GetComponent<UnityMeshManager>();
            stepManager.Setup(mainObject: mainObject, currentMaterialReference: stepMaterial);
            steps.Add(stepManager);
        }
    }
}
