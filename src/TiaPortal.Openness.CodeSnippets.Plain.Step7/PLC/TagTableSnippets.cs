// © Siemens 2025
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using System.Xml.Linq;
using NUnit.Framework;
using Siemens.Engineering;
using Siemens.Engineering.HW.Features;
using Siemens.Engineering.SW;
using TiaPortal.Openness.CodeSnippets.Plain.Setup;

namespace TiaPortal.Openness.CodeSnippets.Plain.Step7.PLC;

[TestFixture("Step7.zap20")]
public class TagTableSnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    [TearDown]
    public void CleanupTempFile()
    {
        if (!string.IsNullOrEmpty(_lastTempPath) && File.Exists(_lastTempPath))
        {
            File.Delete(_lastTempPath);
            _lastTempPath = null;
        }
    }

    private string _lastTempPath;

    [Test]
    public void EditExistingTags()
    {
        var plcDevice = Project.Devices.First(x => x.Name == "PLC_S120Democase");
        var plcSoftware = plcDevice.DeviceItems[1].GetService<SoftwareContainer>().Software as PlcSoftware;
        var myTagTable = plcSoftware?.TagTableGroup.TagTables.FirstOrDefault(x => x.Name == "DemocaseAppTagTable");
        var myTag = myTagTable?.Tags.FirstOrDefault();

        if (myTag != null)
        {
            myTag.Name = "NewTagName2";
        }
    }

    [Test]
    public void EditExistingTagsViaExportImport()
    {
        var plcDevice = Project.Devices.First(x => x.Name == "PLC_S120Democase");
        var plcSoftware = plcDevice.DeviceItems[1].GetService<SoftwareContainer>().Software as PlcSoftware;
        var myTagTable = plcSoftware?.TagTableGroup.TagTables.FirstOrDefault(x => x.Name == "DemocaseAppTagTable");

        // Use a generic temp path for export
        var tempPath = Path.Combine(Path.GetTempPath(), "DemocaseAppTagTable_export.xml");
        _lastTempPath = tempPath;
        myTagTable?.Export(new FileInfo(tempPath), ExportOptions.WithDefaults);

        // Read the exported XML file content
        var xmlContent = File.ReadAllText(tempPath);

        var xdoc = XDocument.Parse(xmlContent);

        // Get all SW.Tags.PlcTag elements
        var plcTags = xdoc.Descendants("SW.Tags.PlcTag").ToList();

        var firstTag = plcTags.FirstOrDefault();
        if (firstTag != null)
        {
            var nameElement = firstTag.Element("AttributeList")?.Element("Name");
            if (nameElement != null)
            {
                nameElement.Value = "NewTagName";
            }
        }

        // Save the modified XML back to the file
        xdoc.Save(tempPath);

        // Import the modified XML back into the tag table
        plcSoftware?.TagTableGroup.TagTables?.Import(new FileInfo(tempPath), ImportOptions.Override);
    }
}
