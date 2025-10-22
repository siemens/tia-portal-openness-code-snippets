// © Siemens 2025
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using System.Reflection;
using Microsoft.Win32;

namespace TiaPortal.Openness.CodeSnippets.Plain.Setup;

public class OpennessAssemblyResolverSnippet
{
    private const string SiemensEngineeringDllName = "Siemens.Engineering";
    private const string SubKeyName = @"SOFTWARE\Siemens\Automation\Openness";

    public static Assembly ResolveSiemensEngineeringAssembly(object sender, ResolveEventArgs args)
    {
        var assemblyName = new AssemblyName(args.Name);
        if (assemblyName.Name != SiemensEngineeringDllName)
        {
            return null;
        }

        var assemblyVersion =
            args.Name.Split(',').FirstOrDefault(x => x.Contains("Version")).Split('=')[1].Split('.')[0];


        using var regBaseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
            RegistryView.Registry64);
        using var opennessBaseKey = regBaseKey.OpenSubKey(SubKeyName);
        using var registryKeyLatestTiaVersion = opennessBaseKey?
            .OpenSubKey(opennessBaseKey.GetSubKeyNames().FirstOrDefault(x => x.Contains(assemblyVersion)));
        var requestedVersionOfAssembly = assemblyName.Version.ToString();

        using var assemblyVersionSubKey = registryKeyLatestTiaVersion
            ?.OpenSubKey("PublicAPI")
            ?.OpenSubKey(requestedVersionOfAssembly);
        var siemensEngineeringAssemblyPath = assemblyVersionSubKey?
            .GetValue(SiemensEngineeringDllName).ToString();
        if (siemensEngineeringAssemblyPath == null
            || !File.Exists(siemensEngineeringAssemblyPath))
        {
            return null;
        }

        var assembly = Assembly.LoadFrom(siemensEngineeringAssemblyPath);
        return assembly;
    }
}
