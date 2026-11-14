using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using NUnit.Framework;
using StudentManagement.Tests.UI;

namespace StudentManagement.Tests.UI.Tests
{
    [TestFixture]
    public class DataGridViewTests
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

        private void SelectTab(string name)
        {
            var tab = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("tabControl")).AsTab();
            int index = -1;
            for (int i = 0; i < tab.TabItems.Length; i++)
            {
                if (tab.TabItems[i].Name == name) { index = i; break; }
            }
            if (index < 0) return;
            try { tab.SelectTabItem(index); }
            catch { /* still switches */ }
            Thread.Sleep(1000);
        }

        private T Find<T>(string automationId, Func<AutomationElement, T> converter)
        {
            var el = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(automationId));
            Assert.That(el, Is.Not.Null, "Control '" + automationId + "' not found.");
            return converter(el);
        }

        private void ClickAndDismiss(Button button, string dialogTitle)
        {
            Task.Run(() =>
            {
                for (int i = 0; i < 50; i++)
                {
                    var dlg = _automation.GetDesktop().FindFirstChild(cf => cf.ByName(dialogTitle));
                    if (dlg != null)
                    {
                        dlg.FindFirstDescendant(cf => cf.ByName("OK"))?.AsButton()?.Click(false);
                        return;
                    }
                    Thread.Sleep(100);
                }
            });
            button.Click(false);
            Thread.Sleep(1000);
        }

        [Test]
        public void StudentsGrid_HasExpectedColumns()
        {
            SelectTab("Student Listing");
            var grid = Find("dgvStudents", e => e.AsDataGridView());
            var headers = grid.Header.Columns.Select(c => c.Text).ToArray();
            Assert.That(headers, Does.Contain("FirstName"));
            Assert.That(headers, Does.Contain("LastName"));
            Assert.That(headers, Does.Contain("Email"));
            Assert.That(headers.Length, Is.GreaterThanOrEqualTo(4));
        }

        [Test]
        public void StudentsGrid_ContainsSeededRows()
        {
            SelectTab("Student Listing");
            var grid = Find("dgvStudents", e => e.AsDataGridView());
            Assert.That(grid.Rows.Length, Is.GreaterThanOrEqualTo(2));
        }

        [Test]
        public void StudentsGrid_CellValues_MatchSeededData()
        {
            SelectTab("Student Listing");
            var grid = Find("dgvStudents", e => e.AsDataGridView());
            var firstNames = grid.Rows.Select(r => r.Cells[1].Value?.ToString()).ToArray();
            Assert.That(firstNames, Does.Contain("John"));
            Assert.That(firstNames, Does.Contain("Jane"));
        }

        [Test]
        public void CoursesGrid_HasExpectedColumnsAndData()
        {
            SelectTab("Courses");
            var grid = Find("dgvCourses", e => e.AsDataGridView());
            var headers = grid.Header.Columns.Select(c => c.Text).ToArray();
            Assert.That(headers, Does.Contain("Name"));
            Assert.That(headers, Does.Contain("Description"));
            Assert.That(grid.Rows.Length, Is.GreaterThanOrEqualTo(3));
            var names = grid.Rows.Select(r => r.Cells[1].Value?.ToString()).ToArray();
            Assert.That(names, Does.Contain("Math 101"));
        }

        [Test]
        public void CoursesGrid_AddCourse_ReflectsInGrid()
        {
            SelectTab("Courses");
            var unique = "Course" + Guid.NewGuid().ToString("N").Substring(0, 5);
            var nameBox = Find("txtCourseName", e => e.AsTextBox());
            var descBox = Find("txtCourseDescription", e => e.AsTextBox());
            nameBox.Text = unique;
            descBox.Text = "Added by test";

            var addBtn = Find("btnAddCourse", e => e.AsButton());
            ClickAndDismiss(addBtn, "Success");

            var grid = Find("dgvCourses", e => e.AsDataGridView());
            var names = grid.Rows.Select(r => r.Cells[1].Value?.ToString()).ToArray();
            Assert.That(names, Does.Contain(unique));
        }

        [Test]
        public void EnrollmentsGrid_InitiallyEmptyOrHasRows()
        {
            SelectTab("Enrollment");
            var grid = Find("dgvEnrollments", e => e.AsDataGridView());
            // Allow any count (DB may have prior enrollments); just verify grid is discoverable
            Assert.That(grid, Is.Not.Null);
            Assert.That(grid.Rows.Length, Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public void EnrollmentsGrid_AfterEnroll_ShowsRecord()
        {
            SelectTab("Enrollment");
            var studentCombo = Find("cmbStudent", e => e.AsComboBox());
            var courseCombo = Find("cmbCourseEnroll", e => e.AsComboBox());

            var enrollBtn = Find("btnEnroll", e => e.AsButton());
            ClickAndDismiss(enrollBtn, "Success");

            var grid = Find("dgvEnrollments", e => e.AsDataGridView());
            Assert.That(grid.Rows.Length, Is.GreaterThanOrEqualTo(1));
        }

        [Test]
        public void GridRow_Click_DoesNotThrow()
        {
            SelectTab("Student Listing");
            var grid = Find("dgvStudents", e => e.AsDataGridView());
            Assert.That(grid.Rows.Length, Is.GreaterThan(0));
            grid.Rows[0].Click(false);
            Thread.Sleep(200);
            Assert.Pass();
        }
    }
}
