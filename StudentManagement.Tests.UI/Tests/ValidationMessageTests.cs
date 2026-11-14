using System;
using NUnit.Framework;
using StudentManagement.Tests.UI.Tests;

namespace StudentManagement.Tests.UI.Tests
{
    [TestFixture]
    public class ValidationMessageTests : UiTestBase
    {
        [Test]
        public void Save_EmptyFirstName_ShowsValidation()
        {
            FindTextBox("txtFirstName").Text = "";
            FindTextBox("txtLastName").Text = "User";
            ClickAndDismiss(FindButton("btnSave"), "Validation");
            Assert.Pass();
        }

        [Test]
        public void Save_EmptyLastName_ShowsValidation()
        {
            FindTextBox("txtFirstName").Text = "Temp";
            FindTextBox("txtLastName").Text = "";
            ClickAndDismiss(FindButton("btnSave"), "Validation");
            Assert.Pass();
        }

        [Test]
        public void Save_InvalidEmail_ShowsValidation()
        {
            FindTextBox("txtFirstName").Text = "Temp";
            FindTextBox("txtLastName").Text = "User";
            FindTextBox("txtEmail").Text = "not-an-email";
            FindRadioButton("rdoMale").Click(false);
            ClickAndDismiss(FindButton("btnSave"), "Validation");
            Assert.Pass();
        }

        [Test]
        public void Save_NoGender_ShowsValidation()
        {
            FindTextBox("txtFirstName").Text = "Temp";
            FindTextBox("txtLastName").Text = "User";
            ClickAndDismiss(FindButton("btnSave"), "Validation");
            Assert.Pass();
        }

        [Test]
        public void AddCourse_EmptyName_ShowsValidation()
        {
            SelectTab("Courses");
            FindTextBox("txtCourseName").Text = "";
            ClickAndDismiss(FindButton("btnAddCourse"), "Validation");
            Assert.Pass();
        }

        [Test]
        public void EnrollButton_Exists()
        {
            SelectTab("Enrollment");
            Assert.That(FindButton("btnEnroll"), Is.Not.Null);
        }
    }
}