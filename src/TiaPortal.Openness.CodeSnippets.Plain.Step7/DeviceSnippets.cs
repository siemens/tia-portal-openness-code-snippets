// © Siemens 2025 - 2026
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using NUnit.Framework;
using Siemens.Engineering;
using Siemens.Engineering.HW;
using TiaPortal.Openness.CodeSnippets.Plain.Setup;

namespace TiaPortal.Openness.CodeSnippets.Plain.Step7;

[TestFixture("Step7.zap21")]
public class DeviceSnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    [Test]
    public void EnumerateAllDevicesIncludesRootDevices()
    {
        var devices = EnumerateAllDevices(Project);

        Assert.That(devices, Is.SupersetOf(Project.Devices));
        Assert.That(devices.Select(device => device.Name), Has.All.Not.Empty);
    }

    [Test]
    public void EnumerateAllDevices_IncludesRootNestedGroupsAndUngroupedDevicesExactlyOnce()
    {
        var expectedNames = new List<string>();
        AddDeviceNames(expectedNames, Project.Devices);
        foreach (var group in Project.DeviceGroups)
        {
            AddGroupDeviceNames(expectedNames, group);
        }

        if (Project.UngroupedDevicesGroup != null)
        {
            AddDeviceNames(expectedNames, Project.UngroupedDevicesGroup.Devices);
        }

        const string plcTypeIdentifier = "OrderNumber:6ES7 515-2UM01-0AB0/V2.9";
        const string ungroupedTypeIdentifier = "OrderNumber:6ES7 155-6AU00-0CN0/V3.0";

        var rootDevice = Project.Devices.CreateWithItem(plcTypeIdentifier, "EnumRootItem", "EnumRootDevice");
        var outerGroup = Project.DeviceGroups.Create("EnumOuterGroup");
        var outerDevice = outerGroup.Devices.CreateWithItem(plcTypeIdentifier, "EnumOuterItem", "EnumOuterDevice");
        var innerGroup = outerGroup.Groups.Create("EnumInnerGroup");
        var innerDevice = innerGroup.Devices.CreateWithItem(plcTypeIdentifier, "EnumInnerItem", "EnumInnerDevice");
        var ungroupedDevice = Project.UngroupedDevicesGroup.Devices.CreateWithItem(
            ungroupedTypeIdentifier, "EnumUngroupedItem", "EnumUngroupedDevice");

        Assert.That(rootDevice.Name, Is.EqualTo("EnumRootDevice"));
        Assert.That(outerDevice.Name, Is.EqualTo("EnumOuterDevice"));
        Assert.That(innerDevice.Name, Is.EqualTo("EnumInnerDevice"));
        Assert.That(ungroupedDevice.Name, Is.EqualTo("EnumUngroupedDevice"));
        Assert.That(innerGroup.Name, Is.EqualTo("EnumInnerGroup"));
        Assert.That(outerGroup.Groups.Select(group => group.Name), Does.Contain("EnumInnerGroup"));

        AddDeviceNames(expectedNames, new[] { rootDevice, outerDevice, innerDevice, ungroupedDevice });

        var actualNames = EnumerateAllDevices(Project).Select(device => device.Name).ToList();

        Assert.That(actualNames, Has.Count.EqualTo(expectedNames.Count));
        Assert.That(actualNames, Is.EquivalentTo(expectedNames));
    }

    /// <summary>
    /// Plain Openness equivalent of <c>Project.AllDevices()</c> from the Openness Extensions package.
    /// Devices can live in the project root, in nested device groups, or in the ungrouped devices group.
    /// </summary>
    public static IReadOnlyList<Device> EnumerateAllDevices(Project project)
    {
        var devices = new List<Device>();

        CollectDevices(project.Devices, devices);
        foreach (var group in project.DeviceGroups)
        {
            CollectGroup(group, devices);
        }

        var ungrouped = project.UngroupedDevicesGroup;
        if (ungrouped != null)
        {
            CollectDevices(ungrouped.Devices, devices);
        }

        return devices;
    }

    private static void CollectGroup(DeviceUserGroup group, List<Device> devices)
    {
        CollectDevices(group.Devices, devices);
        foreach (var child in group.Groups)
        {
            CollectGroup(child, devices);
        }
    }

    private static void CollectDevices(IEnumerable<Device> devices, List<Device> result)
    {
        foreach (var device in devices)
        {
            result.Add(device);
        }
    }

    private static void AddGroupDeviceNames(List<string> names, DeviceUserGroup group)
    {
        AddDeviceNames(names, group.Devices);
        foreach (var child in group.Groups)
        {
            AddGroupDeviceNames(names, child);
        }
    }

    private static void AddDeviceNames(List<string> names, IEnumerable<Device> devices)
    {
        foreach (var device in devices)
        {
            if (!names.Contains(device.Name))
            {
                names.Add(device.Name);
            }
        }
    }
}
