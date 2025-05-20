using System.Text.RegularExpressions;
using Application.Services;

namespace Application.Extensions;

public static class StringExtensions
{
    public static bool IsEmail(this string input)
        => RegexHelper.ForEmail().IsMatch(input);
}