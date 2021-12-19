namespace Ensek.Net.MeterReading.Api.Tests.Unit.Handlers;

[ExcludeFromCodeCoverage]
public class MeterReadingHandlerTests
{
    private readonly IFileProcessor _fileProcessorFake;
    private readonly IMeterReadingProcessor _meterReadingProcessorFake;
    private readonly IAuditRepository _auditRepositoryFake;
    private readonly IFormFileToByteArrayConverter _formFileToByteArrayConverterFake;
    private readonly IFormFile _formFileFake;

    public MeterReadingHandlerTests()
    {
        _fileProcessorFake = A.Fake<IFileProcessor>();
        _meterReadingProcessorFake = A.Fake<IMeterReadingProcessor>();
        _auditRepositoryFake = A.Fake<IAuditRepository>();
        _formFileToByteArrayConverterFake = A.Fake<IFormFileToByteArrayConverter>();
        _formFileFake = A.Fake<IFormFile>();
        var fileName = "test.csv";
        A.CallTo(() => _formFileFake.FileName).Returns(fileName);
    }

    [Fact]
    public void MeterReadingHandler_Implements_IMeterReadingHandler()
    {
        typeof(IMeterReadingHandler).IsAssignableFrom(typeof(MeterReadingHandler));
    }

    [Fact]
    public void Constructor_WhenIFileProcessorIsNull_ThrowsArgumentNullException()
    {
        Action act = () => new MeterReadingHandler(
            default(IFileProcessor)!,
            _meterReadingProcessorFake,
            _auditRepositoryFake,
            _formFileToByteArrayConverterFake);

        act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'fileProcessor')");
    }

    [Fact]
    public void Constructor_WhenIMeterReadingProcessorIsNull_ThrowsArgumentNullException()
    {
        Action act = () => new MeterReadingHandler(
            _fileProcessorFake,
            default(IMeterReadingProcessor)!,
            _auditRepositoryFake,
            _formFileToByteArrayConverterFake);

        act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'meterReadingProcessor')");
    }

    [Fact]
    public void Constructor_WhenIAuditRepositoryIsNull_ThrowsArgumentNullException()
    {
        Action act = () => new MeterReadingHandler(
            _fileProcessorFake,
            _meterReadingProcessorFake,
            default(IAuditRepository)!,
            _formFileToByteArrayConverterFake);

        act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'auditRepository')");
    }

    [Fact]
    public void Constructor_WhenIFormFileToByteArrayConverterIsNull_ThrowsArgumentNullException()
    {
        Action act = () => new MeterReadingHandler(
            _fileProcessorFake,
            _meterReadingProcessorFake,
            _auditRepositoryFake,
            default(IFormFileToByteArrayConverter)!);

        act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'formFileToByteArrayConverter')");
    }

    [Fact]
    public async Task HandleMeterReadings_WithValidIFormFile_ReturnsPopulatedMeterReadingFileUploadResponse()
    {
        var meterReadingFileBytes = new byte[1]{35};
        var importFileAudits = new List<ImportFileAudit>();
        var importFile = new ImportFile(importFileAudits){
            AuditId = 1
        };
        var uploadResponse = new MeterReadingFileUploadResponse();
        A.CallTo(() => _formFileToByteArrayConverterFake.Convert(_formFileFake)).Returns(meterReadingFileBytes);
        A.CallTo(() => _fileProcessorFake.Process(_formFileFake.Name, meterReadingFileBytes)).Returns(importFile);
        A.CallTo(() => _meterReadingProcessorFake.Process(importFile.AuditId, importFile.ImportFileAudits)).Returns(uploadResponse);

        var meterReadingHandler = new MeterReadingHandler(_fileProcessorFake, _meterReadingProcessorFake,
             _auditRepositoryFake, _formFileToByteArrayConverterFake);

        var actualResponse = await meterReadingHandler.HandleMeterReadings(_formFileFake);

        actualResponse.Should().BeEquivalentTo(uploadResponse);
    }
}
