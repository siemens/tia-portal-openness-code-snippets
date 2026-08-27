// © Siemens 2025 - 2026
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using NUnit.Framework;
using Siemens.Engineering.Connection;
using Siemens.Engineering.HW;
using Siemens.Engineering.MC.Drives;
using Siemens.Engineering.MC.Drives.DFI;
using Siemens.Engineering.Online;
using Siemens.Engineering.Upload;
using Siemens.Engineering.Upload.Configurations;
using TiaPortal.Openness.CodeSnippets.Plain.Setup;

namespace TiaPortal.Openness.CodeSnippets.Plain.Startdrive;

[TestFixture("Startdrive.zap21")]
public class OnlineDriveSnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    private const string DeviceName = "S120Democase";
    private const string DriveObjectName = "RedAxis";
    private const string ConfigurationModeName = "PN/IE";
    // Adapt this to the network adapter connected to the drive.
    private const string PcInterfaceName = "Intel(R) Ethernet Connection I217 - LM";
    private const string TargetInterfaceName = "CU X127";
    private static readonly TimeSpan s_onlineTimeout = TimeSpan.FromSeconds(30);

    [Test]
    public void UploadDriveParametersToExistingStation()
    {
        using var exclusiveAccess = TiaPortalInstance.ExclusiveAccess("Upload drive parameters");
        exclusiveAccess.Text = "Uploading drive parameters to the existing station";

        var device = GetDevice();
        var headModule = GetHeadModule(device);
        var uploadProvider = headModule.GetService<ParameterUploadProvider>()
                            ?? throw new InvalidOperationException(
                                "ParameterUploadProvider is not available on this device.");
        var targetInterface = GetTargetInterface(uploadProvider.Configuration);
        // A null address uses the selected target interface; set one only for a custom address.
        var targetAddress = targetInterface.Addresses.Count == 0
            ? null
            : targetInterface.Addresses.Single();

        var uploadResult = uploadProvider.ParameterUpload(
            targetInterface,
            targetAddress,
            uploadConfiguration =>
            {
                Console.WriteLine(uploadConfiguration.Message);
                switch (uploadConfiguration)
                {
                    case OverrideTelegramMismatch telegramMismatch:
                        telegramMismatch.Checked = true;
                        break;
                    case OverwriteOfflineConfiguration offlineConfigurationMismatch:
                        offlineConfigurationMismatch.Checked = true;
                        break;
                    case UploadPasswordConfiguration:
                        throw new InvalidOperationException(
                            "The device requires a password. Set it on UploadPasswordConfiguration.");
                }
            });

        Console.WriteLine(
            $"Parameter upload state: {uploadResult.State}, errors: {uploadResult.ErrorCount}, " +
            $"warnings: {uploadResult.WarningCount}");

        if (uploadResult.State == UploadResultState.Error)
        {
            throw new InvalidOperationException("The drive parameter upload failed.");
        }

        Project.Save();
    }

    [Test]
    public void SaveAllDriveObjectsRamToRom()
    {
        using var exclusiveAccess = TiaPortalInstance.ExclusiveAccess("Save drive parameters to ROM");
        exclusiveAccess.Text = "Saving all drive parameters from RAM to ROM";

        var device = GetDevice();
        var headModule = GetHeadModule(device);
        var onlineProvider = headModule.GetService<OnlineProvider>()
                             ?? throw new InvalidOperationException(
                                 "OnlineProvider is not available on the drive headmodule.");
        var wasOnline = onlineProvider.State == OnlineState.Online;
        var connectedBySnippet = false;
        try
        {
            var driveObjectProvider = device.DeviceItems.FirstOrDefault(
                                          x => x.GetService<DriveObjectContainer>() != null)
                                      ?? throw new InvalidOperationException(
                                          $"No drive object provider found on '{device.Name}'.");
            var offlineDriveObject = driveObjectProvider.GetService<DriveObjectContainer>()?
                                         .DriveObjects.FirstOrDefault()
                                     ?? throw new InvalidOperationException(
                                         $"No drive object found on '{driveObjectProvider.Name}'.");
            EnsureOnline(onlineProvider);
            connectedBySnippet = !wasOnline;
            var onlineDriveObject = WaitForOnlineDriveObject(device, offlineDriveObject.DriveObjectNumber);
            var onlineDriveFunctionInterface =
                onlineDriveObject.GetService<OnlineDriveFunctionInterface>()
                ?? throw new InvalidOperationException(
                    "OnlineDriveFunctionInterface is not available on the online drive object.");

            var success = onlineDriveFunctionInterface.DriveDomainFunctions
                .PerformRAMtoROMCopyAllDriveObject();
            if (!success)
            {
                throw new InvalidOperationException("The RAM-to-ROM operation failed.");
            }

            Console.WriteLine($"RAM-to-ROM copy completed for device '{device.Name}'.");
        }
        finally
        {
            if (connectedBySnippet)
            {
                onlineProvider.GoOffline();
            }
        }
    }

    [Test]
    public void WriteDriveParameterOnline()
    {
        using var exclusiveAccess = TiaPortalInstance.ExclusiveAccess("Write drive parameter online");
        exclusiveAccess.Text = "Writing a drive parameter directly to device RAM";

        var device = GetDevice();
        var headModule = GetHeadModule(device);
        var driveAxis = device.DeviceItems.Single(x => x.Name == DriveObjectName);
        var onlineProvider = headModule.GetService<OnlineProvider>()
                             ?? throw new InvalidOperationException(
                                 "OnlineProvider is not available on the drive headmodule.");
        var wasOnline = onlineProvider.State == OnlineState.Online;
        var connectedBySnippet = false;
        try
        {
            var offlineDriveObject = driveAxis.GetService<DriveObjectContainer>()?.DriveObjects.First()
                                     ?? throw new InvalidOperationException(
                                         $"No drive object found on '{DriveObjectName}'.");
            EnsureOnline(onlineProvider);
            connectedBySnippet = !wasOnline;
            var onlineDriveObject = WaitForOnlineDriveObject(device, offlineDriveObject.DriveObjectNumber);

            // p2900[0] is a freely configurable fixed value. Verify that it is not used as a BiCo source
            // before adapting this example to another project.
            const string ParameterName = "p2900[0]";
            var parameter = WaitForOnlineParameter(onlineDriveObject, ParameterName);
            var oldValue = parameter.Value
                           ?? throw new InvalidOperationException(
                               $"Online parameter '{ParameterName}' has no value.");
            if (oldValue is not float oldFloatValue)
            {
                throw new InvalidOperationException(
                    $"Online parameter '{ParameterName}' is not a Single value.");
            }

            var newValue = oldFloatValue == 1.0f ? 2.0f : 1.0f;
            try
            {
                parameter.Value = newValue;
                var writtenValue = WaitForOnlineParameterValue(
                    device,
                    offlineDriveObject.DriveObjectNumber,
                    ParameterName,
                    newValue);
                Console.WriteLine($"Online parameter {parameter.Name}: {oldValue} -> {writtenValue}");
            }
            finally
            {
                onlineDriveObject = WaitForOnlineDriveObject(
                    device,
                    offlineDriveObject.DriveObjectNumber);
                parameter = WaitForOnlineParameter(onlineDriveObject, ParameterName);
                parameter.Value = oldValue;
                WaitForOnlineParameterValue(
                    device,
                    offlineDriveObject.DriveObjectNumber,
                    ParameterName,
                    oldValue);
                Console.WriteLine($"Online parameter {ParameterName} restored to {oldValue}.");
            }
        }
        finally
        {
            if (connectedBySnippet)
            {
                onlineProvider.GoOffline();
            }
        }
    }

    private Device GetDevice()
    {
        return Project.Devices.First(x => x.Name == DeviceName);
    }

    private static DeviceItem GetHeadModule(Device device)
    {
        return device.DeviceItems.First(x => x.Classification == DeviceItemClassifications.HM);
    }

    private static ConfigurationTargetInterface GetTargetInterface(ConnectionConfiguration configuration)
    {
        var configurationMode = configuration.Modes.Find(ConfigurationModeName)
                                ?? throw new InvalidOperationException(
                                    $"Connection mode '{ConfigurationModeName}' was not found.");
        var pcInterface = configurationMode.PcInterfaces.Find(PcInterfaceName, 1)
                          ?? throw new InvalidOperationException(
                              $"PG/PC interface '{PcInterfaceName}' was not found. " +
                              "Adapt PcInterfaceName to an interface connected to the drive. " +
                              $"Available interfaces: {string.Join(", ", configurationMode.PcInterfaces.Select(x => x.Name))}");
        var targetInterface = pcInterface.TargetInterfaces.SingleOrDefault(x => x.Name == TargetInterfaceName)
                              ?? throw new InvalidOperationException(
                                  $"Target interface '{TargetInterfaceName}' was not found.");

        Console.WriteLine($"Using PG/PC interface '{pcInterface.Name}' and target '{targetInterface.Name}'.");
        return targetInterface;
    }

    private static void EnsureOnline(OnlineProvider onlineProvider)
    {
        if (onlineProvider.State == OnlineState.Online)
        {
            return;
        }

        if (onlineProvider.State != OnlineState.Offline)
        {
            throw new InvalidOperationException(
                $"Cannot establish the online connection while its state is '{onlineProvider.State}'.");
        }

        var targetInterface = GetTargetInterface(onlineProvider.Configuration);
        if (!onlineProvider.Configuration.ApplyConfiguration(targetInterface))
        {
            throw new InvalidOperationException("The online connection configuration could not be applied.");
        }

        onlineProvider.GoOnline();
        var timeout = DateTime.UtcNow.Add(s_onlineTimeout);
        while (onlineProvider.State != OnlineState.Online && DateTime.UtcNow < timeout)
        {
            Thread.Sleep(200);
        }

        if (onlineProvider.State != OnlineState.Online)
        {
            throw new TimeoutException("The online connection was not established within 30 seconds.");
        }
    }

    private static OnlineDriveObject WaitForOnlineDriveObject(Device device, ushort driveObjectNumber)
    {
        var timeout = DateTime.UtcNow.Add(s_onlineTimeout);
        while (DateTime.UtcNow < timeout)
        {
            var onlineDriveObject = FindOnlineDriveObject(device.DeviceItems, driveObjectNumber);
            if (onlineDriveObject != null)
            {
                return onlineDriveObject;
            }

            Thread.Sleep(200);
        }

        throw new TimeoutException(
            $"Online drive object {driveObjectNumber} on '{device.Name}' was not available within 30 seconds.");
    }

    private static OnlineDriveObject? FindOnlineDriveObject(
        IEnumerable<DeviceItem> deviceItems,
        ushort driveObjectNumber)
    {
        foreach (var deviceItem in deviceItems)
        {
            var onlineDriveObject = deviceItem.GetService<OnlineDriveObjectContainer>()?.OnlineDriveObjects
                .SingleOrDefault(x => x.DriveObjectNumber == driveObjectNumber);
            if (onlineDriveObject != null)
            {
                return onlineDriveObject;
            }

            onlineDriveObject = FindOnlineDriveObject(deviceItem.DeviceItems, driveObjectNumber);
            if (onlineDriveObject != null)
            {
                return onlineDriveObject;
            }
        }

        return null;
    }

    private static DriveParameter WaitForOnlineParameter(OnlineDriveObject onlineDriveObject, string parameterName)
    {
        var timeout = DateTime.UtcNow.Add(s_onlineTimeout);
        while (DateTime.UtcNow < timeout)
        {
            var parameter = onlineDriveObject.Parameters.Find(parameterName);
            if (parameter?.Value != null)
            {
                return parameter;
            }

            Thread.Sleep(200);
        }

        throw new TimeoutException($"Online parameter '{parameterName}' was not available within 30 seconds.");
    }

    private static object WaitForOnlineParameterValue(
        Device device,
        ushort driveObjectNumber,
        string parameterName,
        object expectedValue)
    {
        var timeout = DateTime.UtcNow.Add(s_onlineTimeout);
        while (DateTime.UtcNow < timeout)
        {
            var onlineDriveObject = FindOnlineDriveObject(device.DeviceItems, driveObjectNumber);
            var currentValue = onlineDriveObject?.Parameters.Find(parameterName)?.Value;
            if (Equals(currentValue, expectedValue))
            {
                return currentValue;
            }

            Thread.Sleep(200);
        }

        throw new TimeoutException(
            $"Online parameter '{parameterName}' did not reach value '{expectedValue}' within 30 seconds.");
    }
}
