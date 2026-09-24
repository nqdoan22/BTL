using BTLWinForms.Data;
using BTLWinForms.UI;

namespace BTLWinForms;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (_, e) => ShowUnexpectedError(e.Exception);
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            if (e.ExceptionObject is Exception ex) ShowUnexpectedError(ex);
        };

        Application.Run(new MainForm(ConnectionSettings.Load()));
    }

    private static void ShowUnexpectedError(Exception ex)
    {
        MessageBox.Show(
            $"Đã xảy ra lỗi không mong muốn:\n{ex.Message}\n\nNếu lỗi lặp lại, hãy khởi động lại ứng dụng.",
            "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
