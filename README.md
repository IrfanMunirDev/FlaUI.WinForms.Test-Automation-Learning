# FlaUI.WinForms.Learning

A small Windows Forms desktop application created as a learning target for automated UI testing with **FlaUI** (`FlaUI.UIA3`).

This is intentionally **not** a production student-management system. It exists to provide stable, discoverable UI controls for practicing FlaUI automation.

## Solution structure

```
FlaUI.WinForms.Learning.sln
├── StudentManagement.Data          (EF Core entities + DbContext + LocalDB)
├── StudentManagement.WinForms      (Windows Forms desktop app, .NET Framework 4.8)
└── StudentManagement.Tests.UI      (NUnit + FlaUI.UIA3 automation tests)
```

## Prerequisites

- Visual Studio 2022 (17.x) with the **.NET desktop development** workload
- .NET Framework 4.8 Developer Pack (targeting pack)
- .NET SDK 6+ (for building SDK-style projects; 10.x is fine)
- SQL Server LocalDB (instance `MSSQLLocalDB`)
- NuGet package restore (internet access)

## Technology stack

- C# / Windows Forms
- .NET Framework 4.8
- Entity Framework Core **3.1.32** (see note below)
- SQL Server LocalDB
- NUnit 4.3.2
- FlaUI 5.0.0 (UIA3)
- Microsoft.NET.Test.Sdk 17.11.1

## Entity Framework Core 3.1 — end-of-support notice

EF Core 3.1 is the **last** EF Core branch that supports .NET Framework 4.8, because EF Core 5.0+ targets .NET Standard 2.1, which .NET Framework 4.8 does not implement.

EF Core 3.1 reached **end of support on December 13, 2022**. It is used here only because the application targets .NET Framework 4.8 for learning purposes. For new/production work, target a modern .NET (6/8) and a current EF Core version.

## Building

1. Open `FlaUI.WinForms.Learning.sln` in Visual Studio 2022.
2. Restore NuGet packages (automatic on build, or right-click solution → Restore NuGet Packages).
3. Build the solution: `Ctrl+Shift+B` (or `dotnet build FlaUI.WinForms.Learning.sln`).

## Running the application

1. Start LocalDB if needed:
   ```
   sqllocaldb start MSSQLLocalDB
   ```
2. Set `StudentManagement.WinForms` as the startup project and press `F5`.

The app opens with four tabs: **Students**, **Courses**, **Student Listing**, **Enrollment**.

## Running the UI tests

- Open Test Explorer (`Test → Test Explorer`) and run all tests, or
- `dotnet test StudentManagement.Tests.UI/StudentManagement.Tests.UI.csproj`

The test project currently contains only a smoke test; full FlaUI interaction tests are added incrementally.

## Screenshots and reporting

UI tests capture screenshots and write a simple text report to help diagnose failures.

### Automatic screenshots (failed / errored tests)

- When a test does **not** pass (failed or errored), `UiTestBase.TearDown` captures a screenshot of the main window and attaches it to the NUnit result via `TestContext.AddTestAttachment`.
- Screenshots are saved under the test output folder:
  - `<test output>/Screenshots/<TestName>_<timestamp>.png`
- In Visual Studio Test Explorer, open the failed test and view **Attachments** to see the screenshot. In `.trx`/CI output, attachments are listed with the test result.

### Manual screenshots (any scenario)

Use `ScreenshotHelper` to capture a window or control at any point in a test:

```csharp
var path = ScreenshotHelper.CaptureWindow(MainWindow, "AfterSave");
var btnPath = ScreenshotHelper.CaptureElement(FindButton("btnSave"), "SaveButton");
```

Both methods create the `Screenshots` folder if needed and return the saved file path.

### Test report

- A plain-text report is written incrementally as tests run:
  - `<test output>/TestReport.txt`
- Each line contains timestamp, test name, status, and (on failure) the message:
  - `[2026-...] TestName: Failed - ...`
- The report is reset once per test run by `TestSuiteSetup` (`[SetUpFixture]`).

### Notes

- Automatic screenshots are only taken for non-passing tests to keep successful runs fast and avoid large output folders.
- Manual capture can be used in passing tests or for documentation purposes.


## Automation identifiers

