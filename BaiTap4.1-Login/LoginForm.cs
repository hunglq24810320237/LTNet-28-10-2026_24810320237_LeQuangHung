namespace LoginApp;

internal sealed class LoginForm : Form
{
    private readonly TextBox txtUsername = new();
    private readonly TextBox txtPassword = new();
    private readonly CheckBox chkShowPassword = new();
    private readonly Button btnLogin = new();
    private readonly Button btnExit = new();
    private readonly ErrorProvider errorProvider = new();

    public LoginForm()
    {
        Text = "Đăng nhập · Bài tập 4.1";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(480, 535);
        MinimumSize = new Size(440, 500);
        BackColor = Color.FromArgb(246, 248, 252);
        Font = new Font("Segoe UI", 10F);
        AcceptButton = btnLogin;
        CancelButton = btnExit;
        errorProvider.ContainerControl = this;
        BuildLayout();
        chkShowPassword.CheckedChanged += (_, _) => txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
        btnLogin.Click += Login;
        btnExit.Click += (_, _) => Close();
        txtUsername.Validating += (_, _) => ValidateRequired(txtUsername, "Vui lòng nhập tên đăng nhập.");
        txtPassword.Validating += (_, _) => ValidateRequired(txtPassword, "Vui lòng nhập mật khẩu.");
    }

    private void BuildLayout()
    {
        var card = new Panel { BackColor = Color.White, Size = new Size(390, 445), Location = new Point(45, 45), Padding = new Padding(30) };
        card.Paint += (_, e) => ControlPaint.DrawBorder(e.Graphics, card.ClientRectangle, Color.FromArgb(229, 233, 241), ButtonBorderStyle.Solid);
        Controls.Add(card);
        card.Controls.Add(new Label { Text = "◉", Font = new Font("Segoe UI Symbol", 30, FontStyle.Bold), ForeColor = Color.FromArgb(79, 70, 229), AutoSize = true, Location = new Point(28, 22) });
        card.Controls.Add(new Label { Text = "Chào mừng trở lại", Font = new Font("Segoe UI", 21, FontStyle.Bold), ForeColor = Color.FromArgb(26, 35, 55), AutoSize = true, Location = new Point(28, 82) });
        card.Controls.Add(new Label { Text = "Đăng nhập để tiếp tục vào hệ thống học viên.", ForeColor = Color.FromArgb(105, 115, 135), AutoSize = true, Location = new Point(30, 125) });
        AddField(card, "Tên đăng nhập", txtUsername, 175);
        AddField(card, "Mật khẩu", txtPassword, 255);
        txtPassword.UseSystemPasswordChar = true;
        chkShowPassword.Text = "Hiện mật khẩu";
        chkShowPassword.AutoSize = true;
        chkShowPassword.Location = new Point(31, 340);
        chkShowPassword.ForeColor = Color.FromArgb(85, 95, 115);
        card.Controls.Add(chkShowPassword);
        btnLogin.Text = "Đăng nhập";
        btnLogin.Location = new Point(30, 377);
        btnLogin.Size = new Size(150, 42);
        StyleButton(btnLogin, Color.FromArgb(79, 70, 229), Color.White);
        card.Controls.Add(btnLogin);
        btnExit.Text = "Thoát";
        btnExit.Location = new Point(192, 377);
        btnExit.Size = new Size(130, 42);
        StyleButton(btnExit, Color.FromArgb(239, 242, 248), Color.FromArgb(55, 65, 85));
        card.Controls.Add(btnExit);
        card.Controls.Add(new Label { Text = "Dùng thử: student  /  123456", ForeColor = Color.FromArgb(130, 140, 158), AutoSize = true, Location = new Point(30, 425) });
    }

    private static void AddField(Control parent, string title, TextBox input, int top)
    {
        parent.Controls.Add(new Label { Text = title, AutoSize = true, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Color.FromArgb(55, 65, 85), Location = new Point(30, top) });
        input.Location = new Point(30, top + 25);
        input.Size = new Size(292, 34);
        input.BorderStyle = BorderStyle.FixedSingle;
        input.Font = new Font("Segoe UI", 11F);
        parent.Controls.Add(input);
    }

    private static void StyleButton(Button button, Color back, Color fore)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.BackColor = back;
        button.ForeColor = fore;
        button.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        button.Cursor = Cursors.Hand;
    }

    private bool ValidateRequired(Control input, string message)
    {
        var valid = !string.IsNullOrWhiteSpace(input.Text);
        errorProvider.SetError(input, valid ? string.Empty : message);
        return valid;
    }

    private void Login(object? sender, EventArgs e)
    {
        var usernameOk = ValidateRequired(txtUsername, "Vui lòng nhập tên đăng nhập.");
        var passwordOk = ValidateRequired(txtPassword, "Vui lòng nhập mật khẩu.");
        if (!usernameOk || !passwordOk) return;
        if (Database.Validate(txtUsername.Text.Trim(), txtPassword.Text))
            MessageBox.Show(this, $"Đăng nhập thành công. Xin chào {txtUsername.Text.Trim()}!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        else
        {
            errorProvider.SetError(txtPassword, "Tên đăng nhập hoặc mật khẩu không chính xác.");
            MessageBox.Show(this, "Thông tin đăng nhập chưa chính xác.", "Không thể đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
