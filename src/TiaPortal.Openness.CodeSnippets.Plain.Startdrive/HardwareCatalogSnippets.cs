// © Siemens 2025 - 2026
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using System.Text;
using NUnit.Framework;
using Siemens.Engineering.HW;
using TiaPortal.Openness.CodeSnippets.Plain.Setup;

namespace TiaPortal.Openness.CodeSnippets.Plain.Step7;

[TestFixture("Startdrive.zap21")]
public class HardwareCatalogSnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    [Test]
    public void HardwareCatalog_AllEntries()
    {
        var hardwareCatalog = TiaPortalInstance.HardwareCatalog.Find(string.Empty);

        var output = new StringBuilder();
        output.AppendLine($"Total catalog entries: {hardwareCatalog.Count}");

        var entries = string.Join(Environment.NewLine,
            hardwareCatalog.Select(entry =>
                $"TypeIdentifier: {entry.TypeIdentifier}, CatalogPath: {entry.CatalogPath}"));

        output.AppendLine(entries);
        Console.WriteLine(output.ToString());
    }

    [Test]
    public void HardwareCatalog_AllEntriesForS120()
    {
        var hardwareCatalog = TiaPortalInstance.HardwareCatalog.Find("S120");

        var catalogEntries = hardwareCatalog.ToList();

        Console.WriteLine($"Total S120 catalog entries: {catalogEntries.Count}");

        foreach (var entry in catalogEntries)
        {
            Console.WriteLine($"TypeIdentifier: {entry.TypeIdentifier}, CatalogPath: {entry.CatalogPath}");
        }
    }

    [Test]
    public void HardwareCatalog_DriveCliqMotorTypeIdentifier()
    {
        var hardwareCatalog = TiaPortalInstance.HardwareCatalog;

        var motorEntries = hardwareCatalog.Find("motors")
            .Where(x => x.CatalogPath != null && x.CatalogPath.ToLower().Contains("cliq"))
            .ToList();

        if (motorEntries.Count == 0)
        {
            Console.WriteLine("No DriveCliq motor entries found in the hardware catalog.");
        }

        foreach (var entry in motorEntries)
        {
            Console.WriteLine($"TypeIdentifier: {entry.TypeIdentifier}, CatalogPath: {entry.CatalogPath}");
        }
    }

    [Test]
    public void CreateMasterCopy()
    {
        var masterCopiesFolder = Project.ProjectLibrary.MasterCopyFolder;

        var device = Project.Devices.First(x =>
            x.DeviceItems.SingleOrDefault(n => n.Classification == DeviceItemClassifications.HM).Name.Contains("S120"));

        var masterCopy = masterCopiesFolder.MasterCopies.Create(device);
        device.Delete();

        var newDevice = Project.Devices.CreateFrom(masterCopy);
        Console.WriteLine($"Device could be created: {newDevice != null}");
    }
}
