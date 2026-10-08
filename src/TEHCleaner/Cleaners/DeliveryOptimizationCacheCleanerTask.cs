using TEHCleaner.Models;
using TEHCleaner.Services;

namespace TEHCleaner.Cleaners;

public sealed class DeliveryOptimizationCacheCleanerTask : CleanerTaskBase
{
    private readonly string _cachePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.Windows),
        "ServiceProfiles",
        "NetworkService",
        "AppData",
        "Local",
        "Microsoft",
        "Windows",
        "DeliveryOptimization",
        "Cache");

    public DeliveryOptimizationCacheCleanerTask(AppLogger logger) : base(logger) { }

    public override string Id => "delivery-optimization-cache";
    public override string Category => "Windows Update";
    public override string Name => "Delivery Optimization";
    public override string Description => "Очищает кэш Оптимизации доставки штатной командой Windows. Закреплённые (pinned) файлы не удаляются; при необходимости Windows скачает данные повторно.";
    public override bool RequiresAdministrator => true;

    public override async Task<ScanResult> ScanAsync(CancellationToken cancellationToken)
    {
        if (!Directory.Exists(_cachePath))
            return new ScanResult(0, 0, 0, "Кэш управляется Windows. Локальный каталог сейчас пуст или расположен в другом месте по политике системы.");

        ScanResult result = await FileSystemService.ScanAsync(
            [new CleanupLocation(_cachePath, DisplayName: "Delivery Optimization Cache")],
            cancellationToken).ConfigureAwait(false);

        return result with
        {
            Note = result.Items > 0
                ? "Найден кэш Delivery Optimization. Для очистки будет использована штатная команда Windows."
                : "Кэш Delivery Optimization пуст."
        };
    }

    public override async Task<CleanupResult> CleanAsync(CancellationToken cancellationToken)
    {
        if (!ElevationService.IsAdministrator())
            return CleanupResult.Failed("Требуются права администратора.");

        ScanResult before = Directory.Exists(_cachePath)
            ? await FileSystemService.ScanAsync([new CleanupLocation(_cachePath)], cancellationToken).ConfigureAwait(false)
            : new ScanResult(0, 0);

        const string arguments = "-NoLogo -NoProfile -NonInteractive -Command \"Import-Module DeliveryOptimization -ErrorAction Stop; Delete-DeliveryOptimizationCache -Force -ErrorAction Stop\"";
        await Logger.InfoAsync("[Delivery Optimization] запуск штатной очистки кэша Windows").ConfigureAwait(false);

        ProcessRunResult command = await ProcessRunner.RunAsync(
            "powershell.exe",
            arguments,
            TimeSpan.FromMinutes(2),
            cancellationToken).ConfigureAwait(false);

        if (!command.Success)
        {
            string note = command.TimedOut
                ? "Очистка Delivery Optimization превысила время ожидания."
                : $"Не удалось очистить Delivery Optimization. Код: {command.ExitCode}. {command.StandardError}".Trim();
            await Logger.WarningAsync($"[Delivery Optimization] {note}").ConfigureAwait(false);
            return CleanupResult.Failed(note);
        }

        ScanResult after = Directory.Exists(_cachePath)
            ? await FileSystemService.ScanAsync([new CleanupLocation(_cachePath)], CancellationToken.None).ConfigureAwait(false)
            : new ScanResult(0, 0);

        long bytesFreed = Math.Max(0, before.Bytes - after.Bytes);
        int itemsRemoved = Math.Max(0, before.Items - after.Items);

        List<CleanupDetail> details =
        [
            new CleanupDetail(
                "Delivery Optimization",
                Directory.Exists(_cachePath) ? _cachePath : "Delete-DeliveryOptimizationCache -Force",
                itemsRemoved,
                bytesFreed)
        ];

        string noteText = bytesFreed > 0
            ? $"Кэш Delivery Optimization очищен: {FormatHelper.Bytes(bytesFreed)}."
            : "Штатная очистка Delivery Optimization выполнена. Точный объём мог быть недоступен для предварительного подсчёта.";

        await Logger.InfoAsync($"[Delivery Optimization] {noteText}").ConfigureAwait(false);
        return new CleanupResult(bytesFreed, itemsRemoved, 0, true, noteText, details);
    }
}
