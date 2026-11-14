using System.IO;
using NUnit.Framework;
using StudentManagement.Tests.UI.Infrastructure;
using StudentManagement.Tests.UI.Tests;

namespace StudentManagement.Tests.UI.Tests
{
    [TestFixture]
    public class ReportingTests : UiTestBase
    {
        [Test]
        public void ReportFile_IsCreated()
        {
            // TearDown records result; force an entry by checking file exists after at least one test.
            Assert.That(File.Exists(ReportHelper.ReportPath), Is.True);
        }

        [Test]
        public void ReportFile_ContainsEntries()
        {
            var content = File.ReadAllText(ReportHelper.ReportPath);
            Assert.That(content, Does.Contain(":"));
        }
    }
}