// © Siemens 2025
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using NUnit.Framework;
using Siemens.Collaboration.Net.TiaPortal.Openness.Extensions.AttributeHandling;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;
using TiaPortal.Openness.CodeSnippets.WithExtensions.Setup;

namespace TiaPortal.Openness.CodeSnippets.WithExtensions.StartDrive;

[TestFixture("Startdrive.zap21")]
public class AttributePathSnippets(string tiaArchiveName) : BaseClass(tiaArchiveName)
{
    [Test]
    public void ReadOutAllDevicePaths()
    {
        var devices = TiaPortalInstance.ProjectBase().AllDevices();
        foreach (var device in devices)
        {
            Console.WriteLine(device.GetPath());
        }
    }

    [Test]
    public void ReadOutAllMotorModulePaths()
    {
        var motorModules = TiaPortalInstance.ProjectBase()
            .AllStartdriveDevices()
            .Where(d => d.IsSinamicsS120())
            .SelectMany(s => s.DeviceItems)
            .SelectMany(i => i.MotorPowerModules())
            .ToList();

        Console.WriteLine($"Motor Modules Found: {motorModules.Count()}");
        Console.WriteLine($"{"DeviceItem Name",-25}\tPath to Motor Module");

        foreach (var motorModule in motorModules)
        {
            Console.WriteLine($"{motorModule.Parent<DeviceItem>().Name,-25}\t{motorModule.GetPath()}");
        }
    }

    [Test]
    public void PathConcept_FirstIntroduction()
    {
        var tia = TiaPortalInstance;
        var project = Project;

        var engineeringObject = project.Devices.First(x => x.Name == "S120Democase").HeadModule().DeviceItems[0]
            .GetService<NetworkInterface>().Nodes.First();

        //Save in string
        var path = engineeringObject.GetPath();

        //return in engineeringObject
        var engineeringObjectNew = path.ToEngineeringObject(tia);

        Console.WriteLine($"Objects are equal: {Equals(engineeringObject, engineeringObjectNew)}");
    }

    [Test]
    public void PathConcept_GetEngineeringObjectWithIdentifier()
    {
        var device = Project.Devices.First(x => x.Name == "S120Democase");

        var allDeviceItems = device.AllDeviceItems().ToCachedEnumerable();

        var allMotors = allDeviceItems.GetEngineeringObjectsWithIdentifier(ObjectIdentifier.Motor).ToList();
        var allEncoder = allDeviceItems.GetEngineeringObjectsWithIdentifier(ObjectIdentifier.Encoder).ToList();

        allMotors.ForEach(x =>
            Console.WriteLine($"Objects are equal: {Equals(ObjectIdentifier.Motor, x.GetObjectIdentifierEnum())}"));
        allEncoder.ForEach(x =>
            Console.WriteLine($"Objects are equal: {Equals(ObjectIdentifier.Encoder, x.GetObjectIdentifierEnum())}"));
    }

    [Test]
    public void CompositionPath()
    {
        var device = Project.Devices.First(x => x.Name == "S120Democase");
        var nodeComposition = device.HeadModule().ProfinetInterfaces().First().AsNetworkInterface().Nodes;
        var nodeCompositionPath = nodeComposition.GetPath();
        Console.WriteLine(nodeCompositionPath);
    }
}
