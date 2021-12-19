namespace Ensek.Net.MeterReading.Shared.Exceptions;

[ExcludeFromCodeCoverage]
public class FileProcessorException : Exception
{
    public FileProcessorException()
    {
    }

    public FileProcessorException(string message)
        : base(message)
    {
    }

    public FileProcessorException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
