using System;
using System.IO;
using System.Reflection;
using FlaUI.Core;
using FlaUI.UIA3;
using NUnit.Framework;

namespace StudentManagement.Tests.UI
{
    public static class AppLauncher
    {
        public static string GetApplicationPath()
        {
            // The WinForms output is copied next to the test assembly via project reference.
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(dir, "StudentManagement.WinForms.exe");
        }

        public static Application Launch()
        {
            var path = GetApplicationPath();
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("WinForms executable not found. Build the WinForms project first.", path);
            }

            return Application.Launch(path);
        }

        public static UIA3Automation CreateAutomation()
        {
            return new UIA3Automation();
        }
    }
}
