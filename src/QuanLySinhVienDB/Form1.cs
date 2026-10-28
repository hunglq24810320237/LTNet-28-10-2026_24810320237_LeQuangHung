using MySql.Data.MySqlClient;
using System.Data;
using System.Globalization;

namespace QuanLySinhVienDB;

public sealed class Form1 : Form
{
    private readonly TextBox txtMaSV = new();
    private readonly TextBox txtHoTen = new();
    private readonly DateTimePicker dtpNgaySinh = new();
    private readonly ComboBox cboGioiTinh = new();
    private readonly TextBox txtLop = new();
    private readonly NumericUpDown nudDiemTB = new();
    private readonly DataGridView dgvSinhVien = new();
    private readonly ErrorProvider errorProvider = new();
    private readonly Label lblStatus = new();

    private readonly Button btnThem = new();
    private readonly Button btnCapNhat = new();
    private readonly Button btnXoa = new();
    private readonly Button btnLamMoi = new();

    private static string ConnectionString
    {
        get
        {
            var host = Environment.GetEnvironmentVariable("BAITAP_MYSQL_SERVER") ?? "127.0.0.1";
            var port = Environment.GetEnvironmentVariable("BAITAP_MYSQL_PORT") ?? "3307";
            var user = Environment.GetEnvironmentVariable("BAITAP_MYSQL_USER") ?? "root";
            var password = Environment.GetEnvironmentVariable("BAITAP_MYSQL_PASSWORD") ?? "";
            return $"Server={host};Port={port};Database=ql_sinhvien;Uid={user};Pwd={password};Charset=utf8mb4;";
        }
    }

    public Form1()
    {
        Text = "Hệ thống Quản lý Sinh viên - Kết nối MySQL Database";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(850, 520);
        Size = new Size(950, 620);
        Font = new Font("Segoe UI", 10F);
        BuildInterface();
        Load += (_, _) => LoadStudents(showError: true);
    }

    private void BuildInterface()
    {
        var details = new GroupBox
        {
            Text = "Thông tin chi tiết sinh viên",
            Dock = DockStyle.Top,
            Height = 200,
            Padding = new Padding(12)
        };
        Controls.Add(details);

        AddField(details, "Mã SV:", txtMaSV, 20, 26, 215);
        AddField(details, "Họ và tên:", txtHoTen, 285, 26, 230);
        dtpNgaySinh.Format = DateTimePickerFormat.Custom;
        dtpNgaySinh.CustomFormat = "dd/MM/yyyy";
        AddField(details, "Ngày sinh:", dtpNgaySinh, 625, 26, 190);

        cboGioiTinh.DropDownStyle = ComboBoxStyle.DropDownList;
        cboGioiTinh.Items.AddRange(["Nam", "Nữ", "Khác"]);
        cboGioiTinh.SelectedIndex = 0;
        AddField(details, "Giới tính:", cboGioiTinh, 20, 72, 215);
        AddField(details, "Lớp học:", txtLop, 285, 72, 230);

        nudDiemTB.DecimalPlaces = 2;
        nudDiemTB.Minimum = 0;
        nudDiemTB.Maximum = 10;
        nudDiemTB.Increment = 0.25M;
        nudDiemTB.Width = 190;
        AddField(details, "Điểm TB:", nudDiemTB, 625, 72, 190);

        StyleButton(btnThem, "Thêm mới", Color.FromArgb(0, 120, 215));
        StyleButton(btnCapNhat, "Cập nhật", Color.WhiteSmoke, Color.Black);
        StyleButton(btnXoa, "Xóa SV", Color.FromArgb(220, 75, 75));
        StyleButton(btnLamMoi, "Làm mới", Color.WhiteSmoke, Color.Black);
        btnThem.Click += (_, _) => AddStudent();
        btnCapNhat.Click += (_, _) => UpdateStudent();
        btnXoa.Click += (_, _) => DeleteStudent();
        btnLamMoi.Click += (_, _) => ClearForm();

        var buttons = new FlowLayoutPanel
        {
            Location = new Point(100, 123),
            Size = new Size(620, 45),
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight
        };
        foreach (var button in new[] { btnThem, btnCapNhat, btnXoa, btnLamMoi })
        {
            button.Margin = new Padding(0, 0, 18, 0);
            buttons.Controls.Add(button);
        }
        details.Controls.Add(buttons);

        dgvSinhVien.Dock = DockStyle.Fill;
        dgvSinhVien.ReadOnly = true;
        dgvSinhVien.AllowUserToAddRows = false;
        dgvSinhVien.AllowUserToDeleteRows = false;
        dgvSinhVien.MultiSelect = false;
        dgvSinhVien.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvSinhVien.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvSinhVien.RowHeadersWidth = 48;
        dgvSinhVien.BackgroundColor = Color.White;
        dgvSinhVien.SelectionChanged += (_, _) => CopySelectedRowToForm();
        dgvSinhVien.ColumnHeadersDefaultCellStyle.Font = new Font(Font, FontStyle.Regular);

        lblStatus.Dock = DockStyle.Bottom;
        lblStatus.Height = 26;
        lblStatus.Padding = new Padding(8, 3, 0, 0);
        lblStatus.ForeColor = Color.DimGray;
        Controls.Add(dgvSinhVien);
        Controls.Add(lblStatus);

        errorProvider.ContainerControl = this;
    }

