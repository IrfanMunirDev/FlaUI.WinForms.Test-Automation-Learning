using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Data;
using StudentManagement.Data.Entities;

namespace StudentManagement.WinForms
{
    public class MainForm : Form
    {
        private readonly AppDbContext _db = new AppDbContext();

        private List<Student> _students = new List<Student>();
        private List<Course> _courses = new List<Course>();
        private List<Enrollment> _enrollments = new List<Enrollment>();

        private MenuStrip menuStrip;
        private TabControl tabControl;

        // Students tab controls
        private TextBox txtFirstName;
        private TextBox txtLastName;
        private TextBox txtEmail;
        private GroupBox grpGender;
        private RadioButton rdoMale;
        private RadioButton rdoFemale;
        private ComboBox cmbCourse;
        private DateTimePicker dtpDateOfBirth;
        private CheckBox chkActive;
        private Button btnSave;
        private Button btnClear;

        // Student listing tab
        private DataGridView dgvStudents;

        // Courses tab
        private TextBox txtCourseName;
        private TextBox txtCourseDescription;
        private Button btnAddCourse;
        private DataGridView dgvCourses;

        // Enrollment tab
        private ComboBox cmbStudent;
        private ComboBox cmbCourseEnroll;
        private Button btnEnroll;
        private DataGridView dgvEnrollments;

        public MainForm()
        {
            Text = "Student Management Demo";
            Width = 920;
            Height = 640;
            StartPosition = FormStartPosition.CenterScreen;

            FormClosed += (s, e) => _db.Dispose();

            menuStrip = new MenuStrip();
            var fileMenu = new ToolStripMenuItem("File");
            var exitItem = new ToolStripMenuItem("Exit");
            exitItem.Click += (s, e) => Close();
            fileMenu.DropDownItems.Add(exitItem);
            menuStrip.Items.Add(fileMenu);
            MainMenuStrip = menuStrip;
            Controls.Add(menuStrip);

            tabControl = new TabControl { Name = "tabControl", Dock = DockStyle.Fill };
            Controls.Add(tabControl);

            var tabStudents = new TabPage("Students") { Name = "tabStudents" };
            BuildStudentsTab(tabStudents);
            tabControl.TabPages.Add(tabStudents);

            var tabCourses = new TabPage("Courses") { Name = "tabCourses" };
            BuildCoursesTab(tabCourses);
            tabControl.TabPages.Add(tabCourses);

            var tabListing = new TabPage("Student Listing") { Name = "tabStudentListing" };
            BuildListingTab(tabListing);
            tabControl.TabPages.Add(tabListing);

            var tabEnrollment = new TabPage("Enrollment") { Name = "tabEnrollment" };
            BuildEnrollmentTab(tabEnrollment);
            tabControl.TabPages.Add(tabEnrollment);

            tabControl.BringToFront();
        }

        private void BuildStudentsTab(TabPage page)
        {
            int x = 20, y = 20, labelWidth = 110, row = 32;

            AddLabelAndControl(page, "First Name", txtFirstName = MakeTextBox("txtFirstName"), x, y); y += row;
            AddLabelAndControl(page, "Last Name", txtLastName = MakeTextBox("txtLastName"), x, y); y += row;
            AddLabelAndControl(page, "Email", txtEmail = MakeTextBox("txtEmail"), x, y); y += row;

            var lblGender = new Label { Text = "Gender", Left = x, Top = y + 3, Width = labelWidth };
            page.Controls.Add(lblGender);
            grpGender = new GroupBox { Name = "grpGender", Text = "Gender", Left = x + labelWidth, Top = y - 5, Width = 220, Height = 52 };
            rdoMale = new RadioButton { Name = "rdoMale", Text = "Male", Left = 10, Top = 20, Width = 80 };
            rdoFemale = new RadioButton { Name = "rdoFemale", Text = "Female", Left = 100, Top = 20, Width = 90 };
            grpGender.Controls.Add(rdoMale);
            grpGender.Controls.Add(rdoFemale);
            page.Controls.Add(grpGender);
            y += 62;

            AddLabelAndControl(page, "Course", cmbCourse = MakeComboBox("cmbCourse"), x, y); y += row;
            AddLabelAndControl(page, "Date of Birth", dtpDateOfBirth = MakeDateTimePicker("dtpDateOfBirth"), x, y); y += row;

            chkActive = new CheckBox { Name = "chkActive", Text = "Active", Left = x + labelWidth, Top = y, Width = 220, Checked = true };
            page.Controls.Add(chkActive);
            y += row;

            btnSave = new Button { Name = "btnSave", Text = "Save", Left = x + labelWidth, Top = y, Width = 90 };
            btnClear = new Button { Name = "btnClear", Text = "Clear", Left = x + labelWidth + 100, Top = y, Width = 90 };
            btnSave.Click += BtnSave_Click;
            btnClear.Click += (s, e) => ClearStudentForm();
            page.Controls.Add(btnSave);
            page.Controls.Add(btnClear);
        }

