namespace Ensek.Net.MeterReading.Data.Tests.Unit;

[ExcludeFromCodeCoverage]
public class AuditRepositoryTests
{
    private const string SqliteInMemoryConnectionString = "Data Source=InMemorySample1;Mode=Memory;Cache=Shared";
    private const string CreateAuditTableSql = "CREATE TABLE Audit(AuditId INTEGER PRIMARY KEY AUTOINCREMENT, FileName VARCHAR(50) NOT NULL, UploadedDateTimeStamp INTEGER NOT NULL, NumberOfSuccessfullyImportedRecords INTEGER, NumberOfFailedRecords INTEGER, FailedRecordDetails VARCHAR(1000))";

    [Fact]
    public void AuditRepository_Implements_IAuditRepository()
    {
        typeof(IAuditRepository).IsAssignableFrom(typeof(AuditRepository));
    }

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

    [Fact]
    public void CreateNewAuditRecord_WithNonEmptyFilename_ReturnsNonZeroAuditId()
    {
        using var connection = new SqliteConnection(SqliteInMemoryConnectionString);
        
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = CreateAuditTableSql;
        command.ExecuteNonQuery();

        var auditRepository = new AuditRepository(connection);

        var auditId = auditRepository.CreateNewAuditRecord("Hello.txt");

        auditId.Should().BeGreaterThan(0);
    }
    
    [Fact]
    public void UpdateAuditRecord_WithNonExistantAuditId_ThrowsAuditRecordNotFoundException()
    {
        using var connection = new SqliteConnection(SqliteInMemoryConnectionString);
        
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = CreateAuditTableSql;
        command.ExecuteNonQuery();

        var auditRepository = new AuditRepository(connection);

        Action act = () => auditRepository.UpdateAuditRecord(1, 1, 0, string.Empty);

        act.Should().Throw<AuditRecordNotFoundException>().WithMessage("AuditId 1 not found");
    }
    
    [Fact]
    public void UpdateAuditRecord_WithExistingAuditRecord_UpdatesAuditRecordSuccessfully()
    {
        using var connection = new SqliteConnection(SqliteInMemoryConnectionString);
        
        connection.Open();

        using var createAuditTableCommand = connection.CreateCommand();
        createAuditTableCommand.CommandText = CreateAuditTableSql;
        createAuditTableCommand.ExecuteNonQuery();

        var stm = $"INSERT INTO [Audit] ([FileName], [UploadedDateTimeStamp]) VALUES ('Hello.txt', strftime ('%s', 'now'));";

        using var insertCommand = new SqliteCommand(stm, connection);

        insertCommand.ExecuteNonQuery();
        insertCommand.Dispose();

        var auditRepository = new AuditRepository(connection);

        auditRepository.UpdateAuditRecord(1, 1, 0, string.Empty, true);

        stm = $"SELECT [NumberOfSuccessfullyImportedRecords] FROM [Audit] WHERE [AuditId] = 1;";

        using var selectCmd = new SqliteCommand(stm, connection);

        var reader = selectCmd.ExecuteReader();
        var numberOfSuccessfullyImportedRecords = 0;
        while (reader.Read())
        {
            Int32.TryParse(Convert.ToString(reader[0]), out numberOfSuccessfullyImportedRecords);
        }
        selectCmd.Dispose();
        connection.Close();
        
        numberOfSuccessfullyImportedRecords.Should().Be(1);
    }
}
