namespace TEHCleaner.Models;

public sealed record ScanResult(
    long Bytes,
    int Items,
    int Skipped = 0,
    string? Note = null,
    bool IsAvailable = true)
{
    public static ScanResult Unavailable(string note) => new(0, 0, 0, note, false);
}
