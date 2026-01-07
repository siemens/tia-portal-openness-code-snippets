// © Siemens 2025 - 2026
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using NUnit.Framework;
using Siemens.Engineering.MC.Drives;
using TiaPortal.Openness.CodeSnippets.Plain.Setup;

namespace TiaPortal.Openness.CodeSnippets.Plain.Startdrive;

[TestFixture("Startdrive.zap21")]
public class ParameterSnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    [Test]
    public void WritingBiCoParameters()
    {
        var device = Project.Devices.First(x => x.Name == "S120Democase");

        var redAxis = device.DeviceItems.Single(n => n.Name == "RedAxis");
        var controlUnit = device.DeviceItems[0];

        var bicoParameter = redAxis.GetService<DriveObjectContainer>().DriveObjects.First().Parameters
            .Find(840, 0);
        var parameterBits = controlUnit.GetService<DriveObjectContainer>().DriveObjects.First().Parameters
            .Find(2139, -1).Bits;

        //Code to find parameter 2139.7
        //Code that will get the wrong parameter:
        var incorrectParameter = parameterBits[7];
        Console.WriteLine(
            $"Lookup with index 'parameterBits[7]' results in incorrect parameter: {incorrectParameter.Name}");

        //Better:
        var connectedParameter = parameterBits.First(x => x.Name.Substring(x.Name.IndexOf('.') + 1) == "7");
        Console.WriteLine($"Lookup by name results in parameter as expected: {connectedParameter.Name}");

        bicoParameter.Value = connectedParameter;
    }

    [Test]
    public void GetIndexParameter()
    {
        var drive = Project.Devices.First(x => x.Name == "S120Democase");
        var driveObject = drive.DeviceItems.First().GetService<DriveObjectContainer>().DriveObjects.First();

        var parameters = driveObject.Parameters;

        var parameterViaIndexAccess = parameters[100];
        Console.WriteLine(parameterViaIndexAccess.Name);
        var parameterViaStringAccess = parameters.Find("r945[2]");
        Console.WriteLine($"{parameterViaStringAccess.Name}={parameterViaStringAccess.Value}");
    }
}
