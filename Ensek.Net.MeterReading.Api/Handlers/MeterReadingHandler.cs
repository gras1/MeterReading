namespace Ensek.Net.MeterReading.Api.Handlers;

public class MeterReadingHandler : IMeterReadingHandler
{
    private readonly IFileProcessor _fileProcessor;
    private readonly IMeterReadingProcessor _meterReadingProcessor;
    private readonly IAuditRepository _auditRepository;
    private readonly IFormFileToByteArrayConverter _formFileToByteArrayConverter;

    public MeterReadingHandler(IFileProcessor fileProcessor, IMeterReadingProcessor meterReadingProcessor,
        IAuditRepository auditRepository, IFormFileToByteArrayConverter formFileToByteArrayConverter)
    {
        Guard.Against.Null(fileProcessor, nameof(fileProcessor));
        Guard.Against.Null(meterReadingProcessor, nameof(meterReadingProcessor));
        Guard.Against.Null(auditRepository, nameof(auditRepository));
        Guard.Against.Null(formFileToByteArrayConverter, nameof(formFileToByteArrayConverter));
        _fileProcessor = fileProcessor;
        _meterReadingProcessor = meterReadingProcessor;
        _auditRepository = auditRepository;
        _formFileToByteArrayConverter = formFileToByteArrayConverter;
    }

    public async Task<MeterReadingFileUploadResponse> HandleMeterReadings(IFormFile meterReadingFile)
    {
        byte[] meterReadingFileBytes = await _formFileToByteArrayConverter.Convert(meterReadingFile);

        var importFile = _fileProcessor.Process(meterReadingFile.Name, meterReadingFileBytes);

        var uploadResponse = _meterReadingProcessor.Process(importFile.AuditId, importFile.ImportFileAudits);
        
        string failedRecordDetails = null!;
        if (uploadResponse.ImportFailureReasons != null && uploadResponse.ImportFailureReasons.Count > 0)
        {
            var failureReasons = (from ifr in uploadResponse.ImportFailureReasons
                                  select ifr.FailureReason).ToList();
            failedRecordDetails = string.Join(",", failureReasons);
        }
        
        _auditRepository.UpdateAuditRecord(importFile.AuditId, uploadResponse.NumberOfRecordsSuccessfullyImported,
            uploadResponse.NumberOfRecordsFailedToImport, failedRecordDetails, false);

        return uploadResponse;
    }
}
