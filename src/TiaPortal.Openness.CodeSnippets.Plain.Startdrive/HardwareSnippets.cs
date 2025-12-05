// © Siemens 2025
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using System.Security.Cryptography.X509Certificates;
using NUnit.Framework;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;
using Siemens.Engineering.MC.Drives;
using Siemens.Engineering.MC.Drives.DFI;
using TiaPortal.Openness.CodeSnippets.Plain.Setup;

namespace TiaPortal.Openness.CodeSnippets.Plain.Startdrive;

[TestFixture("Startdrive.zap20")]
public class HardwareSnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    [Test]
    public void SetDriveCliqConnections()
    {
        var device = Project.Devices.First(x => x.Name == "S120Democase");
        var driveAxis = device.DeviceItems.Single(n => n.Name == "BlueAxis");

        var driveCliqInterface = driveAxis.DeviceItems.First().DeviceItems.First();
        var axisPorts = driveCliqInterface.DeviceItems.Select(x => x.GetService<NetworkPort>()).ToList();

        foreach (var port in axisPorts)
        {
            var connectedPort = port.ConnectedPorts.FirstOrDefault();
            if (connectedPort != null)
            {
                port.DisconnectFromPort(connectedPort);
            }
        }

        var firstAxisPort = axisPorts.First();
        var firstCuPort = device.DeviceItems[1].DeviceItems[2].DeviceItems.First();

        var firstCuNetworkPort = firstCuPort.GetService<NetworkPort>();
        firstAxisPort.ConnectToPort(firstCuNetworkPort);
    }

    [Test]
    public void PlugMotorModuleAndMotor_S120()
    {
        var device = Project.Devices.First(x => x.Name == "S120Democase");
        var motorModule = device.PlugNew("OrderNumber:6SL3120-1TE15-0Axx//10002", "MotorModule", 65535);

        var driveAxis = motorModule.Parent as DeviceItem;

        var motor = driveAxis?.PlugNew("OrderNumber:1FK7011-xAK21-xTGx", "", 65535);

        Console.WriteLine($"Motor could be created: {motor != null}");
    }

    [Test]
    public void CreateAndChangeMotor_S210()
    {
        // Create S210 Device
        var s210TypeIdentifier = "OrderNumber:6SL3210-5HB10-1xFx/V5.2.3/S210";
        var device = Project.Devices.CreateWithItem(s210TypeIdentifier, "S210_New", "S210_NewDevice");


        // Remove existing "dummy" synchronous motor
        var rack = device.DeviceItems.Single(x => x.TypeIdentifier.ToLower().Contains("system:rack"));
        var existingMotor = rack.DeviceItems.Single(x => x.TypeIdentifier.ToLower().Contains("xxxxx-xxxx"));
        existingMotor.Delete();

        // Plug new Motor
        var motorTypeIdentifier = "OrderNumber:1FK2102-0AG1x-xSxx";
        var newMotor = rack.PlugNew(motorTypeIdentifier, "NewMotor", 65535);
    }

    [Test]
    public void G115DModuleCreationWallMount()
    {
        const string TypeIdentifier = "OrderNumber:6SL3500-0XE50-7FA_/-";

        var g115d = Project.Devices.CreateWithItem("OrderNumber:6SL3500-xxxxx-xFxx/4.7.14", "", "");
        var deviceItem = g115d.DeviceItems.Single(x => x.Classification == DeviceItemClassifications.HM)
            .PlugNew(TypeIdentifier, "", 3);

        Console.WriteLine($"DeviceItem could be created: {deviceItem != null}");
    }


    [Test]
    public void G115DModuleCreation_MotorMountWith_2KJ8()
    {
        const string TypeIdentifier = "OrderNumber:6SL3500-0XE50-7FA_/-";

        var g115d = Project.Devices.CreateWithItem("OrderNumber:6SL3500-xxxxx-xFxx/4.7.14", "", "");

        var driveObject = g115d.DeviceItems.Single(x => x.Classification == DeviceItemClassifications.HM)
            .GetService<DriveObjectContainer>().DriveObjects.First();
        var dfi = driveObject.GetService<DriveFunctionInterface>();

        dfi.Commissioning.SetSimoGearMlfb("2KJ8001-2EA10-3FG1-D0X");
    }
}
