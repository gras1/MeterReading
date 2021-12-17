namespace Ensek.Net.MeterReading.Data;

public static class GuardClauseExtensions
{
    public static void MustNotBeNullOrWhitespaceIfComparitorIsGreaterThanZero(this IGuardClause guardClause,
        int comparitor, string input, string parameterName)
    {
        if (comparitor > 0 && string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException("input should not be null or white space", parameterName);
        }
    }
}
