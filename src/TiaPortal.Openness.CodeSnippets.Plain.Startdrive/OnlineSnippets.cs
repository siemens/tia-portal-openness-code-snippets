// © Siemens 2025
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using NUnit.Framework;
using Siemens.Engineering.HW;
using Siemens.Engineering.Online;
using TiaPortal.Openness.CodeSnippets.Plain.Setup;

namespace TiaPortal.Openness.CodeSnippets.Plain.Startdrive;

[TestFixture("Startdrive.zap20")]
public class OnlineSnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    [Test]
    public void OnlineProvider_CurrentOnlineState()
    {
        var s120Device = Project.Devices.First(x => x.Name == "S120Democase");

        var onlineProvider = s120Device.DeviceItems
            .First(x => x.Classification == DeviceItemClassifications.HM).GetService<OnlineProvider>();
        var currentState = onlineProvider.State;
        Console.WriteLine(currentState);
    }
}
