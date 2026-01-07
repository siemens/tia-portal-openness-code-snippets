// © Siemens 2025
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using NUnit.Framework;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;
using Siemens.Engineering.SW;
using TiaPortal.Openness.CodeSnippets.Plain.Setup;

namespace TiaPortal.Openness.CodeSnippets.Plain.Step7.PLC;

[TestFixture("Step7.zap21")]
public class TechnologyObjectSnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    [Test]
    public void CreateTechnologyObject()
    {
        var plcDevice = Project.Devices.First(x => x.Name == "PLC_S120Democase");
        var plcSoftware = plcDevice.DeviceItems[1].GetService<SoftwareContainer>().Software as PlcSoftware;

        const string NameOfTo = "ABC";
        const string TypeOfTo = "TO_SpeedAxis";
        Version versionOfTo = new(6, 0);

        var toCreated = plcSoftware?.TechnologicalObjectGroup.TechnologicalObjects.Create(NameOfTo,
            TypeOfTo,
            versionOfTo);

        Console.WriteLine($"To created: {toCreated != null}");
    }

    [Test]
    public void SetTechnologyObjectHwLimits()
    {
        var plc = Project.Devices.First(x => x.Name == "PLC_S120Democase");
        var cpu = plc.DeviceItems.Single(x => x.Classification == DeviceItemClassifications.CPU);

        var plcSoftware = cpu.GetService<SoftwareContainer>().Software as PlcSoftware;
        var to =
            plcSoftware?.TechnologicalObjectGroup.TechnologicalObjects.First(x => x.Name == "PositioningAxis_blue");

        if (to == null)
        {
            Console.WriteLine("to could not be found");
            return;
        }

        to.Parameters.Find("PositionLimits_HW.Active").Value = true;

        var connectedTag = plcSoftware?.TagTableGroup.TagTables.First(x => x.Name.ToLower().Contains("default")).Tags
            .Find("test");

        to.Parameters.Find("_PositionLimits_HW.MinSwitchAddress").Value = connectedTag;
    }

    [Test]
    public void WriteParametersInTechnologyObject()
    {
        var plcDevice = Project.Devices.First(x => x.Name == "PLC_S120Democase");

        var plcSoftware = plcDevice.DeviceItems[1].GetService<SoftwareContainer>().Software as PlcSoftware;

        var technologyObject =
            plcSoftware?.TechnologicalObjectGroup.TechnologicalObjects.First(x => x.Name == "PositioningAxis_blue");

        if (technologyObject == null)
        {
            Console.WriteLine("To not found");
            return;
        }

        technologyObject.Parameters.Find("Actor.InverseDirection").Value = true;
    }
}
