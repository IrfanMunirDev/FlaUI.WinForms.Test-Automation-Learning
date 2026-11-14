using System;
using NUnit.Framework;
using StudentManagement.Tests.UI.Tests;

namespace StudentManagement.Tests.UI.Tests
{
    [TestFixture]
    public class DialogTests : UiTestBase
    {
        [Test]
        public void Save_ValidData_ShowsSuccessDialog()
        {
            var unique = "Dlg" + Guid.NewGuid().ToString("N").Substring(0, 6);
            FindTextBox("txtFirstName").Text = unique;
            FindTextBox("txtLastName").Text = "User";
            FindTextBox("txtEmail").Text = unique + "@test.com";
            FindRadioButton("rdoFemale").Click(false);
            ClickAndDismiss(FindButton("btnSave"), "Success");
            Assert.Pass();
        }

        [Test]
        public void AddCourse_ValidName_ShowsSuccessDialog()
        {
            SelectTab("Courses");
            var unique = "DlgCourse" + Guid.NewGuid().ToString("N").Substring(0, 5);
            FindTextBox("txtCourseName").Text = unique;
            FindTextBox("txtCourseDescription").Text = "Added by dialog test";
            ClickAndDismiss(FindButton("btnAddCourse"), "Success");
            Assert.Pass();
        }

        [Test]
        public void Enroll_ValidSelection_ShowsSuccessDialog()
        {
            SelectTab("Enrollment");
            ClickAndDismiss(FindButton("btnEnroll"), "Success");
            Assert.Pass();
        }
    }
}