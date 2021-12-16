namespace Ensek.Net.MeterReading.Data.Tests.Integration;

[ExcludeFromCodeCoverage]
public class AuditRepositoryTests
{
    private const string SqliteInMemoryConnectionString = "Data Source=InMemorySample;Mode=Memory;Cache=Shared";
    private const string CreateAuditTableSql = "CREATE TABLE Audit(AuditId INTEGER PRIMARY KEY AUTOINCREMENT, FileName VARCHAR(50) NOT NULL, UploadedDateTimeStamp INTEGER NOT NULL, NumberOfSuccessfullyImportedRecords INTEGER, NumberOfFailedRecords INTEGER, FailedRecordDetails VARCHAR(1000))";

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
}