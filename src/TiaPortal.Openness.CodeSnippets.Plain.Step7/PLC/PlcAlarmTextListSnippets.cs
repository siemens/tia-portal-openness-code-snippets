// © Siemens 2025
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using NUnit.Framework;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;
using Siemens.Engineering.Library;
using Siemens.Engineering.SW;
using Siemens.Engineering.SW.Alarm;
using TiaPortal.Openness.CodeSnippets.Plain.Setup;

namespace TiaPortal.Openness.CodeSnippets.Plain.Step7.PLC;

[TestFixture("Step7.zap20")]
public class PlcAlarmTextListSnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    [Test]
    public void GetAlarmTextProvider()
    {
        var softwareContainer = TiaPortalInstance.Projects.First().Devices
            .First(x => x.TypeIdentifier.Equals("System:Device.S71500"))
            .DeviceItems
            .First(x => x.Classification.Equals(DeviceItemClassifications.CPU)).GetService<SoftwareContainer>();

        var software = softwareContainer.Software as PlcSoftware;
        if (software == null)
        {
            Console.WriteLine("PlcSoftware not found");
        }

        var alarmTextProvider = software?.GetService<PlcAlarmTextProvider>();
        Console.WriteLine($"PlcAlarmTextProvider found: {alarmTextProvider != null}");
    }

    [Test]
    public void AddPlcAlarmTextListToGlobalLibrary()
    {
        DirectoryInfo destination = new(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));

        var globalLibrary = TiaPortalInstance.GlobalLibraries.Create<UserGlobalLibrary>(destination, "newLibrary");
        var plcSoftware =
            Project.Devices.First(x => x.Name == "PLC_S120Democase").DeviceItems
                .Single(x => x.Classification == DeviceItemClassifications.CPU).GetService<SoftwareContainer>()
                .Software as PlcSoftware;

        //only user text lists could be added (PlcAlarmSystemTextlists not working!)
        var alarmTextList =
            plcSoftware.PlcAlarmTextlistGroup.PlcAlarmUserTextlists.FirstOrDefault(x =>
                x.Name == "USER_1");
        var copy = globalLibrary.MasterCopyFolder.MasterCopies.Create(alarmTextList);

        Console.WriteLine($"Could create a copy: {copy != null}");

        globalLibrary.Close();
        destination.Delete(true);
    }
}