All interactive controls have stable `Name` (and `AccessibleName`) values so FlaUI can discover them by `AutomationId`/`Name`, e.g. `txtFirstName`, `txtLastName`, `txtEmail`, `rdoMale`, `rdoFemale`, `cmbCourse`, `dtpDateOfBirth`, `chkActive`, `btnSave`, `btnClear`, `dgvStudents`, `tabStudents`, `tabCourses`, `tabStudentListing`, `tabEnrollment`, `cmbStudent`, `cmbCourseEnroll`, `btnEnroll`, `btnAddCourse`, `dgvCourses`.

Do not depend on control coordinates or screen positions.

## Database

Connection string (in `AppDbContext`): `Server=(localdb)\MSSQLLocalDB;Database=StudentManagement;Trusted_Connection=True;`. Database creation/migrations are added in later steps.

## Development history (prompts)

This project was built incrementally, one prompt at a time:

- **Prompt 1 — Solution scaffold.** Created `FlaUI.WinForms.Learning.sln` with three projects: `StudentManagement.WinForms` (WinForms, .NET Framework 4.8), `StudentManagement.Data` (EF Core entities + DbContext + LocalDB), and `StudentManagement.Tests.UI` (NUnit + FlaUI.UIA3). Documented EF Core 3.1 end-of-support (last EF Core branch supporting .NET Framework 4.8).
- **Prompt 2 — WinForms application.** Built the 4 tabs (Students, Courses, Student Listing, Enrollment) with registration controls, validation, MessageBox dialogs, and sample data.
- **Prompt 3 — SQL Server + EF Core.** Connected to LocalDB via `AppDbContext`, `EnsureCreated`, idempotent seed, CRUD operations for students, courses, and enrollments.
- **Prompt 4 — First FlaUI automation tests.** Application launch, window discovery, textbox interaction, button clicks (9 tests).
- **Prompt 5 — DataGridView automation.** Grid discovery, columns, rows, cells, selection, and data verification, including add-course and enroll flows (8 tests).
- **Prompt 6 — Complete the learning test suite.** Added dropdown, checkbox, tab, validation-message, dialog, screenshot, and reporting tests, plus shared `UiTestBase`, `ScreenshotHelper`, `ReportHelper`, and a `[SetUpFixture]` report reset.
  - Tests: `DropdownTests`, `CheckboxTests`, `TabTests`, `ValidationMessageTests`, `DialogTests`, `ScreenshotTests`, `ReportingTests`.
  - Infrastructure: `UiTestBase` (shared setup/teardown, find helpers, `SelectTab`, `ClickAndDismiss`, `GetComboBoxItems`), `ScreenshotHelper`, `ReportHelper`, `TestSuiteSetup`.
  - Screenshots are captured automatically on failed/errored tests and attached to NUnit results; manual capture is available via `ScreenshotHelper`.
  - A per-run text report is written to `<test output>/TestReport.txt`.

## Why FlaUI (and why not TestComplete)

- **FlaUI** is free, open-source (MIT), built on native Windows UI Automation (UIA2/UIA3), and integrates with NUnit/xUnit/MSTest. There is no license cost, no vendor lock-in, and skills transfer across any .NET desktop project. It is ideal for learning.
- **TestComplete** (SmartBear) is a commercial, paid tool with per-seat licensing, proprietary scripting, and a heavier setup. We intentionally do **not** use it, to keep the learning stack free, transparent, and transferable. FlaUI gives us the same core capability — automating Windows Forms controls — at zero cost.

## Next goal

The next phase will introduce **Infragistics WinForms controls** (third-party/commercial controls) and automate them with FlaUI, to learn how to handle non-standard control automation (custom AutomationIds, composite controls, vendor-specific grids). This will be developed on a separate branch and merged via Pull Request.

## What's better for learning (and for new devs)

- Start with standard WinForms controls and stable, meaningful `AutomationId`/`Name` values.
- Keep architecture flat — no unnecessary repositories, abstractions, or patterns.
- Write small, focused UI tests; discover controls semantically (by AutomationId/Name), never by coordinates.
- Use free tooling (FlaUI + NUnit) so the skills transfer to any project.
- Build incrementally: scaffold → app → data → first tests → grids → advanced scenarios.
