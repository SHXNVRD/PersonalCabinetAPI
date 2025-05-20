using System.Text.RegularExpressions;

namespace Application.Services;

public partial class RegexHelper
{
    [GeneratedRegex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,}$", RegexOptions.IgnoreCase, "ru-RU")]
    public static partial Regex ForEmail();
}