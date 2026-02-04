namespace FitCoachPro.Application.Common.Extensions;

public static class DateExtensions
{
    public static DateOnly ToDateOnly(this DateTime dateTime) =>
            DateOnly.FromDateTime(dateTime);

    public static DateTime ToDateTime(this DateOnly dateOnly) =>
        dateOnly.ToDateTime();
}
