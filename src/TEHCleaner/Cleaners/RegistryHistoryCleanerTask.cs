using Microsoft.Win32;
using TEHCleaner.Models;
using TEHCleaner.Services;

namespace TEHCleaner.Cleaners;

public sealed class RegistryHistoryCleanerTask : CleanerTaskBase
{
    private sealed record Target(string DisplayName, string RelativePath, bool Recursive);

    private static readonly Target[] Targets =
    [
        new("Выполнить — RunMRU", @"Software\Microsoft\Windows\CurrentVersion\Explorer\RunMRU", false),
        new("Пути Проводника — TypedPaths", @"Software\Microsoft\Windows\CurrentVersion\Explorer\TypedPaths", false),
        new("Поиск Проводника — WordWheelQuery", @"Software\Microsoft\Windows\CurrentVersion\Explorer\WordWheelQuery", false),
        new("Открыть/Сохранить — OpenSavePidlMRU", @"Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\OpenSavePidlMRU", true),
        new("Последние папки — LastVisitedPidlMRU", @"Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\LastVisitedPidlMRU", true),
        new("Недавние документы — RecentDocs", @"Software\Microsoft\Windows\CurrentVersion\Explorer\RecentDocs", true)
    ];

    public RegistryHistoryCleanerTask(AppLogger logger) : base(logger) { }

    public override string Id => "registry-history";
    public override string Category => "Реестр";
    public override string Name => "История и MRU Windows";
    public override string Description =>
        "Очищает только пользовательские списки истории Windows в реестре: Выполнить, введённые пути, поиск, диалоги Открыть/Сохранить и RecentDocs. " +
        "Перед удалением TEHCleaner автоматически экспортирует затрагиваемые ветки в .reg. Ассоциации файлов, автозагрузка, CLSID, службы и Uninstall не изменяются.";
    public override bool Recommended => false;
    public override bool Advanced => false;
    public override bool RequiresAdministrator => false;

    public override Task<ScanResult> ScanAsync(CancellationToken cancellationToken)
    {
        int items = 0;
        int skipped = 0;

        foreach (Target target in Targets)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                using RegistryKey? key = Registry.CurrentUser.OpenSubKey(target.RelativePath, writable: false);
                if (key is null)
                    continue;

                items += target.Recursive ? CountTreeItems(key, cancellationToken) : key.GetValueNames().Length;
            }
            catch
            {
                skipped++;
            }
        }

        string note = items > 0
            ? "Найдены только пользовательские MRU/история. Размер реестра не используется как критерий очистки."
            : "Записей истории для очистки не найдено.";

        return Task.FromResult(new ScanResult(0, items, skipped, note));
    }

    public override async Task<CleanupResult> CleanAsync(CancellationToken cancellationToken)
    {
        List<(Target Target, string FullPath)> existing = [];
        foreach (Target target in Targets)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                using RegistryKey? key = Registry.CurrentUser.OpenSubKey(target.RelativePath, writable: false);
                if (key is not null)
                    existing.Add((target, $@"HKCU\{target.RelativePath}"));
            }
            catch
            {
                // The actual cleanup pass will count inaccessible keys as skipped.
            }
        }

        if (existing.Count == 0)
            return new CleanupResult(0, 0, 0, true, "Записей истории для очистки не найдено.");

        RegistryBackupResult backup = await RegistryBackupService.ExportKeysAsync(
            existing.Select(x => (x.FullPath, x.Target.DisplayName)),
            cancellationToken).ConfigureAwait(false);

        if (!backup.Success)
        {
            string note = $"Очистка отменена: не удалось создать резервную копию реестра. {backup.Error}".Trim();
            await Logger.WarningAsync($"[{Name}] {note}").ConfigureAwait(false);
            return CleanupResult.Failed(note);
        }

        int removed = 0;
        int skipped = 0;
        List<CleanupDetail> details = [];

        foreach ((Target target, string fullPath) in existing)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int targetRemoved = 0;
            int targetSkipped = 0;
            try
            {
                using RegistryKey? key = Registry.CurrentUser.OpenSubKey(target.RelativePath, writable: true);
                if (key is null)
                    continue;

                if (target.Recursive)
                {
                    foreach (string subKeyName in key.GetSubKeyNames())
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        try
                        {
                            using RegistryKey? child = key.OpenSubKey(subKeyName, writable: false);
                            int childItems = child is null ? 1 : CountTreeItems(child, cancellationToken) + 1;
                            key.DeleteSubKeyTree(subKeyName, throwOnMissingSubKey: false);
                            targetRemoved += childItems;
                        }
                        catch
                        {
                            targetSkipped++;
                        }
                    }
                }

                foreach (string valueName in key.GetValueNames())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    try
                    {
                        key.DeleteValue(valueName, throwOnMissingValue: false);
                        targetRemoved++;
                    }
                    catch
                    {
                        targetSkipped++;
                    }
                }
            }
            catch
            {
                targetSkipped++;
            }

            removed += targetRemoved;
            skipped += targetSkipped;
            details.Add(new CleanupDetail(
                target.DisplayName,
                fullPath,
                targetRemoved,
                0,
                targetSkipped));
        }

        details.Add(new CleanupDetail(
            "Резервная копия (.reg)",
            backup.DirectoryPath,
            0,
            0,
            0));

        string resultNote = removed > 0
            ? $"Удалено записей: {removed:N0}. Резервная копия: {backup.DirectoryPath}"
            : $"Удалять было нечего. Резервная копия: {backup.DirectoryPath}";

        await Logger.InfoAsync($"[{Name}] {resultNote}; пропущено: {skipped:N0}").ConfigureAwait(false);
        return new CleanupResult(0, removed, skipped, true, resultNote, details);
    }

    private static int CountTreeItems(RegistryKey key, CancellationToken cancellationToken)
    {
        int count = key.GetValueNames().Length;
        foreach (string subKeyName in key.GetSubKeyNames())
        {
            cancellationToken.ThrowIfCancellationRequested();
            count++;
            try
            {
                using RegistryKey? child = key.OpenSubKey(subKeyName, writable: false);
                if (child is not null)
                    count += CountTreeItems(child, cancellationToken);
            }
            catch
            {
                // Inaccessible child is still counted as one subkey.
            }
        }
        return count;
    }
}
