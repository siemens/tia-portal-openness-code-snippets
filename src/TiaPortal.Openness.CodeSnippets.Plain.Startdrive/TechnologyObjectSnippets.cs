// © Siemens 2025
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using NUnit.Framework;
using Siemens.Engineering;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;
using Siemens.Engineering.MC.Drives;
using Siemens.Engineering.SW;
using Siemens.Engineering.SW.TechnologicalObjects;
using Siemens.Engineering.SW.TechnologicalObjects.Motion;
using TiaPortal.Openness.CodeSnippets.Plain.Setup;

namespace TiaPortal.Openness.CodeSnippets.Plain.Startdrive;

[TestFixture("Startdrive.zap21")]
public class TechnologyObjectSnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    [Test]
    public void GetConnectedDriveObjectFromTo()
    {
        var plc = Project.Devices.First(x => x.Name == "PLC_S120Democase");
        var to = (plc.DeviceItems[1].GetService<SoftwareContainer>().Software as PlcSoftware)?.TechnologicalObjectGroup
            .TechnologicalObjects.First(x => x.Name == "PositioningAxis_blue");
        if (to == null)
        {
            Console.WriteLine("To not found");
            return;
        }

        var connectedDriveObject = GetConnectedDriveAxisFromToSmootherWay(to);
        Console.WriteLine(connectedDriveObject.Parent.Parent.GetAttribute("Name"));
    }

    private DriveObject? GetConnectedDriveAxisFromToSmootherWay(TechnologicalInstanceDB to)
    {
        var plc = GetParentDevice(to) ?? throw new InvalidOperationException("Parent device not found.");
        var inputAddress = (int)to.GetService<AxisHardwareConnectionProvider>().ActorInterface
            .GetAttribute("InputAddress") / 8;

        var cpu = plc.DeviceItems.Single(x => x.Classification == DeviceItemClassifications.CPU);
        var connectedIoSystems = GetAllConnectedIoSystems(cpu);
        var connectedIoDevices = connectedIoSystems.SelectMany(ioSystem => ioSystem.ConnectedIoDevices);

        var connectedSinamicsDevices = connectedIoDevices
            .Select(GetParentDevice)
            .Where(IsSinamics)
            .ToList();

        DriveObject? connectedDriveObject = null;

        foreach (var drive in connectedSinamicsDevices)
        {
            connectedDriveObject = GetConnectedDriveObject(drive, inputAddress);
            if (connectedDriveObject != null)
            {
                break;
            }
        }

        return connectedDriveObject;
    }

    private Device? GetParentDevice(IEngineeringObject engineeringObject)
    {
        while (engineeringObject != null && engineeringObject is not Device)
        {
            engineeringObject = engineeringObject.Parent;
        }
        
        return (Device?)engineeringObject;
    }

    private static IEnumerable<IoSystem> GetAllConnectedIoSystems(DeviceItem deviceItem)
    {
        var allNetworkInterfaces =
            deviceItem.DeviceItems.Select(x => x.GetService<NetworkInterface>()).Where(x => x != null);
        var ioSystems = allNetworkInterfaces.SelectMany(x => x.IoControllers).Select(x => x.IoSystem)
            .Where(x => x != null);
        return ioSystems;
    }

    public DriveObject? GetConnectedDriveObject(Device drive, int inputAddress)
    {
        var allDriveObjects = drive.DeviceItems.Select(x => x.GetService<DriveObjectContainer>()).Where(x => x != null)
            .Select(x => x.DriveObjects.First());

        return (from driveObject in allDriveObjects
            let telegrams = driveObject.Telegrams
            where telegrams.Any(telegram => (int)telegram.Addresses[0].GetAttribute("StartAddress") == inputAddress)
            select driveObject).FirstOrDefault();
    }

    public bool IsSinamics(Device device)
    {
        return device.DeviceItems.Any<DeviceItem>((Func<DeviceItem, bool>)(di =>
            di.GetService<DriveObjectContainer>() != null));
    }
}
