using NUnit.Framework;
using Siemens.Engineering.HW;
using Siemens.Engineering.Safety;
using TiaPortal.Openness.CodeSnippets.Plain.Setup;

namespace TiaPortal.Openness.CodeSnippets.Plain.Step7.PLC;

[TestFixture("Step7.zap21")]
public class SafetySnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    [Test]
    public void ReadoutSafetyAdministration()
    {
        // Find CPU
        var plc = Project.Devices
            .First(d => d.DeviceItems.Any(i => i.Classification == DeviceItemClassifications.CPU));
        var cpu = plc.DeviceItems.Single(x => x.Classification == DeviceItemClassifications.CPU);

        // Readout Safety Administration
        var safetyAdmin = cpu.GetService<SafetyAdministration>();
        var runtimeGroup = safetyAdmin.RuntimeGroups.First();

        var mainSafetyBlockName = runtimeGroup.GetAttribute("MainSafetyBlockName");
        Console.WriteLine($"Could get block name: {mainSafetyBlockName != null}");
        var mainSafetyDbName = runtimeGroup.GetAttribute("MainSafetyBlockIDbName");
        Console.WriteLine($"Could get safety db name: {mainSafetyDbName != null}");
    }
}
