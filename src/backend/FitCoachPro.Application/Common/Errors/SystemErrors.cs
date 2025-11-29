using FitCoachPro.Application.Common.Response;

namespace FitCoachPro.Application.Common.Errors;

public class SystemErrors
{
    public static Error TransactionFailed => new("System.TransactionFailed", "Failed to complete the operation. Please try again later.");
    public static Error DbConnectionFailed => new("System.DbConnectionFailed", "Database connection failed.");
    public static Error ConcurrencyConflict =>
       new("System.ConcurrencyConflict", "The data was modified by another user. Please refresh and try again.");
}
