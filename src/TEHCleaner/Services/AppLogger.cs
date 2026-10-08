using System.Text;

namespace TEHCleaner.Services;

public sealed class AppLogger : IDisposable
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly string _logDirectory;
    private bool _disposed;

    public AppLogger()
    {
        _logDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "TEHCleaner",
            "Logs");
        Directory.CreateDirectory(_logDirectory);
        CurrentLogPath = Path.Combine(_logDirectory, $"TEHCleaner_{DateTime.Now:yyyyMMdd_HHmmss}.log");
    }

    public string CurrentLogPath { get; }
    public string LogDirectory => _logDirectory;

    public event EventHandler<string>? MessageLogged;

    public Task InfoAsync(string message) => WriteAsync("INFO", message);
    public Task WarningAsync(string message) => WriteAsync("WARN", message);
    public Task ErrorAsync(string message) => WriteAsync("ERROR", message);

    private async Task WriteAsync(string level, string message)
    {
        if (_disposed) return;

        string line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
        bool entered = false;
        try
        {
            await _gate.WaitAsync().ConfigureAwait(false);
            entered = true;
            await File.AppendAllTextAsync(CurrentLogPath, line + Environment.NewLine, Encoding.UTF8).ConfigureAwait(false);
        }
        catch (ObjectDisposedException)
        {
            return;
        }
        catch
        {
            // Logging must never break cleanup.
        }
        finally
        {
            if (entered)
            {
                try { _gate.Release(); } catch (ObjectDisposedException) { }
            }
        }

        if (!_disposed) MessageLogged?.Invoke(this, line);
    }

    public void Dispose()
    {
        _disposed = true;
        _gate.Dispose();
    }
}
