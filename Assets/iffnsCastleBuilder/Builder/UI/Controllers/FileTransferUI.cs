using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    public class FileTransferUI : MonoBehaviour
    {
        [SerializeField] ControlBox ExporterLines;
        [SerializeField] SaveAndLoadSystem linkedSaveAndLoadSystem;

        InputLine exportFieldToCopy;

        private void Start()
        {
            SetLines();
        }

        void SetLines()
        {
            ExporterLines.Clear();

            ExporterLines.AddTextLine(text: "File transfer lines", bold: true);

            exportFieldToCopy = ExporterLines.AddOutputField("Export text", null, null);
            exportFieldToCopy.InputFieldLegacy.lineType = UnityEngine.UI.InputField.LineType.MultiLineNewline;

            ExporterLines.AddButtonLine(text: "Output current data", call: delegate { OutputCurrentData(); });
            ExporterLines.AddButtonLine(text: "Import data from field", call: delegate { ImportDataFromField(); });
        }

        void OutputCurrentData()
        {
            string text = linkedSaveAndLoadSystem.GetCurrentFileAsString();

            exportFieldToCopy.InputFieldLegacy.SetTextWithoutNotify(text);
        }

        void ImportDataFromField()
        {
            string text = exportFieldToCopy.InputField.text;

            linkedSaveAndLoadSystem.LoadDataFromString(text);
        }
    }
}