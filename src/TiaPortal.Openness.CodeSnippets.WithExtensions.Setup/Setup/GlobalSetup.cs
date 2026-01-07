// © Siemens 2025 - 2026
// Licensed under: "Royalty-free Software provided by Siemens on sharing platforms for developers/users of Siemens products". See LICENSE.md.

using NUnit.Framework;
using Siemens.Collaboration.Net;

//This should not be in a namespace
[SetUpFixture]
public class GlobalSetup
{
    [OneTimeSetUp]
    public void GlobalOneTimeSetup()
    {
        Api.Global.Openness().Initialize();
    }

    [OneTimeTearDown]
    public void GlobalOneTimeTearDown()
    {
    }
}
