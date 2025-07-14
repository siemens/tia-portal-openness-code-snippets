// © Siemens 2025
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using System.Security;
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

    // _blockComposition is initialized in the Setup method.
    // The Setup method will be run before each test method.
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

            // _blockComposition is initialized in the Setup method.
            // The Setup method will be run before each test method.
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

        // _blockComposition is initialized in the Setup method.
        // The Setup method will be run before each test method.
        _blockComposition?.Import(new FileInfo(filePath), ImportOptions.Override);
    }

    [Test]
    public void ExportProgramBlock()
    {
        FileInfo fileInfo = new(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));

        // _blockComposition is initialized in the Setup method.
        // The Setup method will be run before each test method.
        var block = _blockComposition.First(b => b.Name == "Axis_blue");
        var compiler = block?.GetService<ICompilable>();
        var result = compiler?.Compile();

        Console.WriteLine($"Compilation succeeded: {result?.ErrorCount == 0}");

        block?.Export(fileInfo, ExportOptions.None);

        Console.WriteLine($"File exists: {fileInfo.Exists}");

        fileInfo.Delete();
    }

    [Test]
    public void ProtectPlcBlock()
    {
        // _blockComposition is initialized in the Setup method.
        // The Setup method will be run before each test method.
        var block = _blockComposition?.First(b => b.Name == "Axis_blue");
        var plcBlockProtectionProvider = block?.GetService<PlcBlockProtectionProvider>();

        // Get invalid password characters
        var invalidChars = plcBlockProtectionProvider?.GetInvalidPasswordCharacters().ToList();
        var validPassword = "@ValidPassword123";

        var passwordIsValid = ValidatePassword(validPassword, invalidChars);
        if (passwordIsValid)
        {
            var secureValidPassword = CreateSecureString(validPassword);
            plcBlockProtectionProvider?.Protect(secureValidPassword);
        }
        else
        {
            Console.WriteLine("Password is invalid");
        }
    }

    private static SecureString CreateSecureString(string password)
    {
        var securePassword = new SecureString();
        foreach (var c in password)
        {
            securePassword.AppendChar(c);
        }
        securePassword.MakeReadOnly();
        return securePassword;
    }

    private static bool ValidatePassword(string password, List<char> invalidChars)
    {
        //The password requirements:
        //    The password must be at least 8 characters long.
        //    The password must not exceed the maximum permissible length of 120 characters.
        //    The password must contain at least one number.
        //    The password must contain at least one special character.
        //    The password must contain at least one lower-case and one upper -case character.

        // Check for invalid characters
        if (password.Any(invalidChars.Contains))
            return false;

        // Check length requirements
        if (password.Length < 8 || password.Length > 120)
            return false;

        // Check for at least one number
        if (!password.Any(char.IsDigit))
            return false;

        // Check for at least one lowercase character
        if (!password.Any(char.IsLower))
            return false;

        // Check for at least one uppercase character
        if (!password.Any(char.IsUpper))
            return false;

        // Check for at least one special character (non-alphanumeric)
        if (password.All(char.IsLetterOrDigit))
            return false;

        return true;
    }
}
