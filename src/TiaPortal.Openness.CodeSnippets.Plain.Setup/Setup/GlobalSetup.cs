// © Siemens 2025
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using NUnit.Framework;
using TiaPortal.Openness.CodeSnippets.Plain.Setup;

//This should not be in a namespace
[SetUpFixture]
public abstract class GlobalSetup
{
    [OneTimeSetUp]
    public void GlobalOneTimeSetup()
    {
        AppDomain.CurrentDomain.AssemblyResolve += OpennessAssemblyResolverSnippet.ResolveSiemensEngineeringAssembly;
    }

    [OneTimeTearDown]
    public void GlobalOneTimeTearDown()
    {
    }
}
