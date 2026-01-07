// © Siemens 2025 - 2026
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using NUnit.Framework;
using Siemens.Engineering.HW;
using TiaPortal.Openness.CodeSnippets.Plain.Setup;

namespace TiaPortal.Openness.CodeSnippets.Plain.Startdrive;

[TestFixture("Startdrive.zap21")]
public class StartdriveSnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    [Test]
    public void ShowHwEditorDevice()
    {
        var device = Project.Devices.First(x => x.Name == "S120Democase");
        device.ShowInEditor(View.Device);
    }

    [Test]
    public void GetInstalledStartdriveVersion()
    {
        var tiaPortalProcess = TiaPortalInstance.GetCurrentProcess();

        var installedStartdrive = tiaPortalProcess.InstalledSoftware.First(x => x.Name.Contains("Startdrive"));
        var startdriveVersion = installedStartdrive.Version;

        Console.WriteLine($"StartDrive Version: {startdriveVersion}");
    }
}
