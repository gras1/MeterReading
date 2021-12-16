namespace Ensek.Net.MeterReading.Data;

public class AuditRepository : IAuditRepository
{
    private readonly SqliteConnection _dbConnection;

    public AuditRepository(SqliteConnection dbConnection)
    {
        Guard.Against.Null(dbConnection, nameof(dbConnection));
        _dbConnection = dbConnection;
    }

    public int CreateNewAuditRecord(string fileName)
    {
        Guard.Against.NullOrWhiteSpace(fileName, nameof(fileName));
        
        switch (_dbConnection.State)
        {
            case ConnectionState.Closed:
            case ConnectionState.Broken:
                _dbConnection.Open();
                break;
        }

        var stm = $"INSERT INTO [Audit] ([FileName], [UploadedDateTimeStamp]) VALUES ('{fileName}', strftime ('%s', 'now')); SELECT [AuditId] FROM [Audit] ORDER BY [AuditId] DESC LIMIT 1;";

        using var cmd = new SqliteCommand(stm, _dbConnection);

        var reader = cmd.ExecuteReader();
        var id = 0;
        while (reader.Read())
        {
            Int32.TryParse(Convert.ToString(reader[0]), out id);
        }

        _dbConnection.Close();
        
        return id;
    }

    public void UpdateAuditRecord(int auditId, int numberOfSuccessfullyImportedRecords,
        int numberOfFailedRecords, string failedRecordDetails)
    {
        Guard.Against.NegativeOrZero(auditId, nameof(auditId));
        Guard.Against.Negative(numberOfSuccessfullyImportedRecords, nameof(numberOfSuccessfullyImportedRecords));
        Guard.Against.Negative(numberOfFailedRecords, nameof(numberOfFailedRecords));
        Guard.Against.MustNotBeNullOrWhitespaceIfComparitorIsGreaterThanZero(numberOfFailedRecords, failedRecordDetails, nameof(failedRecordDetails));
        
        switch (_dbConnection.State)
        {
            case ConnectionState.Closed:
            case ConnectionState.Broken:
                _dbConnection.Open();
                break;
        }

        var stm = $"SELECT COUNT([AuditId]) FROM [Audit] WHERE [AuditId] = {auditId};";

        using var selectCmd = new SqliteCommand(stm, _dbConnection);

        var reader = selectCmd.ExecuteReader();
        var auditIdCount = 0;
        while (reader.Read())
        {
            Int32.TryParse(Convert.ToString(reader[0]), out auditIdCount);
        }
        selectCmd.Dispose();
        if (auditIdCount < 1)
        {
            throw new AuditRecordNotFoundException($"AuditId {auditId} not found");
        }

        stm = $"UPDATE [Audit] SET [NumberOfSuccessfullyImportedRecords] = {numberOfSuccessfullyImportedRecords}, [NumberOfFailedRecords] = {numberOfFailedRecords}, [FailedRecordDetails] = '{failedRecordDetails}' WHERE [AuditId] = {auditId};";

        using var updateCmd = new SqliteCommand(stm, _dbConnection);

        updateCmd.ExecuteNonQuery();

        _dbConnection.Close();
    }
}