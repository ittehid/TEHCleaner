using TEHCleaner.Cleaners;
using TEHCleaner.Models;

namespace TEHCleaner.Services;

public static class CleanupCatalog
{
    public static IReadOnlyList<ICleanerTask> Create(AppLogger logger)
    {
        string local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string roaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string windows = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        string common = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

        string chromeRoot = Path.Combine(local, "Google", "Chrome", "User Data");
        string edgeRoot = Path.Combine(local, "Microsoft", "Edge", "User Data");
        string braveRoot = Path.Combine(local, "BraveSoftware", "Brave-Browser", "User Data");
        string vivaldiRoot = Path.Combine(local, "Vivaldi", "User Data");
        string yandexRoot = Path.Combine(local, "Yandex", "YandexBrowser", "User Data");

        List<ICleanerTask> tasks =
        [
            new FileCleanerTask(
                logger,
                "user-temp",
                "Windows",
                "Временные файлы пользователя",
                "Очищает содержимое %TEMP%. Занятые работающими программами файлы автоматически пропускаются.",
                () => [new CleanupLocation(Path.GetTempPath(), DisplayName: "%TEMP% пользователя")],
                recommended: true),

            new FileCleanerTask(
                logger,
                "windows-temp",
                "Windows",
                "Временные системные и установочные файлы",
                "Очищает C:\\Windows\\Temp, где Windows, обновления и установщики оставляют временные файлы. Защищённые и занятые объекты не удаляются.",
                () => [new CleanupLocation(Path.Combine(windows, "Temp"), DisplayName: "Windows\\Temp")],
                recommended: true,
                requiresAdministrator: true,
                availability: () => Directory.Exists(Path.Combine(windows, "Temp"))),

            new FileCleanerTask(
                logger,
                "thumbnail-cache",
                "Windows",
                "Кэш миниатюр Проводника",
                "Удаляет только thumbcache_*.db. Windows создаст миниатюры заново по мере необходимости.",
                () => [new CleanupLocation(Path.Combine(local, "Microsoft", "Windows", "Explorer"), CleanupLocationKind.FilePattern, "thumbcache_*.db", "Кэш миниатюр")],
                recommended: true),

            new FileCleanerTask(
                logger,
                "directx-cache",
                "Windows",
                "Кэш шейдеров DirectX",
                "Удаляет D3DSCache текущего пользователя. Кэш автоматически пересоздаётся играми и приложениями.",
                () => [new CleanupLocation(Path.Combine(local, "D3DSCache"), DisplayName: "DirectX D3DSCache")],
                recommended: true,
                availability: () => Directory.Exists(Path.Combine(local, "D3DSCache"))),

            new RecycleBinCleanerTask(logger),

            new RegistryHistoryCleanerTask(logger),

            new FileCleanerTask(
                logger,
                "recent-documents",
                "Конфиденциальность",
                "Недавние документы",
                "Удаляет только верхнеуровневые ярлыки .lnk из списка недавних документов. Jump Lists и закреплённые элементы не затрагиваются.",
                () => [new CleanupLocation(Path.Combine(roaming, "Microsoft", "Windows", "Recent"), CleanupLocationKind.FilePattern, "*.lnk", "Недавние документы (.lnk)")],
                advanced: true,
                availability: () => Directory.Exists(Path.Combine(roaming, "Microsoft", "Windows", "Recent"))),

            new FileCleanerTask(
                logger,
                "crash-dumps",
                "Расширенная",
                "Дампы сбоев приложений",
                "Удаляет пользовательские .dmp из LocalAppData\\CrashDumps. Оставьте их, если разбираете причины падений программ.",
                () => [new CleanupLocation(Path.Combine(local, "CrashDumps"), CleanupLocationKind.FilePattern, "*.dmp", "CrashDumps (.dmp)")],
                advanced: true,
                availability: () => Directory.Exists(Path.Combine(local, "CrashDumps"))),

            new FileCleanerTask(
                logger,
                "wer-reports",
                "Расширенная",
                "Отчёты Windows об ошибках",
                "Удаляет локальные очереди и архивы Windows Error Reporting. Не рекомендуется, если отчёты нужны для диагностики.",
                () =>
                [
                    new CleanupLocation(Path.Combine(common, "Microsoft", "Windows", "WER", "ReportArchive"), DisplayName: "WER ReportArchive"),
                    new CleanupLocation(Path.Combine(common, "Microsoft", "Windows", "WER", "ReportQueue"), DisplayName: "WER ReportQueue"),
                    new CleanupLocation(Path.Combine(local, "Microsoft", "Windows", "WER"), DisplayName: "WER пользователя")
                ],
                advanced: true,
                requiresAdministrator: true),

            new BrowserCacheCleanerTask(
                logger,
                "chrome",
                "Google Chrome",
                "Очищает Cache, Code Cache, GPUCache и shader-кэши профилей. Cookies, пароли, история и закладки не затрагиваются.",
                () => BrowserPathService.Chromium(chromeRoot),
                ["chrome"],
                Directory.Exists(chromeRoot)),

            new BrowserCacheCleanerTask(
                logger,
                "edge",
                "Microsoft Edge",
                "Очищает только кэш Chromium-профилей Edge; личные данные входа не удаляются.",
                () => BrowserPathService.Chromium(edgeRoot),
                ["msedge"],
                Directory.Exists(edgeRoot)),

            new BrowserCacheCleanerTask(
                logger,
                "firefox",
                "Mozilla Firefox",
                "Очищает cache2 и startupCache профилей Firefox. Cookies, пароли, история и закладки не затрагиваются.",
                BrowserPathService.Firefox,
                ["firefox"],
                Directory.Exists(Path.Combine(local, "Mozilla", "Firefox", "Profiles")) || Directory.Exists(Path.Combine(roaming, "Mozilla", "Firefox", "Profiles"))),

            new BrowserCacheCleanerTask(
                logger,
                "opera",
                "Opera / Opera GX",
                "Очищает файловые кэши Opera и Opera GX без удаления пользовательского профиля.",
                BrowserPathService.Opera,
                ["opera", "opera_gx"],
                Directory.Exists(Path.Combine(roaming, "Opera Software")) || Directory.Exists(Path.Combine(local, "Opera Software"))),

            new BrowserCacheCleanerTask(
                logger,
                "brave",
                "Brave",
                "Очищает Cache, Code Cache, GPUCache и shader-кэши Brave без удаления cookies и паролей.",
                () => BrowserPathService.Chromium(braveRoot),
                ["brave"],
                Directory.Exists(braveRoot)),

            new BrowserCacheCleanerTask(
                logger,
                "vivaldi",
                "Vivaldi",
                "Очищает файловые кэши профилей Vivaldi без удаления пользовательских данных.",
                () => BrowserPathService.Chromium(vivaldiRoot),
                ["vivaldi"],
                Directory.Exists(vivaldiRoot)),

            new BrowserCacheCleanerTask(
                logger,
                "yandex",
                "Яндекс Браузер",
                "Очищает файловые кэши профилей Яндекс Браузера без удаления cookies, паролей и закладок.",
                () => BrowserPathService.Chromium(yandexRoot),
                ["browser"],
                Directory.Exists(yandexRoot)),

            new WindowsUpdateCacheCleanerTask(logger),

            new DeliveryOptimizationCacheCleanerTask(logger),

            new FileCleanerTask(
                logger,
                "windows-update-old-logs",
                "Windows Update",
                "Старые логи обновлений и установки",
                "Удаляет только старые журналы CBS, DISM, MoSetup и Windows Setup возрастом более 30 дней. Свежие журналы диагностики сохраняются.",
                () =>
                [
                    new CleanupLocation(Path.Combine(windows, "Logs", "CBS"), CleanupLocationKind.FilePattern, "CbsPersist_*.log", "CBS: старые журналы", TimeSpan.FromDays(30)),
                    new CleanupLocation(Path.Combine(windows, "Logs", "CBS"), CleanupLocationKind.FilePattern, "CbsPersist_*.cab", "CBS: архивы журналов", TimeSpan.FromDays(30)),
                    new CleanupLocation(Path.Combine(windows, "Logs", "DISM"), CleanupLocationKind.FilePattern, "*.log", "DISM: старые журналы", TimeSpan.FromDays(30)),
                    new CleanupLocation(Path.Combine(windows, "Logs", "MoSetup"), CleanupLocationKind.FilePattern, "*.log", "MoSetup: старые журналы", TimeSpan.FromDays(30)),
                    new CleanupLocation(Path.Combine(windows, "Panther"), CleanupLocationKind.FilePattern, "*.log", "Windows Setup: старые журналы", TimeSpan.FromDays(30)),
                    new CleanupLocation(Path.Combine(windows, "SoftwareDistribution"), CleanupLocationKind.FilePattern, "ReportingEvents.log", "Windows Update: ReportingEvents.log", TimeSpan.FromDays(30))
                ],
                requiresAdministrator: true,
                availability: () =>
                    Directory.Exists(Path.Combine(windows, "Logs", "CBS")) ||
                    Directory.Exists(Path.Combine(windows, "Logs", "DISM")) ||
                    Directory.Exists(Path.Combine(windows, "Logs", "MoSetup")) ||
                    Directory.Exists(Path.Combine(windows, "Panther")) ||
                    File.Exists(Path.Combine(windows, "SoftwareDistribution", "ReportingEvents.log"))),

            new CommandCleanerTask(
                logger,
                "dns-cache",
                "Служебные действия",
                "DNS-кэш Windows",
                "Выполняет ipconfig /flushdns. Это не освобождает заметное место, но может помочь после изменения DNS или сетевых настроек.",
                "ipconfig.exe",
                "/flushdns")
        ];

        return tasks;
    }
}
