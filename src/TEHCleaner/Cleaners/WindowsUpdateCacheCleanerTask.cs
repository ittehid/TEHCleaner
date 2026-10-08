using TEHCleaner.Models;
using TEHCleaner.Services;

namespace TEHCleaner.Cleaners;

public sealed class WindowsUpdateCacheCleanerTask : CleanerTaskBase
{
    private readonly string _downloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "SoftwareDistribution", "Download");

    public WindowsUpdateCacheCleanerTask(AppLogger logger) : base(logger) { }

    public override string Id => "windows-update-cache";
    public override string Category => "Windows Update";
    public override string Name => "Старый кэш обновлений Windows";
    public override string Description => "Останавливает службы Windows Update/BITS, очищает загруженные временные пакеты из SoftwareDistribution\\Download и восстанавливает исходное состояние служб. Установленные обновления не удаляются.";
    public override bool RequiresAdministrator => true;
    public override bool IsAvailable => Directory.Exists(_downloadPath);

    public override Task<ScanResult> ScanAsync(CancellationToken cancellationToken) =>
        IsAvailable
            ? FileSystemService.ScanAsync([new CleanupLocation(_downloadPath, DisplayName: "SoftwareDistribution\\Download")], cancellationToken)
            : Task.FromResult(ScanResult.Unavailable("Кэш Windows Update не найден."));

    public override async Task<CleanupResult> CleanAsync(CancellationToken cancellationToken)
    {
        if (!ElevationService.IsAdministrator())
            return CleanupResult.Failed("Требуются права администратора.");

        bool updateWasRunning = WindowsServiceState.IsRunning("wuauserv");
        bool bitsWasRunning = WindowsServiceState.IsRunning("bits");

        await Logger.InfoAsync("[Windows Update] подготовка служб wuauserv и BITS").ConfigureAwait(false);
        try
        {
            if (updateWasRunning)
                await StopServiceAsync("wuauserv", cancellationToken).ConfigureAwait(false);
            if (bitsWasRunning)
                await StopServiceAsync("bits", cancellationToken).ConfigureAwait(false);

            return await FileSystemService.CleanAsync([new CleanupLocation(_downloadPath, DisplayName: "SoftwareDistribution\\Download")], Logger, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            // Restore only services that were running before TEHCleaner touched them.
            if (bitsWasRunning)
                await StartServiceAsync("bits", CancellationToken.None).ConfigureAwait(false);
            if (updateWasRunning)
                await StartServiceAsync("wuauserv", CancellationToken.None).ConfigureAwait(false);
            await Logger.InfoAsync("[Windows Update] исходное состояние служб восстановлено").ConfigureAwait(false);
        }
    }

    private static async Task StopServiceAsync(string serviceName, CancellationToken cancellationToken)
    {
        await ProcessRunner.RunAsync("sc.exe", $"stop {serviceName}", TimeSpan.FromSeconds(20), cancellationToken).ConfigureAwait(false);
        await Task.Delay(800, cancellationToken).ConfigureAwait(false);
    }

    private static Task<ProcessRunResult> StartServiceAsync(string serviceName, CancellationToken cancellationToken) =>
        ProcessRunner.RunAsync("sc.exe", $"start {serviceName}", TimeSpan.FromSeconds(20), cancellationToken);
}
