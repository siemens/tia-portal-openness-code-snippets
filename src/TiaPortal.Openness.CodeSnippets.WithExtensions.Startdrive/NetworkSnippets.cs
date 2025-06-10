// © Siemens 2025
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using NUnit.Framework;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;
using TiaPortal.Openness.CodeSnippets.WithExtensions.Setup;

namespace TiaPortal.Openness.CodeSnippets.Plain.Startdrive;

[TestFixture("Startdrive.zap20")]
public class NetworkSnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    [Test]
    public void IpAddressAndOnlineConfig()
    {
        var device = Project.AllStartdriveDevices().First(x => x.Name.Equals("S120Democase"));

        //Set Ip Address:
        var node = device.NetworkInterfaces().Single(n =>
            n.InterfaceType == NetType.Ethernet && n.TryGetAttribute("TypeName", out string typeName) &&
            typeName == "PROFINET interface").Nodes.First();
        node.SetAttribute("Address", "192.168.0.15");

        //Change PG/PC interface:
        var onlineProvider = device.HeadModule().AsOnlineProvider();

        var pn = onlineProvider.Configuration.Modes.First();
        var pcInterfaces = pn.PcInterfaces;

        var chosenPcInterface = pcInterfaces.First(); //i => i.Name.Contains("Intel(R) Ethernet"));
        var chosenTargetInterface =
            chosenPcInterface.TargetInterfaces.Single(x => x.Name == "CU X127");

        onlineProvider.Configuration.ApplyConfiguration(chosenTargetInterface);
    }

    [Test]
    public void PLC_S120Connection()
    {
        var project = Project;

        var plc = project.Devices.CreateWithItem("OrderNumber:6ES7 515-2UM01-0AB0/V2.9", "", "");
        var s120 = project.Devices.CreateWithItem("OrderNumber:6SL3040-1MA01-0Axx/V5.2.3/S120", "", "");

        //Get NetworkInterface for plc and s120 (here the first profinet interface)
        var networkInterfacePlc = plc.DeviceItems.First(x => x.Classification == DeviceItemClassifications.CPU)
            .ProfinetInterfaces().First().GetService<NetworkInterface>();

        var networkInterfaces120 = s120.HeadModule().ProfinetInterfaces().First().GetService<NetworkInterface>();

        //Get the nodes for connection of the subnet
        var s120Node = networkInterfaces120.Nodes.First();
        var plcNode = networkInterfacePlc.Nodes.First();

        //Code if profinet interface from plc is already connected to subnet
        var plcSubnet = plcNode.ConnectedSubnet;

        //If not connected, create subnet
        plcSubnet ??= plcNode.CreateAndConnectToSubnet("TestSubnet");

        //ConnectToASubnet
        s120Node.ConnectToSubnet(plcSubnet);


        //Get IoConnector for connection of the io system
        var ioConnectors120 = networkInterfaces120.IoConnectors.First();

        //Code if profinet interface from plc is already connected to ioSystem
        //If not connected, create ioSystem
        var plcIoSystem = networkInterfacePlc.IoControllers.First().IoSystem ??
                          networkInterfacePlc.IoControllers.First().CreateIoSystem("TestIOSystem");

        //Connect ioSystem from s120
        ioConnectors120.ConnectToIoSystem(plcIoSystem);
    }
}
