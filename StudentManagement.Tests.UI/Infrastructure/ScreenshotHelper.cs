using System;
using System.IO;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Capturing;
using FlaUI.UIA3;

namespace StudentManagement.Tests.UI.Infrastructure
{
    public static class ScreenshotHelper
    {
        public static string OutputDirectory =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Screenshots");

        public static string CaptureWindow(AutomationElement window, string name)
        {
            Directory.CreateDirectory(OutputDirectory);
            var path = Path.Combine(OutputDirectory, Sanitize(name) + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + ".png");
            var capture = Capture.Element(window, new CaptureSettings());
            capture.ToFile(path);
            capture.Dispose();
            return path;
        }

        public static string CaptureElement(AutomationElement element, string name)
        {
            return CaptureWindow(element, name);
        }

        private static string Sanitize(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name;
        }
    }
}