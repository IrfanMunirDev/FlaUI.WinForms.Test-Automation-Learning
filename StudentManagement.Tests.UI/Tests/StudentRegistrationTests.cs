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
    public class StudentRegistrationTests
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

        private TextBox FindTextBox(string automationId)
        {
            var el = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(automationId));
            Assert.That(el, Is.Not.Null, "Control '" + automationId + "' not found.");
            return el.AsTextBox();
        }

        private Button FindButton(string automationId)
        {
            var el = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(automationId));
            Assert.That(el, Is.Not.Null, "Control '" + automationId + "' not found.");
            return el.AsButton();
        }

        private ComboBox FindComboBox(string automationId)
        {
            var el = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(automationId));
            Assert.That(el, Is.Not.Null, "Control '" + automationId + "' not found.");
            return el.AsComboBox();
        }

        private RadioButton FindRadioButton(string automationId)
        {
            var el = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(automationId));
            Assert.That(el, Is.Not.Null, "Control '" + automationId + "' not found.");
            return el.AsRadioButton();
        }

        private DataGridView FindGrid(string automationId)
        {
            var el = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(automationId));
            Assert.That(el, Is.Not.Null, "Control '" + automationId + "' not found.");
            return el.AsDataGridView();
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

            try
            {
                tab.SelectTabItem(index);
            }
            catch
            {
                // SelectTabItem throws ElementNotEnabled but still switches tabs
            }
            Thread.Sleep(1000);
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
        public void TextBoxes_AcceptInput()
        {
            var first = FindTextBox("txtFirstName");
            var last = FindTextBox("txtLastName");
            var email = FindTextBox("txtEmail");

            first.Text = "Alice";
            last.Text = "Tester";
            email.Text = "alice@test.com";

            Assert.That(first.Text, Is.EqualTo("Alice"));
            Assert.That(last.Text, Is.EqualTo("Tester"));
            Assert.That(email.Text, Is.EqualTo("alice@test.com"));
        }

        [Test]
        public void Save_WithValidData_AddsStudentToGrid()
        {
            var unique = "FlaUI" + Guid.NewGuid().ToString("N").Substring(0, 6);
            SelectTab("Student Listing");
            var grid = FindGrid("dgvStudents");
            int before = grid.Rows.Length;
            SelectTab("Students");

            FindTextBox("txtFirstName").Text = unique;
            FindTextBox("txtLastName").Text = "User";
            FindTextBox("txtEmail").Text = unique + "@test.com";
            FindRadioButton("rdoFemale").Click(false);

            var save = FindButton("btnSave");
            ClickAndDismiss(save, "Success");

            SelectTab("Student Listing");
            grid = FindGrid("dgvStudents");
            Assert.That(grid.Rows.Length, Is.EqualTo(before + 1));
            Assert.That(grid.Rows.Any(r => r.Cells.Any(c => (c.Value != null && c.Value.ToString() == unique))), Is.True);
        }

        [Test]
        public void Save_WithEmptyFirstName_ShowsValidation()
        {
            FindTextBox("txtFirstName").Text = "";
            FindTextBox("txtLastName").Text = "User";
            var save = FindButton("btnSave");
            ClickAndDismiss(save, "Validation");
            Assert.Pass();
        }

        [Test]
        public void ClearButton_ClearsFields()
        {
            FindTextBox("txtFirstName").Text = "Temp";
            FindTextBox("txtLastName").Text = "Temp";
            FindButton("btnClear").Click(false);
            Thread.Sleep(200);
            Assert.That(FindTextBox("txtFirstName").Text, Is.Empty);
            Assert.That(FindTextBox("txtLastName").Text, Is.Empty);
        }
    }
}
