namespace Ensek.Net.MeterReading.Api.Tests.Unit.Processing;

[ExcludeFromCodeCoverage]
public class MeterReadingProcessorTests
{
    private DateTime ValidImportDateTime = new DateTime(2015, 4, 19, 2, 23, 9);
    private string ValidAccountId = "5001";
    private string ValidMeterReadingValue = "00050";
    private readonly IMeterReadingsRepository _meterReadingsRepositoryFake;
    private readonly MeterReadingProcessor _meterReadingProcessor;

    public MeterReadingProcessorTests()
    {
        _meterReadingsRepositoryFake = A.Fake<IMeterReadingsRepository>();
        _meterReadingProcessor = new MeterReadingProcessor(_meterReadingsRepositoryFake);
    }

    [Fact]
    public void MeterReadingProcessor_Implements_IMeterReadingProcessor()
    {
        typeof(IMeterReadingProcessor).IsAssignableFrom(typeof(MeterReadingProcessor));
    }

    [Fact]
    public void Constructor_WhenIMeterReadingsRepositoryIsNull_ThrowsArgumentNullException()
    {
        Action act = () => new MeterReadingProcessor(default(IMeterReadingsRepository)!);

        act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'meterReadingsRepository')");
    }

    [Fact]
    public void Process_WhenAuditIdIsZero_ThrowsArgumentException()
    {
        var importFileAudits = new List<ImportFileAudit>();
        Action act = () => _meterReadingProcessor.Process(0, importFileAudits);

        act.Should().Throw<ArgumentException>().WithMessage("Required input auditId cannot be zero or negative. (Parameter 'auditId')");
    }

    [Fact]
    public void Process_WhenImportFileAuditsIsNull_ThrowsArgumentNullException()
    {
        Action act = () => _meterReadingProcessor.Process(1, default(List<ImportFileAudit>)!);

        act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'importFileAudits')");
    }

    [Fact]
    public void Process_WhenImportFileAuditsContainsNullAccountId_ReturnsMeterReadingFileUploadResponseContainingImportFailureReasons()
    {
        var importFileAudits = new List<ImportFileAudit>{
            new ImportFileAudit(default(string)!, ValidImportDateTime.ToString("yyyy-MM-dd hh:mm:ss"), ValidMeterReadingValue)
        };
        var expectedImportFailureReasons = new List<ImportFileAudit>{
            new ImportFileAudit(null!, ValidImportDateTime.ToString("yyyy-MM-dd hh:mm:ss"), ValidMeterReadingValue){
                FailureReason = "account id is not provided"
            }
        };

        var actualResponse = _meterReadingProcessor.Process(1, importFileAudits);

        actualResponse.ImportFailureReasons.Should().BeEquivalentTo(expectedImportFailureReasons);
    }

    [Fact]
    public void Process_WhenImportFileAuditsContainsNonIntegerAccountId_ReturnsMeterReadingFileUploadResponseContainingImportFailureReasons()
    {
        var importFileAudits = new List<ImportFileAudit>{
            new ImportFileAudit("X", ValidImportDateTime.ToString("yyyy-MM-dd hh:mm:ss"), ValidMeterReadingValue)
        };
        var expectedImportFailureReasons = new List<ImportFileAudit>{
            new ImportFileAudit("X", ValidImportDateTime.ToString("yyyy-MM-dd hh:mm:ss"), ValidMeterReadingValue){
                FailureReason = "account id is not valid"
            }
        };

        var actualResponse = _meterReadingProcessor.Process(1, importFileAudits);

        actualResponse.ImportFailureReasons.Should().BeEquivalentTo(expectedImportFailureReasons);
    }

    [Fact]
    public void Process_WhenImportFileAuditsContainsNegativeAccountId_ReturnsMeterReadingFileUploadResponseContainingImportFailureReasons()
    {
        var importFileAudits = new List<ImportFileAudit>{
            new ImportFileAudit("-50", ValidImportDateTime.ToString("yyyy-MM-dd hh:mm:ss"), ValidMeterReadingValue)
        };
        var expectedImportFailureReasons = new List<ImportFileAudit>{
            new ImportFileAudit("-50", ValidImportDateTime.ToString("yyyy-MM-dd hh:mm:ss"), ValidMeterReadingValue){
                FailureReason = "account id cannot have a value of zero or less"
            }
        };

        var actualResponse = _meterReadingProcessor.Process(1, importFileAudits);

        actualResponse.ImportFailureReasons.Should().BeEquivalentTo(expectedImportFailureReasons);
    }

    [Fact]
    public void Process_WhenImportFileAuditsContainsNullMeterReadingDateTime_ReturnsMeterReadingFileUploadResponseContainingImportFailureReasons()
    {
        var importFileAudits = new List<ImportFileAudit>{
            new ImportFileAudit(ValidAccountId, default(string)!, ValidMeterReadingValue)
        };
        var expectedImportFailureReasons = new List<ImportFileAudit>{
            new ImportFileAudit(ValidAccountId, null!, ValidMeterReadingValue){
                FailureReason = "meter reading date time is not provided"
            }
        };

        var actualResponse = _meterReadingProcessor.Process(1, importFileAudits);

        actualResponse.ImportFailureReasons.Should().BeEquivalentTo(expectedImportFailureReasons);
    }

    [Fact]
    public void Process_WhenImportFileAuditsContainsNonDateTimeMeterReadingDateTime_ReturnsMeterReadingFileUploadResponseContainingImportFailureReasons()
    {
        var importFileAudits = new List<ImportFileAudit>{
            new ImportFileAudit(ValidAccountId, "X", ValidMeterReadingValue)
        };
        var expectedImportFailureReasons = new List<ImportFileAudit>{
            new ImportFileAudit(ValidAccountId, "X", ValidMeterReadingValue){
                FailureReason = "meter reading date time is not valid"
            }
        };

        var actualResponse = _meterReadingProcessor.Process(1, importFileAudits);

        actualResponse.ImportFailureReasons.Should().BeEquivalentTo(expectedImportFailureReasons);
    }

    [Fact]
    public void Process_WhenImportFileAuditsContainsMinimumMeterReadingDateTime_ReturnsMeterReadingFileUploadResponseContainingImportFailureReasons()
    {
        var importFileAudits = new List<ImportFileAudit>{
            new ImportFileAudit(ValidAccountId, DateTime.MinValue.ToString("yyyy-MM-dd"), ValidMeterReadingValue)
        };
        var expectedImportFailureReasons = new List<ImportFileAudit>{
            new ImportFileAudit(ValidAccountId, DateTime.MinValue.ToString("yyyy-MM-dd"), ValidMeterReadingValue){
                FailureReason = "meter reading date time cannot have a minimum value"
            }
        };

        var actualResponse = _meterReadingProcessor.Process(1, importFileAudits);

        actualResponse.ImportFailureReasons.Should().BeEquivalentTo(expectedImportFailureReasons);
    }

    [Fact]
    public void Process_WhenImportFileAuditsContainsNullMeterReadingValue_ReturnsMeterReadingFileUploadResponseContainingImportFailureReasons()
    {
        var importFileAudits = new List<ImportFileAudit>{
            new ImportFileAudit(ValidAccountId, ValidImportDateTime.ToString("yyyy-MM-dd hh:mm:ss"), default(string)!)
        };
        var expectedImportFailureReasons = new List<ImportFileAudit>{
            new ImportFileAudit(ValidAccountId, ValidImportDateTime.ToString("yyyy-MM-dd hh:mm:ss"), null!){
                FailureReason = "meter reading value is not provided"
            }
        };

        var actualResponse = _meterReadingProcessor.Process(1, importFileAudits);

        actualResponse.ImportFailureReasons.Should().BeEquivalentTo(expectedImportFailureReasons);
    }

    [Fact]
    public void Process_WhenImportFileAuditsContainsNonIntegerMeterReadingValue_ReturnsMeterReadingFileUploadResponseContainingImportFailureReasons()
    {
        var importFileAudits = new List<ImportFileAudit>{
            new ImportFileAudit(ValidAccountId, ValidImportDateTime.ToString("yyyy-MM-dd hh:mm:ss"), "X")
        };
        var expectedImportFailureReasons = new List<ImportFileAudit>{
            new ImportFileAudit(ValidAccountId, ValidImportDateTime.ToString("yyyy-MM-dd hh:mm:ss"), "X"){
                FailureReason = "meter reading value is not valid"
            }
        };

        var actualResponse = _meterReadingProcessor.Process(1, importFileAudits);

        actualResponse.ImportFailureReasons.Should().BeEquivalentTo(expectedImportFailureReasons);
    }

    [Fact]
    public void Process_WhenImportFileAuditsContainsNegativeMeterReadingValue_ReturnsMeterReadingFileUploadResponseContainingImportFailureReasons()
    {
        var importFileAudits = new List<ImportFileAudit>{
            new ImportFileAudit(ValidAccountId, ValidImportDateTime.ToString("yyyy-MM-dd hh:mm:ss"), "-50")
        };
        var expectedImportFailureReasons = new List<ImportFileAudit>{
            new ImportFileAudit(ValidAccountId, ValidImportDateTime.ToString("yyyy-MM-dd hh:mm:ss"), "-50"){
                FailureReason = "meter reading value cannot be a negative number"
            }
        };

        var actualResponse = _meterReadingProcessor.Process(1, importFileAudits);

        actualResponse.ImportFailureReasons.Should().BeEquivalentTo(expectedImportFailureReasons);
    }

    [Fact]
    public void Process_WhenMeterReadingsRepositoryCreateNewMeterReadingRecordThrowsMeterReadingRecordNotCreatedException_ReturnsMeterReadingFileUploadResponseContainingImportFailureReasons()
    {
        var importFileAudits = new List<ImportFileAudit>{
            new ImportFileAudit(ValidAccountId, ValidImportDateTime.ToString("yyyy-MM-dd hh:mm:ss"), ValidMeterReadingValue)
        };
        var expectedImportFailureReasons = new List<ImportFileAudit>{
            new ImportFileAudit(ValidAccountId, ValidImportDateTime.ToString("yyyy-MM-dd hh:mm:ss"), ValidMeterReadingValue){
                FailureReason = "test"
            }
        };
        A.CallTo(() => _meterReadingsRepositoryFake.CreateNewMeterReadingRecord(Convert.ToInt32(ValidAccountId), ValidImportDateTime, Convert.ToInt32(ValidMeterReadingValue), 1, false)).Throws(new MeterReadingRecordNotCreatedException("test"));

        var actualResponse = _meterReadingProcessor.Process(1, importFileAudits);

        actualResponse.ImportFailureReasons.Should().BeEquivalentTo(expectedImportFailureReasons);
    }

    [Fact]
    public void Process_WhenMeterReadingsRepositoryCreateNewMeterReadingRecordThrowsArgumentOutOfRangeException_ReturnsMeterReadingFileUploadResponseContainingImportFailureReasons()
    {
        var importFileAudits = new List<ImportFileAudit>{
            new ImportFileAudit(ValidAccountId, ValidImportDateTime.ToString("yyyy-MM-dd hh:mm:ss"), ValidMeterReadingValue)
        };
        var expectedImportFailureReasons = new List<ImportFileAudit>{
            new ImportFileAudit(ValidAccountId, ValidImportDateTime.ToString("yyyy-MM-dd hh:mm:ss"), ValidMeterReadingValue){
                FailureReason = "Specified argument was out of the range of valid values. (Parameter 'test')"
            }
        };
        A.CallTo(() => _meterReadingsRepositoryFake.CreateNewMeterReadingRecord(Convert.ToInt32(ValidAccountId), ValidImportDateTime, Convert.ToInt32(ValidMeterReadingValue), 1, false)).Throws(new ArgumentOutOfRangeException("test"));

        var actualResponse = _meterReadingProcessor.Process(1, importFileAudits);

        actualResponse.ImportFailureReasons.Should().BeEquivalentTo(expectedImportFailureReasons);
    }

    [Fact]
    public void Process_WhenMeterReadingsRepositoryCreateNewMeterReadingRecordThrowsArgumentException_ReturnsMeterReadingFileUploadResponseContainingImportFailureReasons()
    {
        var importFileAudits = new List<ImportFileAudit>{
            new ImportFileAudit(ValidAccountId, ValidImportDateTime.ToString("yyyy-MM-dd hh:mm:ss"), ValidMeterReadingValue)
        };
        var expectedImportFailureReasons = new List<ImportFileAudit>{
            new ImportFileAudit(ValidAccountId, ValidImportDateTime.ToString("yyyy-MM-dd hh:mm:ss"), ValidMeterReadingValue){
                FailureReason = "test"
            }
        };
        A.CallTo(() => _meterReadingsRepositoryFake.CreateNewMeterReadingRecord(Convert.ToInt32(ValidAccountId), ValidImportDateTime, Convert.ToInt32(ValidMeterReadingValue), 1, false)).Throws(new ArgumentException("test"));

        var actualResponse = _meterReadingProcessor.Process(1, importFileAudits);

        actualResponse.ImportFailureReasons.Should().BeEquivalentTo(expectedImportFailureReasons);
    }

    [Fact]
    public void Process_WhenValidImportFileAudits_ReturnsMeterReadingFileUploadResponseWithNumberOfRecordsSuccessfullyImportedAs1()
    {
        var importFileAudits = new List<ImportFileAudit>{
            new ImportFileAudit(ValidAccountId, ValidImportDateTime.ToString("yyyy-MM-dd hh:mm:ss"), ValidMeterReadingValue)
        };

        var actualResponse = _meterReadingProcessor.Process(1, importFileAudits);

        actualResponse.NumberOfRecordsSuccessfullyImported.Should().Be(1);
    }

    [Fact]
    public void Process_WhenInvalidImportFileAudits_ReturnsMeterReadingFileUploadResponseWithNumberOfRecordsFailedToImportAs1()
    {
        var importFileAudits = new List<ImportFileAudit>{
            new ImportFileAudit(ValidAccountId, ValidImportDateTime.ToString("yyyy-MM-dd hh:mm:ss"), "9999999")
        };

        A.CallTo(() => _meterReadingsRepositoryFake.CreateNewMeterReadingRecord(Convert.ToInt32(ValidAccountId), ValidImportDateTime, 9999999, 1, false)).Throws(new MeterReadingRecordNotCreatedException("test"));

        var actualResponse = _meterReadingProcessor.Process(1, importFileAudits);

        actualResponse.NumberOfRecordsFailedToImport.Should().Be(1);
    }
}
