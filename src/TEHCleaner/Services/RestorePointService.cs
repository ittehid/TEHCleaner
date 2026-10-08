namespace TEHCleaner.Services;

public sealed class RestorePointService
{
    private readonly AppLogger _logger;

    public RestorePointService(AppLogger logger)
    {
        _logger = logger;
    }

    public async Task<(bool Success, string Message)> TryCreateAsync(CancellationToken cancellationToken)
    {
        if (!ElevationService.IsAdministrator())
            return (false, "Для точки восстановления нужны права администратора.");

        const string command = "Checkpoint-Computer -Description 'TEHCleaner 2.0 - Before cleanup' -RestorePointType 'MODIFY_SETTINGS'";
        ProcessRunResult result = await ProcessRunner.RunAsync(
            "powershell.exe",
            $"-NoLogo -NoProfile -NonInteractive -ExecutionPolicy Bypass -Command \"{command}\"",
            TimeSpan.FromMinutes(2),
            cancellationToken).ConfigureAwait(false);

        if (result.Success)
        {
            await _logger.InfoAsync("Создана точка восстановления системы.").ConfigureAwait(false);
            return (true, "Точка восстановления создана.");
        }

        string message = result.TimedOut
            ? "Создание точки восстановления превысило время ожидания."
            : string.IsNullOrWhiteSpace(result.StandardError)
                ? "Windows не создала точку восстановления. Возможно, защита системы отключена или действует системное ограничение частоты создания точек."
                : result.StandardError;

        await _logger.WarningAsync($"Точка восстановления не создана: {message}").ConfigureAwait(false);
        return (false, message);
    }
}
