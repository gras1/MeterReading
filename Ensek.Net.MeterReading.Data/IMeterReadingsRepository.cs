namespace Ensek.Net.MeterReading.Data;

public interface IMeterReadingsRepository
{
    void CreateNewMeterReadingRecord(int accountId, DateTime meterReadingDateTime, int meterReading,
        int auditId, bool leaveDbConnectionOpen);
}
