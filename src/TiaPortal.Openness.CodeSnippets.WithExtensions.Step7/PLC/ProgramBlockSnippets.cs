// © Siemens 2025
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using NUnit.Framework;
using Siemens.Engineering.SW.Blocks;
using Siemens.Engineering.SW.ExternalSources;
using TiaPortal.Openness.CodeSnippets.WithExtensions.Setup;

namespace TiaPortal.Openness.CodeSnippets.WithExtensions.Step7.PLC;

[TestFixture("Step7.zap20")]
public class ProgramBlockSnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    [Test]
    public void SclExportTest()
    {
        var software = Project.Devices.First(x => x.Name.Equals("PLC_S120Democase")).AsPlc();

        FileInfo fileInfo = new(Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid().ToString()}.scl"));

        var exportableBlocks = software.BlockGroup.AllFBs().Where(b =>
            !b.IsKnowHowProtected && b.ProgrammingLanguage is ProgrammingLanguage.SCL or ProgrammingLanguage.STL);

        software.ExternalSourceGroup.GenerateSource(exportableBlocks, fileInfo, GenerateOptions.None);

        Console.WriteLine($"File exists: {fileInfo.Exists}");
        fileInfo.Delete();
    }
}
