using System;
using System.IO;

namespace StudentManagement.Tests.UI.Infrastructure
{
    public static class ReportHelper
    {
        public static string ReportPath =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestReport.txt");

        public static void Reset()
        {
            try { if (File.Exists(ReportPath)) File.Delete(ReportPath); } catch { }
        }

        public static void Record(string testName, string status, string message)
        {
            try
            {
                var line = $"[{DateTime.Now:O}] {testName}: {status}" +
                           (string.IsNullOrWhiteSpace(message) ? string.Empty : " - " + message.Replace(Environment.NewLine, " ")) +
                           Environment.NewLine;
                File.AppendAllText(ReportPath, line);
            }
            catch { }
        }
    }
}