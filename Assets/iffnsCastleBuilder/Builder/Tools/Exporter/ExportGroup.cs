using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using iffnsStuff.iffnsBaseSystemForUnity;
using iffnsStuff.iffnsBaseSystemForUnity.Tools;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class ExportGroupOrganizer
    {
        

        //public readonly MeshManager MeshTemplate;

        public ExportProperties currentExportProperties;

        public List<ExportGroup> ObjectGroups;

        public readonly Transform exportReadyObject;
        readonly Transform worldTransform;

        public ExportGroupOrganizer(Transform exportReadyObject, ExportProperties currentExportProperties, Transform worldTransform)
        {
            this.currentExportProperties = currentExportProperties;

            //this.MeshTemplate = meshTemplate;
            ObjectGroups = new List<ExportGroup>();

            this.exportReadyObject = exportReadyObject;

            this.worldTransform = worldTransform;
        }

        public void AddOrIncludeObjectGroup(string name, Vector3 localPosition, string materialIdentifier, bool hasCollider, List<int> hierarchyPosition, TriangleMeshInfo newInfo, Transform originalTransform)
        {
            foreach (ExportGroup group in ObjectGroups)
            {
                if (currentExportProperties.HierarchyIdentifier)
                {
                    if (hierarchyPosition.Count != group.hierarchyPosition.Count) continue;

                    for (int i = 0; i < hierarchyPosition.Count; i++)
                    {
                        if (hierarchyPosition[i] != group.hierarchyPosition[i]) continue;
                    }
                }

                if (currentExportProperties.SeparateFloors)
                {
                    if (hierarchyPosition.Count == 0 || group.hierarchyPosition.Count == 0) continue;

                    if (hierarchyPosition[0] != group.hierarchyPosition[0]) continue;
                }

                if (currentExportProperties.MaterialIdentifier || currentExportProperties.IncludeInvisibleMeshes)
                {
                    if (!materialIdentifier.Equals(group.materialIdentifier)) continue;
                }

                if (currentExportProperties.ColliderIdentifier)
                {
                    if (hasCollider != group.hasCollider) continue;
                }

                /*
                if (currentExportProperties.SeparateNonStrucuturalObjects)
                {
                    if (!group.specialIdentifier.Equals(specialIdentifier)) continue;
                }
                */

                group.AddMesh(newInfo: newInfo, originalTransform: originalTransform);

                return;
            }

            ExportGroup newObject = new(organizer: this, name: name, localPosition: localPosition, materialIdentifier: materialIdentifier, hasCollider: hasCollider, hierarchyPosition: hierarchyPosition, worldTransform: worldTransform);

            newObject.AddMesh(newInfo: newInfo, originalTransform: originalTransform);
        }
    }


    public class ExportGroup
    {
        //Properties
        //public MeshManager meshInfo;
        public string name;
        public Vector3 localPosition;
        public TriangleMeshInfo currentInfo;
        public string materialIdentifier;
        public bool hasCollider;
        public List<int> hierarchyPosition = new();
        readonly Transform worldTransform;

        readonly ExportGroupOrganizer organizer;

        public ExportGroup(ExportGroupOrganizer organizer, string name, Vector3 localPosition, string materialIdentifier, bool hasCollider, List<int> hierarchyPosition, Transform worldTransform)
        {
            //Organization
            organizer.ObjectGroups.Add(this);
            this.organizer = organizer;

            //if(worldTransform == null) worldTransform = new GameObject().transform;

            //Mesh info
            /*
            meshInfo = GameObject.Instantiate(organizer.MeshTemplate).GetComponent<MeshManager>();
            meshInfo.transform.parent = organizer.exportReadyObject;
            meshInfo.transform.localPosition = Vector3.zero;
            meshInfo.transform.localRotation = Quaternion.identity;
            meshInfo.transform.localScale = Vector3.one;
            */
            currentInfo = new TriangleMeshInfo();

            //Set properties
            this.localPosition = localPosition;
            this.name = name;
            this.materialIdentifier = materialIdentifier;
            this.hasCollider = hasCollider;
            this.hierarchyPosition = new List<int>(hierarchyPosition);
            this.worldTransform = worldTransform;
            //meshInfo.transform.name = IdentifierString;
        }

        readonly string NameIdentifier = "Name";
        readonly string LocalPositionIdentifier = "Local position";
        readonly string HierarchyPositionIdentifier = "Hierarchy position";

        public string IdentifierString
        {
            get
            {
                string IdentifierSeparation = " - ";

                string returnString = "";

                if (!name.Equals(""))
                {
                    returnString += $"Name = {name}";
                }

                AddSeparator();

                returnString += $"Local position = {localPosition}";

                if (organizer.currentExportProperties.HierarchyIdentifier)
                {
                    AddSeparator();
                    returnString += "Hierarchy position = ";

                    foreach (int pos in hierarchyPosition)
                    {
                        returnString += $"{pos}-";
                    }

                    returnString = returnString.Remove(returnString.Length - 1);

                }

                if (organizer.currentExportProperties.MaterialIdentifier
                    || organizer.currentExportProperties.IncludeInvisibleMeshes && materialIdentifier.Equals("Invisible"))
                {
                    AddSeparator();
                    returnString += $"Material = {materialIdentifier}";
                }

                if (organizer.currentExportProperties.ColliderIdentifier)
                {
                    AddSeparator();
                    returnString += $"Collider = {hasCollider}";
                }

                //returnString.Replace(":", "=");

                return returnString;

                void AddSeparator()
                {
                    if (returnString.Length != 0)
                    {
                        returnString += IdentifierSeparation;
                    }
                }
            }
        }

        public void AddMesh(TriangleMeshInfo newInfo, Transform originalTransform)
        {
            currentInfo.Add(newInfo: newInfo, originalTransform: originalTransform, thisTransform: worldTransform);
        }

        public int VertexCount
        {
            get
            {
                return currentInfo.VerticesHolder.Count;
            }
        }

        public List<string> GetObjText(string currentId, int triangleIndexOffset)
        {
            List<string> returnList = new();

            if (currentInfo.VerticesHolder.Count == 0) return returnList;

            string meshName = $"{currentId} - {IdentifierString}";

            returnList = ObjExporter.GetObjLines(meshName: meshName, vertices: currentInfo.VerticesHolder.Vertices, uvs: currentInfo.UVs, triangles: currentInfo.AllTrianglesDirectly, triangleIndexOffset: triangleIndexOffset, upDirection: ObjExporter.UpDirection.Y);

            return returnList;
        }

        public List<string> GetObjText(int triangleIndexOffset)
        {
            List<string> returnList = new();

            if (currentInfo.VerticesHolder.Count == 0) return returnList;

            returnList = ObjExporter.GetObjLines(meshName: IdentifierString, vertices: currentInfo.VerticesHolder.Vertices, uvs: currentInfo.UVs, triangles: currentInfo.AllTrianglesDirectly, triangleIndexOffset: triangleIndexOffset, upDirection: ObjExporter.UpDirection.Y);

            return returnList;
        }
    }

}