// © Siemens 2025 - 2026
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using NUnit.Framework;
using Siemens.Engineering.Library;
using TiaPortal.Openness.CodeSnippets.Plain.Setup;

namespace TiaPortal.Openness.CodeSnippets.Plain.Step7;

[TestFixture("Step7.zap21")]
public class LibrarySnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    [Test]
    public void CreateGlobalLibrary()
    {
        DirectoryInfo destination = new(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
        var globalLibrary = TiaPortalInstance.GlobalLibraries.Create<UserGlobalLibrary>(destination, "newLibrary");
        Console.WriteLine($"Global library could be created: {globalLibrary != null}");
    }
}
