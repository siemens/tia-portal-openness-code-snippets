using System.Security;
using NUnit.Framework;
using Siemens.Engineering.Download;
using Siemens.Engineering.Download.Configurations;
using Siemens.Engineering.HW;
using Siemens.Engineering.Online;
using TiaPortal.Openness.CodeSnippets.Plain.Setup;

namespace TiaPortal.Openness.CodeSnippets.Plain.Step7;

[TestFixture("Step7.zap20")]
public class OnlineSnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    [Test]
    public void GoOnline()
    {
        var device = Project.Devices.First(x => x.Name == "PLC_S120Democase");
        var onlineProvider = device.DeviceItems.Single(x => x.Classification == DeviceItemClassifications.CPU)
            .GetService<OnlineProvider>();
        var configuration = onlineProvider.Configuration;
        var configurationMode = configuration.Modes.Find("PN/IE");
        const string InterfaceName = "Intel(R) Ethernet Connection I217 - LM";
        try
        {
            var pcInterface = configurationMode.PcInterfaces.Find(InterfaceName, 1);
            if (pcInterface == null)
            {
                throw new Exception($"Could not find interface by name: {InterfaceName}");
            }

            var targetConfiguration = pcInterface.TargetInterfaces[0];
            configuration.ApplyConfiguration(targetConfiguration);
            var onlineState = onlineProvider.GoOnline();
            Console.WriteLine(onlineState);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    [Test]
    public void Download()
    {
        // Get the DownloadProvider
        var device = Project.Devices.First(x => x.Name == "PLC_S120Democase");
        var downloadProvider = device.DeviceItems.Single(x => x.Classification == DeviceItemClassifications.CPU)
            .GetService<DownloadProvider>();

        // Get the TargetConfiguration 
        var configuration = downloadProvider.Configuration;
        var configurationMode = configuration.Modes.Find("PN/IE");
        const string InterfaceName = "PLCSIM";
        try
        {
            var pcInterface = configurationMode.PcInterfaces.Find(InterfaceName, 1);
            if (pcInterface == null)
            {
                throw new Exception($"Could not find interface by name: {InterfaceName}");
            }

            var targetConfiguration = pcInterface.TargetInterfaces[0];

            // Set the DownloadOptions
            const DownloadOptions DownloadOptions = DownloadOptions.SoftwareOnlyChanges;
            const string SafetyPassword = "HelloWorld!";

            // Start Download
            downloadProvider.Download(targetConfiguration, preDownload =>
            {
                if (preDownload is DownloadPasswordConfiguration downloadPasswordConfiguration)
                {
                    downloadPasswordConfiguration.SetPassword(GetSecureString(SafetyPassword));
                }
            }, _ => { }, DownloadOptions);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static SecureString GetSecureString(string value)
    {
        var secureStr = new SecureString();
        if (string.IsNullOrEmpty(value))
        {
            return secureStr;
        }

        foreach (var t in value)
        {
            secureStr.AppendChar(t);
        }

        secureStr.MakeReadOnly();
        return secureStr;
    }
}
