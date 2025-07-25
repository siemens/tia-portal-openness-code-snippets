// © Siemens 2025
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using NUnit.Framework;
using Siemens.Engineering.MC.Drives;
using Siemens.Engineering.MC.Drives.DFI;
using TiaPortal.Openness.CodeSnippets.Plain.Setup;

namespace TiaPortal.Openness.CodeSnippets.Plain.Startdrive;

[TestFixture("Startdrive.zap20")]
public class SafetySnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    [Test]
    public void SafetyIntegratedCalculateChecksumForNextGeneration()
    {
        var device = Project.Devices.First(x => x.Name == "SINAMICS S210V61");
        var driveControl = device.DeviceItems.First();

        var driveObject = driveControl.GetService<DriveObjectContainer>().DriveObjects.First();
        var driveFunctionInterface = driveObject.GetService<DriveFunctionInterface>();
        var succeeded = driveFunctionInterface.SafetyCommissioning.UpdateCheckSums();

        Console.WriteLine($"CalculateCheckSum succeeded: {succeeded}");
    }

    [Test]
    public void SafetyIntegratedExampleNewGeneration_ActivateSTO()
    {
        var device = Project.Devices.First(x => x.Name == "SINAMICS S210V61");
        var driveControl = device.DeviceItems.First();

        var driveObject = driveControl.GetService<DriveObjectContainer>().DriveObjects.First();
        var parameters = driveObject.Parameters;

        //Enable Safety via ProfiSafe
        parameters.Find("p9603").Bits.Single(x => x.Name == "p9603.1").Value = 1;

        //Enable STO
        parameters.Find("p9604").Bits.Single(x => x.Name == "p9604.0").Value = 1;

        var driveFunctionInterface = driveObject.GetService<DriveFunctionInterface>();
        var succeeded = driveFunctionInterface.SafetyCommissioning.UpdateCheckSums();

        Console.WriteLine($"CalculateCheckSum succeeded: {succeeded}");
    }

    [Test]
    public void SafetyIntegratedExampleS120_ActivateSTO()
    {
        var device = Project.Devices.First(x => x.Name == "S120Democase");
        var axis = device.DeviceItems.First(x => x.Name == "RedAxis");

        var driveObject = axis.GetService<DriveObjectContainer>().DriveObjects.First();
        var parameters = driveObject.Parameters;

        //Enable BasicSafety via ProfiSafe
        parameters.Find("p9601").Bits.Single(x => x.Name == "p9601.3").Value = 1;
    }
}
