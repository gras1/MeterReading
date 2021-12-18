namespace Ensek.Net.MeterReading.Data.Tests.Unit;

[ExcludeFromCodeCoverage]
public class MeterReadingsRepositoryTests
{
    private const string SqliteInMemoryConnectionString = "Data Source=InMemorySample2;Mode=Memory;Cache=Shared";
    private const string CreateAuditTableSql = "CREATE TABLE Audit(AuditId INTEGER PRIMARY KEY AUTOINCREMENT, FileName VARCHAR(50) NOT NULL, UploadedDateTimeStamp INTEGER NOT NULL, NumberOfSuccessfullyImportedRecords INTEGER, NumberOfFailedRecords INTEGER, FailedRecordDetails VARCHAR(1000))";
    private const string CreateAccountTableSql = "CREATE TABLE Accounts(AccountId INTEGER PRIMARY KEY ASC, FirstName VARCHAR(20) NOT NULL, LastName VARCHAR(20) NOT NULL)";
    private const string CreateMeterReadingsTableSql = "CREATE TABLE MeterReadings(MeterReadingId INTEGER PRIMARY KEY AUTOINCREMENT, AccountId INTEGER NOT NULL, AuditId INTEGER NOT NULL, DateTime INTEGER NOT NULL, Value VARCHAR(5) NOT NULL, FOREIGN KEY(AccountId) REFERENCES Accounts(AccountId), FOREIGN KEY(AuditId) REFERENCES Audit(AuditId))";

    [Fact]
    public void MeterReadingsRepository_Implements_IMeterReadingsRepository()
    {
        typeof(IMeterReadingsRepository).IsAssignableFrom(typeof(MeterReadingsRepository));
    }

