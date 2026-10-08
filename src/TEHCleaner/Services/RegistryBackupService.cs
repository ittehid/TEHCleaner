using System.Text;

namespace TEHCleaner.Services;

public sealed record RegistryBackupResult(bool Success, string DirectoryPath, IReadOnlyList<string> Files, string? Error = null);

public static class RegistryBackupService
{
    public static string RootDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "TEHCleaner",
        "RegistryBackups");

    public static async Task<RegistryBackupResult> ExportKeysAsync(
        IEnumerable<(string FullPath, string DisplayName)> keys,
        CancellationToken cancellationToken)
    {
        string directory = Path.Combine(RootDirectory, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
        Directory.CreateDirectory(directory);

        List<string> files = [];
        int index = 1;

        foreach ((string fullPath, string displayName) in keys)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string safeName = SanitizeFileName(displayName);
            string output = Path.Combine(directory, $"{index:00}_{safeName}.reg");
            string args = $"export \"{fullPath}\" \"{output}\" /y";

            ProcessRunResult result = await ProcessRunner.RunAsync(
                "reg.exe",
                args,
                TimeSpan.FromSeconds(30),
                cancellationToken).ConfigureAwait(false);

            if (!result.Success)
            {
                try
                {
                    if (Directory.Exists(directory) && !Directory.EnumerateFileSystemEntries(directory).Any())
                        Directory.Delete(directory);
                }
                catch { }

                string error = string.IsNullOrWhiteSpace(result.StandardError)
                    ? $"reg.exe завершился с кодом {result.ExitCode}."
                    : result.StandardError;
                return new RegistryBackupResult(false, directory, files, error);
            }

            if (File.Exists(output))
                files.Add(output);

            index++;
        }

        try
        {
            string readme = Path.Combine(directory, "README.txt");
            await File.WriteAllTextAsync(
                readme,
                "TEHCleaner — резервная копия реестра\r\n" +
                $"Создано: {DateTime.Now:dd.MM.yyyy HH:mm:ss}\r\n\r\n" +
                "Для ручного восстановления дважды щёлкните нужный .reg-файл и подтвердите импорт.\r\n" +
                "Импорт возвращает значения, существовавшие на момент резервного копирования.\r\n",
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: true),
                cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            // README is informational only; a successful .reg export is enough.
        }

        return new RegistryBackupResult(true, directory, files);
    }

    private static string SanitizeFileName(string name)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');
        return string.IsNullOrWhiteSpace(name) ? "registry" : name;
    }
}
