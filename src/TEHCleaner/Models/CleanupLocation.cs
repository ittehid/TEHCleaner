namespace TEHCleaner.Models;

public enum CleanupLocationKind
{
    DirectoryContents,
    FilePattern
}

public sealed record CleanupLocation(
    string Path,
    CleanupLocationKind Kind = CleanupLocationKind.DirectoryContents,
    string SearchPattern = "*",
    string? DisplayName = null,
    TimeSpan? MinimumAge = null);
