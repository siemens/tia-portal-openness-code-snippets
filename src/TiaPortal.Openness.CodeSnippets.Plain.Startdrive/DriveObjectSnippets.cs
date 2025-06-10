// © Siemens 2025
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using NUnit.Framework;
using Siemens.Engineering.MC.Drives;
using Siemens.Engineering.MC.Drives.DFI;
using Siemens.Engineering.MC.Drives.Enums;
using TiaPortal.Openness.CodeSnippets.Plain.Setup;

namespace TiaPortal.Openness.CodeSnippets.Plain.Startdrive;

[TestFixture("Startdrive.zap20")]
public class DriveObjectSnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    [Test]
    public void DriveObjectType()
    {
        var s120 = Project.Devices.First(x => x.Name == "S120Democase");

        var driveFunctionInterface = s120.DeviceItems[3].GetService<DriveObjectContainer>().DriveObjects.First()
            .GetService<DriveFunctionInterface>();

        var driveObjectTypeHandler = driveFunctionInterface.DriveObjectFunctions.DriveObjectTypeHandler;
        var currentType = driveObjectTypeHandler.CurrentDriveObjectType;

        var possibleDriveObjectTypesList = driveObjectTypeHandler.PossibleDriveObjectTypes.Select(x => x.Name).ToList();
        var typeFromPossibleTypes = possibleDriveObjectTypesList.Single(x => x == currentType.Name);
        var typeIndex = possibleDriveObjectTypesList.IndexOf(typeFromPossibleTypes);
        Console.WriteLine(typeIndex);
    }

    [Test]
    public void DeactivateDriveObjectTest()
    {
        var drive = Project.Devices.First(x => x.Name == "S120Democase");

        var driveObject = drive.DeviceItems.Single(x => x.Name == "BlueAxis").GetService<DriveObjectContainer>()
            .DriveObjects.First();

        var dfi = driveObject.GetService<DriveFunctionInterface>();
        dfi.DriveObjectFunctions.DriveObjectActivation.ChangeActivationState(DriveObjectActivationState.Deactivate);
    }
}
