using System;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using NUnit.Framework;
using StudentManagement.Tests.UI;

namespace StudentManagement.Tests.UI.Tests
{
    [TestFixture]
    public class ApplicationLaunchTests
    {
        private Application _app;
        private UIA3Automation _automation;
        private Window _mainWindow;

        [SetUp]
        public void SetUp()
        {
            _app = AppLauncher.Launch();
            _automation = AppLauncher.CreateAutomation();
            _mainWindow = _app.GetMainWindow(_automation, TimeSpan.FromSeconds(15));
        }

        [TearDown]
        public void TearDown()
        {
            _automation?.Dispose();
            if (_app != null && !_app.HasExited) _app.Close();
            _app?.Dispose();
        }

        [Test]
        public void Launch_Application_MainWindowIsFound()
        {
            Assert.That(_mainWindow, Is.Not.Null);
        }

        [Test]
        public void MainWindow_Title_ContainsExpectedText()
        {
            Assert.That(_mainWindow.Title, Does.Contain("Student Management"));
        }

        [Test]
        public void MainWindow_HasTabControl_WithFourTabs()
        {
            var tabControl = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("tabControl"));
            Assert.That(tabControl, Is.Not.Null);

            var tabs = tabControl.FindAllDescendants(cf => cf.ByControlType(ControlType.TabItem));
            Assert.That(tabs.Length, Is.EqualTo(4));
        }

        [Test]
        public void MainWindow_ContainsExpectedStudentControls()
        {
            Assert.That(_mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("txtFirstName")), Is.Not.Null);
            Assert.That(_mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("txtLastName")), Is.Not.Null);
            Assert.That(_mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("btnSave")), Is.Not.Null);
            Assert.That(_mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("btnClear")), Is.Not.Null);
        }
    }
}
