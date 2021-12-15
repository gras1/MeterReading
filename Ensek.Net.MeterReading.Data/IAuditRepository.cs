namespace Ensek.Net.MeterReading.Data;

public interface IAuditRepository
{
    int CreateNewAuditRecord(string fileName);

    void UpdateAuditRecord(int auditId, int numberOfSuccessfullyImportedRecords, int numberOfFailedRecords, string failedRecordDetails);
}
