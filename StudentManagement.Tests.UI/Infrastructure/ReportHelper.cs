using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StudentManagement.Tests.UI.Infrastructure
{
    public class TestResultRecord
    {
        public DateTime Timestamp { get; set; }
        public string TestClass { get; set; }
        public string TestName { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string ScreenshotPath { get; set; }
    }

    public static class ReportHelper
    {
        private static readonly List<TestResultRecord> _records = new List<TestResultRecord>();
        private static readonly object _lock = new object();

        public static string ReportPath =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestReport.txt");

        public static string HtmlReportPath =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestReport.html");

        public static string ScreenshotsRelativeFolder => "Screenshots";

        public static void Reset()
        {
            try { if (File.Exists(ReportPath)) File.Delete(ReportPath); } catch { }
            try { if (File.Exists(HtmlReportPath)) File.Delete(HtmlReportPath); } catch { }
            lock (_lock) { _records.Clear(); }
        }

        public static void Record(string testName, string testClass, string status, string message, string screenshotPath)
        {
            var record = new TestResultRecord
            {
                Timestamp = DateTime.Now,
                TestClass = string.IsNullOrWhiteSpace(testClass) ? "Unknown" : testClass,
                TestName = testName,
                Status = status,
                Message = message ?? string.Empty,
                ScreenshotPath = screenshotPath
            };

            lock (_lock)
            {
                _records.Add(record);
            }

            try
            {
                var line = $"[{record.Timestamp:O}] {record.TestClass}.{record.TestName}: {status}" +
                           (string.IsNullOrWhiteSpace(message) ? string.Empty : " - " + message.Replace(Environment.NewLine, " ")) +
                           (string.IsNullOrWhiteSpace(screenshotPath) ? string.Empty : " [screenshot: " + screenshotPath + "]") +
                           Environment.NewLine;
                File.AppendAllText(ReportPath, line);
            }
            catch { }
        }

        public static void Record(string testName, string status, string message)
        {
            Record(testName, "Unknown", status, message, null);
        }

        public static void GenerateSummary()
        {
            try
            {
                var total = _records.Count;
                var passed = _records.Count(r => r.Status == "Passed");
                var failed = _records.Count(r => r.Status == "Failed");
                var other = total - passed - failed;

                var sb = new StringBuilder();
                sb.AppendLine();
                sb.AppendLine("===== TEST RUN SUMMARY =====");
                sb.AppendLine($"Timestamp : {DateTime.Now:O}");
                sb.AppendLine($"Machine   : {Environment.MachineName}");
                sb.AppendLine($"Total     : {total}");
                sb.AppendLine($"Passed    : {passed}");
                sb.AppendLine($"Failed    : {failed}");
                sb.AppendLine($"Other     : {other}");
                sb.AppendLine("============================");
                File.AppendAllText(ReportPath, sb.ToString());
            }
            catch { }
        }

        public static void GenerateHtml()
        {
            try
            {
                var total = _records.Count;
                var passed = _records.Count(r => r.Status == "Passed");
                var failed = _records.Count(r => r.Status == "Failed");
                var other = total - passed - failed;

                var gitInfo = GetGitInfo();

                var sb = new StringBuilder();
                sb.AppendLine("<!DOCTYPE html>");
                sb.AppendLine("<html lang=\"en\"><head><meta charset=\"utf-8\" />");
                sb.AppendLine("<title>Test Run Report</title>");
                sb.AppendLine("<style>");
                sb.AppendLine("body{font-family:Segoe UI,Arial,sans-serif;margin:20px;color:#222;}");
                sb.AppendLine(".header{background:#f4f6f8;padding:16px;border-radius:8px;margin-bottom:16px;}");
                sb.AppendLine(".summary{display:flex;gap:16px;margin-bottom:20px;}");
                sb.AppendLine(".card{background:#fff;border:1px solid #ddd;border-radius:8px;padding:12px 16px;min-width:120px;}");
                sb.AppendLine(".passed{color:#1e7e34;font-weight:bold;}");
                sb.AppendLine(".failed{color:#c82333;font-weight:bold;}");
                sb.AppendLine(".other{color:#856404;font-weight:bold;}");
                sb.AppendLine(".class{margin-top:24px;}");
                sb.AppendLine(".class h2{border-bottom:2px solid #ddd;padding-bottom:4px;}");
                sb.AppendLine(".test{border:1px solid #eee;border-radius:6px;padding:10px;margin:8px 0;background:#fafafa;}");
                sb.AppendLine(".msg{white-space:pre-wrap;color:#555;margin-top:6px;font-size:13px;}");
                sb.AppendLine("img{max-width:100%;border:1px solid #ccc;margin-top:8px;border-radius:4px;}");
                sb.AppendLine("</style></head><body>");

                sb.AppendLine("<div class=\"header\">");
                sb.AppendLine("<h1>Test Run Report</h1>");
                sb.AppendLine($"<div><strong>Date/Time:</strong> {DateTime.Now:O}</div>");
                sb.AppendLine($"<div><strong>Machine:</strong> {Environment.MachineName}</div>");
                if (!string.IsNullOrWhiteSpace(gitInfo))
                    sb.AppendLine($"<div><strong>Git:</strong> {HtmlEncode(gitInfo)}</div>");
                sb.AppendLine($"<div><strong>Output folder:</strong> {HtmlEncode(AppDomain.CurrentDomain.BaseDirectory)}</div>");
                sb.AppendLine("</div>");

                sb.AppendLine("<div class=\"summary\">");
                sb.AppendLine($"<div class=\"card\">Total<br/><strong>{total}</strong></div>");
                sb.AppendLine($"<div class=\"card passed\">Passed<br/><strong>{passed}</strong></div>");
                sb.AppendLine($"<div class=\"card failed\">Failed<br/><strong>{failed}</strong></div>");
                sb.AppendLine($"<div class=\"card other\">Other<br/><strong>{other}</strong></div>");
                sb.AppendLine("</div>");

                var byClass = _records.GroupBy(r => r.TestClass).OrderBy(g => g.Key);
                foreach (var group in byClass)
                {
                    sb.AppendLine("<div class=\"class\">");
                    sb.AppendLine($"<h2>{HtmlEncode(group.Key)}</h2>");
                    foreach (var r in group.OrderBy(r => r.TestName))
                    {
                        var statusClass = r.Status == "Passed" ? "passed" : r.Status == "Failed" ? "failed" : "other";
                        sb.AppendLine("<div class=\"test\">");
                        sb.AppendLine($"<div><strong>{HtmlEncode(r.TestName)}</strong> - <span class=\"{statusClass}\">{HtmlEncode(r.Status)}</span> <span style=\"color:#999;font-size:12px;\">{r.Timestamp:O}</span></div>");
                        if (!string.IsNullOrWhiteSpace(r.Message))
                            sb.AppendLine($"<div class=\"msg\">{HtmlEncode(r.Message)}</div>");
                        if (r.Status != "Passed" && !string.IsNullOrWhiteSpace(r.ScreenshotPath))
                        {
                            var relative = r.ScreenshotPath;
                            try
                            {
                                if (Path.IsPathRooted(relative))
                                    relative = Path.GetFileName(relative);
                            }
                            catch { }
                            var imgPath = ScreenshotsRelativeFolder + "/" + relative.Replace("\\", "/");
                            if (!imgPath.StartsWith(ScreenshotsRelativeFolder + "/"))
                                imgPath = ScreenshotsRelativeFolder + "/" + Path.GetFileName(relative).Replace("\\", "/");
                            sb.AppendLine($"<div><strong>Screenshot:</strong> {HtmlEncode(imgPath)}</div>");
                            sb.AppendLine($"<img src=\"{HtmlEncode(imgPath)}\" alt=\"screenshot\" />");
                        }
                        sb.AppendLine("</div>");
                    }
                    sb.AppendLine("</div>");
                }

                sb.AppendLine("</body></html>");
                File.WriteAllText(HtmlReportPath, sb.ToString());
            }
            catch { }
        }

        private static string GetGitInfo()
        {
            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = "rev-parse --abbrev-ref HEAD",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                };
                using (var p = System.Diagnostics.Process.Start(psi))
                {
                    var branch = p.StandardOutput.ReadToEnd().Trim();
                    p.WaitForExit(2000);
                    if (string.IsNullOrWhiteSpace(branch)) branch = "unknown";
                    // commit
                    var psi2 = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "git",
                        Arguments = "rev-parse --short HEAD",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                    };
                    using (var p2 = System.Diagnostics.Process.Start(psi2))
                    {
                        var commit = p2.StandardOutput.ReadToEnd().Trim();
                        p2.WaitForExit(2000);
                        if (string.IsNullOrWhiteSpace(commit)) commit = "unknown";
                        return branch + " @ " + commit;
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string HtmlEncode(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return value.Replace("&", "&amp;")
                       .Replace("<", "&lt;")
                       .Replace(">", "&gt;")
                       .Replace("\"", "&quot;");
        }
    }
}
