namespace FitCoachPro.Application.Common.Extensions;
public static class StringExtensions
{
    public static string NormalizeValue(this string? value) =>
        value?.Trim().ToLower() ?? string.Empty;
}
