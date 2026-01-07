// © Siemens 2025
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using NUnit.Framework;
using Siemens.Engineering;
using TiaPortal.Openness.CodeSnippets.Plain.Setup;

namespace TiaPortal.Openness.CodeSnippets.Plain.Step7;

[TestFixture("Step7.zap21")]
public class TiaProcessSnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    [Test]
    public void StartTiaTest()
    {
        // Start a new TIA Portal
        var tia = new Siemens.Engineering.TiaPortal(TiaPortalMode.WithUserInterface);
        // Wait 2 seconds
        Thread.Sleep(2000);
        // Close TIA Portal
        tia.GetCurrentProcess().Dispose();
    }

    [Test]
    public void StartTiaAndOpenProjectTest()
    {
        Siemens.Engineering.TiaPortal tia = new(TiaPortalMode.WithUserInterface);
        FileInfo fileInfo = new("C:/MyPathToTheTiaProject.ap19"); //TODO modify the path as needed

        tia.Projects.Open(fileInfo);

        tia.GetCurrentProcess().Dispose();
    }

    [Test]
    public void StartTiaAndRetrieveProjectTest()
    {
        // Start a new TIA Portal
        Siemens.Engineering.TiaPortal tia = new(TiaPortalMode.WithUserInterface);
        var myStep7ProjectArchivePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources", "Step7.zap21");
        FileInfo sourcePath = new(myStep7ProjectArchivePath);
        DirectoryInfo targetPath = new(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));

        var tiaProject = tia.Projects.Retrieve(sourcePath, targetPath);
        Console.WriteLine($"Tia Project retrieved {tiaProject != null}");
        tiaProject?.Close();
        tia.GetCurrentProcess().Dispose();
        targetPath.Delete(true);
    }

    [Test]
    public void ArchiveProjectTest()
    {
        var project = Project;
        project.Save();
        project.Archive(new DirectoryInfo(Path.GetTempPath()), "dummy.zap21",
            ProjectArchivationMode.Compressed);

        var fileInfo = new FileInfo(Path.Combine(Path.GetTempPath(), "dummy.zap21"));
        if (fileInfo.Exists)
        {
            fileInfo.Delete();
        }
    }

    [Test]
    public void ExclusiveAccess()
    {
        using var exclusiveAccess = TiaPortalInstance.ExclusiveAccess("Test");
        exclusiveAccess.Text = "Start of the program";

        //...

        exclusiveAccess.Text = "End of the program";
    }

    [Test]
    public void Transaction()
    {
        using var exclusiveAccess = TiaPortalInstance.ExclusiveAccess("Test");

        using var transaction = exclusiveAccess.Transaction(Project, "Test");
        var i = 1;

        do
        {
            i++;
        } while (i < 10000);

        exclusiveAccess.Text = "Test 123";

        transaction.CommitOnDispose();
    }
}
