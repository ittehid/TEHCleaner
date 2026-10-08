using TEHCleaner.Models;

namespace TEHCleaner.Services;

public static class BrowserPathService
{
    private static readonly string[] ChromiumCacheNames = ["Cache", "Code Cache", "GPUCache", "GrShaderCache", "ShaderCache"];

    public static IEnumerable<CleanupLocation> Chromium(string userDataRoot)
    {
        if (!Directory.Exists(userDataRoot)) yield break;

        foreach (string cacheName in ChromiumCacheNames)
        {
            string rootCache = Path.Combine(userDataRoot, cacheName);
            if (Directory.Exists(rootCache))
                yield return new CleanupLocation(rootCache, DisplayName: $"Общий профиль — {cacheName}");
        }

        string[] profiles;
        try { profiles = Directory.GetDirectories(userDataRoot); }
        catch { yield break; }

        foreach (string profile in profiles)
        {
            string name = Path.GetFileName(profile);
            if (!IsChromiumProfile(name)) continue;

            foreach (string cacheName in ChromiumCacheNames)
            {
                string path = Path.Combine(profile, cacheName);
                if (Directory.Exists(path))
                    yield return new CleanupLocation(path, DisplayName: $"{name} — {cacheName}");
            }
        }
    }

    public static IEnumerable<CleanupLocation> Firefox()
    {
        string localProfiles = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Mozilla", "Firefox", "Profiles");
        string roamingProfiles = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Mozilla", "Firefox", "Profiles");

        foreach (string basePath in new[] { localProfiles, roamingProfiles })
        {
            if (!Directory.Exists(basePath)) continue;
            string[] profiles;
            try { profiles = Directory.GetDirectories(basePath); }
            catch { continue; }

            foreach (string profile in profiles)
            {
                string profileName = Path.GetFileName(profile);
                foreach (string relative in new[] { "cache2", "startupCache" })
                {
                    string path = Path.Combine(profile, relative);
                    if (Directory.Exists(path))
                        yield return new CleanupLocation(path, DisplayName: $"{profileName} — {relative}");
                }
            }
        }
    }

    public static IEnumerable<CleanupLocation> Opera()
    {
        string roaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        foreach (string basePath in new[]
        {
            Path.Combine(roaming, "Opera Software", "Opera Stable"),
            Path.Combine(roaming, "Opera Software", "Opera GX Stable"),
            Path.Combine(local, "Opera Software", "Opera Stable"),
            Path.Combine(local, "Opera Software", "Opera GX Stable")
        })
        {
            if (!Directory.Exists(basePath)) continue;
            string browserName = basePath.Contains("GX", StringComparison.OrdinalIgnoreCase) ? "Opera GX" : "Opera";
            foreach (string cacheName in ChromiumCacheNames)
            {
                string path = Path.Combine(basePath, cacheName);
                if (Directory.Exists(path))
                    yield return new CleanupLocation(path, DisplayName: $"{browserName} — {cacheName}");
            }
        }
    }

    private static bool IsChromiumProfile(string name) =>
        name.Equals("Default", StringComparison.OrdinalIgnoreCase) ||
        name.Equals("Guest Profile", StringComparison.OrdinalIgnoreCase) ||
        name.Equals("System Profile", StringComparison.OrdinalIgnoreCase) ||
        name.StartsWith("Profile ", StringComparison.OrdinalIgnoreCase);
}
