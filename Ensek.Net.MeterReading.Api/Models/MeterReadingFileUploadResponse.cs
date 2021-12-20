namespace Ensek.Net.MeterReading.Api.Models
{
    [ExcludeFromCodeCoverage]
    public class MeterReadingFileUploadResponse
    {
        [JsonPropertyName("numberOfRecordsSuccessfullyImported")]
        public int NumberOfRecordsSuccessfullyImported { get; set; }

        [JsonPropertyName("numberOfRecordsFailedToImport")]
        public int NumberOfRecordsFailedToImport { get; set; }

        [JsonPropertyName("importFailureReasons")]
        public List<ImportFileAudit>? ImportFailureReasons { get; set; }
    }
}