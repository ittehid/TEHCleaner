using TEHCleaner.Models;

namespace TEHCleaner.Cleaners;

public interface ICleanerTask
{
    string Id { get; }
    string Category { get; }
    string Name { get; }
    string Description { get; }
    bool Recommended { get; }
    bool Advanced { get; }
    bool RequiresAdministrator { get; }
    bool IsAvailable { get; }

    Task<ScanResult> ScanAsync(CancellationToken cancellationToken);
    Task<CleanupResult> CleanAsync(CancellationToken cancellationToken);
}
