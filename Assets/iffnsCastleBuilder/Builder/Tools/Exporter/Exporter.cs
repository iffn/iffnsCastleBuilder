using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using iffnsStuff.iffnsBaseSystemForUnity;
using iffnsStuff.iffnsBaseSystemForUnity.Tools;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class Exporter : MonoBehaviour
    {
        [SerializeField] HumanBuilderController currentBuilderController = null;
        [SerializeField] ExporterUI linkedExporterUI = null;
        [SerializeField] List<GameObject> FailedObjects = new List<GameObject>();

        string exportFolderPath;

        public ExportProperties CurrentExportProperties;

        // Start is called before the first frame update
        void Start()
        {
            Setup();
        }

        void Setup()
        {
            exportFolderPath = Path.Combine(Application.streamingAssetsPath, "Exports");

            //currentExporter = new FbxObjectExporter(exportFolderPath: exportFolderPath);

            CurrentExportProperties = new ExportProperties();

            CurrentExportProperties.Set3DPrintingObjProperties();

            linkedExporterUI.Setup(linkedExporter: this);
        }

        string SaveFileNameWithoutEnding
        {
            get
            {
                return currentBuilderController.CurrentBuildingToolController.CurrentSaveAndLoadSystem.CurrentFileNameWithoutEnding;
            }
        }

        public void SetFileNamePropertyFromSaveSystem()
        {
            string saveFileName = currentBuilderController.CurrentBuildingToolController.CurrentSaveAndLoadSystem.CurrentFileNameWithoutEnding;

            CurrentExportProperties.FileNameWithoutEnding = saveFileName;
        }

        ObjectGroupOrganizer currentObjectGroupOrganizer;
        //GameObject exportReadyObject;
        HumanBuildingController exportBaseObject;

        public void ExportObject()
        {
            //Prepare
            currentBuilderController.CurrentBuilding.BackupVisibilityAndShowAll();

            exportBaseObject = currentBuilderController.CurrentBuilding;

            if (exportBaseObject == null)
            {
                Restore();
                return;
            }

            string fileNamewihtoutEnding = CurrentExportProperties.FileNameWithoutEnding;

            if (string.IsNullOrWhiteSpace(fileNamewihtoutEnding))
            {
                Restore();
                return;
            }

            //Prepare export info
            PrepareExportInfo();

            //Export object
            ExportObjectFromGatheredInfo();

            //Restore
            Restore();

            void Restore()
            {
                currentBuilderController.CurrentBuilding.RestoreVisibility();
                currentObjectGroupOrganizer = null;
                //if (exportReadyObject != null) GameObject.Destroy(exportReadyObject);
            }
        }

        public void PrepareExportInfo()
        {
            currentObjectGroupOrganizer = new ObjectGroupOrganizer(exportReadyObject: exportBaseObject.transform, currentExportProperties: CurrentExportProperties, worldTransform: transform);
            //exportReadyObject = new GameObject();

            List<int> hierarchyPosition = new List<int>();

            switch (CurrentExportProperties.FloorSeparation)
            {
                case ExportProperties.FloorSeparationStates.None:
                    hierarchyPosition.Add(0);
                    GetAndAddMeshFromObject(newObject: exportBaseObject, hierarchyPosition: hierarchyPosition);
                    break;
                case ExportProperties.FloorSeparationStates.FloorAndWallsTogether:
                    hierarchyPosition.Add(0);
                    foreach (FloorController floor in exportBaseObject.CurrentListOfFloors)
                    {
                        GetAndAddMeshFromObject(newObject: floor, hierarchyPosition: hierarchyPosition);
                    }
                    hierarchyPosition.Add(0);
                    break;
                case ExportProperties.FloorSeparationStates.FloorAndWallsSeparate:
                    //Not yet implemented
                    //ToDo: Implement Floor and Wall separate
                    break;
                default:
                    break;
            }

            void GetAndAddMeshFromObject(IBaseObject newObject, List<int> hierarchyPosition)
            {
                hierarchyPosition[hierarchyPosition.Count - 1] += 1;

                if (newObject is BaseGameObject)
                {
                    AddMeshFromObject(newObject as BaseGameObject, hierarchyPosition);
                }

                hierarchyPosition.Add(0);

                List<IBaseObject> SubObjects = newObject.SubObjects;

                foreach (IBaseObject subObject in SubObjects)
                {
                    if (subObject is VirtualBlock) continue;
                    GetAndAddMeshFromObject(subObject, hierarchyPosition);
                }

                hierarchyPosition.RemoveAt(hierarchyPosition.Count - 1);
            }

            void AddMeshFromObject(BaseGameObject newObject, List<int> hierarchyPosition)
            {
                List<TriangleMeshInfo> allMeshes = newObject.AllStaticTriangleInfosAsNewList;

                foreach (UnityMeshManager manager in newObject.UnmanagedMeshes)
                {
                    allMeshes.Add(manager.TriangleInfo);
                }

                string objectIdentifier = "";

                if (newObject is OnFloorObject)
                {
                    OnFloorObject currentOnFloorObject = newObject as OnFloorObject;

                    if (!currentOnFloorObject.IsStructural)
                    {
                        Vector3 localPosition = MathHelper.ConvertPointIntoOriginTransform(baseObject: newObject.transform, originObject: exportBaseObject.transform, vector: Vector3.zero);

                        objectIdentifier = newObject.IdentifierString + " " + localPosition;
                    }
                }

                foreach (TriangleMeshInfo currentInfo in allMeshes)
                {

                    //MeshRenderer currentRenderer = currentMesh.transform.GetComponent<MeshRenderer>();

                    bool hasCollider = currentInfo.ActiveCollider;

                    Material currentMaterial = currentInfo.MaterialToBeUsed;

                    string materialIdentifier = "Not assigned";

                    if (currentMaterial != null)
                    {
                        if (currentInfo.MaterialReference != null) materialIdentifier = currentInfo.MaterialReference.Val.Identifier;
                        else materialIdentifier = currentInfo.AlternativeMaterial.name; //Only call when Alternative material is assigned!
                    }

                    string specialIdentifier = objectIdentifier;

                    if (CurrentExportProperties.FloorSeparation == ExportProperties.FloorSeparationStates.FloorAndWallsTogether)
                    {
                        if (hierarchyPosition.Count == 1)
                        {
                            specialIdentifier = "Floor " + (hierarchyPosition[0] - exportBaseObject.NegativeFloors - 1);
                        }
                    }

                    currentObjectGroupOrganizer.AddOrIncludeObjectGroup(specialIdentifier: specialIdentifier, materialIdentifier: materialIdentifier, hasCollider: hasCollider, hierarchyPosition: hierarchyPosition, newInfo: currentInfo, originalTransform: newObject.transform);
                }

                /*
                foreach (MeshFilter filter in newObject.AllStaticMeshes.unmanagedMeshes)
                {
                    Destroy(filter.transform.gameObject.GetComponent<MeshManager>());
                }
                */

            }
        }


        void VisualizeObject()
        {
            /*
            foreach (ObjectGroup mesh in currentObjectGroupOrganizer.ObjectGroups)
            {
                mesh.meshInfo.FinishMesh(updateColliders: false);
            }
            */
        }

        void ExportObjectFromGatheredInfo()
        {
            switch (CurrentExportProperties.OutputFormat)
            {
                case ExportProperties.OutputFormatStates.STLMulti:
                    break;
                case ExportProperties.OutputFormatStates.ObjSignle:
                    SaveAsSingleObjFile();
                    break;
                case ExportProperties.OutputFormatStates.ObjMulti:
                    SaveAsMultipleObjFiles();
                    break;
                case ExportProperties.OutputFormatStates.Multi3MF:
                    break;
                case ExportProperties.OutputFormatStates.FBXSingle:
                    break;
                default:
                    break;
            }
        }

        void SaveAsSingleObjFile()
        {
            List<string> fileLines = new List<string>();

            int currentOffset = 0;

            foreach (ObjectGroup currentObject in currentObjectGroupOrganizer.ObjectGroups)
            {
                fileLines.AddRange(currentObject.GetObjText(triangleIndexOffset: currentOffset));

                currentOffset += currentObject.vertexCount;
            }

            string completeFileLocation = Path.Combine(exportFolderPath, CurrentExportProperties.FileNameWithoutEnding + ".obj");

            StaticSaveAndLoadSystem.SaveLinesTextToFile(fileContent: fileLines, completeFileLocation: completeFileLocation);
        }

        void SaveAsMultipleObjFiles()
        {
            string folderName = CurrentExportProperties.FileNameWithoutEnding;

            string folderLocation = StaticSaveAndLoadSystem.CreateFolder(folderName: folderName, folderPath: exportFolderPath);

            foreach (ObjectGroup currentObject in currentObjectGroupOrganizer.ObjectGroups)
            {
                List<string> fileLines = new List<string>();

                fileLines.AddRange(currentObject.GetObjText(triangleIndexOffset: 0));

                string fileName = CurrentExportProperties.FileNameWithoutEnding + "-" + currentObject.IdentifierString + ".obj";

                string completeFileLocation = Path.Combine(folderLocation, fileName);

                StaticSaveAndLoadSystem.SaveLinesTextToFile(fileContent: fileLines, completeFileLocation: completeFileLocation);
            }
        }

        class ObjectGroupOrganizer
        {
            //public readonly MeshManager MeshTemplate;

            public ExportProperties currentExportProperties;

            public List<ObjectGroup> ObjectGroups;

            public readonly Transform exportReadyObject;
            readonly Transform worldTransform;

            public ObjectGroupOrganizer(Transform exportReadyObject, ExportProperties currentExportProperties, Transform worldTransform)
            {
                this.currentExportProperties = currentExportProperties;

                //this.MeshTemplate = meshTemplate;
                ObjectGroups = new List<ObjectGroup>();

                this.exportReadyObject = exportReadyObject;

                this.worldTransform = worldTransform;
            }

            public void AddOrIncludeObjectGroup(string specialIdentifier, string materialIdentifier, bool hasCollider, List<int> hierarchyPosition, TriangleMeshInfo newInfo, Transform originalTransform)
            {
                foreach (ObjectGroup group in ObjectGroups)
                {
                    if (currentExportProperties.HierarchyIdentifier)
                    {
                        if (hierarchyPosition.Count != group.hierarchyPosition.Count) continue;

                        for (int i = 0; i < hierarchyPosition.Count; i++)
                        {
                            if (hierarchyPosition[i] != group.hierarchyPosition[i]) continue;
                        }
                    }

                    if (currentExportProperties.FloorSeparation != ExportProperties.FloorSeparationStates.None)
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

                    if (currentExportProperties.SeparateNonStrucuturalObjects)
                    {
                        if (!group.specialIdentifier.Equals(specialIdentifier)) continue;
                    }

                    group.AddMesh(newInfo: newInfo, originalTransform: originalTransform);

                    return;
                }

                ObjectGroup newObject = new ObjectGroup(organizer: this, specialIdentifier: specialIdentifier, materialIdentifier: materialIdentifier, hasCollider: hasCollider, hierarchyPosition: hierarchyPosition, worldTransform: worldTransform);

                newObject.AddMesh(newInfo: newInfo, originalTransform: originalTransform);
            }


        }

        class ObjectGroup
        {
            //Properties
            //public MeshManager meshInfo;
            public TriangleMeshInfo currentInfo;
            public string specialIdentifier;
            public string materialIdentifier;
            public bool hasCollider;
            public List<int> hierarchyPosition = new List<int>();
            readonly Transform worldTransform;

            ObjectGroupOrganizer organizer;

            public ObjectGroup(ObjectGroupOrganizer organizer, string specialIdentifier, string materialIdentifier, bool hasCollider, List<int> hierarchyPosition, Transform worldTransform)
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
                this.specialIdentifier = specialIdentifier;
                this.materialIdentifier = materialIdentifier;
                this.hasCollider = hasCollider;
                this.hierarchyPosition = new List<int>(hierarchyPosition);
                this.worldTransform = worldTransform;
                //meshInfo.transform.name = IdentifierString;
            }

            public string IdentifierString
            {
                get
                {
                    string IdentifierSeparation = " - ";

                    string returnString = "";

                    if (!specialIdentifier.Equals(""))
                    {
                        returnString += "Name = " + specialIdentifier;
                    }

                    if (organizer.currentExportProperties.HierarchyIdentifier)
                    {
                        AddSeparator();
                        returnString += "Hierarchy position = ";

                        foreach (int pos in hierarchyPosition)
                        {
                            returnString += pos + "-";
                        }

                        returnString.Remove(returnString.Length - 1);

                    }

                    if (organizer.currentExportProperties.MaterialIdentifier
                        || organizer.currentExportProperties.IncludeInvisibleMeshes && materialIdentifier.Equals("Invisible"))
                    {
                        AddSeparator();
                        returnString += "Material = " + materialIdentifier;
                    }

                    if (organizer.currentExportProperties.ColliderIdentifier)
                    {
                        AddSeparator();
                        returnString += "Collider = " + hasCollider.ToString();
                    }

                    returnString.Replace(":", "=");

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

            public int vertexCount
            {
                get
                {
                    return currentInfo.VerticesHolder.Count;
                }
            }
            
            public List<string> GetObjText(int triangleIndexOffset)
            {
                List<string> returnList = new List<string>();

                if (currentInfo.VerticesHolder.Count == 0) return returnList;

                returnList = ObjExporter.GetObjLines(meshName: IdentifierString, vertices: currentInfo.VerticesHolder.Vertices, uvs: currentInfo.UVs, triangles: currentInfo.AllTrianglesDirectly, triangleIndexOffset: triangleIndexOffset, upDirection: ObjExporter.UpDirection.Y);

                return returnList;
            }
        }


        /*
        public void ExportFBXObject()
        {
            //Prepare
            currentBuilderController.CurrentBuilding.BackupVisibilityAndShowAll();

            GameObject exportObject = currentBuilderController.CurrentBuilding.gameObject;

            if (exportObject == null) return;

            string fileNamewihtoutEnding = CurrentExportProperties.FileNameWithoutEnding;

            if (string.IsNullOrWhiteSpace(fileNamewihtoutEnding))
            {
                return;
            }

            GameObject exportReadyObject = null;

            exportReadyObject = CreateExportHierarchy(exportObject);

            if(exportReadyObject != null)
            {
                DestroyInactiveSubObjects(exportReadyObject.transform);

                currentExporter.ExportGameObject(exportObject: exportReadyObject, fileNameWithoutEnding: CurrentExportProperties.FileNameWithoutEnding);
            }

            //Restore
            currentBuilderController.CurrentBuilding.RestoreVisibility();
            if (exportReadyObject != null) GameObject.Destroy(exportReadyObject);
        }

        GameObject CreateExportHierarchy(GameObject template)
        {
            GameObject returnObject;

            //Backup activation state and set active
            bool activationState = gameObject.activeSelf;
            gameObject.SetActive(true);

            returnObject = CreateObjectCopy(baseObject: template.transform.GetChild(0).gameObject, parent: transform);

            //DestroyInactiveSubObjects(parent: returnObject.transform);

            //Restore activation state
            gameObject.SetActive(activationState);

            return returnObject;
        }

        GameObject CreateObjectCopy(GameObject baseObject, Transform parent)
        {
            GameObject returnObject;

            if (ObjectHasValidMesh(baseObject))
            {
                returnObject = GameObject.Instantiate(original: MeshTemplate.gameObject, parent: parent.transform);

                returnObject.transform.localPosition = baseObject.transform.localPosition;
                returnObject.transform.localRotation = baseObject.transform.localRotation;
                returnObject.transform.localScale = baseObject.transform.localScale;

                returnObject.transform.GetComponent<MeshFilter>().mesh = baseObject.transform.GetComponent<MeshFilter>().mesh;
                ValidateParent(parent: returnObject.transform.parent);
            }
            else
            {
                returnObject = GameObject.Instantiate(original: EmptyTemplate, parent: parent.transform);

                returnObject.transform.localPosition = baseObject.transform.localPosition;
                returnObject.transform.localRotation = baseObject.transform.localRotation;
                returnObject.transform.localScale = baseObject.transform.localScale;

                returnObject.SetActive(false);
            }

            returnObject.name = baseObject.name;

            materialNames = new List<string>();

            foreach(Material material in MaterialLibrary)
            {
                materialNames.Add(material.name + " (Instance)");
            }

            foreach(Transform child in baseObject.transform)
            {
                if (child.gameObject.activeSelf)
                {
                    GameObject newObject = CreateObjectCopy(baseObject: child.gameObject, parent: returnObject.transform);

                    MeshRenderer renderer = child.GetComponent<MeshRenderer>();

                    if (renderer != null)
                    {
                        foreach(string materialName in materialNames)
                        {
                            if ((materialName).Equals(renderer.material.name))
                            {
                                newObject.name += " [" + renderer.material.name.Replace(" (Instance)", "") + "]";
                                break;
                            }
                        }
                    }
                }
            }

            return returnObject;

            void ValidateParent(Transform parent)
            {
                if (!parent.gameObject.activeSelf)
                {
                    parent.gameObject.SetActive(true);

                    ValidateParent(parent: parent.parent);

                    if(parent.parent != transform) //Not needed since first object is always active
                    {

                    }
                }
            }
        }
        */

        bool ObjectHasValidMesh(GameObject checkObject)
        {
            MeshFilter filter = checkObject.transform.GetComponent<MeshFilter>();

            if (filter != null)
            {
                if (filter.mesh.isReadable)
                {
                    if (filter.mesh.vertexCount > 2 && filter.mesh.triangles.Length > 0)
                    {
                        MeshRenderer renderer = checkObject.transform.GetComponent<MeshRenderer>();

                        if (renderer != null)
                        {
                            if (renderer.isVisible == true)
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (filter.mesh.vertexCount > 0)
                        {
                            Debug.Log("Error");
                        }


                        int v = filter.mesh.vertexCount;
                        int l = filter.mesh.triangles.Length;

                        bool b1 = filter.mesh.vertexCount > 2;
                        bool b2 = filter.mesh.triangles.Length > 0;

                        bool final = filter.mesh.vertexCount > 2 && filter.mesh.triangles.Length > 0;

                    }
                }
                else
                {
                    FailedObjects.Add(checkObject);
                }
            }

            return false;
        }

        void DestroyInactiveSubObjects(Transform parent)
        {
            if (parent.childCount == 0) return;

            for (int i = 0; i < parent.childCount; i++)
            {
                GameObject subObject = parent.GetChild(i).gameObject;

                if (subObject.activeSelf)
                {
                    DestroyInactiveSubObjects(parent: subObject.transform);
                }
                else
                {
                    Destroy(subObject);
                }
            }
        }

        public class ExportProperties
        {
            Mailbox mailbox;

            public List<MailboxLineSingle> SingleMailboxLines
            {
                get
                {
                    return mailbox.SingleMailboxLines;
                }
            }

            //File name
            MailboxLineString fileNameWithoutEnding;

            public string FileNameWithoutEnding
            {
                get
                {
                    return fileNameWithoutEnding.Val;


                }
                set
                {
                    fileNameWithoutEnding.Val = value;
                }
            }

            //Floor separation
            MailboxLineDistinctNamed floorSeparation;

            readonly List<string> floorSeparationStrings = new List<string>()
            {
                "None",
                "Floor & Walls together",
                "Floor & Walls separate (Not yet implemented)"
            };

            public enum FloorSeparationStates
            {
                None,
                FloorAndWallsTogether,
                FloorAndWallsSeparate
            }

            public FloorSeparationStates FloorSeparation
            {
                get
                {
                    return (FloorSeparationStates)floorSeparation.Val;
                }
                set
                {
                    floorSeparation.Val = (int)value;
                }
            }

            //Output format
            MailboxLineDistinctNamed outputFormat;

            readonly List<string> OutputFormatStrings = new List<string>()
            {
                "Multple STL",
                "1 OBJ",
                "Multiple OBJ",
                "Multiple 3MF (Not yet implemented)",
                "Single FBX (Not yet implemented)"
            };

            public enum OutputFormatStates
            {
                STLMulti,
                ObjSignle,
                ObjMulti,
                Multi3MF,
                FBXSingle
            }

            public OutputFormatStates OutputFormat
            {
                get
                {
                    return (OutputFormatStates)outputFormat.Val;
                }
                set
                {
                    outputFormat.Val = (int)value;
                }
            }

            //Up direction
            MailboxLineDistinctNamed upDirection;

            readonly List<string> UpDirectionStrings = new List<string>()
            {
                "Y",
                "Z"
            };

            public enum UpDirectionStates
            {
                Y,
                Z
            }

            public UpDirectionStates UpDirection
            {
                get
                {
                    return (UpDirectionStates)upDirection.Val;
                }
                set
                {
                    upDirection.Val = (int)value;
                }
            }

            //Reconstruciton indicators
            MailboxLineBool materialIdentifier;
            public bool MaterialIdentifier
            {
                get
                {
                    return materialIdentifier.Val;
                }
                set
                {
                    materialIdentifier.Val = value;
                }
            }

            MailboxLineBool colliderIdentifier;
            public bool ColliderIdentifier
            {
                get
                {
                    return colliderIdentifier.Val;
                }
                set
                {
                    colliderIdentifier.Val = value;
                }
            }

            MailboxLineBool separateDummyObjects;
            public bool SeparateNonStrucuturalObjects
            {
                get
                {
                    return separateDummyObjects.Val;
                }
                set
                {
                    separateDummyObjects.Val = value;
                }
            }

            MailboxLineBool groupMaterials;
            public bool GroupMaterials
            {
                get
                {
                    return groupMaterials.Val;
                }
                set
                {
                    groupMaterials.Val = value;
                }
            }

            MailboxLineBool includeInvisibleMeshes;
            public bool IncludeInvisibleMeshes
            {
                get
                {
                    return includeInvisibleMeshes.Val;
                }
                set
                {
                    includeInvisibleMeshes.Val = value;
                }
            }

            MailboxLineBool hierarchyIdentifier;
            public bool HierarchyIdentifier
            {
                get
                {
                    return hierarchyIdentifier.Val;
                }
                set
                {
                    hierarchyIdentifier.Val = value;
                }
            }

            MailboxLineBool includeSpecialObjects;
            public bool IncludeSpecialObjects
            {
                get
                {
                    return includeSpecialObjects.Val;
                }
                set
                {
                    includeSpecialObjects.Val = value;
                }
            }

            //Constructor

            public ExportProperties()
            {
                mailbox = new Mailbox(null);

                fileNameWithoutEnding = new MailboxLineString(name: "File name without ending", objectHolder: mailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: "");

                floorSeparation = new MailboxLineDistinctNamed(name: "Floor separation", objectHolder: mailbox, valueType: Mailbox.ValueType.buildParameter, entries: floorSeparationStrings);

                separateDummyObjects = new MailboxLineBool(name: "Separate dummy objects", objectHolder: mailbox, valueType: Mailbox.ValueType.buildParameter);

                materialIdentifier = new MailboxLineBool(name: "Material identifier", objectHolder: mailbox, valueType: Mailbox.ValueType.buildParameter);
                colliderIdentifier = new MailboxLineBool(name: "Collider identifier", objectHolder: mailbox, valueType: Mailbox.ValueType.buildParameter);
                hierarchyIdentifier = new MailboxLineBool(name: "Hierarchy identifier", objectHolder: mailbox, valueType: Mailbox.ValueType.buildParameter);
                includeSpecialObjects = new MailboxLineBool(name: "Include special objects", objectHolder: mailbox, valueType: Mailbox.ValueType.buildParameter);

                groupMaterials = new MailboxLineBool(name: "Group materials", objectHolder: mailbox, valueType: Mailbox.ValueType.buildParameter);
                includeInvisibleMeshes = new MailboxLineBool(name: "Include invisible meshes", objectHolder: mailbox, valueType: Mailbox.ValueType.buildParameter);

                upDirection = new MailboxLineDistinctNamed(name: "UpDirection", objectHolder: mailbox, valueType: Mailbox.ValueType.buildParameter, entries: UpDirectionStrings);
                outputFormat = new MailboxLineDistinctNamed(name: "Output format", objectHolder: mailbox, valueType: Mailbox.ValueType.buildParameter, entries: OutputFormatStrings);
            }

            public void SetVrcObjProperties()
            {
                FloorSeparation = FloorSeparationStates.None;
                SeparateNonStrucuturalObjects = true;
                MaterialIdentifier = true;
                ColliderIdentifier = true;
                HierarchyIdentifier = false;
                IncludeSpecialObjects = true;
                GroupMaterials = true;
                IncludeInvisibleMeshes = true;
                UpDirection = UpDirectionStates.Y;
                OutputFormat = OutputFormatStates.ObjSignle;


            }

            public void Set3DPrintingObjProperties()
            {
                FloorSeparation = FloorSeparationStates.FloorAndWallsTogether;
                SeparateNonStrucuturalObjects = true;
                MaterialIdentifier = false;
                ColliderIdentifier = false;
                HierarchyIdentifier = false;
                IncludeSpecialObjects = false;
                GroupMaterials = false;
                IncludeInvisibleMeshes = false;
                UpDirection = UpDirectionStates.Z;
                OutputFormat = OutputFormatStates.ObjMulti;
            }
        }
    }
}