using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using iffnsStuff.iffnsBaseSystemForUnity;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class DefaultCastleMaterialIntegrator : MaterialLibraryExtenderTemplate
    {
        public Material InvisibleMaterial;
        public Material DefaultPlaster;
        public Material DefaultStoneBricks;
        public Material DefaultWoodPlanks;
        public Material DefaultWoodSolid;
        public Material DefaultCeiling;
        public Material DefaultRoof;
        public Material DefaultGlass;

        Dictionary<string, MaterialManager> materialManagerLibary = new Dictionary<string, MaterialManager>();

        public override Dictionary<string, MaterialManager> MaterialManagerLibary
        {
            get
            {
                return materialManagerLibary;
            }
        }

        public override void Setup()
        {
            materialManagerLibary = new Dictionary<string, MaterialManager>();

            setup(AllMaterials);

            DefaultCastleMaterials.Setup(libraryIntegrator: this);
        }

        public override List<Material> AllMaterials
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

                return returnList;
            }
        }
    }

    public static class DefaultCastleMaterials
    {
        static DefaultCastleMaterialIntegrator linkedLibraryIntegrator;

        static Dictionary<string, MaterialManager> MaterialManagerLibary;
        //public static List<MaterialManager> AllMaterialManagers { get; private set; }

        public static MaterialManager InvisibleMaterial { get; private set; }
        public static MaterialManager DefaultPlaster { get; private set; }
        public static MaterialManager DefaultStoneBricks { get; private set; }
        public static MaterialManager DefaultWoodPlanks { get; private set; }
        public static MaterialManager DefaultWoodSolid { get; private set; }
        public static MaterialManager DefaultCeiling { get; private set; }
        public static MaterialManager DefaultRoof { get; private set; }
        public static MaterialManager DefaultGlass { get; private set; }

        public static void Setup(DefaultCastleMaterialIntegrator libraryIntegrator)
        {
            linkedLibraryIntegrator = libraryIntegrator;

            MaterialManagerLibary = libraryIntegrator.MaterialManagerLibary;

            InvisibleMaterial = MaterialManagerLibary[libraryIntegrator.InvisibleMaterial.name];
            DefaultPlaster = MaterialManagerLibary[libraryIntegrator.DefaultPlaster.name];
            DefaultStoneBricks = MaterialManagerLibary[libraryIntegrator.DefaultStoneBricks.name];
            DefaultWoodPlanks = MaterialManagerLibary[libraryIntegrator.DefaultWoodPlanks.name];
            DefaultWoodSolid = MaterialManagerLibary[libraryIntegrator.DefaultWoodSolid.name];
            DefaultCeiling = MaterialManagerLibary[libraryIntegrator.DefaultCeiling.name];
            DefaultRoof = MaterialManagerLibary[libraryIntegrator.DefaultRoof.name];
            DefaultGlass = MaterialManagerLibary[libraryIntegrator.DefaultGlass.name];
        }

        public static MaterialManager GetMaterialFromIdentifier(string identifier)
        {
            return linkedLibraryIntegrator.GetMaterialFromIdentifier(identifier);
        }
    }
}