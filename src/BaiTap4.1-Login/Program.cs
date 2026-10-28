namespace LoginApp;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        try
        {
            Database.Initialize();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Không kết nối được MySQL. Hãy bật MySQL trong XAMPP và chạy script ../Database/ql_sinhvien.sql trước.\n\nChi tiết: {ex.Message}",
                "Lỗi kết nối CSDL",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return;
        }
        Application.Run(new LoginForm());
    }
}
