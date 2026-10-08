using TEHCleaner.Models;

namespace TEHCleaner.Services;

public static class FileSystemService
{
    public static Task<ScanResult> ScanAsync(IEnumerable<CleanupLocation> locations, CancellationToken cancellationToken) =>
        Task.Run(() => Scan(locations, cancellationToken), cancellationToken);

    public static Task<CleanupResult> CleanAsync(IEnumerable<CleanupLocation> locations, AppLogger logger, CancellationToken cancellationToken) =>
        Task.Run(() => Clean(locations, logger, cancellationToken), cancellationToken);

    private static ScanResult Scan(IEnumerable<CleanupLocation> locations, CancellationToken cancellationToken)
    {
        long bytes = 0;
        int items = 0;
        int skipped = 0;
        HashSet<string> seen = new(StringComparer.OrdinalIgnoreCase);

        foreach (CleanupLocation location in locations)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(location.Path)) continue;

            string normalized;
            try { normalized = Path.GetFullPath(location.Path); }
            catch { skipped++; continue; }

            if (!IsSafeCleanupLocation(normalized)) { skipped++; continue; }
            if (!seen.Add($"{location.Kind}|{normalized}|{location.SearchPattern}|{location.MinimumAge}")) continue;

            if (location.Kind == CleanupLocationKind.FilePattern)
            {
                if (!Directory.Exists(normalized)) continue;
                try
                {
                    foreach (string file in Directory.EnumerateFiles(normalized, location.SearchPattern, SearchOption.TopDirectoryOnly))
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        try
                        {
                            FileInfo info = new(file);
                            if (info.Attributes.HasFlag(FileAttributes.ReparsePoint)) { skipped++; continue; }
                            if (!IsOldEnough(info, location.MinimumAge)) continue;
                            bytes += info.Length;
                            items++;
                        }
                        catch { skipped++; }
                    }
                }
                catch { skipped++; }

