// © Siemens 2025
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using NUnit.Framework;
using Siemens.Engineering;
using Siemens.Engineering.Compiler;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;
using Siemens.Engineering.SW;
using Siemens.Engineering.SW.Blocks;
using TiaPortal.Openness.CodeSnippets.Plain.Setup;

namespace TiaPortal.Openness.CodeSnippets.Plain.Step7.PLC.ProgramBlocks;

[TestFixture("Step7.zap20")]
public class ProgramBlockSnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    [SetUp]
    public void Setup()
    {
        var softwareContainer = Project.Devices
            .First(x => x.Name.Equals("PLC_S120Democase"))
            .DeviceItems
            .First(x => x.Classification.Equals(DeviceItemClassifications.CPU)).GetService<SoftwareContainer>();

        if (softwareContainer.Software is null)
        {
            throw new InvalidOperationException(
                "The software container for device 'PLC_S120Democase' does not contain any software instance (null)."
            );
        }

        if (softwareContainer.Software is not PlcSoftware software)
        {
            throw new InvalidOperationException(
                $"The software container for device 'PLC_S120Democase' contains a software instance of type '{softwareContainer.Software.GetType().FullName}', but a PlcSoftware was expected."
            );
        }

        _blockComposition = software.BlockGroup.Blocks;
    }

    private PlcBlockComposition? _blockComposition;

    [Test]
    [TestCase(@"PLC\ProgramBlocks\Resources\PLCBlocks",
        new[] { "Block_1", "Block_2", "Block_3", "Block_4", "Block_5" })]
    public void ImportProgramBlockComposition(string relativeImportFolderPath, string[] editedBlockNamesArray)
    {
        var importFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativeImportFolderPath);
        HashSet<string> editedBlockNames = new(editedBlockNamesArray);
        List<PlcBlock> blocksToImport = [];

        if (_blockComposition == null)
        {
            Console.WriteLine("No Block composition");
            return;
        }

        blocksToImport.AddRange(_blockComposition.Where(plcBlock =>
            editedBlockNames.Contains(plcBlock.Name) && !plcBlock.IsKnowHowProtected));

        Parallel.ForEach(blocksToImport, plcBlock =>
        {
            var blockName = plcBlock.Name;
            var filePath = Path.Combine(importFolderPath, blockName);

            _blockComposition.Import(new FileInfo(filePath), ImportOptions.Override);
            editedBlockNames.Remove(blockName);
        });
    }

    [Test]
    [TestCase(@"PLC\ProgramBlocks\Resources\PLCBlocks",
        "Block_Merged")]
    public void ImportMergedProgramBlockComposition(string relativeImportFolderPath, string blockName)
    {
        var importFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativeImportFolderPath);
        var filePath = Path.Combine(importFolderPath, blockName);
        _blockComposition?.Import(new FileInfo(filePath), ImportOptions.Override);
    }

    [Test]
    public void ExportProgramBlock()
    {
        var softwareContainer = Project.Devices
            .First(x => x.Name.Equals("PLC_S120Democase"))
            .DeviceItems
            .First(x => x.Classification.Equals(DeviceItemClassifications.CPU)).GetService<SoftwareContainer>();

        var software = softwareContainer.Software as PlcSoftware;
        FileInfo fileInfo = new(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
        var block = software?.BlockGroup.Blocks.First(b => b.Name == "Axis_blue");
        var compiler = block?.GetService<ICompilable>();
        var result = compiler?.Compile();

        Console.WriteLine($"Compilation succeeded: {result?.ErrorCount == 0}");

        block?.Export(fileInfo, ExportOptions.None);

        Console.WriteLine($"File exists: {fileInfo.Exists}");

        fileInfo.Delete();
    }
}
