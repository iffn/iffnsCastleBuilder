using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using iffnsStuff.iffnsBaseSystemForUnity;
using iffnsStuff.iffnsBaseSystemForUnity.Tools;
using static iffnsStuff.iffnsBaseSystemForUnity.StaticSaveAndLoadSystem;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class Exporter : MonoBehaviour
    {
        [SerializeField] CastleBuilderController currentBuilderController = null;
        [SerializeField] ExporterUI linkedExporterUI = null;
        [SerializeField] List<GameObject> FailedObjects = new List<GameObject>();

        string exporterVersion = "1.0";

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

            CurrentExportProperties.SetVrcObjProperties();

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

        ExportGroupOrganizer currentObjectGroupOrganizer;
        //GameObject exportReadyObject;
        CastleController exportBaseObject;

        List<string> GetExportText()
        {
            currentBuilderController.CurrentBuilding.optimizeMeshes = CurrentExportProperties.OptimizeMeshes;

            currentBuilderController.CurrentBuilding.ApplyBuildParameters();

            currentBuilderController.CurrentBuilding.BackupVisibilityAndShowAll();

            exportBaseObject = currentBuilderController.CurrentBuilding;

            if (exportBaseObject == null)
            {
                Restore();
                return new List<string>();
            }

            string fileNamewihtoutEnding = CurrentExportProperties.FileNameWithoutEnding;

            //No need to check file name for clipboard

            //Prepare export info
            PrepareExportInfo();

            //Export object
            List<string> returnStrings = GetPreparedSingleObjFile();

            //Restore
            Restore();

            void Restore()
            {
                currentBuilderController.CurrentBuilding.optimizeMeshes = false;
                currentBuilderController.CurrentBuilding.RestoreVisibility();
                currentObjectGroupOrganizer = null;
                //if (exportReadyObject != null) GameObject.Destroy(exportReadyObject);
            }

            return returnStrings;
        }

        public void CopyExportTextToClipboard()
        {
            List<string> lines = GetExportText();

            Debug.Log(lines.Count);

            if (lines == null || lines.Count == 0) return;

            string output = string.Join(separator: MyStringComponents.newLine, values: lines);


            GUIUtility.systemCopyBuffer = output;
        }

        public void ExportObject()
        {
            //Prepare
            currentBuilderController.CurrentBuilding.optimizeMeshes = CurrentExportProperties.OptimizeMeshes;

            currentBuilderController.CurrentBuilding.ApplyBuildParameters();

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
                currentBuilderController.CurrentBuilding.optimizeMeshes = false;
                currentBuilderController.CurrentBuilding.RestoreVisibility();
                currentObjectGroupOrganizer = null;
                //if (exportReadyObject != null) GameObject.Destroy(exportReadyObject);
            }
        }

        public void PrepareExportInfo()
        {
            currentObjectGroupOrganizer = new ExportGroupOrganizer(exportReadyObject: exportBaseObject.transform, currentExportProperties: CurrentExportProperties, worldTransform: transform);

            List<int> hierarchyPosition = new();

            if (CurrentExportProperties.SeparateFloors)
            {
                hierarchyPosition.Add(0);
                foreach (FloorController floor in exportBaseObject.CurrentListOfFloors)
                {
                    GetAndAddMeshFromObject(newObject: floor, hierarchyPosition: hierarchyPosition);
                }
                hierarchyPosition.Add(0);
            }
            else
            {
                hierarchyPosition.Add(0);
                GetAndAddMeshFromObject(newObject: exportBaseObject, hierarchyPosition: hierarchyPosition);
            }

            void GetAndAddMeshFromObject(IBaseObject newObject, List<int> hierarchyPosition)
            {
                //Add object
                if (newObject is BaseGameObject)
                {
                    bool stillNeedsToBeAdded = true;

                    if (newObject is OnFloorObject floorObject)
                    {
                        if (floorObject.IsStructural)
                        {
                            //Add structural object
                            List<int> floorHierarchy = new(hierarchyPosition);

                            floorHierarchy.RemoveAt(floorHierarchy.Count - 1);

                            AddMeshFromObject(newObject: floorObject, hierarchyPosition: floorHierarchy, positionObject: floorObject.LinkedFloor.transform);
                        }
                        else
                        {
                            //Add furniture
                            if (CurrentExportProperties.IncludeFurniture)
                            {
                                hierarchyPosition[^1] += 1;

                                BaseGameObject newGameObject = newObject as BaseGameObject;

                                AddMeshFromObject(newObject: newGameObject, hierarchyPosition: hierarchyPosition, positionObject: newGameObject.transform);
                            }
                        }

                        stillNeedsToBeAdded = false;
                    }
                    else if(newObject is NodeWallSystem nodeWallSystem)
                    {
                        List<int> floorHierarchy = new(hierarchyPosition);

                        floorHierarchy.RemoveAt(floorHierarchy.Count - 1);

                        AddMeshFromObject(newObject: nodeWallSystem, hierarchyPosition: floorHierarchy, positionObject: nodeWallSystem.LinkedFloor.transform);

                        stillNeedsToBeAdded = false;
                    }

                    if (stillNeedsToBeAdded)
                    {
                        //Add rest
                        if(!(newObject is NodeWallSystem)) hierarchyPosition[^1] += 1;

                        BaseGameObject newGameObject = newObject as BaseGameObject;

                        AddMeshFromObject(newObject: newGameObject, hierarchyPosition: hierarchyPosition, positionObject: newGameObject.transform);
                    }
                }

                List<IBaseObject> SubObjects = newObject.SubObjects;

                if (SubObjects.Count == 0) return;

                //Add sub objects
                hierarchyPosition.Add(0);

                foreach (IBaseObject subObject in SubObjects)
                {
                    if (subObject is VirtualBlock) continue;
                    GetAndAddMeshFromObject(subObject, hierarchyPosition);
                }

                hierarchyPosition.RemoveAt(hierarchyPosition.Count - 1);
            }

            void AddMeshFromObject(BaseGameObject newObject, List<int> hierarchyPosition, Transform positionObject)
            {
                List<TriangleMeshInfo> allMeshes = newObject.AllStaticTriangleInfosAsNewList;

                foreach (UnityMeshManager manager in newObject.UnmanagedMeshes)
                {
                    allMeshes.Add(manager.TriangleInfo);
                }

                foreach (TriangleMeshInfo currentInfo in allMeshes)
                {
                    bool hasCollider;

                    switch (currentInfo.ActiveCollider)
                    {
                        case TriangleMeshInfo.ColliderStates.VisibleCollider:
                            hasCollider = true;
                            break;
                        case TriangleMeshInfo.ColliderStates.SeeThroughCollider:
                            hasCollider = true;
                            break;
                        case TriangleMeshInfo.ColliderStates.VisbleWithoutCollider:
                            hasCollider = false;
                            break;
                        default:
                            Debug.LogWarning("Missing enum state");
                            hasCollider = true;
                            break;
                    }

                    //bool hasCollider = currentInfo.ActiveCollider;

                    Material currentMaterial = currentInfo.MaterialToBeUsed;

                    string materialIdentifier = "Not assigned";

                    if (currentMaterial != null)
                    {
                        if (currentInfo.MaterialReference != null) materialIdentifier = currentInfo.MaterialReference.Val.Identifier;
                        else materialIdentifier = currentInfo.AlternativeMaterial.name; //Only call when Alternative material is assigned!
                    }

                    string name;

                    if(newObject.Name.Length > 0)
                    {
                        name = newObject.Name;
                    }
                    else
                    {
                        name = newObject.IdentifierString;
                    }

                    Vector3 localPosition = positionObject.localPosition;

                    if (CurrentExportProperties.SeparateFloors)
                    {
                        if (hierarchyPosition.Count == 1)
                        {
                            name = "Floor " + (hierarchyPosition[0] - exportBaseObject.NegativeFloors - 1);
                        }
                    }

                    currentObjectGroupOrganizer.AddOrIncludeObjectGroup(name: name, localPosition: localPosition, materialIdentifier: materialIdentifier, hasCollider: hasCollider, hierarchyPosition: hierarchyPosition, newInfo: currentInfo, originalTransform: newObject.transform);
                }

                /*
                foreach (MeshFilter filter in newObject.AllStaticMeshes.unmanagedMeshes)
                {
                    Destroy(filter.transform.gameObject.GetComponent<MeshManager>());
                }
                */

            }
        }

        void ExportObjectFromGatheredInfo()
        {
            switch (CurrentExportProperties.OutputFormat)
            {
                case ExportProperties.OutputFormatStates.ObjSignle:
                    SaveAsSingleObjFile();
                    break;
                case ExportProperties.OutputFormatStates.ObjMulti:
                    SaveAsMultipleObjFiles();
                    break;
                default:
                    Debug.LogWarning("Missing enum state");
                    break;
            }
        }

        void SaveAsSingleObjFile()
        {
            List<string> lines = GetPreparedSingleObjFile();

            string completeFileLocation = Path.Combine(exportFolderPath, CurrentExportProperties.FileNameWithoutEnding + ".obj");

            StaticSaveAndLoadSystem.SaveLinesTextToFile(fileContent: lines, completeFileLocation: completeFileLocation);
        }

        List<string> GetPreparedSingleObjFile()
        {
            List<string> fileLines = new();

            fileLines.Add("# Created with iffn's Castle Buiilder");
            fileLines.Add($"# Exporter version: {exporterVersion}");
            fileLines.Add("");

            fileLines.Add("# Object identifiers:");
            fileLines.Add("");

            int currentTriangleIndexOffset = 0;

            int id = 1;

            List<string> identifiers = new();

            foreach (ExportGroup currentObject in currentObjectGroupOrganizer.ObjectGroups)
            {
                string idString = id.ToString("D3");

                List<string> newLines = currentObject.GetObjText(currentId: idString, triangleIndexOffset: currentTriangleIndexOffset);

                fileLines.AddRange(newLines);
                identifiers.Add($"# {newLines[0].Substring(startIndex: 2)}"); //without o identifier

                currentTriangleIndexOffset += currentObject.VertexCount;

                id++;
            }

            fileLines.InsertRange(index: 4, identifiers.ToArray());


            return fileLines;
        }

        void SaveAsMultipleObjFiles()
        {
            string folderName = CurrentExportProperties.FileNameWithoutEnding;

            string folderLocation = StaticSaveAndLoadSystem.CreateFolder(folderName: folderName, folderPath: exportFolderPath);

            foreach (ExportGroup currentObject in currentObjectGroupOrganizer.ObjectGroups)
            {
                List<string> fileLines = new();

                fileLines.AddRange(currentObject.GetObjText(triangleIndexOffset: 0));

                string fileName = CurrentExportProperties.FileNameWithoutEnding + "-" + currentObject.IdentifierString + ".obj";

                string completeFileLocation = Path.Combine(folderLocation, fileName);

                StaticSaveAndLoadSystem.SaveLinesTextToFile(fileContent: fileLines, completeFileLocation: completeFileLocation);
            }
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

        public void OpenExportFolder()
        {
            string path = exportFolderPath.Replace('/', '\\');

            StaticSaveAndLoadSystem.OpenFileLocationInExplorer(path);
        }
    }
}