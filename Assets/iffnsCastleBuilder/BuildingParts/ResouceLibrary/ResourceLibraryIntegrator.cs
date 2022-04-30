using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceLibraryIntegrator : MonoBehaviour
{
    public List<BaseGameObject> ObjectTemplates;

    public void Setup()
    {
        ResourceLibrary.Setup(libraryIntegrator: this);
    }
}

public static class ResourceLibrary
{
    static ResourceLibraryIntegrator LibraryIntegrator;

    static Dictionary<string, BaseGameObject> ObjectTempalteLibrary;

    public static void Setup(ResourceLibraryIntegrator libraryIntegrator)
    {
        LibraryIntegrator = libraryIntegrator;

        ObjectTempalteLibrary = new Dictionary<string, BaseGameObject>();

        foreach (BaseGameObject instance in libraryIntegrator.ObjectTemplates)
        {
            ObjectTempalteLibrary.Add(instance.IdentifierString, instance);
        }
    }

    public static IBaseObject GetObjectFromStringIdentifier(string identifier, IBaseObject superObject)
    {
        string searchString = identifier.Replace(MyStringComponents.quote.ToString(), "");

        //Check virtual objects:
        BaseVirtualObject returnVirtualObject = TryGetVirtualObjectFromStringIdentifier(searchString, superObject);

        if (returnVirtualObject != null)
        {
            //Setup already done

            return returnVirtualObject;
        }

        //Check game object:
        BaseGameObject returnGameplayObject = TryGetBaseGameObjectFromStringIdentifier(searchString, superObject);

        if (returnGameplayObject != null)
        {
            //Setup already done

            return returnGameplayObject;
        }

        Debug.LogWarning("Error with resource library: Object not found with identifier: " + identifier);

        return null;
    }

    public static BaseGameObject TryGetTemplateFromStringIdentifier(string identifier)
    {
        if(ObjectTempalteLibrary == null)
        {
            Debug.LogWarning("Error: Library is not set up for some reason");
            return null;
        }

        if (ObjectTempalteLibrary.ContainsKey(identifier) == false) 
        {
            Debug.LogWarning("Error: Library does not contain identifier: " + identifier);
            return null;
        } 

        return ObjectTempalteLibrary[identifier];
    }


    //Base Game object
    public static BaseGameObject TryGetBaseGameObjectFromStringIdentifier(string identifier, IBaseObject superObject)
    {
        BaseGameObject template = null;

        if(!ObjectTempalteLibrary.TryGetValue(key: identifier, value: out template))
        {
            Debug.LogWarning("Error: Library identifier not found, identifier = " + identifier);
            return null;
        }

        BaseGameObject returnValue = GameObject.Instantiate(template) as BaseGameObject;

        returnValue.Setup(superObject);

        return returnValue;
    }

    //Base virtual object
    static BaseVirtualObject TryGetVirtualObjectFromStringIdentifier(string identifier, IBaseObject superObject)
    {
        BaseVirtualObject returnValue = null;

        switch (identifier)
        {
            //ToDo: Find best way to return objects
            case VirtualBlock.staticIdentifierString:
                returnValue = new VirtualBlock(superObject: superObject);
                break;
            case NodeWall.constIdentifierString:
                returnValue = new NodeWall(superObject: superObject);
                break;
            default:
                break;
        }

        return returnValue;

    }
}