    private static void AddField(Control parent, string caption, Control input, int x, int y, int width)
    {
        var label = new Label
        {
            Text = caption,
            Location = new Point(x, y + 3),
            Size = new Size(78, 25),
            TextAlign = ContentAlignment.MiddleLeft
        };
        input.Location = new Point(x + 78, y);
        input.Size = new Size(width - 78, 29);
        if (input is TextBox textBox)
            textBox.MaxLength = caption.StartsWith("Mã") ? 20 : 100;
        parent.Controls.Add(label);
        parent.Controls.Add(input);
    }

    private static void StyleButton(Button button, string text, Color backColor, Color? foreColor = null)
    {
        button.Text = text;
        button.Size = new Size(110, 38);
        button.BackColor = backColor;
        button.ForeColor = foreColor ?? Color.White;
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderColor = backColor == Color.WhiteSmoke ? Color.LightGray : backColor;
        button.UseVisualStyleBackColor = false;
    }

    private bool ValidateInput()
    {
        errorProvider.Clear();
        var valid = true;
        if (string.IsNullOrWhiteSpace(txtMaSV.Text))
        {
            errorProvider.SetError(txtMaSV, "Vui lòng nhập mã sinh viên.");
            valid = false;
        }
        if (string.IsNullOrWhiteSpace(txtHoTen.Text))
        {
            errorProvider.SetError(txtHoTen, "Vui lòng nhập họ và tên.");
            valid = false;
        }
        if (string.IsNullOrWhiteSpace(txtLop.Text))
        {
            errorProvider.SetError(txtLop, "Vui lòng nhập lớp.");
            valid = false;
        }
        return valid;
    }

    private void AddStudent()
    {
        if (!ValidateInput()) return;
        ExecuteWrite(
            "INSERT INTO SinhVien (MaSV, HoTen, NgaySinh, GioiTinh, Lop, DiemTB) " +
            "VALUES (@ma, @ten, @ngaySinh, @gioiTinh, @lop, @diem)",
            "Thêm mới sinh viên thành công!", "Không thể thêm sinh viên", clearAfter: true);
    }

    private void UpdateStudent()
    {
        if (!ValidateInput()) return;
        ExecuteWrite(
            "UPDATE SinhVien SET HoTen=@ten, NgaySinh=@ngaySinh, GioiTinh=@gioiTinh, Lop=@lop, DiemTB=@diem WHERE MaSV=@ma",
            "Cập nhật sinh viên thành công!", "Không thể cập nhật sinh viên", clearAfter: false);
    }

