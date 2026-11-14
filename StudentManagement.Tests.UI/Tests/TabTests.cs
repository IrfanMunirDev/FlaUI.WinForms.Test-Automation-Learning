using System.Threading;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using NUnit.Framework;
using StudentManagement.Tests.UI.Tests;

namespace StudentManagement.Tests.UI.Tests
{
    [TestFixture]
    public class TabTests : UiTestBase
    {
        [Test]
        public void TabControl_HasFourTabs()
        {
            var tabControl = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("tabControl"));
            Assert.That(tabControl, Is.Not.Null);
            var tab = tabControl.AsTab();
            Assert.That(tab.TabItems.Length, Is.EqualTo(4));
        }

        [Test]
        public void Tab_SwitchToCourses_ShowsCourseControls()
        {
            SelectTab("Courses");
            Assert.That(Find("txtCourseName"), Is.Not.Null);
            Assert.That(Find("dgvCourses"), Is.Not.Null);
        }

        [Test]
        public void Tab_SwitchToStudentListing_ShowsGrid()
        {
            SelectTab("Student Listing");
            Assert.That(Find("dgvStudents"), Is.Not.Null);
        }

        [Test]
        public void Tab_SwitchToEnrollment_ShowsEnrollmentControls()
        {
            SelectTab("Enrollment");
            Assert.That(Find("cmbStudent"), Is.Not.Null);
            Assert.That(Find("dgvEnrollments"), Is.Not.Null);
        }

        [Test]
        public void Tab_SwitchBackToStudents_ShowsStudentControls()
        {
            SelectTab("Courses");
            SelectTab("Students");
            Assert.That(Find("txtFirstName"), Is.Not.Null);
            Assert.That(Find("btnSave"), Is.Not.Null);
        }
    }
}