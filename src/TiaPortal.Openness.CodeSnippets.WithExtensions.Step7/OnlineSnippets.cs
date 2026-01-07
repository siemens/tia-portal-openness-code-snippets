// © Siemens 2025 - 2026
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using NUnit.Framework;
using Siemens.Engineering.Online;
using TiaPortal.Openness.CodeSnippets.WithExtensions.Setup;

namespace TiaPortal.Openness.CodeSnippets.WithExtensions.Step7;

[TestFixture("Step7.zap21")]
public class OnlineSnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    [Test]
    public void GoOfflineAllDevices()
    {
        var devices = Project.AllDevices();
        var onlineProviders = devices
            .SelectMany(device => device.DeviceItems.Select(deviceItem => deviceItem.AsOnlineProvider()))
            .Where(provider => provider != null).ToList();

        onlineProviders.Where(x => x.State == OnlineState.Online).ForEach(x => x.GoOffline());
    }
}
