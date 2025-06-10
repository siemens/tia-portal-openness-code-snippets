// © Siemens 2025
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using NUnit.Framework;
using Siemens.Engineering.MC.Drives;
using Siemens.Engineering.MC.Drives.DFI;
using TiaPortal.Openness.CodeSnippets.Plain.Setup;

namespace TiaPortal.Openness.CodeSnippets.Plain.Startdrive;

[TestFixture("Startdrive.zap20")]
public class MotorSnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    //Function for projecting third party motor configuration
    [Test]
    public void ProjectMotorConfiguration()
    {
        var drive = Project.Devices.First(x => x.Name == "S120Democase");

        var driveObject = drive.DeviceItems.Single(n => n.Name == "AxisWith3rdPartyMotor")
            .GetService<DriveObjectContainer>()
            .DriveObjects.First();

        var hardwareProjection = driveObject.GetService<DriveFunctionInterface>().HardwareProjection;
        var motorConfiguration = hardwareProjection.GetCurrentMotorConfiguration(0);

        motorConfiguration.SetEquivalentCircuitDiagramData(false);

        motorConfiguration.RequiredConfigurationEntries.ToList().ForEach(ce =>
        {
            ce.Value = ce.Number switch
            {
                305 => 1.0,
                311 => 1.0,
                314 => 1.0,
                316 => 1.0,
                322 => 1.0,
                323 => 1.0,
                _ => ce.Value
            };
        });
        motorConfiguration.OptionalConfigurationEntries.ToList().ForEach(ce =>
        {
            ce.Value = ce.Number switch
            {
                317 => 1.0,
                348 => 1.1,
                _ => ce.Value
            };
        });

        hardwareProjection.ProjectMotorConfiguration(motorConfiguration, 0);
    }
}
