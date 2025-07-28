﻿// © Siemens 2025
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using NUnit.Framework;
using Siemens.Engineering;

namespace TiaPortal.Openness.CodeSnippets.WithExtensions.Setup;

[SetUpFixture]
public abstract class BaseClass(string tiaArchiveName) : GlobalSetup
{
    private DirectoryInfo _tempProjectDirectory;

    public Siemens.Engineering.TiaPortal TiaPortalInstance { get; private set; }

    public Project Project { get; private set; }

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        var tiaProcesses = Siemens.Engineering.TiaPortal.GetProcesses();
        foreach (var tiaPortalProcess in tiaProcesses)
        {
            TiaPortalInstance = tiaPortalProcess.Attach();
            foreach (var project in TiaPortalInstance.Projects)
            {
                if (project.Name == tiaArchiveName.Remove(tiaArchiveName.IndexOf(".zap", StringComparison.Ordinal)))
                {
                    Project = project;
                    return;
                }
            }
        }

        TiaPortalInstance = new Siemens.Engineering.TiaPortal(TiaPortalMode.WithUserInterface);
        var resourceDict = Path.Combine(TestContext.CurrentContext.TestDirectory, "resources");
        var myStep7ProjectArchivePath = Path.Combine(resourceDict, tiaArchiveName);
        FileInfo sourcePath = new(myStep7ProjectArchivePath);

        _tempProjectDirectory = new DirectoryInfo(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
        Project = TiaPortalInstance.Projects.Retrieve(sourcePath, _tempProjectDirectory);
    }
}
