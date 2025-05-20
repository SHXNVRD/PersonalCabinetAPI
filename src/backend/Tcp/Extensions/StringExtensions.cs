namespace Tcp.Extensions;

public static class StringExtensions
{
    public static string? GetBetweenOrDefault(
        this string source, 
        string start, 
        string end, 
        StringComparison comparisonType = StringComparison.CurrentCulture)
    {
        if (!(source.Contains(start) && source.Contains(end)))
            return default;
        
        var startIndex = source.IndexOf(start, comparisonType) + start.Length;
        var endIndex = source.IndexOf(end, startIndex, comparisonType);

        if (startIndex == -1 || endIndex == -1)
            return default;
        
        return source.Substring(startIndex, endIndex - startIndex);
    }
}