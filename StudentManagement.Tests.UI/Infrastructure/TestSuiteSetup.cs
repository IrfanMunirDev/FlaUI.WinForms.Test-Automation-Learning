using System;
using System.IO;
using NUnit.Framework;
using StudentManagement.Tests.UI.Infrastructure;

namespace StudentManagement.Tests.UI
{
    [SetUpFixture]
    public class TestSuiteSetup
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            ReportHelper.Reset();
            Directory.CreateDirectory(ScreenshotHelper.OutputDirectory);
        }

        [OneTimeTearDown]
        public void RunAfterAllTests()
        {
            ReportHelper.GenerateSummary();
            ReportHelper.GenerateHtml();
        }
    }
}