using System.Diagnostics;
using TEHCleaner.Models;
using TEHCleaner.Services;

namespace TEHCleaner.Cleaners;

public sealed class BrowserCacheCleanerTask : CleanerTaskBase
{
    private readonly Func<IEnumerable<CleanupLocation>> _locations;
    private readonly string[] _processNames;
    private readonly bool _available;

    public BrowserCacheCleanerTask(
        AppLogger logger,
        string id,
        string name,
        string description,
        Func<IEnumerable<CleanupLocation>> locations,
        string[] processNames,
        bool available) : base(logger)
    {
        Id = id;
        Name = name;
        Description = description;
        _locations = locations;
        _processNames = processNames;
        _available = available;
    }

    public override string Id { get; }
    public override string Category => "Браузеры";
    public override string Name { get; }
    public override string Description { get; }
    public override bool Recommended => true;
    public override bool IsAvailable => _available;

    public override async Task<ScanResult> ScanAsync(CancellationToken cancellationToken)
    {
        if (!IsAvailable) return ScanResult.Unavailable("Браузер не найден.");
        ScanResult result = await FileSystemService.ScanAsync(_locations().ToArray(), cancellationToken).ConfigureAwait(false);
        if (IsRunning())
            return result with { Note = "Браузер запущен. Для безопасной очистки TEHCleaner попросит закрыть его." };
        return result;
    }

    public override async Task<CleanupResult> CleanAsync(CancellationToken cancellationToken)
    {
        if (!IsAvailable) return CleanupResult.Failed("Браузер не найден.");
        if (IsRunning())
        {
            const string note = "Закройте браузер и повторите очистку. TEHCleaner не завершает пользовательские приложения принудительно.";
            await Logger.WarningAsync($"[{Name}] очистка пропущена: браузер запущен.").ConfigureAwait(false);
            return CleanupResult.Failed(note);
        }

        return await FileSystemService.CleanAsync(_locations().ToArray(), Logger, cancellationToken).ConfigureAwait(false);
    }

    private bool IsRunning()
    {
        foreach (string processName in _processNames)
        {
            try
            {
                Process[] processes = Process.GetProcessesByName(processName);
                bool running = processes.Length > 0;
                foreach (Process process in processes)
                    process.Dispose();
                if (running) return true;
            }
            catch
            {
                // Process enumeration can fail for protected processes; ignore.
            }
        }
        return false;
    }
}
