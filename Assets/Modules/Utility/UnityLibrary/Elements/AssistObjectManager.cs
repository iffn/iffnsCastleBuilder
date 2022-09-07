using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using iffnsStuff.iffnsBaseSystemForUnity;
using iffnsStuff.iffnsCastleBuilder;

public abstract class AssistObjectManager : MonoBehaviour
{
    public abstract List<TriangleMeshInfo> ApplyBuildParameters(Transform UVBaseObject);

    /*
    public class ValueContainer
    {
        Transform originTransform;
        Transform targetTransform;

        public ValueContainer(Transform originTransform, Transform targetTransform)
        {
            this.originTransform = originTransform;
            this.targetTransform = targetTransform;

            staticMeshes = new List<TriangleMeshInfo>();
        }

        public void AddStaticMeshAndConvertToTarget(TriangleMeshInfo info)
        {
            TriangleMeshInfo additionValue = info.Clone;

            additionValue.Transorm(origin: originTransform, target: targetTransform);

            staticMeshes.Add(additionValue);
        }

        List<TriangleMeshInfo> staticMeshes;
        public List<TriangleMeshInfo> ConvertedStaticMeshes
        {
            get
            {
                return staticMeshes;
            }
        }
    }
    */
}
