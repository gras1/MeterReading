namespace Ensek.Net.MeterReading.Shared.Exceptions;

[ExcludeFromCodeCoverage]
public class MeterReadingRecordNotCreatedException : Exception
{
    public MeterReadingRecordNotCreatedException()
    {
    }

    public MeterReadingRecordNotCreatedException(string message)
        : base(message)
    {
    }

    public MeterReadingRecordNotCreatedException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
