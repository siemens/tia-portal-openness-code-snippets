// © Siemens 2025
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using NUnit.Framework;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;
using TiaPortal.Openness.CodeSnippets.Plain.Setup;

namespace TiaPortal.Openness.CodeSnippets.Plain.Startdrive;

[TestFixture("Startdrive.zap21")]
public class NetworkSnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    [Test]
    public void ProfinetConfigurations()
    {
        var g120 =
            TiaPortalInstance.Projects.First().Devices.First(x => x.Name == "SINAMICS G120");

        var headModule = g120.DeviceItems.First(x => x.Classification == DeviceItemClassifications.HM);
        var profinetInterface = headModule.DeviceItems[0];
        var networkInterface = profinetInterface.GetService<NetworkInterface>();
        var node = networkInterface.Nodes.First();

        //Set IP Address 
        node.SetAttribute("Address", "192.168.0.2");

        //Set subnet mask
        node.SetAttribute("SubnetMask", "255.255.0.0");

        //Set PnDeviceName
        node.SetAttribute("PnDeviceNameAutoGeneration", false);
        node.SetAttribute("PnDeviceName", "name");
    }

    [Test]
    public void GetProfinetInterface_Drive()
    {
        var device = Project.Devices.First(x => x.Name == "S120Democase");

        var headModule = device.DeviceItems.First(x => x.Classification == DeviceItemClassifications.HM);
        var profinetInterface = headModule.DeviceItems.FirstOrDefault();

        Console.WriteLine($"Profinet Interface found: {profinetInterface != null}");
    }
}
