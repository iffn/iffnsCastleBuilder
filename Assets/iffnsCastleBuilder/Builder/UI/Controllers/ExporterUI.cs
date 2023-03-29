using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class ExporterUI : MonoBehaviour
    {
        [SerializeField] ControlBox ExporterLines;

        Exporter linkedExporter;

        InputLine exportFieldToCopy;

        public void Setup(Exporter linkedExporter)
        {
            this.linkedExporter = linkedExporter;

            SetLines();
        }

        void SetLines()
        {
            ExporterLines.Clear();

            ExporterLines.AddTextLine(text: "Export options", bold: true);

            ExporterLines.AddButtonLine(text: "Get file name", call: delegate { SetFileNameFromSaveSystem(); });

            ExporterLines.AddMailboxLines(lines: linkedExporter.CurrentExportProperties.SingleMailboxLines, lineOwner: null);

            ExporterLines.AddButtonLine(text: "Set Unity settings", call: delegate { linkedExporter.CurrentExportProperties.SetVrcObjProperties(); SetLines(); });
            ExporterLines.AddButtonLine(text: "Set 3D printing settings", call: delegate { linkedExporter.CurrentExportProperties.Set3DPrintingObjProperties(); SetLines(); });

            //ExporterLines.AddButtonLine("Copy export text to clipboard", delegate { linkedExporter.CopyExportTextToClipboard(); });
            ExporterLines.AddButtonLine("Export lines to text field", delegate { linkedExporter.ExportToExportField(); });

            exportFieldToCopy = ExporterLines.AddOutputField("Export text", null, null);
            exportFieldToCopy.InputField.lineType = TMPro.TMP_InputField.LineType.MultiLineNewline;
            exportFieldToCopy.InputField.readOnly = true;

            #if !UNITY_WEBGL
            ExporterLines.AddButtonLine(text: "Export object (name required)", call: delegate { linkedExporter.ExportObject(); });
            ExporterLines.AddButtonLine(text: "Open export folder", call: delegate { linkedExporter.OpenExportFolder(); });
            #endif

            //ExporterLines.AddTextLine(text: @"The exported files can be found in iffnsCastleBuilder_Data\StreamingAssets\Exports", bold: false);
        }

        public void WriteExportText(string text)
        {
            exportFieldToCopy.InputField.text = text; 
        }

        void SetFileNameFromSaveSystem()
        {
            linkedExporter.SetFileNamePropertyFromSaveSystem();

            ExporterLines.Clear();

            SetLines();
        }

        // Start is called before the first frame update


    }
}