using iffnsStuff.iffnsBaseSystemForUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class BaseCounter : AssistObjectManager
    {
        BaseGameObject mainObject;

        public float Width = 2f / 3f;
        public float Length = 1f;
        public float Indent = 0.05f;
        public float topHeight = 0.1f;
        public float totalHeight = 0.8f;

        public MailboxLineMaterial baseMaterial;
        public MailboxLineMaterial topMaterial;

        public void Setup(BaseGameObject mainObject)
        {
            this.mainObject = mainObject;
        }

        public ValueContainer SetBuildParameters(BaseGameObject mainObject, Transform UVBaseObject, float width, float length)
        {
            Width = width;
            Length = length;

            return ApplyBuildParameters(UVBaseObject: UVBaseObject);
        }

        public override ValueContainer ApplyBuildParameters(Transform UVBaseObject)
        {
            ValueContainer returnValue = new ValueContainer(originTransform: transform, targetTransform: mainObject.transform);

            TriangleMeshInfo baseMesh;
            TriangleMeshInfo topMesh;

            void FinishMesh()
            {
                baseMesh.MaterialReference = baseMaterial;
                topMesh.MaterialReference = topMaterial;

                returnValue.AddStaticMeshAndConvertToTarget(baseMesh);
                returnValue.AddStaticMeshAndConvertToTarget(topMesh);
            }

            baseMesh = MeshGenerator.FilledShapes.BoxAroundCenter(size: new Vector3(Length - Indent * 2, totalHeight - topHeight, Width - Indent * 2));
            baseMesh.Move(offset: new Vector3(Length / 2, (totalHeight - topHeight) / 2, Width / 2));

            topMesh = MeshGenerator.FilledShapes.BoxAroundCenter(size: new Vector3(Length - MathHelper.SmallFloat, topHeight, Width - MathHelper.SmallFloat));
            topMesh.Move(offset: new Vector3(Length / 2, totalHeight - topHeight / 2, Width / 2));

            FinishMesh();

            return returnValue;
        }
    }
}