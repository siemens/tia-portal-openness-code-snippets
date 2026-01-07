// © Siemens 2025
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using NUnit.Framework;
using Siemens.Engineering.CrossReference;
using Siemens.Engineering.HW.Features;
using Siemens.Engineering.SW;
using Siemens.Engineering.SW.Blocks;
using TiaPortal.Openness.CodeSnippets.Plain.Setup;

namespace TiaPortal.Openness.CodeSnippets.Plain.Step7.PLC;

[TestFixture("Step7.zap21")]
public class DataBlockSnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    [Test]
    public void GetCrossReferencesForDb()
    {
        var plcDevice = Project.Devices.First(x => x.Name == "PLC_S120Democase");

        var plcSoftware = plcDevice.DeviceItems[1].GetService<SoftwareContainer>().Software as PlcSoftware;

        var db = plcSoftware?.BlockGroup.Blocks.First(x => x.Name == "Axis_red_DB");
        var crossReferenceService = db?.GetService<CrossReferenceService>();
        var crossReferences =
            crossReferenceService?.GetCrossReferences(CrossReferenceFilter.AllObjects).Sources.ToList();


        if (crossReferences != null)
        {
            Console.WriteLine(string.Join("\n", crossReferences.Select(c => $"{c.Name}")));
        }
    }

    [Test]
    public void GetDbInterfaceMembers()
    {
        var plcDevice = Project.Devices.First(x => x.Name == "PLC_S120Democase");
        if (plcDevice.DeviceItems[1].GetService<SoftwareContainer>().Software is not PlcSoftware plcSoftware)
        {
            throw new InvalidOperationException("The device does not contain a software container");
        }

        if (plcSoftware.BlockGroup.Blocks.First(x => x.Name == "Axis_red_DB") is not InstanceDB instanceDb)
        {
            throw new InvalidOperationException("Axis_red_DB is not a InstanceDB");
        }

        if (plcSoftware.BlockGroup.Blocks.First(x => x.Name == "GlobalBlock") is not GlobalDB globalDb)
        {
            throw new InvalidOperationException("GlobalBlock is not a GlobalDB");
        }

        var instanceMembers = instanceDb.Interface.Members;
        Console.WriteLine(string.Join("\n", instanceMembers.Select(m => m.Name)));
        var globalMembers = globalDb.Interface.Members;
        Console.WriteLine(string.Join("\n", globalMembers.Select(m => m.Name)));
    }
}
