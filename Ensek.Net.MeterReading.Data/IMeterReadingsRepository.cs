namespace Ensek.Net.MeterReading.Data;

public interface IMeterReadingsRepository
{
    int CreateNewMeterReadingRecord(int accountId, DateTime meterReadingDateTime, int meterReading,
        int auditId, bool leaveDbConnectionOpen);
}
