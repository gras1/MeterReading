namespace Ensek.Net.MeterReading.Api.Models
{
    [ExcludeFromCodeCoverage]
    public class MeterReadingFileUploadResponse
    {
        public int NumberOfRecordsSuccessfullyImported { get; set; }
        public int NumberOfRecordsFailedToImport { get; set; }
        public List<ImportFileAudit>? ImportFailureReasons { get; set; }
    }
}