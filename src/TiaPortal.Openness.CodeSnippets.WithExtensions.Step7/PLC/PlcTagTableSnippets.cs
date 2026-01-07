// © Siemens 2025
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using NUnit.Framework;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;
using Siemens.Engineering.SW;
using Siemens.Engineering.SW.Tags;
using Siemens.Engineering.SW.Units;
using TiaPortal.Openness.CodeSnippets.WithExtensions.Setup;

namespace TiaPortal.Openness.CodeSnippets.WithExtensions.Step7.PLC;

[TestFixture("Step7.zap21")]
public class PlcTagTableSnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    [Test]
    public void GetAllPlcTags()
    {
        var device = Project.Devices.First(x => x.Name == "PLC_S120Democase");
        var allPlcTags = GetAllPlcTags(device);
        Console.WriteLine(string.Join("\n"), allPlcTags.Select(l => $"{l.Name}"));
    }

    private static List<PlcTag> GetAllPlcTags(Device plcDevice)
    {
        var allTagTables = GetAllPlcTagTables(plcDevice);
        return allTagTables.SelectMany(x => x.Tags).ToList();
    }

    private static List<PlcTagTable> GetAllPlcTagTables(Device plcDevice)
    {
        if (plcDevice.DeviceItems.Single(x => x.Classification == DeviceItemClassifications.CPU)
                .GetService<SoftwareContainer>().Software is not PlcSoftware software)
        {
            throw new InvalidOperationException("Device does not contain any software instance (null).");
        }

        var tagTables = software.TagTableGroup.AllTagTables();

        var units = software.GetService<PlcUnitProvider>().UnitGroup.Units;
        var unitTagTables = units.SelectMany(x => x.TagTableGroup.AllTagTables());
        var allTagTables = tagTables.Concat(unitTagTables);
        return allTagTables.ToList();
    }
}