                continue;
            }

            if (!Directory.Exists(normalized)) continue;
            ScanDirectory(normalized, location.MinimumAge, ref bytes, ref items, ref skipped, cancellationToken);
        }

        return new ScanResult(bytes, items, skipped);
    }

    private static void ScanDirectory(string directory, TimeSpan? minimumAge, ref long bytes, ref int items, ref int skipped, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            DirectoryInfo dirInfo = new(directory);
            if (dirInfo.Attributes.HasFlag(FileAttributes.ReparsePoint))
            {
                skipped++;
                return;
            }
        }
        catch
        {
            skipped++;
            return;
        }

        IEnumerable<string> files;
        try { files = Directory.EnumerateFiles(directory); }
        catch { skipped++; files = []; }

        foreach (string file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                FileInfo info = new(file);
                if (info.Attributes.HasFlag(FileAttributes.ReparsePoint)) { skipped++; continue; }
                if (!IsOldEnough(info, minimumAge)) continue;
                bytes += info.Length;
                items++;
            }
            catch { skipped++; }
        }

        IEnumerable<string> directories;
        try { directories = Directory.EnumerateDirectories(directory); }
        catch { skipped++; directories = []; }

        foreach (string child in directories)
            ScanDirectory(child, minimumAge, ref bytes, ref items, ref skipped, cancellationToken);
    }

    private static CleanupResult Clean(IEnumerable<CleanupLocation> locations, AppLogger logger, CancellationToken cancellationToken)
    {
        long bytesFreed = 0;
        int removed = 0;
        int skipped = 0;
        HashSet<string> seen = new(StringComparer.OrdinalIgnoreCase);
        List<CleanupDetail> details = [];

        foreach (CleanupLocation location in locations)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(location.Path)) continue;

            string normalized;
            try { normalized = Path.GetFullPath(location.Path); }
            catch { skipped++; continue; }

            if (!IsSafeCleanupLocation(normalized)) { skipped++; continue; }
            if (!seen.Add($"{location.Kind}|{normalized}|{location.SearchPattern}|{location.MinimumAge}")) continue;

            long beforeBytes = bytesFreed;
            int beforeRemoved = removed;
            int beforeSkipped = skipped;

            if (location.Kind == CleanupLocationKind.FilePattern)
            {
                if (Directory.Exists(normalized))
                {
                    try
                    {
                        foreach (string file in Directory.EnumerateFiles(normalized, location.SearchPattern, SearchOption.TopDirectoryOnly).ToArray())
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            DeleteFile(file, location.MinimumAge, ref bytesFreed, ref removed, ref skipped);
                        }
                    }
                    catch { skipped++; }
                }
            }
            else if (Directory.Exists(normalized))
            {
                DeleteDirectoryContents(normalized, location.MinimumAge, ref bytesFreed, ref removed, ref skipped, cancellationToken);
            }

            int removedDelta = removed - beforeRemoved;
            long bytesDelta = bytesFreed - beforeBytes;
            int skippedDelta = skipped - beforeSkipped;
            if (removedDelta > 0 || bytesDelta > 0 || skippedDelta > 0)
            {
                details.Add(new CleanupDetail(
                    location.DisplayName ?? BuildDisplayName(normalized, location),
                    normalized,
                    removedDelta,
                    bytesDelta,
                    skippedDelta));
            }
        }

        return new CleanupResult(
            bytesFreed,
            removed,
            skipped,
            true,
            skipped > 0 ? "Часть занятых или защищённых объектов пропущена." : null,
            details);
    }

    private static string BuildDisplayName(string normalized, CleanupLocation location)
    {
        string name = Path.GetFileName(normalized.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        if (string.IsNullOrWhiteSpace(name)) name = normalized;
        return location.Kind == CleanupLocationKind.FilePattern && location.SearchPattern != "*"
            ? $"{name} ({location.SearchPattern})"
            : name;
    }

    private static void DeleteDirectoryContents(string directory, TimeSpan? minimumAge, ref long bytesFreed, ref int removed, ref int skipped, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            DirectoryInfo info = new(directory);
            if (info.Attributes.HasFlag(FileAttributes.ReparsePoint))
            {
                skipped++;
                return;
            }
        }
        catch
        {
            skipped++;
            return;
        }

        string[] files;
        try { files = Directory.GetFiles(directory); }
        catch { skipped++; files = []; }

        foreach (string file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();
            DeleteFile(file, minimumAge, ref bytesFreed, ref removed, ref skipped);
        }

        string[] directories;
        try { directories = Directory.GetDirectories(directory); }
        catch { skipped++; directories = []; }

        foreach (string child in directories)
        {
            cancellationToken.ThrowIfCancellationRequested();
            DeleteDirectoryContents(child, minimumAge, ref bytesFreed, ref removed, ref skipped, cancellationToken);
            try
            {
                DirectoryInfo childInfo = new(child);
                if (!childInfo.Attributes.HasFlag(FileAttributes.ReparsePoint) && !Directory.EnumerateFileSystemEntries(child).Any())
                {
                    childInfo.Attributes &= ~FileAttributes.ReadOnly;
                    childInfo.Delete();
                }
            }
            catch { /* A non-empty or locked cache folder is fine. */ }
        }
    }

    private static bool IsSafeCleanupLocation(string path)
    {
        string candidate = NormalizeForComparison(path);
        if (string.IsNullOrWhiteSpace(candidate)) return false;

        string[] protectedRoots =
        [
            Path.GetPathRoot(path) ?? string.Empty,
            Environment.GetFolderPath(Environment.SpecialFolder.Windows),
            Environment.GetFolderPath(Environment.SpecialFolder.System),
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
        ];

        foreach (string protectedRoot in protectedRoots)
        {
            if (string.IsNullOrWhiteSpace(protectedRoot)) continue;
            string normalizedProtected = NormalizeForComparison(protectedRoot);
            if (candidate.Equals(normalizedProtected, StringComparison.OrdinalIgnoreCase))
                return false;
        }

        return true;
    }

    private static string NormalizeForComparison(string path)
    {
        try
        {
            string full = Path.GetFullPath(path);
            return full.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
        catch
        {
            return string.Empty;
        }
    }

    private static bool IsOldEnough(FileInfo info, TimeSpan? minimumAge)
    {
        if (minimumAge is null || minimumAge.Value <= TimeSpan.Zero)
            return true;

        try
        {
            return info.LastWriteTimeUtc <= DateTime.UtcNow.Subtract(minimumAge.Value);
        }
        catch
        {
            return false;
        }
    }

    private static void DeleteFile(string file, TimeSpan? minimumAge, ref long bytesFreed, ref int removed, ref int skipped)
    {
        try
        {
            FileInfo info = new(file);
            if (info.Attributes.HasFlag(FileAttributes.ReparsePoint))
            {
                skipped++;
                return;
            }

            if (!IsOldEnough(info, minimumAge))
                return;

            long length = 0;
            try { length = info.Length; } catch { }
            if (info.IsReadOnly) info.IsReadOnly = false;
            info.Delete();
            bytesFreed += length;
            removed++;
        }
        catch
        {
            skipped++;
        }
    }
}
