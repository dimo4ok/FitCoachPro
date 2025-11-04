using FitCoachPro.Application.Common.Response;

namespace FitCoachPro.Application.Common.Errors;

public class DomainErrors
{
    public static Error EmptyDomainData => new("Domain.DataMissing", "The user account is missing domain data.");
}
