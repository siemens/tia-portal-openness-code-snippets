// © Siemens 2025
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using NUnit.Framework;
using TiaPortal.Openness.CodeSnippets.WithExtensions.Setup;

namespace TiaPortal.Openness.CodeSnippets.WithExtensions.Startdrive;

[TestFixture("Startdrive.zap21")]
public class ParameterSnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    [Test]
    public void GetParametersWithExtension()
    {
        var device = Project.AllStartdriveDevices().First(x => x.Name == "S120Democase");
        var driveAxes = device.DriveAxes();
        var parameters = driveAxes.First(x => x.Name.Contains("Red")).AsDriveObjectContainer().Parameters();

        Console.WriteLine("First 10 parameters:");
        Console.WriteLine(string.Join("\n", parameters.Take(10).Select(p => $"{p.Name}")));
    }

    [Test]
    public void GetParameterValues_InParallel_WithOptions()
    {
        var drive = Project.AllStartdriveDevices().First(x => x.Name == "S120Democase");
        var driveAxis = drive.DriveAxes().First();

        var parameterComposition = driveAxis.AsDriveObjectContainer().Parameters();
        var parameters = new List<(string name, object value)>();

        var options = new ParallelOptions { MaxDegreeOfParallelism = 10 };

        Parallel.ForEach(parameterComposition, options, parameter =>
        {
            parameters.Add((parameter.Name, parameter.Value));
        });

        Console.WriteLine("First 10 parameters:");
        Console.WriteLine(string.Join("\n", parameters.Take(10).Select(p => $"{p.name}")));
    }
}
