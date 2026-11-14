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

## Automation identifiers

All interactive controls have stable `Name` (and `AccessibleName`) values so FlaUI can discover them by `AutomationId`/`Name`, e.g. `txtFirstName`, `txtLastName`, `txtEmail`, `rdoMale`, `rdoFemale`, `cmbCourse`, `dtpDateOfBirth`, `chkActive`, `btnSave`, `btnClear`, `dgvStudents`, `tabStudents`, `tabCourses`, `tabStudentListing`, `tabEnrollment`, `cmbStudent`, `cmbCourseEnroll`, `btnEnroll`, `btnAddCourse`, `dgvCourses`.

Do not depend on control coordinates or screen positions.

## Database

Connection string (in `AppDbContext`): `Server=(localdb)\MSSQLLocalDB;Database=StudentManagement;Trusted_Connection=True;`. Database creation/migrations are added in later steps.
