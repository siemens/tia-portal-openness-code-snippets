// © Siemens 2025
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using NUnit.Framework;
using Siemens.Engineering.MC.Drives;
using Siemens.Engineering.MC.Drives.DFI;
using TiaPortal.Openness.CodeSnippets.Plain.Setup;

namespace TiaPortal.Openness.CodeSnippets.Plain.Startdrive;

[TestFixture("Startdrive.zap20")]
public class EncoderSnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    //Function for projecting third party encoder configuration
    [Test]
    public void ProjectEncoderConfiguration()
    {
        var drive = Project.Devices.First(x => x.Name == "S120Democase");

        var driveObject = drive.DeviceItems.Single(x => x.Name == "AxisWith3rdPartyMotor")
            .GetService<DriveObjectContainer>()
            .DriveObjects.First();

        var hardwareProjection = driveObject.GetService<DriveFunctionInterface>().HardwareProjection;

        var encoderConfig = hardwareProjection.GetCurrentEncoderConfiguration(0);

        encoderConfig.RequiredConfigurationEntries.ToList().ForEach(ce =>
        {
            ce.Value = ce.Name switch
            {
                "p404.20" => true,
                "p404.21" => false,
                "p427" => 0,
                "p429.0" or "p429.2" or "p429.6" => true,
                "p428" or "p433" or "p434" or "p435" or "p436" or "p446" or "p447" or "p448" or "p449" => 30,
                _ => ce.Value
            };
        });
        hardwareProjection.ProjectEncoderConfiguration(encoderConfig, 1);
    }
}