        private void BuildCoursesTab(TabPage page)
        {
            int x = 20, y = 20, labelWidth = 110, row = 32;

            AddLabelAndControl(page, "Course Name", txtCourseName = MakeTextBox("txtCourseName"), x, y); y += row;
            AddLabelAndControl(page, "Description", txtCourseDescription = MakeTextBox("txtCourseDescription"), x, y); y += row;

            btnAddCourse = new Button { Name = "btnAddCourse", Text = "Add Course", Left = x + labelWidth, Top = y, Width = 110 };
            btnAddCourse.Click += BtnAddCourse_Click;
            page.Controls.Add(btnAddCourse);
            y += 42;

            dgvCourses = new DataGridView
            {
                Name = "dgvCourses", Left = x, Top = y, Width = 860, Height = 400,
                AllowUserToAddRows = false, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            page.Controls.Add(dgvCourses);
        }

        private void BuildListingTab(TabPage page)
        {
            dgvStudents = new DataGridView
            {
                Name = "dgvStudents", Left = 20, Top = 20, Width = 860, Height = 520,
                AllowUserToAddRows = false, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            page.Controls.Add(dgvStudents);
        }

        private void BuildEnrollmentTab(TabPage page)
        {
            int x = 20, y = 20, labelWidth = 110, row = 32;

            AddLabelAndControl(page, "Student", cmbStudent = MakeComboBox("cmbStudent"), x, y); y += row;
            AddLabelAndControl(page, "Course", cmbCourseEnroll = MakeComboBox("cmbCourseEnroll"), x, y); y += row;

            btnEnroll = new Button { Name = "btnEnroll", Text = "Enroll", Left = x + labelWidth, Top = y, Width = 90 };
            btnEnroll.Click += BtnEnroll_Click;
            page.Controls.Add(btnEnroll);
            y += 42;

            dgvEnrollments = new DataGridView
            {
                Name = "dgvEnrollments", Left = x, Top = y, Width = 860, Height = 400,
                AllowUserToAddRows = false, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            page.Controls.Add(dgvEnrollments);
        }

        private static TextBox MakeTextBox(string name) => new TextBox { Name = name, Width = 220 };
        private static ComboBox MakeComboBox(string name) => new ComboBox { Name = name, Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
        private static DateTimePicker MakeDateTimePicker(string name) => new DateTimePicker { Name = name, Width = 220, Format = DateTimePickerFormat.Short };

        private static void AddLabelAndControl(TabPage page, string labelText, Control control, int x, int y)
        {
            var label = new Label { Text = labelText, Left = x, Top = y + 3, Width = 110 };
            control.Left = x + 110;
            control.Top = y;
            page.Controls.Add(label);
            page.Controls.Add(control);
            control.AccessibleName = control.Name;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            try
            {
                _db.Database.EnsureCreated();
                Seed();
                RefreshAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database initialization failed:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            SetAccessibleNames(this);
        }

        private void Seed()
        {
            if (!_db.Courses.Any())
            {
                _db.Courses.AddRange(
                    new Course { Name = "Math 101", Description = "Basic mathematics" },
                    new Course { Name = "Physics 101", Description = "Basic physics" },
                    new Course { Name = "Chemistry 101", Description = "Basic chemistry" });
                _db.SaveChanges();
            }

            if (!_db.Students.Any())
            {
                var courses = _db.Courses.ToList();
                _db.Students.AddRange(
                    new Student { FirstName = "John", LastName = "Doe", Email = "john@example.com", Gender = "Male", DateOfBirth = new DateTime(2000, 1, 15), CourseId = courses.FirstOrDefault(c => c.Name == "Math 101")?.Id, IsActive = true },
                    new Student { FirstName = "Jane", LastName = "Smith", Email = "jane@example.com", Gender = "Female", DateOfBirth = new DateTime(1999, 5, 20), CourseId = courses.FirstOrDefault(c => c.Name == "Physics 101")?.Id, IsActive = true });
                _db.SaveChanges();
            }
        }

        private void RefreshAll()
        {
            _students = _db.Students.AsNoTracking().Include(s => s.Course).ToList();
            _courses = _db.Courses.AsNoTracking().ToList();
            _enrollments = _db.Enrollments.AsNoTracking().Include(e => e.Student).Include(e => e.Course).ToList();

            dgvStudents.DataSource = _students.Select(s => new
            {
                s.Id,
                s.FirstName,
                s.LastName,
                s.Email,
                Gender = s.Gender,
                DateOfBirth = s.DateOfBirth,
                Course = s.Course != null ? s.Course.Name : null,
                s.IsActive
            }).ToList();

            dgvCourses.DataSource = _courses.Select(c => new { c.Id, c.Name, c.Description }).ToList();

            dgvEnrollments.DataSource = _enrollments.Select(en => new
            {
                en.Id,
                Student = en.Student != null ? (en.Student.FirstName + " " + en.Student.LastName) : null,
                Course = en.Course != null ? en.Course.Name : null,
                en.EnrolledOn
            }).ToList();

            cmbCourse.Items.Clear();
            foreach (var c in _courses) cmbCourse.Items.Add(c.Name);
            if (cmbCourse.Items.Count > 0 && cmbCourse.SelectedIndex < 0) cmbCourse.SelectedIndex = 0;

            cmbCourseEnroll.Items.Clear();
            foreach (var c in _courses) cmbCourseEnroll.Items.Add(c.Name);
            if (cmbCourseEnroll.Items.Count > 0 && cmbCourseEnroll.SelectedIndex < 0) cmbCourseEnroll.SelectedIndex = 0;

            cmbStudent.Items.Clear();
            foreach (var s in _students) cmbStudent.Items.Add(s.FirstName + " " + s.LastName);
            if (cmbStudent.Items.Count > 0 && cmbStudent.SelectedIndex < 0) cmbStudent.SelectedIndex = 0;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text)) { Warn("First Name is required."); return; }
            if (string.IsNullOrWhiteSpace(txtLastName.Text)) { Warn("Last Name is required."); return; }
            if (!string.IsNullOrWhiteSpace(txtEmail.Text) && !IsValidEmail(txtEmail.Text)) { Warn("Invalid email format."); return; }
            var gender = rdoMale.Checked ? "Male" : rdoFemale.Checked ? "Female" : string.Empty;
            if (string.IsNullOrEmpty(gender)) { Warn("Please select a gender."); return; }
            if (cmbCourse.SelectedItem == null) { Warn("Please select a course."); return; }

            try
            {
                var course = _db.Courses.FirstOrDefault(c => c.Name == cmbCourse.SelectedItem.ToString());

                _db.Students.Add(new Student
                {
                    FirstName = txtFirstName.Text.Trim(),
                    LastName = txtLastName.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Gender = gender,
                    DateOfBirth = dtpDateOfBirth.Value,
                    CourseId = course?.Id,
                    IsActive = chkActive.Checked
                });
                _db.SaveChanges();

                RefreshAll();
                MessageBox.Show("Student saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearStudentForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Save failed:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearStudentForm()
        {
            txtFirstName.Clear();
            txtLastName.Clear();
            txtEmail.Clear();
            rdoMale.Checked = false;
            rdoFemale.Checked = false;
            chkActive.Checked = true;
            if (cmbCourse.Items.Count > 0) cmbCourse.SelectedIndex = 0;
            dtpDateOfBirth.Value = DateTime.Today;
        }

        private void BtnAddCourse_Click(object sender, EventArgs e)
        {
            var name = txtCourseName.Text.Trim();
            if (string.IsNullOrWhiteSpace(name)) { Warn("Course name is required."); return; }
            try
            {
                if (_db.Courses.AsEnumerable().Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase))) { Warn("Course already exists."); return; }
                _db.Courses.Add(new Course { Name = name, Description = txtCourseDescription.Text.Trim() });
                _db.SaveChanges();
                txtCourseName.Clear();
                txtCourseDescription.Clear();
                RefreshAll();
                MessageBox.Show("Course added.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Add course failed:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEnroll_Click(object sender, EventArgs e)
        {
            if (cmbStudent.SelectedItem == null) { Warn("Select a student."); return; }
            if (cmbCourseEnroll.SelectedItem == null) { Warn("Select a course."); return; }

            try
            {
                var student = _students.FirstOrDefault(s => (s.FirstName + " " + s.LastName) == cmbStudent.SelectedItem.ToString());
                var course = _courses.FirstOrDefault(c => c.Name == cmbCourseEnroll.SelectedItem.ToString());
                if (student == null || course == null) { Warn("Invalid selection."); return; }
                if (_db.Enrollments.Any(en => en.StudentId == student.Id && en.CourseId == course.Id)) { Warn("Student is already enrolled in this course."); return; }

                _db.Enrollments.Add(new Enrollment { StudentId = student.Id, CourseId = course.Id, EnrolledOn = DateTime.Now });
                _db.SaveChanges();
                RefreshAll();
                MessageBox.Show("Enrollment successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Enrollment failed:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Warn(string message) => MessageBox.Show(message, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private static void SetAccessibleNames(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (!string.IsNullOrEmpty(c.Name)) c.AccessibleName = c.Name;
                if (c.HasChildren) SetAccessibleNames(c);
            }
        }
    }
}
