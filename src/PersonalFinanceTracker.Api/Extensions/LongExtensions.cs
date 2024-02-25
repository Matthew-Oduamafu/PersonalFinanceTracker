namespace PersonalFinanceTracker.Api.Extensions;

public static class LongExtensions
{
    public static string ToReadableSize(this long length)
    {
        string[] sizes = ["B", "KB", "MB", "GB", "TB"];
        var order = 0;
        while (length >= 1024 && order < sizes.Length - 1)
        {
            order++;
            length = length / 1024L;
        }

        return string.Format("{0:0.##} {1}", length, sizes[order]);
    }
}