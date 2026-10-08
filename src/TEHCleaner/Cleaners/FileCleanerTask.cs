using TEHCleaner.Models;
using TEHCleaner.Services;

namespace TEHCleaner.Cleaners;

public sealed class FileCleanerTask : CleanerTaskBase
{
    private readonly Func<IEnumerable<CleanupLocation>> _locations;
    private readonly bool _recommended;
    private readonly bool _advanced;
    private readonly bool _requiresAdministrator;
    private readonly Func<bool>? _availability;

    public FileCleanerTask(
        AppLogger logger,
        string id,
        string category,
        string name,
        string description,
        Func<IEnumerable<CleanupLocation>> locations,
        bool recommended = false,
        bool advanced = false,
        bool requiresAdministrator = false,
        Func<bool>? availability = null) : base(logger)
    {
        Id = id;
        Category = category;
        Name = name;
        Description = description;
        _locations = locations;
        _recommended = recommended;
        _advanced = advanced;
        _requiresAdministrator = requiresAdministrator;
        _availability = availability;
    }

    public override string Id { get; }
    public override string Category { get; }
    public override string Name { get; }
    public override string Description { get; }
    public override bool Recommended => _recommended;
    public override bool Advanced => _advanced;
    public override bool RequiresAdministrator => _requiresAdministrator;
    public override bool IsAvailable => _availability?.Invoke() ?? true;

    public override async Task<ScanResult> ScanAsync(CancellationToken cancellationToken)
    {
        if (!IsAvailable) return ScanResult.Unavailable("Не найдено в системе.");
        return await FileSystemService.ScanAsync(_locations().ToArray(), cancellationToken).ConfigureAwait(false);
    }

    public override async Task<CleanupResult> CleanAsync(CancellationToken cancellationToken)
    {
        if (!IsAvailable) return CleanupResult.Failed("Компонент не найден.");
        await Logger.InfoAsync($"[{Name}] начало очистки").ConfigureAwait(false);
        CleanupResult result = await FileSystemService.CleanAsync(_locations().ToArray(), Logger, cancellationToken).ConfigureAwait(false);
        await Logger.InfoAsync($"[{Name}] завершено: {FormatHelper.Bytes(result.BytesFreed)}").ConfigureAwait(false);
        return result;
    }
}
