namespace Ensek.Net.MeterReading.Data;

public class MeterReadingsRepository : IMeterReadingsRepository
{
    private readonly SqliteConnection _dbConnection;

    public MeterReadingsRepository(IDbConnection dbConnection)
    {
        Guard.Against.Null(dbConnection, nameof(dbConnection));
        _dbConnection = (SqliteConnection)dbConnection;
    }

    public int CreateNewMeterReadingRecord(int accountId, DateTime meterReadingDateTime, int meterReading,
        int auditId, bool leaveDbConnectionOpen = false)
    {
        Guard.Against.NegativeOrZero(accountId, nameof(accountId));
        Guard.Against.OutOfRange(meterReading, nameof(meterReading), 0, 99999);
        Guard.Against.NegativeOrZero(auditId, nameof(auditId));

        switch (_dbConnection.State)
        {
            case ConnectionState.Closed:
            case ConnectionState.Broken:
                _dbConnection.Open();
                break;
        }

        var stm = $"SELECT COUNT([AccountId]) FROM [Accounts] WHERE [AccountId] = {accountId};";

        using var selectCmd = new SqliteCommand(stm, _dbConnection);

        var reader = selectCmd.ExecuteReader();
        var accountIdCount = 0;
        while (reader.Read())
        {
            Int32.TryParse(Convert.ToString(reader[0]), out accountIdCount);
        }
        selectCmd.Dispose();
        if (accountIdCount < 1)
        {
            throw new MeterReadingRecordNotCreatedException($"AccountId {accountId} does not exist");
        }

        throw new NotImplementedException();
    }

    // public int CreateNewAuditRecord(string fileName)
    // {
    //     Guard.Against.NullOrWhiteSpace(fileName, nameof(fileName));
        
    //     switch (_dbConnection.State)
    //     {
    //         case ConnectionState.Closed:
    //         case ConnectionState.Broken:
    //             _dbConnection.Open();
    //             break;
    //     }

    //     var stm = $"INSERT INTO [Audit] ([FileName], [UploadedDateTimeStamp]) VALUES ('{fileName}', strftime ('%s', 'now')); SELECT [AuditId] FROM [Audit] ORDER BY [AuditId] DESC LIMIT 1;";

    //     using var insertCommand = new SqliteCommand(stm, _dbConnection);

    //     var reader = insertCommand.ExecuteReader();
    //     var id = 0;
    //     while (reader.Read())
    //     {
    //         Int32.TryParse(Convert.ToString(reader[0]), out id);
    //     }
    //     insertCommand.Dispose();
    //     _dbConnection.Close();
        
    //     return id;
    // }

    // public void UpdateAuditRecord(int auditId, int numberOfSuccessfullyImportedRecords,
    //     int numberOfFailedRecords, string failedRecordDetails, bool leaveDbConnectionOpen = false)
    // {
    //     Guard.Against.NegativeOrZero(auditId, nameof(auditId));
    //     Guard.Against.Negative(numberOfSuccessfullyImportedRecords, nameof(numberOfSuccessfullyImportedRecords));
    //     Guard.Against.Negative(numberOfFailedRecords, nameof(numberOfFailedRecords));
    //     Guard.Against.MustNotBeNullOrWhitespaceIfComparitorIsGreaterThanZero(numberOfFailedRecords, failedRecordDetails, nameof(failedRecordDetails));
        
    //     switch (_dbConnection.State)
    //     {
    //         case ConnectionState.Closed:
    //         case ConnectionState.Broken:
    //             _dbConnection.Open();
    //             break;
    //     }

    //     var stm = $"SELECT COUNT([AuditId]) FROM [Audit] WHERE [AuditId] = {auditId};";

    //     using var selectCmd = new SqliteCommand(stm, _dbConnection);

    //     var reader = selectCmd.ExecuteReader();
    //     var auditIdCount = 0;
    //     while (reader.Read())
    //     {
    //         Int32.TryParse(Convert.ToString(reader[0]), out auditIdCount);
    //     }
    //     selectCmd.Dispose();
    //     if (auditIdCount < 1)
    //     {
    //         throw new AuditRecordNotFoundException($"AuditId {auditId} not found");
    //     }

    //     stm = $"UPDATE [Audit] SET [NumberOfSuccessfullyImportedRecords] = {numberOfSuccessfullyImportedRecords}, [NumberOfFailedRecords] = {numberOfFailedRecords}, [FailedRecordDetails] = '{failedRecordDetails}' WHERE [AuditId] = {auditId};";

    //     using var updateCmd = new SqliteCommand(stm, _dbConnection);

    //     updateCmd.ExecuteNonQuery();
    //     updateCmd.Dispose();

    //     if (!leaveDbConnectionOpen)
    //     {
    //         _dbConnection.Close();
    //     }
    // }
}
