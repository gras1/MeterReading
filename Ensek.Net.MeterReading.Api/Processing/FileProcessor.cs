namespace Ensek.Net.MeterReading.Api.Processing;

public class FileProcessor : IFileProcessor
{
    private readonly IAuditRepository _auditRepository;

    public FileProcessor(IAuditRepository auditRepository)
    {
        Guard.Against.Null(auditRepository, nameof(auditRepository));
        _auditRepository = auditRepository;
    }

    public ImportFile Process(string meterReadingFileName, byte[] meterReadingFileBytes)
    {
        var importFileAudits = new List<ImportFileAudit>();

        using var stream = new MemoryStream(meterReadingFileBytes);
        using var streamReader = new StreamReader(stream);
        var csvInput = streamReader.ReadToEnd();
        using var csvReader = new StringReader(csvInput);
        using var parser = new CsvTextFieldParser(csvReader);
        
        if (!parser.EndOfData)
        {
            parser.ReadFields();
        }

        while (!parser.EndOfData)
        {
            var csvLine = parser.ReadFields();
            var importFileAudit = new ImportFileAudit(csvLine[0], csvLine[1], csvLine[2]);
            importFileAudits.Add(importFileAudit);
        }

        var importFile = new ImportFile(importFileAudits);

        if (importFileAudits.Count > 0)
        {
            importFile.AuditId = _auditRepository.CreateNewAuditRecord(meterReadingFileName);
        }
        
        return importFile;
    }
}