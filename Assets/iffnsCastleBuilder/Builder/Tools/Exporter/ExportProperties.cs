using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using iffnsStuff.iffnsBaseSystemForUnity;
using iffnsStuff.iffnsBaseSystemForUnity.Tools;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class ExportProperties
    {
        readonly Mailbox mailbox;

        public List<MailboxLineSingle> SingleMailboxLines
        {
            get
            {
                return mailbox.SingleMailboxLines;
            }
        }

        //File name
        readonly MailboxLineString fileNameWithoutEnding;

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
        readonly MailboxLineBool separateFloors;

        public bool SeparateFloors
        {
            get
            {
                return separateFloors.Val;
            }
            set
            {
                separateFloors.Val = value;
            }
        }

        //Output format
        readonly MailboxLineDistinctNamed outputFormat;

        readonly List<string> OutputFormatStrings = new()
            {
                //"Multple STL",
                "1 OBJ",
                "Multiple OBJ"
                //"Multiple 3MF (Not yet implemented)",
                //"Single FBX (Not yet implemented)"
            };

        public enum OutputFormatStates
        {
            //STLMulti,
            ObjSignle,
            ObjMulti
            //Multi3MF,
            //FBXSingle
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
        readonly MailboxLineDistinctNamed upDirection;

        readonly List<string> UpDirectionStrings = new()
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
        readonly MailboxLineBool materialIdentifier;
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

        readonly MailboxLineBool colliderIdentifier;
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

        readonly MailboxLineBool includeInvisibleMeshes;
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

        readonly MailboxLineBool hierarchyIdentifier;
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

        readonly MailboxLineBool includeFurniture;
        public bool IncludeFurniture
        {
            get
            {
                return includeFurniture.Val;
            }
            set
            {
                includeFurniture.Val = value;
            }
        }

        //Constructor

        public ExportProperties()
        {
            mailbox = new Mailbox(null);

            fileNameWithoutEnding = new MailboxLineString(name: "File name without ending", objectHolder: mailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: "");

            separateFloors = new MailboxLineBool(name: "Separate floors", objectHolder: mailbox, valueType: Mailbox.ValueType.buildParameter, DefaultValue: true);

            //separateDummyObjects = new MailboxLineBool(name: "Separate dummy objects", objectHolder: mailbox, valueType: Mailbox.ValueType.buildParameter);

            materialIdentifier = new MailboxLineBool(name: "Material identifier", objectHolder: mailbox, valueType: Mailbox.ValueType.buildParameter);
            colliderIdentifier = new MailboxLineBool(name: "Collider identifier", objectHolder: mailbox, valueType: Mailbox.ValueType.buildParameter);
            hierarchyIdentifier = new MailboxLineBool(name: "Hierarchy identifier", objectHolder: mailbox, valueType: Mailbox.ValueType.buildParameter);

            includeFurniture = new MailboxLineBool(name: "Include furniture", objectHolder: mailbox, valueType: Mailbox.ValueType.buildParameter);
            includeInvisibleMeshes = new MailboxLineBool(name: "Include invisible meshes", objectHolder: mailbox, valueType: Mailbox.ValueType.buildParameter);

            //groupMaterials = new MailboxLineBool(name: "Group materials", objectHolder: mailbox, valueType: Mailbox.ValueType.buildParameter);

            upDirection = new MailboxLineDistinctNamed(name: "UpDirection", objectHolder: mailbox, valueType: Mailbox.ValueType.buildParameter, entries: UpDirectionStrings);
            outputFormat = new MailboxLineDistinctNamed(name: "Output format", objectHolder: mailbox, valueType: Mailbox.ValueType.buildParameter, entries: OutputFormatStrings);
        }

        public void SetVrcObjProperties()
        {
            //FloorSeparation = FloorSeparationStates.None;
            //SeparateNonStrucuturalObjects = true;
            SeparateFloors = true;
            MaterialIdentifier = true;
            ColliderIdentifier = true;
            HierarchyIdentifier = true;
            IncludeFurniture = true;
            //GroupMaterials = true;
            IncludeInvisibleMeshes = true;
            UpDirection = UpDirectionStates.Y;
            OutputFormat = OutputFormatStates.ObjSignle;


        }

        public void Set3DPrintingObjProperties()
        {
            //FloorSeparation = FloorSeparationStates.FloorAndWallsTogether;
            //SeparateNonStrucuturalObjects = true;
            SeparateFloors = true;
            MaterialIdentifier = false;
            ColliderIdentifier = false;
            HierarchyIdentifier = false;
            IncludeFurniture = false;
            //GroupMaterials = false;
            IncludeInvisibleMeshes = false;
            UpDirection = UpDirectionStates.Z;
            OutputFormat = OutputFormatStates.ObjMulti;
        }
    }
}