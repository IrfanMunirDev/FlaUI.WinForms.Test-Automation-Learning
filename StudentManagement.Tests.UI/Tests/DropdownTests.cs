using System;
using System.Linq;
using System.Threading;
using FlaUI.Core.AutomationElements;
using NUnit.Framework;
using StudentManagement.Tests.UI.Tests;

namespace StudentManagement.Tests.UI.Tests
{
    [TestFixture]
    public class DropdownTests : UiTestBase
    {
        [Test]
        public void CourseDropdown_ContainsSeededCourses()
        {
            var combo = FindComboBox("cmbCourse");
            var items = GetComboBoxItems(combo);
            Assert.That(items, Does.Contain("Math 101"));
            Assert.That(items, Does.Contain("Physics 101"));
            Assert.That(items.Length, Is.GreaterThanOrEqualTo(3));
        }

        [Test]
        public void CourseDropdown_SelectionChanges()
        {
            var combo = FindComboBox("cmbCourse");
            combo.Value = "Physics 101";
            Thread.Sleep(200);
            Assert.That(combo.Value, Is.EqualTo("Physics 101"));
        }

        [Test]
        public void EnrollmentDropdowns_ContainData()
        {
            SelectTab("Enrollment");
            var students = FindComboBox("cmbStudent");
            var courses = FindComboBox("cmbCourseEnroll");
            var studentItems = GetComboBoxItems(students);
            var courseItems = GetComboBoxItems(courses);
            Assert.That(studentItems.Length, Is.GreaterThanOrEqualTo(2));
            Assert.That(courseItems.Length, Is.GreaterThanOrEqualTo(3));
        }

        [Test]
        public void EnrollmentDropdown_SelectionChanges()
        {
            SelectTab("Enrollment");
            var courses = FindComboBox("cmbCourseEnroll");
            courses.Value = "Chemistry 101";
            Thread.Sleep(200);
            Assert.That(courses.Value, Is.EqualTo("Chemistry 101"));
        }
    }
}