namespace RegistrationApp;

internal sealed class RegistrationForm : Form
{
    private readonly TextBox txtName = new();
    private readonly MaskedTextBox mtxtPhone = new("(000) 000-0000");
    private readonly DateTimePicker dtpBirthDate = new();
    private readonly ComboBox cboCourse = new();
    private readonly RadioButton rdoFemale = new();
    private readonly RadioButton rdoMale = new();
    private readonly CheckBox chkOnline = new();
    private readonly CheckBox chkMaterials = new();
    private readonly Label lblSaved = new();
    private readonly ErrorProvider errors = new();

    public RegistrationForm()
    {
        Text = "Đăng ký học viên · Bài tập 4.2";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(840, 650);
        MinimumSize = new Size(790, 620);
        BackColor = Color.FromArgb(246, 248, 252);
        Font = new Font("Segoe UI", 10F);
        errors.ContainerControl = this;
        BuildLayout();
        LoadCourses();
    }

    private void BuildLayout()
    {
        Controls.Add(new Label { Text = "Hồ sơ học viên", Font = new Font("Segoe UI", 23, FontStyle.Bold), ForeColor = Color.FromArgb(27, 39, 64), AutoSize = true, Location = new Point(45, 27) });
        Controls.Add(new Label { Text = "Điền thông tin bên dưới để lưu đăng ký vào cơ sở dữ liệu.", ForeColor = Color.FromArgb(105, 115, 135), AutoSize = true, Location = new Point(48, 73) });
        var card = new Panel { BackColor = Color.White, Location = new Point(42, 112), Size = new Size(755, 440), Padding = new Padding(25) };
        card.Paint += (_, e) => ControlPaint.DrawBorder(e.Graphics, card.ClientRectangle, Color.FromArgb(229, 233, 241), ButtonBorderStyle.Solid);
        Controls.Add(card);
        AddField(card, "Họ và tên", txtName, 25, 25, 310);
        AddField(card, "Số điện thoại", mtxtPhone, 390, 25, 310);
        AddField(card, "Ngày sinh", dtpBirthDate, 25, 112, 310);
        AddField(card, "Khóa học", cboCourse, 390, 112, 310);
        dtpBirthDate.Format = DateTimePickerFormat.Custom;
        dtpBirthDate.CustomFormat = "dd/MM/yyyy";
        dtpBirthDate.MaxDate = DateTime.Today;
        cboCourse.DropDownStyle = ComboBoxStyle.DropDownList;
        cboCourse.DisplayMember = nameof(Course.Name);
        cboCourse.ValueMember = nameof(Course.Id);
        AddSection(card, "Giới tính", 25, 205);
        rdoFemale.Text = "Nữ"; rdoFemale.Location = new Point(35, 240); rdoFemale.AutoSize = true; rdoFemale.Checked = true;
        rdoMale.Text = "Nam"; rdoMale.Location = new Point(115, 240); rdoMale.AutoSize = true;
        card.Controls.Add(rdoFemale); card.Controls.Add(rdoMale);
        AddSection(card, "Hình thức & dịch vụ", 390, 205);
        chkOnline.Text = "Học trực tuyến"; chkOnline.AutoSize = true; chkOnline.Location = new Point(400, 240);
        chkMaterials.Text = "Nhận tài liệu khóa học"; chkMaterials.AutoSize = true; chkMaterials.Location = new Point(400, 275);
        card.Controls.Add(chkOnline); card.Controls.Add(chkMaterials);
        var hint = new Label { Text = "Thông tin được lưu trong SQL Server.", AutoSize = true, ForeColor = Color.FromArgb(120, 130, 148), Location = new Point(30, 345) };
        card.Controls.Add(hint);
        var register = new Button { Text = "Đăng ký học viên", Location = new Point(30, 380), Size = new Size(190, 42) };
        StyleButton(register, Color.FromArgb(79, 70, 229), Color.White);
        register.Click += Register;
        card.Controls.Add(register);
        lblSaved.Location = new Point(240, 393); lblSaved.AutoSize = true; lblSaved.ForeColor = Color.FromArgb(44, 125, 87);
        card.Controls.Add(lblSaved);
        Controls.Add(new Label { Text = "ComboBox · RadioButton · CheckBox · DateTimePicker · MaskedTextBox", AutoSize = true, ForeColor = Color.FromArgb(110, 120, 140), Location = new Point(46, 575) });
    }

    private void LoadCourses()
    {
        cboCourse.DataSource = Database.GetCourses();
    }

    private static void AddField(Control parent, string title, Control input, int x, int y, int width)
    {
        parent.Controls.Add(new Label { Text = title, AutoSize = true, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Color.FromArgb(55, 65, 85), Location = new Point(x, y) });
        input.Location = new Point(x, y + 27); input.Size = new Size(width, 35);
        input.Font = new Font("Segoe UI", 11F);
        if (input is TextBox box) box.BorderStyle = BorderStyle.FixedSingle;
        if (input is MaskedTextBox masked) { masked.BorderStyle = BorderStyle.FixedSingle; masked.PromptChar = '_'; }
        if (input is DateTimePicker date) date.CalendarMonthBackground = Color.White;
        parent.Controls.Add(input);
    }

    private static void AddSection(Control parent, string title, int x, int y) => parent.Controls.Add(new Label { Text = title, AutoSize = true, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Color.FromArgb(55, 65, 85), Location = new Point(x, y) });

    private static void StyleButton(Button button, Color back, Color fore)
    {
        button.FlatStyle = FlatStyle.Flat; button.FlatAppearance.BorderSize = 0; button.BackColor = back; button.ForeColor = fore;
        button.Font = new Font("Segoe UI", 10F, FontStyle.Bold); button.Cursor = Cursors.Hand;
    }

    private void Register(object? sender, EventArgs e)
    {
        errors.Clear();
        var valid = true;
        if (string.IsNullOrWhiteSpace(txtName.Text)) { errors.SetError(txtName, "Vui lòng nhập họ tên."); valid = false; }
        if (!mtxtPhone.MaskCompleted) { errors.SetError(mtxtPhone, "Nhập đủ số điện thoại theo mẫu (000) 000-0000."); valid = false; }
        if (cboCourse.SelectedItem is not Course) { errors.SetError(cboCourse, "Vui lòng chọn khóa học."); valid = false; }
        if (!valid) return;

        var course = (Course)cboCourse.SelectedItem!;
        var record = new Registration
        {
            FullName = txtName.Text.Trim(), Phone = mtxtPhone.Text, BirthDate = dtpBirthDate.Value.Date,
            Gender = rdoFemale.Checked ? "Nữ" : "Nam", CourseId = course.Id, CourseName = course.Name,
            IsOnline = chkOnline.Checked, NeedsMaterials = chkMaterials.Checked
        };
        record = Database.Save(record);
        lblSaved.Text = $"Đã lưu {record.StudentId} · Tổng {Database.Count()} lượt đăng ký";
        var summary = $"Mã sinh viên: {record.StudentId}\nMã đăng ký: {record.Id}\nHọ tên: {record.FullName}\nĐiện thoại: {record.Phone}\nNgày sinh: {record.BirthDate:dd/MM/yyyy}\nGiới tính: {record.Gender}\nKhóa học: {record.CourseName}\nHọc trực tuyến: {(record.IsOnline ? "Có" : "Không")}\nNhận tài liệu: {(record.NeedsMaterials ? "Có" : "Không")}";
        MessageBox.Show(this, summary, "Đăng ký thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        txtName.Clear(); mtxtPhone.Clear(); txtName.Focus();
    }
}
