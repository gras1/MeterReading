namespace Ensek.Net.MeterReading.Dtos;

public class ImportFile
{
    public ImportFile(List<ImportFileAudit> importFileAudits)
    {
        ImportFileAudits = importFileAudits;
    }

    public List<ImportFileAudit> ImportFileAudits {get; set;}
    public int AuditId { get; set; }
}
