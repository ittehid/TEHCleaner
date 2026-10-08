namespace TEHCleaner;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

        Application.ThreadException += (_, e) =>
            MessageBox.Show($"Непредвиденная ошибка:\n\n{e.Exception.Message}", "TEHCleaner", MessageBoxButtons.OK, MessageBoxIcon.Error);

        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            if (e.ExceptionObject is Exception ex)
            {
                try
                {
                    File.AppendAllText(Path.Combine(Path.GetTempPath(), "TEHCleaner_fatal.log"),
                        $"[{DateTime.Now:O}] {ex}\r\n");
                }
                catch
                {
                    // Last-chance handler: never throw here.
                }
            }
        };

        Application.Run(new MainForm());
    }
}
