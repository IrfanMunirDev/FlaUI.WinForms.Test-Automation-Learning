using System.Threading;
using FlaUI.Core.AutomationElements;
using NUnit.Framework;
using StudentManagement.Tests.UI.Tests;

namespace StudentManagement.Tests.UI.Tests
{
    [TestFixture]
    public class CheckboxTests : UiTestBase
    {
        [Test]
        public void ActiveCheckbox_InitiallyChecked()
        {
            var chk = FindCheckBox("chkActive");
            Assert.That(chk.IsChecked, Is.True);
        }

        [Test]
        public void ActiveCheckbox_CanBeToggled()
        {
            var chk = FindCheckBox("chkActive");
            chk.IsChecked = false;
            Thread.Sleep(200);
            Assert.That(chk.IsChecked, Is.False);
            chk.IsChecked = true;
            Thread.Sleep(200);
            Assert.That(chk.IsChecked, Is.True);
        }

        [Test]
        public void ActiveCheckbox_ToggleMethod()
        {
            var chk = FindCheckBox("chkActive");
            bool before = chk.IsChecked == true;
            chk.Toggle();
            Thread.Sleep(200);
            Assert.That(chk.IsChecked, Is.Not.EqualTo(before));
            chk.Toggle();
            Thread.Sleep(200);
            Assert.That(chk.IsChecked, Is.EqualTo(before));
        }
    }
}