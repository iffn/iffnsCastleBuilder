using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class ExporterUI : MonoBehaviour
    {
        [SerializeField] ControlBox ExporterLines;

        Exporter linkedExporter;

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

            ExporterLines.AddButtonLine(text: "Set 3D printing settings", call: delegate { linkedExporter.CurrentExportProperties.Set3DPrintingObjProperties(); SetLines(); });
            ExporterLines.AddButtonLine(text: "Set VRC settings", call: delegate { linkedExporter.CurrentExportProperties.SetVrcObjProperties(); SetLines(); });

            ExporterLines.AddButtonLine(text: "Export object", call: delegate { linkedExporter.ExportObject(); });
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