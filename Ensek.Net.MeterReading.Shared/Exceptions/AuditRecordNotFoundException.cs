namespace Ensek.Net.MeterReading.Shared.Exceptions;

[ExcludeFromCodeCoverage]
public class AuditRecordNotFoundException : Exception
{
    public AuditRecordNotFoundException()
    {
    }

    public AuditRecordNotFoundException(string message)
        : base(message)
    {
    }

    public AuditRecordNotFoundException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
