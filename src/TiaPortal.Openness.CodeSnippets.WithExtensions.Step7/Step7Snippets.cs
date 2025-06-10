// © Siemens 2025
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using NUnit.Framework;
using Siemens.Collaboration.Net;
using TiaPortal.Openness.CodeSnippets.WithExtensions.Setup;
using Mode = Siemens.Collaboration.Net.TiaPortal.Openness.Resolver.Mode;

namespace TiaPortal.Openness.CodeSnippets.WithExtensions.Step7;

[TestFixture("Step7.zap20")]
public class Step7Snippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    [Test]
    public void GetAllInstalledTiaVersions()
    {
        var apiFolders = Api.Global.Openness().GetPublicApiFoldersFromRegistry();
        var installedVersions = apiFolders
            .Select(folder =>
            {
                var parentName = folder?.Parent?.Name.ToString();
                if (string.IsNullOrWhiteSpace(parentName))
                {
                    return null;
                }

                var nameParts = parentName?.Split(' ');
                return nameParts is { Length: > 0 } ? nameParts.Last() : null;
            })
            .Where(version => !string.IsNullOrWhiteSpace(version))
            .Distinct()
            .ToList();

        // Optionally output for verification
        installedVersions.ForEach(version => Console.WriteLine($"Installed TIA Version: {version}"));
    }

    [Test]
    public void GetTiaPortalProcesses()
    {
        const int SupportedVersion = 18;
        var tiaProcesses = Api.Global.Openness()
            .GetTiaPortalProcesses(Mode.WithUserInterface)
            .Where(p => p.Version.Major >= SupportedVersion).ToList();

        //Here you need a user interaction to choose from the processes
        var chosenProcess = tiaProcesses.First();

        Api.Global.Openness().Initialize(chosenProcess.ProcessId);
    }
}
