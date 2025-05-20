namespace API.Helpers;

public static class StringHelper
{
    public static bool IsUrl(string value)
    {
        if (!Uri.TryCreate(value, UriKind.Absolute, out var url))
            return false;

        return url.Scheme is "http" or "https";
    }
}