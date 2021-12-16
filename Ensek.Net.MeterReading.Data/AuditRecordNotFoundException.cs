namespace Ensek.Net.MeterReading.Data;

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
