// © Siemens 2025
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using NUnit.Framework;
using Siemens.Engineering.MC.Drives;
using Siemens.Engineering.MC.Drives.Dcc;
using TiaPortal.Openness.CodeSnippets.Plain.Setup;

namespace TiaPortal.Openness.CodeSnippets.Plain.StartDrive.Dcc;

[TestFixture("Startdrive.zap21")]
public class DccSnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    [Test]
    public void GetInstalledDccVersion()
    {
        var tiaPortalProcess = TiaPortalInstance.GetCurrentProcess();

        var installedStartdrive = tiaPortalProcess.InstalledSoftware.First(x => x.Name.Contains("Startdrive"));
        var installedDcc = installedStartdrive.Options.First(x => x.Name.Contains("DCC"));

        var dccVersion = installedDcc.Version;
        Console.WriteLine($"DCC Version: {dccVersion}");
        var majorVersion = int.Parse(dccVersion.Substring(1, 2));
        Console.WriteLine($"DCC Major Version: {majorVersion}");

        const string UpdateString = "Update";
        if (!dccVersion.Contains(UpdateString))
        {
            return;
        }

        var index = dccVersion.IndexOf(UpdateString, StringComparison.Ordinal) + UpdateString.Length;
        var dccUpdateVersion = dccVersion.Substring(index, 2);
        Console.WriteLine($"DCC Update Version: {dccUpdateVersion}");
    }

    [Test]
    public void DccImportExport()
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var importDccPath =
            new FileInfo(Path.Combine(baseDirectory, "resources", "DCC_Example.dcc"));
        var exportDccPath =
            new FileInfo(Path.Combine(baseDirectory, "resources",
                "DCC_Example_exported.dcc"));
        var device = Project.Devices.First(x => x.Name.Contains("S120Democase"));

        //First we import a specific chart
        var dccCharts = device.DeviceItems.First(x => x.Name.Contains("BlueAxis")).GetService<DriveObjectContainer>()
            .DriveObjects
            .First().GetService<DriveControlChartContainer>().Charts;
        dccCharts.Import(importDccPath.ToString(), DccImportOptions.RenameOnConflict);

        //Export the charts
        Console.WriteLine(string.Join("\n", dccCharts.Select(n => n.Name)));
        dccCharts.Single(c => c.Name == "DCC_Example").Export(exportDccPath.FullName);

        Console.WriteLine($"Exported dcc plan exists: {exportDccPath.Exists}");
    }
}