    [Fact]
    public void Constructor_WhenIDbConnectionIsNull_ThrowsArgumentNullException()
    {
        Action act = () => new MeterReadingsRepository(default(SqliteConnection)!);

        act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'dbConnection')");
    }

    [Fact]
    public void CreateNewMeterReadingRecord_WhenAuditIdIsZero_ThrowsArgumentException()
    {
        var meterReadingsRepository = new MeterReadingsRepository(new SqliteConnection());

        Action act = () => meterReadingsRepository.CreateNewMeterReadingRecord(0, DateTime.Now, 0, 1);

        act.Should().Throw<ArgumentException>().WithMessage("Required input accountId cannot be zero or negative. (Parameter 'accountId')");
    }

    [Fact]
    public void CreateNewMeterReadingRecord_WhenMeterReadingIsNegative_ThrowsArgumentOutOfRangeException()
    {
        var meterReadingsRepository = new MeterReadingsRepository(new SqliteConnection());

        Action act = () => meterReadingsRepository.CreateNewMeterReadingRecord(1, DateTime.Now, -1, 1);

        act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("Input meterReading was out of range (Parameter 'meterReading')");
    }

    [Fact]
    public void CreateNewMeterReadingRecord_WhenMeterReadingIsGeaterThan99999_ThrowsArgumentOutOfRangeException()
    {
        var meterReadingsRepository = new MeterReadingsRepository(new SqliteConnection());

        Action act = () => meterReadingsRepository.CreateNewMeterReadingRecord(1, DateTime.Now, 100000, 1);

        act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("Input meterReading was out of range (Parameter 'meterReading')");
    }

    [Fact]
    public void CreateNewMeterReadingRecord_WhenAccountIdDoesNotExist_ThrowsMeterReadingRecordNotCreatedException()
    {
        using var connection = new SqliteConnection(SqliteInMemoryConnectionString);
        
        connection.Open();

        CreateDatabaseTables(connection);

        var meterReadingsRepository = new MeterReadingsRepository(connection);

        Action act = () => meterReadingsRepository.CreateNewMeterReadingRecord(1, DateTime.Now, 99999, 1);

        act.Should().Throw<MeterReadingRecordNotCreatedException>().WithMessage("AccountId 1 does not exist");
    }

    [Fact]
    public void CreateNewMeterReadingRecord_WhenRecordAlreadyExists_ThrowsMeterReadingRecordNotCreatedException()
    {
        using var connection = new SqliteConnection(SqliteInMemoryConnectionString);
        
        connection.Open();

        CreateDatabaseTables(connection);

        var meterReadingDateTime = new DateTime(2021, 12, 17, 5, 55, 12);

        var stm = "INSERT INTO [Accounts] VALUES (1, 'Test', 'Tester')";
        using var insertAccountCmd = new SqliteCommand(stm, connection);
        insertAccountCmd.ExecuteNonQuery();
        insertAccountCmd.Dispose();
        
        stm = $"INSERT INTO [Audit] (FileName, UploadedDateTimeStamp) VALUES ('test.txt', strftime ('%s', 'now'))";
        using var insertAuditCmd = new SqliteCommand(stm, connection);
        insertAuditCmd.ExecuteNonQuery();
        insertAuditCmd.Dispose();
        
        stm = $"INSERT INTO [MeterReadings](AccountId, AuditId, DateTime, Value) VALUES (1, 1, strftime ('%s', '{meterReadingDateTime.ToString("yyyy-MM-dd hh:mm:ss")}'), '00453')";
        using var insertMeterReadingCmd = new SqliteCommand(stm, connection);
        insertMeterReadingCmd.ExecuteNonQuery();
        insertMeterReadingCmd.Dispose();

        var meterReadingsRepository = new MeterReadingsRepository(connection);

        Action act = () => meterReadingsRepository.CreateNewMeterReadingRecord(1, meterReadingDateTime, 453, 1);

        act.Should().Throw<MeterReadingRecordNotCreatedException>().WithMessage($"Meter reading record already exists for 1 {meterReadingDateTime.ToString("yyyy-MM-dd hh:mm:ss")} 00453");
    }

    [Fact]
    public void CreateNewMeterReadingRecord_WhenRecordDoesNotExist_InsertsRecord()
    {
        using var connection = new SqliteConnection(SqliteInMemoryConnectionString);
        
        connection.Open();

        CreateDatabaseTables(connection);

        var meterReadingDateTime = new DateTime(2021, 12, 17, 5, 55, 12);

        var stm = "INSERT INTO [Accounts] VALUES (1, 'Test', 'Tester')";
        using var insertAccountCmd = new SqliteCommand(stm, connection);
        insertAccountCmd.ExecuteNonQuery();
        insertAccountCmd.Dispose();
        
        stm = $"INSERT INTO [Audit] (FileName, UploadedDateTimeStamp) VALUES ('test.txt', strftime ('%s', 'now'))";
        using var insertAuditCmd = new SqliteCommand(stm, connection);
        insertAuditCmd.ExecuteNonQuery();
        insertAuditCmd.Dispose();

        var meterReadingsRepository = new MeterReadingsRepository(connection);

        meterReadingsRepository.CreateNewMeterReadingRecord(1, meterReadingDateTime, 453, 1, true);

        stm = $"SELECT COUNT([MeterReadingId]) FROM [MeterReadings] WHERE [AccountId] = 1 AND [DateTime] = strftime ('%s', '{meterReadingDateTime.ToString("yyyy-MM-dd hh:mm:ss")}') AND [Value] = '00453';";
        using var meterReadingExistsCmd = new SqliteCommand(stm, connection);
        var reader = meterReadingExistsCmd.ExecuteReader();
        var meterReadingCount = 0;
        while (reader.Read())
        {
            Int32.TryParse(Convert.ToString(reader[0]), out meterReadingCount);
        }
        meterReadingExistsCmd.Dispose();
        reader.Close();
        connection.Close();
        meterReadingCount.Should().Be(1);
    }

    private void CreateDatabaseTables(SqliteConnection connection)
    {
        using var createAuditTableCommand = connection.CreateCommand();
        createAuditTableCommand.CommandText = CreateAuditTableSql;
        createAuditTableCommand.ExecuteNonQuery();
        using var createAccountTableCommand = connection.CreateCommand();
        createAccountTableCommand.CommandText = CreateAccountTableSql;
        createAccountTableCommand.ExecuteNonQuery();
        using var createMeterReadingsTableCommand = connection.CreateCommand();
        createMeterReadingsTableCommand.CommandText = CreateMeterReadingsTableSql;
        createMeterReadingsTableCommand.ExecuteNonQuery();
    }
}
