namespace FitCoachPro.Tests;

public static class TestCleaner
{
    public static void Clean()
    {
        var context = NSubstitute.Core.SubstitutionContext.Current;
        context.ThreadContext.DequeueAllArgumentSpecifications();
    }
}
