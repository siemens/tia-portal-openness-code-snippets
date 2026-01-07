// © Siemens 2025
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using NUnit.Framework;
using Siemens.Engineering.SW;
using TiaPortal.Openness.CodeSnippets.WithExtensions.Setup;

namespace TiaPortal.Openness.CodeSnippets.Plain.Startdrive;

[TestFixture("Startdrive.zap21")]
public class TechnologyObjectSnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    [Test]
    public void ConnectToToTest()
    {
        var s120 = Project.Devices.First(x => x.Name == "S120Democase");
        var driveObject = s120.DriveAxes().First(x => x.Name == "BlueAxis").AsDriveObjectContainer()
            .DriveObject();

        var plc = Project.Devices.First(x => x.Name == "PLC_S120Democase");
        var plcSoftware = plc.DeviceItems[1].Software() as PlcSoftware;


        var to = plcSoftware?.TechnologicalObjectGroup.AllTechnologicalInstanceDBs()
            .Single(x => x.Name == "PositioningAxis_blue");

        driveObject.ConnectToTechnologyObject(to);
    }
}
