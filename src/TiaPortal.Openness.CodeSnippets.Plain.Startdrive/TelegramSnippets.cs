using NUnit.Framework;
using Siemens.Engineering.HW;
using Siemens.Engineering.MC.Drives;
using Siemens.Engineering.MC.Drives.Enums;
using TiaPortal.Openness.CodeSnippets.Plain.Setup;

namespace TiaPortal.Openness.CodeSnippets.Plain.Startdrive;

[TestFixture("Startdrive.zap21")]
public class TelegramSnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    [Test]
    public void SetPropertiesOfSafetyTelegram_SinamicsG()
    {
        var tia = TiaPortalInstance;

        var g115d = tia.Projects.First().Devices.Single(x => x.Name == "SINAMICS G120");
        var hm = g115d.DeviceItems.Single(x => x.Classification == DeviceItemClassifications.HM);

        var driveObjectContainer = hm.GetService<DriveObjectContainer>();

        var safetyTelegram = driveObjectContainer.DriveObjects.First().Telegrams
            .First(x => x.Type == TelegramType.SafetyTelegram);

        //Set FAddress
        safetyTelegram.SetAttribute("Failsafe_FDestinationAddress", 10);

        //Set FMonitoringTime
        safetyTelegram.SetAttribute("Failsafe_ManualAssignmentFMonitoringtime", true);
        safetyTelegram.SetAttribute("Failsafe_FMonitoringtime", 100);
    }

    [Test]
    public void SetPropertiesOfMainTelegram_SinamicsG()
    {
        var tia = TiaPortalInstance;

        var device = tia.Projects.First().Devices.Single(x => x.Name == "SINAMICS G120");
        var deviceItem = device.DeviceItems.Single(x => x.Classification == DeviceItemClassifications.HM);

        var driveObject = deviceItem.GetService<DriveObjectContainer>().DriveObjects.First();
        var mainTelegram = driveObject.Telegrams
            .First(x => x.Type == TelegramType.MainTelegram);
        mainTelegram.TelegramNumber = 20;
    }

    [Test]
    public void GetAddress()
    {
        var drive = Project.Devices.First(x => x.Name == "S120Democase");
        var driveControl = drive.DeviceItems.FirstOrDefault();

        var telegrams = driveControl?.GetService<DriveObjectContainer>().DriveObjects.FirstOrDefault()?.Telegrams;

        var address = telegrams?.FirstOrDefault(x => x.Type == TelegramType.MainTelegram)?.Addresses.FirstOrDefault();
        Console.WriteLine($"Address found: {address != null}");
    }

    [Test]
    public void CreateAndAdjustSafetyTelegramAndAdjustMainTelegram_S120()
    {
        var device = Project.Devices.First(x => x.Name == "S120Democase");
        var driveAxis = device.DeviceItems.First(x => x.Name == "BlueAxis");

        var driveObjectContainer = driveAxis.GetService<DriveObjectContainer>();
        var telegrams = driveObjectContainer.DriveObjects.First().Telegrams;

        if (telegrams.CanInsertSafetyTelegram(30))
        {
            telegrams.InsertSafetyTelegram(30);
        }

        var safetyTelegram = telegrams.Single(x => x.Type == TelegramType.SafetyTelegram);

        //Set FAddress
        safetyTelegram.SetAttribute("Failsafe_FDestinationAddress", 10);

        var mainTelegram = telegrams.First(x => x.Type == TelegramType.MainTelegram);

        if (mainTelegram.CanChangeTelegram(999))
        {
            mainTelegram.TelegramNumber = 999;
        }
    }

    [Test]
    public void SetTelegramLength()
    {
        var device = Project.Devices.First(x => x.Name == "S120Democase");
        var driveAxis = device.DeviceItems.Single(n => n.Name == "BlueAxis");

        var driveObject = driveAxis.GetService<DriveObjectContainer>().DriveObjects.First();
        var telegrams = driveObject.Telegrams;

        var mainTelegram = telegrams.First(x => x.Type == TelegramType.MainTelegram);
        mainTelegram.ChangeSize(AddressIoType.Input, 10, false);
    }
}
