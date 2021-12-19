namespace Ensek.Net.MeterReading.Api.Tests.Unit;

[ExcludeFromCodeCoverage]
public class FileProcessorTests
{
    private readonly IFileProcessor _fileProcessor;
    private readonly IAuditRepository _auditRepositoryFake;

    public FileProcessorTests()
    {
        _auditRepositoryFake = A.Fake<IAuditRepository>();
        _fileProcessor = new FileProcessor(_auditRepositoryFake);
    }

    [Fact]
    public void FileProcessor_Implements_IFileProcessor()
    {
        typeof(IFileProcessor).IsAssignableFrom(typeof(FileProcessor));
    }

    [Fact]
    public void Constructor_WhenIAuditRepositoryIsNull_ThrowsArgumentNullException()
    {
        Action act = () => new FileProcessor(default(IAuditRepository)!);

        act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'auditRepository')");
    }

    [Fact]
    public void Process_WithValidThreeColumnCsvFileByteArray_CallsAuditRepositoryCreateNewAuditRecordOnce()
    {
        string filename = "test.csv";
        byte[] testMeterReadingFileContentBytes = Encoding.ASCII.GetBytes(Properties.Resources.TestMeterReadingFileContent);
        A.CallTo(() => _auditRepositoryFake.CreateNewAuditRecord(filename)).Returns(1);

        _ = _fileProcessor.Process(filename, testMeterReadingFileContentBytes);

        A.CallTo(() => _auditRepositoryFake.CreateNewAuditRecord(filename)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void Process_WithValidThreeColumnCsvFileByteArray_ReturnsPopulatedImportFile()
    {
        string filename = "test.csv";
        byte[] testMeterReadingFileContentBytes = Encoding.ASCII.GetBytes(Properties.Resources.TestMeterReadingFileContent);
        A.CallTo(() => _auditRepositoryFake.CreateNewAuditRecord(filename)).Returns(1);
        var importFileAudits = new List<ImportFileAudit> { 
            new ImportFileAudit("2344", "22/04/2019 09:24", "01002"),
            new ImportFileAudit("2346", "22/04/2019 12:25", "999999"),
            new ImportFileAudit("2349", "22/04/2019 12:25", "VOID"),
            new ImportFileAudit("2356", "07/05/2019 09:24", "00000"),
            new ImportFileAudit("2344", "08/05/2019 09:24", "0X765"),
            new ImportFileAudit("6776", "09/05/2019 09:24", "-06575"),
            new ImportFileAudit("1235", "13/05/2019 09:24", ""),
            new ImportFileAudit("1241", "11/04/2019 09:24", "00436")
        };
        var expectedResponse = new ImportFile(importFileAudits) { AuditId = 1 };

        var actualResponse = _fileProcessor.Process(filename, testMeterReadingFileContentBytes);

        expectedResponse.Should().BeEquivalentTo(actualResponse);
    }

    [Fact]
    public void Process_WithInvalidTwoColumnCsvFileByteArray_ThrowsFileProcessorException()
    {
        string filename = "test.csv";
        byte[] testMeterReadingFileContentBytes = Encoding.ASCII.GetBytes(Properties.Resources.InvalidTestMeterReadingFileContent);
        A.CallTo(() => _auditRepositoryFake.CreateNewAuditRecord(filename)).Returns(1);

        Action act = () => _fileProcessor.Process(filename, testMeterReadingFileContentBytes);

        act.Should().Throw<FileProcessorException>();
    }

    [Fact]
    public void Process_WithNullMeterReadingFileName_ThrowsFileProcessorException()
    {
        byte[] testMeterReadingFileContentBytes = Encoding.ASCII.GetBytes(Properties.Resources.InvalidTestMeterReadingFileContent);

        Action act = () => _fileProcessor.Process(default(string)!, testMeterReadingFileContentBytes);

        act.Should().Throw<FileProcessorException>();
    }

    [Fact]
    public void Process_WithNullMeterReadingFileBytes_ThrowsFileProcessorException()
    {
        Action act = () => _fileProcessor.Process("test", new List<byte>().ToArray());

        act.Should().Throw<FileProcessorException>();
    }
}