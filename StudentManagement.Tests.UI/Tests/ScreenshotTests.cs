using System.IO;
using NUnit.Framework;
using StudentManagement.Tests.UI.Infrastructure;
using StudentManagement.Tests.UI.Tests;

namespace StudentManagement.Tests.UI.Tests
{
    [TestFixture]
    public class ScreenshotTests : UiTestBase
    {
        [Test]
        public void CaptureWindow_CreatesFile()
        {
            var path = ScreenshotHelper.CaptureWindow(MainWindow, "WindowCapture");
            Assert.That(File.Exists(path), Is.True);
            Assert.That(new FileInfo(path).Length, Is.GreaterThan(0));
        }

        [Test]
        public void CaptureElement_CreatesFile()
        {
            var btn = FindButton("btnSave");
            var path = ScreenshotHelper.CaptureElement(btn, "ButtonCapture");
            Assert.That(File.Exists(path), Is.True);
            Assert.That(new FileInfo(path).Length, Is.GreaterThan(0));
        }
    }
}