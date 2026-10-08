namespace TEHCleaner.Models;

public sealed record CleanupDetail(
    string Area,
    string Path,
    int ItemsRemoved,
    long BytesFreed,
    int Skipped = 0);
