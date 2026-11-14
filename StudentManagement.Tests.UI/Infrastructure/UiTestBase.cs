using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using NUnit.Framework;
using StudentManagement.Tests.UI.Infrastructure;

namespace StudentManagement.Tests.UI.Tests
{
    public abstract class UiTestBase
    {
        protected Application App;
        protected UIA3Automation Automation;
        protected Window MainWindow;

        [SetUp]
        public virtual void SetUp()
        {
            App = AppLauncher.Launch();
            Automation = AppLauncher.CreateAutomation();
            MainWindow = App.GetMainWindow(Automation, TimeSpan.FromSeconds(15));
        }

        [TearDown]
        public virtual void TearDown()
        {
            var ctx = TestContext.CurrentContext;
            var status = ctx.Result.Outcome.Status.ToString();
            string screenshotPath = null;

            if (status != "Passed" && MainWindow != null)
            {
                try
                {
                    screenshotPath = ScreenshotHelper.CaptureWindow(MainWindow, ctx.Test.Name);
                    TestContext.AddTestAttachment(screenshotPath);
                }
                catch { }
            }

            ReportHelper.Record(ctx.Test.Name, GetType().Name, status, ctx.Result.Message, screenshotPath);

            Automation?.Dispose();
            if (App != null && !App.HasExited) App.Close();
            App?.Dispose();
        }

        protected TextBox FindTextBox(string automationId) => Find(automationId).AsTextBox();
        protected Button FindButton(string automationId) => Find(automationId).AsButton();
        protected ComboBox FindComboBox(string automationId) => Find(automationId).AsComboBox();
        protected CheckBox FindCheckBox(string automationId) => Find(automationId).AsCheckBox();
        protected RadioButton FindRadioButton(string automationId) => Find(automationId).AsRadioButton();
        protected DataGridView FindGrid(string automationId) => Find(automationId).AsDataGridView();

        protected AutomationElement Find(string automationId)
        {
            var el = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId(automationId));
            Assert.That(el, Is.Not.Null, "Control '" + automationId + "' not found.");
            return el;
        }

        protected void SelectTab(string name)
        {
            var tab = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("tabControl")).AsTab();
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

        protected string[] GetComboBoxItems(ComboBox combo)
        {
            combo.Click();
            Thread.Sleep(300);
            var items = MainWindow.FindAllDescendants(cf => cf.ByControlType(ControlType.ListItem))
                .Select(e => e.Name).ToArray();
            combo.Click();
            Thread.Sleep(200);
            return items;
        }

        protected void ClickAndDismiss(Button button, string dialogTitle)
        {
            Task.Run(() =>
            {
                for (int i = 0; i < 50; i++)
                {
                    var dlg = Automation.GetDesktop().FindFirstChild(cf => cf.ByName(dialogTitle));
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
    }
}