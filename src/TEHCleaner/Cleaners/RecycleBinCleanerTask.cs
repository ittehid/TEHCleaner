using System.Runtime.InteropServices;
using TEHCleaner.Models;
using TEHCleaner.Services;

namespace TEHCleaner.Cleaners;

public sealed class RecycleBinCleanerTask : CleanerTaskBase
{
    private const uint SHERB_NOCONFIRMATION = 0x00000001;
    private const uint SHERB_NOPROGRESSUI = 0x00000002;
    private const uint SHERB_NOSOUND = 0x00000004;

    public RecycleBinCleanerTask(AppLogger logger) : base(logger) { }

    public override string Id => "recycle-bin";
    public override string Category => "Windows";
    public override string Name => "Корзина";
    public override string Description => "Удаляет содержимое корзины текущего пользователя. По умолчанию не выбрано, чтобы случайно не удалить файлы, которые вы ещё хотели восстановить.";

    public override Task<ScanResult> ScanAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        SHQUERYRBINFO info = new() { cbSize = (uint)Marshal.SizeOf<SHQUERYRBINFO>() };
        int hr = SHQueryRecycleBin(null, ref info);
        return Task.FromResult(hr == 0
            ? new ScanResult(info.i64Size, info.i64NumItems > int.MaxValue ? int.MaxValue : (int)info.i64NumItems)
            : new ScanResult(0, 0, 0, "Не удалось получить размер корзины."));
    }

    public async override Task<CleanupResult> CleanAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ScanResult before = await ScanAsync(cancellationToken).ConfigureAwait(false);
        int hr = SHEmptyRecycleBin(IntPtr.Zero, null, SHERB_NOCONFIRMATION | SHERB_NOPROGRESSUI | SHERB_NOSOUND);
        if (hr != 0)
        {
            await Logger.WarningAsync($"[Корзина] Shell API вернул 0x{hr:X8}").ConfigureAwait(false);
            return CleanupResult.Failed($"Windows вернула ошибку 0x{hr:X8}.", before.Items);
        }

        await Logger.InfoAsync($"[Корзина] очищено {FormatHelper.Bytes(before.Bytes)}").ConfigureAwait(false);
        return new CleanupResult(before.Bytes, before.Items, 0, true, Details: [new CleanupDetail("Корзина пользователя", "Shell:RecycleBinFolder", before.Items, before.Bytes)]);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    private struct SHQUERYRBINFO
    {
        public uint cbSize;
        public long i64Size;
        public long i64NumItems;
    }

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    private static extern int SHQueryRecycleBin(string? pszRootPath, ref SHQUERYRBINFO pSHQueryRBInfo);

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    private static extern int SHEmptyRecycleBin(IntPtr hwnd, string? pszRootPath, uint dwFlags);
}
