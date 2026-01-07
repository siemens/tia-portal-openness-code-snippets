// © Siemens 2025 - 2026
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using NUnit.Framework;
using Siemens.Engineering.HW.Features;
using Siemens.Engineering.SW;
using Siemens.Engineering.SW.Units;
using TiaPortal.Openness.CodeSnippets.Plain.Setup;

namespace TiaPortal.Openness.CodeSnippets.Plain.Step7.PLC;

[TestFixture("Step7.zap21")]
public class SoftwareUnitsSnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    [Test]
    public void GetSoftwareUnits()
    {
        var deviceItem = Project.Devices.First(n => n.Name == "PLC_S120Democase").DeviceItems
            .First(n => n.Name.Contains("PLC"));
        var softwareContainer = deviceItem.GetService<SoftwareContainer>();
        var plcSoftware = softwareContainer.Software as PlcSoftware;
        var plcUnitProvider = plcSoftware?.GetService<PlcUnitProvider>();
        var safetyUnits = plcUnitProvider?.UnitGroup.SafetyUnits;
        Console.WriteLine($"SafetyUnits found: {safetyUnits != null}");
        var units = plcUnitProvider?.UnitGroup.Units;
        Console.WriteLine($"Units found: {units != null}");
    }
}