    private void DeleteStudent()
    {
        var ma = txtMaSV.Text.Trim();
        if (string.IsNullOrWhiteSpace(ma))
        {
            errorProvider.SetError(txtMaSV, "Chọn hoặc nhập mã sinh viên cần xóa.");
            return;
        }
        if (MessageBox.Show($"Bạn có chắc muốn xóa sinh viên {ma}?", "Xác nhận xóa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

        try
        {
            using var connection = new MySqlConnection(ConnectionString);
            using var command = new MySqlCommand("DELETE FROM SinhVien WHERE MaSV=@ma", connection);
            command.Parameters.AddWithValue("@ma", ma);
            connection.Open();
            if (command.ExecuteNonQuery() == 0)
                MessageBox.Show("Không tìm thấy mã sinh viên cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Xóa sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearForm();
            LoadStudents(showError: false);
        }
        catch (MySqlException ex)
        {
            ShowDatabaseError("Không thể xóa sinh viên", ex);
        }
    }

    private void ExecuteWrite(string sql, string successMessage, string failureTitle, bool clearAfter)
    {
        try
        {
            using var connection = new MySqlConnection(ConnectionString);
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ma", txtMaSV.Text.Trim());
            command.Parameters.AddWithValue("@ten", txtHoTen.Text.Trim());
            command.Parameters.AddWithValue("@ngaySinh", dtpNgaySinh.Value.Date);
            command.Parameters.AddWithValue("@gioiTinh", cboGioiTinh.Text);
            command.Parameters.AddWithValue("@lop", txtLop.Text.Trim());
            command.Parameters.AddWithValue("@diem", nudDiemTB.Value);
            connection.Open();
            if (command.ExecuteNonQuery() == 0 && sql.StartsWith("UPDATE", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Không tìm thấy mã sinh viên cần cập nhật.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            MessageBox.Show(successMessage, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (clearAfter) ClearForm();
            LoadStudents(showError: false);
        }
        catch (MySqlException ex)
        {
            ShowDatabaseError(failureTitle, ex);
        }
    }

    private void LoadStudents(bool showError)
    {
        try
        {
            using var connection = new MySqlConnection(ConnectionString);
            using var adapter = new MySqlDataAdapter(
                "SELECT MaSV, HoTen, NgaySinh, GioiTinh, Lop, DiemTB FROM SinhVien ORDER BY MaSV", connection);
            var table = new DataTable();
            adapter.Fill(table);
            dgvSinhVien.DataSource = table;
            if (dgvSinhVien.Columns.Count == 6)
            {
                SetColumn("MaSV", "Mã Sinh Viên");
                SetColumn("HoTen", "Họ và Tên");
                SetColumn("NgaySinh", "Ngày Sinh", "dd/MM/yyyy");
                SetColumn("GioiTinh", "Giới Tính");
                SetColumn("Lop", "Lớp");
                SetColumn("DiemTB", "Điểm TB", "0.00");
            }
            lblStatus.Text = $"Đã tải {table.Rows.Count} sinh viên từ ql_sinhvien.";
        }
        catch (MySqlException ex)
        {
            lblStatus.Text = "Chưa kết nối được MySQL. Kiểm tra dịch vụ và script CSDL.";
            if (showError)
                MessageBox.Show("Không thể tải dữ liệu sinh viên từ MySQL.\n\n" + ex.Message,
                    "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SetColumn(string name, string header, string? format = null)
    {
        var column = dgvSinhVien.Columns[name];
        if (column is null) return;
        column.HeaderText = header;
        if (format is not null) column.DefaultCellStyle.Format = format;
    }

    private void CopySelectedRowToForm()
    {
        if (dgvSinhVien.CurrentRow?.DataBoundItem is not DataRowView row) return;
        txtMaSV.Text = Convert.ToString(row["MaSV"], CultureInfo.InvariantCulture) ?? "";
        txtHoTen.Text = Convert.ToString(row["HoTen"], CultureInfo.InvariantCulture) ?? "";
        if (row["NgaySinh"] != DBNull.Value)
            dtpNgaySinh.Value = Convert.ToDateTime(row["NgaySinh"], CultureInfo.InvariantCulture);
        cboGioiTinh.SelectedItem = Convert.ToString(row["GioiTinh"], CultureInfo.InvariantCulture);
        txtLop.Text = Convert.ToString(row["Lop"], CultureInfo.InvariantCulture) ?? "";
        if (row["DiemTB"] != DBNull.Value)
            nudDiemTB.Value = Math.Clamp(Convert.ToDecimal(row["DiemTB"], CultureInfo.InvariantCulture), nudDiemTB.Minimum, nudDiemTB.Maximum);
        errorProvider.Clear();
    }

    private void ClearForm()
    {
        errorProvider.Clear();
        txtMaSV.Clear();
        txtHoTen.Clear();
        txtLop.Clear();
        dtpNgaySinh.Value = DateTime.Today;
        cboGioiTinh.SelectedIndex = 0;
        nudDiemTB.Value = 0;
        dgvSinhVien.ClearSelection();
        txtMaSV.Focus();
    }

    private static void ShowDatabaseError(string title, MySqlException ex)
    {
        var message = ex.Number == 1062
            ? "Mã sinh viên đã tồn tại. Hãy nhập mã khác."
            : ex.Message;
        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
