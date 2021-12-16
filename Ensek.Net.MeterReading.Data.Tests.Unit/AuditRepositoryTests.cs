namespace Ensek.Net.MeterReading.Data.Tests.Unit;

[ExcludeFromCodeCoverage]
public class AuditRepositoryTests
{
    [Fact]
    public void Constructor_WhenIDbConnectionIsNull_ThrowsArgumentNullException()
    {
        Action act = () => new AuditRepository(default(SqliteConnection)!);

        act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'dbConnection')");
    }

    [Fact]
    public void CreateNewAuditRecord_WhenFilenameIsNull_ThrowsArgumentNullException()
    {
        var auditRepository = new AuditRepository(new SqliteConnection());

        Action act = () => auditRepository.CreateNewAuditRecord(default(string)!);

        act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'fileName')");
    }
    
    [Fact]
    public void CreateNewAuditRecord_WhenFilenameIsEmpty_ThrowsArgumentException()
    {
        var auditRepository = new AuditRepository(new SqliteConnection());

        Action act = () => auditRepository.CreateNewAuditRecord(string.Empty);

        act.Should().Throw<ArgumentException>().WithMessage("Required input fileName was empty. (Parameter 'fileName')");
    }
    
    [Fact]
    public void UpdateAuditRecord_WhenAuditIdIsZero_ThrowsArgumentException()
    {
        var auditRepository = new AuditRepository(new SqliteConnection());

        Action act = () => auditRepository.UpdateAuditRecord(0, 5, 0, string.Empty);

        act.Should().Throw<ArgumentException>().WithMessage("Required input auditId cannot be zero or negative. (Parameter 'auditId')");
    }
    
    [Fact]
    public void UpdateAuditRecord_WhenNumberOfSuccessfullyImportedRecordsIsNegative_ThrowsArgumentException()
    {
        var auditRepository = new AuditRepository(new SqliteConnection());

        Action act = () => auditRepository.UpdateAuditRecord(5, -1, 0, string.Empty);

        act.Should().Throw<ArgumentException>().WithMessage("Required input numberOfSuccessfullyImportedRecords cannot be negative. (Parameter 'numberOfSuccessfullyImportedRecords')");
    }
    
    [Fact]
    public void UpdateAuditRecord_WhenNumberOfFailedRecordsIsNegative_ThrowsArgumentException()
    {
        var auditRepository = new AuditRepository(new SqliteConnection());

        Action act = () => auditRepository.UpdateAuditRecord(5, 0, -1, string.Empty);

        act.Should().Throw<ArgumentException>().WithMessage("Required input numberOfFailedRecords cannot be negative. (Parameter 'numberOfFailedRecords')");
    }
    
    [Fact]
    public void UpdateAuditRecord_WhenNumberOfFailedRecordsIsGreaterThanZeroAndFailedRecordDetailsIsEmpty_ThrowsArgumentException()
    {
        var auditRepository = new AuditRepository(new SqliteConnection());

        Action act = () => auditRepository.UpdateAuditRecord(5, 0, 1, string.Empty);

        act.Should().Throw<ArgumentException>().WithMessage("input should not be null or white space (Parameter 'failedRecordDetails')");
    }
}
