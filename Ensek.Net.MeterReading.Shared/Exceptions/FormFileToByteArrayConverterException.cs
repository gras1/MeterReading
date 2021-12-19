namespace Ensek.Net.MeterReading.Shared.Exceptions;

[ExcludeFromCodeCoverage]
public class FormFileToByteArrayConverterException : Exception
{
    public FormFileToByteArrayConverterException()
    {
    }

    public FormFileToByteArrayConverterException(string message)
        : base(message)
    {
    }

    public FormFileToByteArrayConverterException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
