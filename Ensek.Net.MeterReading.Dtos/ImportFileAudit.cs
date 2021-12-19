namespace Ensek.Net.MeterReading.Dtos;

[ExcludeFromCodeCoverage]
public class ImportFileAudit
{
    public ImportFileAudit(string accountId, string meterReadingDateTime, string meterReadingValue)
    {
        AccountId = accountId;
        MeterReadingDateTime = meterReadingDateTime;
        MeterReadingValue = meterReadingValue;
    }

    public string AccountId { get; set; }
    public string MeterReadingDateTime { get; set; }
    public string MeterReadingValue { get; set; }
    public bool IsSuccess { get; set; }
    public string? FailureReason { get; set; }
}