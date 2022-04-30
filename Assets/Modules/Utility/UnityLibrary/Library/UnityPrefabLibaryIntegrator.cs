using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityPrefabLibaryIntegrator : MonoBehaviour
{
    public SmartMeshManager MeshManagerTemplate;

    public void Setup()
    {
        UnityPrefabLibrary.Setup(libraryIntegrator: this);
    }
}

public static class UnityPrefabLibrary
{
    static UnityPrefabLibaryIntegrator LibraryIntegrator;

    public static void Setup(UnityPrefabLibaryIntegrator libraryIntegrator)
    {
        LibraryIntegrator = libraryIntegrator;
    }

    public static SmartMeshManager NewMeshManager
    {
        get
        {
            return GameObject.Instantiate(LibraryIntegrator.MeshManagerTemplate).GetComponent<SmartMeshManager>();
        }
    }
}
