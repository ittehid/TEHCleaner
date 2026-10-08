using TEHCleaner.Models;
using TEHCleaner.Services;

namespace TEHCleaner.Cleaners;

public sealed class CommandCleanerTask : CleanerTaskBase
{
    private readonly string _fileName;
    private readonly string _arguments;
    private readonly bool _requiresAdmin;

    public CommandCleanerTask(
        AppLogger logger,
        string id,
        string category,
        string name,
        string description,
        string fileName,
        string arguments,
        bool requiresAdmin = false) : base(logger)
    {
        Id = id;
        Category = category;
        Name = name;
        Description = description;
        _fileName = fileName;
        _arguments = arguments;
        _requiresAdmin = requiresAdmin;
    }

    public override string Id { get; }
    public override string Category { get; }
    public override string Name { get; }
    public override string Description { get; }
    public override bool Advanced => true;
    public override bool RequiresAdministrator => _requiresAdmin;

    public override Task<ScanResult> ScanAsync(CancellationToken cancellationToken) =>
        Task.FromResult(new ScanResult(0, 1, 0, "Служебное действие; размер не рассчитывается."));

    public override async Task<CleanupResult> CleanAsync(CancellationToken cancellationToken)
    {
        ProcessRunResult result = await ProcessRunner.RunAsync(_fileName, _arguments, TimeSpan.FromSeconds(30), cancellationToken).ConfigureAwait(false);
        if (!result.Success)
        {
            string note = result.TimedOut ? "Команда превысила время ожидания." : $"Код завершения: {result.ExitCode}. {result.StandardError}".Trim();
            await Logger.WarningAsync($"[{Name}] {note}").ConfigureAwait(false);
            return CleanupResult.Failed(note);
        }

        await Logger.InfoAsync($"[{Name}] выполнено успешно").ConfigureAwait(false);
        return new CleanupResult(0, 1, 0, true, result.StandardOutput, [new CleanupDetail(Name, $"{_fileName} {_arguments}", 1, 0)]);
    }
}
