namespace Ensek.Net.MeterReading.Data;

public class MeterReadingsRepository : IMeterReadingsRepository
{
    private readonly SqliteConnection _dbConnection;

    public MeterReadingsRepository(IDbConnection dbConnection)
    {
        Guard.Against.Null(dbConnection, nameof(dbConnection));
        _dbConnection = (SqliteConnection)dbConnection;
    }

    public void CreateNewMeterReadingRecord(int accountId, DateTime meterReadingDateTime, int meterReading,
        int auditId, bool leaveDbConnectionOpen = false)
    {
        Guard.Against.NegativeOrZero(accountId, nameof(accountId));
        Guard.Against.OutOfRange(meterReading, nameof(meterReading), 0, 99999);
        Guard.Against.NegativeOrZero(auditId, nameof(auditId));

        string meterReadingValue = meterReading.ToString("00000");

        switch (_dbConnection.State)
        {
            case ConnectionState.Closed:
            case ConnectionState.Broken:
                _dbConnection.Open();
                break;
        }

        CheckIfAccountIdExists(accountId);

        CheckIfMeterReadingAlreadyExists(accountId, meterReadingDateTime, meterReadingValue);

        InsertMeterReadingRecord(accountId, auditId, meterReadingDateTime, meterReadingValue);

        if (!leaveDbConnectionOpen)
        {
            _dbConnection.Close();
        }
    }

    private void InsertMeterReadingRecord(int accountId, int auditId, DateTime meterReadingDateTime, string meterReadingValue)
    {
        var stm = $"INSERT INTO [MeterReadings](AccountId, AuditId, DateTime, Value) VALUES ({accountId}, {auditId}, strftime ('%s', '{meterReadingDateTime.ToString("yyyy-MM-dd hh:mm:ss")}'), '{meterReadingValue}')";
        using var insertCmd = new SqliteCommand(stm, _dbConnection);
        insertCmd.ExecuteNonQuery();
        insertCmd.Dispose();
    }

    private void CheckIfAccountIdExists(int accountId)
    {
        var stm = $"SELECT COUNT([AccountId]) FROM [Accounts] WHERE [AccountId] = {accountId};";
        using var accountCountCmd = new SqliteCommand(stm, _dbConnection);
        var reader = accountCountCmd.ExecuteReader();
        var accountIdCount = 0;
        while (reader.Read())
        {
            Int32.TryParse(Convert.ToString(reader[0]), out accountIdCount);
        }
        accountCountCmd.Dispose();
        reader.Close();
        if (accountIdCount < 1)
        {
            throw new MeterReadingRecordNotCreatedException($"AccountId {accountId} does not exist");
        }
    }

    private void CheckIfMeterReadingAlreadyExists(int accountId, DateTime meterReadingDateTime, string meterReadingValue)
    {
        var stm = $"SELECT COUNT([MeterReadingId]) FROM [MeterReadings] WHERE [AccountId] = {accountId} AND [DateTime] = strftime ('%s', '{meterReadingDateTime.ToString("yyyy-MM-dd hh:mm:ss")}') AND [Value] = '{meterReadingValue}';";
        using var meterReadingExistsCmd = new SqliteCommand(stm, _dbConnection);
        var reader = meterReadingExistsCmd.ExecuteReader();
        var meterReadingCount = 0;
        while (reader.Read())
        {
            Int32.TryParse(Convert.ToString(reader[0]), out meterReadingCount);
        }
        meterReadingExistsCmd.Dispose();
        reader.Close();
        if (meterReadingCount > 0)
        {
            throw new MeterReadingRecordNotCreatedException($"Meter reading record already exists for {accountId} {meterReadingDateTime.ToString("yyyy-MM-dd hh:mm:ss")} {meterReadingValue}");
        }
    }
}
