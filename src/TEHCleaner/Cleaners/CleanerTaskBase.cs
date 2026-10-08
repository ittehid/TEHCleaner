using TEHCleaner.Models;
using TEHCleaner.Services;

namespace TEHCleaner.Cleaners;

public abstract class CleanerTaskBase : ICleanerTask
{
    protected CleanerTaskBase(AppLogger logger)
    {
        Logger = logger;
    }

    protected AppLogger Logger { get; }

    public abstract string Id { get; }
    public abstract string Category { get; }
    public abstract string Name { get; }
    public abstract string Description { get; }
    public virtual bool Recommended => false;
    public virtual bool Advanced => false;
    public virtual bool RequiresAdministrator => false;
    public virtual bool IsAvailable => true;

    public abstract Task<ScanResult> ScanAsync(CancellationToken cancellationToken);
    public abstract Task<CleanupResult> CleanAsync(CancellationToken cancellationToken);
}
