using System.IO;
using NUnit.Framework;
using StudentManagement.Tests.UI.Infrastructure;

namespace StudentManagement.Tests.UI.Infrastructure
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
    }
}