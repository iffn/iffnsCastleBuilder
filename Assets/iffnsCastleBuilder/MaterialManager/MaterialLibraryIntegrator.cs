using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialLibraryIntegrator : MonoBehaviour
{
    public Material InvisibleMaterial;
    public Material DefaultPlaster;
    public Material DefaultStoneBricks;
    public Material DefaultWoodPlanks;
    public Material DefaultWoodSolid;
    public Material DefaultCeiling;
    public Material DefaultRoof;
    public Material DefaultGlass;

    public List<Material> OtherMaterials;

    public void Setup()
    {
        MaterialLibrary.Setup(libraryIntegrator: this);
    }

    public List<Material> AllMaterials
    {
        get
        {
            List<Material> returnList = new List<Material>();

            returnList.Add(InvisibleMaterial);
            returnList.Add(DefaultPlaster);
            returnList.Add(DefaultStoneBricks);
            returnList.Add(DefaultWoodPlanks);
            returnList.Add(DefaultWoodSolid);
            returnList.Add(DefaultCeiling);
            returnList.Add(DefaultRoof);
            returnList.Add(DefaultGlass);

            returnList.AddRange(OtherMaterials);

            return returnList;
        }
    }
}

public static class MaterialLibrary
{
    static MaterialLibraryIntegrator LibaryIntegrator;

    static Dictionary<string, MaterialManager> MaterialManagerLibary;
    public static List<MaterialManager> AllMaterialManagers { get; private set; }

    public static MaterialManager InvisibleMaterial { get; private set; }
    public static MaterialManager DefaultPlaster { get; private set; }
    public static MaterialManager DefaultStoneBricks { get; private set; }
    public static MaterialManager DefaultWoodPlanks { get; private set; }
    public static MaterialManager DefaultWoodSolid { get; private set; }
    public static MaterialManager DefaultCeiling { get; private set; }
    public static MaterialManager DefaultRoof { get; private set; }
    public static MaterialManager DefaultGlass { get; private set; }

    public static void Setup(MaterialLibraryIntegrator libraryIntegrator)
    {
        LibaryIntegrator = libraryIntegrator;

        MaterialManagerLibary = new Dictionary<string, MaterialManager>();
        AllMaterialManagers = new List<MaterialManager>();

        List<Material> allMaterials = libraryIntegrator.AllMaterials;

        foreach (Material material in allMaterials)
        {
            AddMaterialToLibrary(material: material);
        }

        InvisibleMaterial = MaterialManagerLibary[libraryIntegrator.InvisibleMaterial.name];
        DefaultPlaster = MaterialManagerLibary[libraryIntegrator.DefaultPlaster.name];
        DefaultStoneBricks = MaterialManagerLibary[libraryIntegrator.DefaultStoneBricks.name];
        DefaultWoodPlanks = MaterialManagerLibary[libraryIntegrator.DefaultWoodPlanks.name];
        DefaultWoodSolid = MaterialManagerLibary[libraryIntegrator.DefaultWoodSolid.name];
        DefaultCeiling = MaterialManagerLibary[libraryIntegrator.DefaultCeiling.name];
        DefaultRoof = MaterialManagerLibary[libraryIntegrator.DefaultRoof.name];
        DefaultGlass = MaterialManagerLibary[libraryIntegrator.DefaultGlass.name];

        MaterialManager AddMaterialToLibrary(Material material)
        {
            string name = material.name;

            if (!MaterialManagerLibary.ContainsKey(name))
            {
                MaterialManager manager = new MaterialManager(identifier: name, linkedMaterial: material);

                MaterialManagerLibary.Add(key: name, value: manager);
                AllMaterialManagers.Add(manager);

                return manager;
            }
            else
            {
                return MaterialManagerLibary[name];
            }
        }
    }

    public static MaterialManager GetMaterialFromIdentifier(string identifier)
    {
        string searchString = identifier.Replace(MyStringComponents.quote.ToString(), "");

        if (LibaryIntegrator == null)
        {
            Debug.Log("Error: Library is not set up for some reason");
            return null;
        }

        if (MaterialManagerLibary.ContainsKey(searchString) == false)
        {
            Debug.LogWarning("Error: Library does not contain identifier: " + identifier);
            return null;
        }

        return MaterialManagerLibary[searchString];
    }

}
