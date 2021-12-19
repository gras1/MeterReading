namespace Ensek.Net.MeterReading.Api.Tests.Unit;

[ExcludeFromCodeCoverage]
public class FileProcessorTests
{
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
        var auditRepositoryFake = A.Fake<IAuditRepository>();
        var fileProcessor = new FileProcessor(auditRepositoryFake);
        byte[] testMeterReadingFileContentBytes = Encoding.ASCII.GetBytes(Properties.Resources.TestMeterReadingFileContent);
        A.CallTo(() => auditRepositoryFake.CreateNewAuditRecord(filename)).Returns(1);

        _ = fileProcessor.Process(filename, testMeterReadingFileContentBytes);

        A.CallTo(() => auditRepositoryFake.CreateNewAuditRecord(filename)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void Process_WithValidThreeColumnCsvFileByteArray_ReturnsPopulatedImportFile()
    {
        string filename = "test.csv";
        var auditRepositoryFake = A.Fake<IAuditRepository>();
        var fileProcessor = new FileProcessor(auditRepositoryFake);
        byte[] testMeterReadingFileContentBytes = Encoding.ASCII.GetBytes(Properties.Resources.TestMeterReadingFileContent);
        A.CallTo(() => auditRepositoryFake.CreateNewAuditRecord(filename)).Returns(1);
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

        var actualResponse = fileProcessor.Process(filename, testMeterReadingFileContentBytes);

        expectedResponse.Should().BeEquivalentTo(actualResponse);
    }

    [Fact]
    public void Process_WithInvalidTwoColumnCsvFileByteArray_ThrowsIndexOutOfRangeException()
    {
        string filename = "test.csv";
        var auditRepositoryFake = A.Fake<IAuditRepository>();
        var fileProcessor = new FileProcessor(auditRepositoryFake);
        byte[] testMeterReadingFileContentBytes = Encoding.ASCII.GetBytes(Properties.Resources.InvalidTestMeterReadingFileContent);
        A.CallTo(() => auditRepositoryFake.CreateNewAuditRecord(filename)).Returns(1);

        Action act = () => fileProcessor.Process(filename, testMeterReadingFileContentBytes);

        act.Should().Throw<IndexOutOfRangeException>();
    }
}