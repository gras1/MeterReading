namespace Ensek.Net.MeterReading.Api.Processing;

public interface IMeterReadingProcessor
{
    MeterReadingFileUploadResponse Process(int auditId, List<ImportFileAudit> importFileAudits);
}
