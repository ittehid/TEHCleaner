namespace TEHCleaner.Services;

public static class FormatHelper
{
    private static readonly string[] Units = ["Б", "КБ", "МБ", "ГБ", "ТБ"];

    public static string Bytes(long value)
    {
        if (value <= 0) return "0 Б";
        double size = value;
        int unit = 0;
        while (size >= 1024 && unit < Units.Length - 1)
        {
            size /= 1024;
            unit++;
        }

        return unit == 0 ? $"{size:0} {Units[unit]}" : $"{size:0.##} {Units[unit]}";
    }

    public static string Items(int value) => value switch
    {
        0 => "—",
        1 => "1 объект",
        _ => $"{value:N0} объектов"
    };
}
