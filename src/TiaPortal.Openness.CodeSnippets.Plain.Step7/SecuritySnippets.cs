// © Siemens 2025
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using System.Security;
using NUnit.Framework;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;
using Siemens.Engineering.Umac;
using TiaPortal.Openness.CodeSnippets.Plain.Setup;

namespace TiaPortal.Openness.CodeSnippets.Plain.Step7;

[TestFixture("Step7.zap20")]
public class SecuritySnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    [Test]
    public void CreateProjectRoles()
    {
        var umacConfigurator = Project.GetService<UmacConfigurator>();

        // create a custom role for the webServer
        var adminRole = umacConfigurator.CustomRoles.Create("Admin", "Admin Role for MCS Web Server Application");

        //get UMAC device from plc device
        var umacDevice = Project.Devices.CreateWithItem("OrderNumber:6ES7 518-4UP00-0AB0/V3.1", "", "")
            .GetService<UmacDevice>();
        if (umacDevice != null)
        {
            var deviceFunctionRightsAssociation = umacDevice.AvailableDeviceFunctionRights;

            Console.WriteLine(string.Join("\n",
                deviceFunctionRightsAssociation.Select(r => $"Name: {r.Name}, Identifier: {r.Identifier}")));

            //Select and assign the needed deviceFunctionRight
            adminRole.AssignDeviceFunctionRight(umacDevice, deviceFunctionRightsAssociation.First());
        }

        umacConfigurator.ProjectUsers.Create("Admin", GetSecureString("AdminAdmin123"));
    }

    [Test]
    public void SetPasswordProtectionTest()
    {
        // Password should contain minimum 8 characters and maximum 120 characters. Also, 1 uppercase, 1 lowercase, 1 number and 1 special character.
        var secureString = GetSecureString("Test12345!");

        var plc = Project.Devices.First(x => x.Name == "PLC_S120Democase");
        var cpu = plc.DeviceItems.Single(x => x.Classification == DeviceItemClassifications.CPU);

        var masterSecretConfigurator = cpu.GetService<PlcMasterSecretConfigurator>();
        masterSecretConfigurator.Protect(secureString);
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
