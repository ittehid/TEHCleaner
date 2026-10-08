namespace TEHCleaner.Models;

public sealed record CleanupResult(
    long BytesFreed,
    int ItemsRemoved,
    int Skipped,
    bool Success,
    string? Note = null,
    IReadOnlyList<CleanupDetail>? Details = null)
{
    public static CleanupResult Failed(string note, int skipped = 0, IReadOnlyList<CleanupDetail>? details = null) =>
        new(0, 0, skipped, false, note, details);
}
