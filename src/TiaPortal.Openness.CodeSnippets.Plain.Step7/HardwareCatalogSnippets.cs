// © Siemens 2025
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using NUnit.Framework;
using TiaPortal.Openness.CodeSnippets.Plain.Setup;

namespace TiaPortal.Openness.CodeSnippets.Plain.Step7;

[TestFixture("Step7.zap21")]
public class HardwareCatalogSnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    [Test]
    public void HardwareCatalog_AllEntries()
    {
        var hardwareCatalog = TiaPortalInstance.HardwareCatalog.Find(string.Empty);

        var catalogEntries = hardwareCatalog.ToList();

        Console.WriteLine($"Total catalog entries: {catalogEntries.Count}");

        foreach (var entry in catalogEntries)
        {
            Console.WriteLine($"TypeIdentifier: {entry.TypeIdentifier}, CatalogPath: {entry.CatalogPath}");
        }
    }
}
