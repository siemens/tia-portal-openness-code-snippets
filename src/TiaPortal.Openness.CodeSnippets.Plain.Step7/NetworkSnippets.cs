// © Siemens 2025
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using NUnit.Framework;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;
using TiaPortal.Openness.CodeSnippets.Plain.Setup;

namespace TiaPortal.Openness.CodeSnippets.Plain.Step7;

[TestFixture("Step7.zap20")]
public class NetworkSnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    [Test]
    public void PLC_Et200Connection()
    {
        var project = Project;

        var plc = project.Devices.CreateWithItem("OrderNumber:6ES7 515-2UM01-0AB0/V2.9", "", "");
        var et200 =
            project.UngroupedDevicesGroup.Devices.CreateWithItem("OrderNumber:6ES7 155-6AU00-0CN0/V3.0", "", "");

        //Get NetworkInterface for plc and et200 (here the first profinet interface)
        var networkInterfacePlc = plc.DeviceItems
            .First(x => x.Classification == DeviceItemClassifications.CPU)
            .DeviceItems
            .First(x => x.Name.Contains("PROFINET")).GetService<NetworkInterface>();

        var networkInterfaceEt200 = et200.DeviceItems
            .First(x => x.Classification == DeviceItemClassifications.HM)
            .DeviceItems.First(x => x.Name.Contains("PROFINET")).GetService<NetworkInterface>();

        //Get the nodes for connection of the subnet
        var et200Node = networkInterfaceEt200.Nodes.First();
        var plcNode = networkInterfacePlc.Nodes.First();

        //Code if profinet interface from plc is already connected to subnet
        var plcSubnet = plcNode.ConnectedSubnet;

        //If not connected, create subnet
        plcSubnet ??= plcNode.CreateAndConnectToSubnet("TestSubnet");

        //ConnectToASubnet
        et200Node.ConnectToSubnet(plcSubnet);


        //Get IoConnector for connection of the io system
        var ioConnectorEt200 = networkInterfaceEt200.IoConnectors.First();

        //Code if profinet interface from plc is already connected to ioSystem
        //If not connected, create ioSystem
        var plcIoSystem = networkInterfacePlc.IoControllers.First().IoSystem ??
                          networkInterfacePlc.IoControllers.First().CreateIoSystem("TestIOSystem");

        //Connect ioSystem from et200
        ioConnectorEt200.ConnectToIoSystem(plcIoSystem);
    }

    [Test]
    public void GetProfinetInterface_PLC()
    {
        var device = Project.Devices.First(x => x.Name == "PLC_S120Democase");

        var cpu = device.DeviceItems.First(x => x.Classification == DeviceItemClassifications.CPU);
        var profinetInterfaces = cpu.DeviceItems
            .Where(d => d.GetAttributeInfos().Select(x => x.Name).Contains("PnSendClock")).ToList();

        Console.WriteLine($"Found {profinetInterfaces.Count} Profinet Interfaces");
    }
}
