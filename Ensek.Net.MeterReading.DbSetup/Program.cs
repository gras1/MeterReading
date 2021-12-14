namespace Ensek.Net.MeterReading.DbSetup;

class Program
{
    static void Main(string[] args)
    {
        var connectionStringBuilder = new SqliteConnectionStringBuilder();
        connectionStringBuilder.DataSource = "../Ensek.Net.MeterReading.Data/MeterReadings.db";

        DropTables(connectionStringBuilder.ConnectionString);
        
        CreateAccountsTable(connectionStringBuilder.ConnectionString);

        CreateAuditTable(connectionStringBuilder.ConnectionString);
        
        CreateMeterReadingsTable(connectionStringBuilder.ConnectionString);

        PopulateAccountsTable(connectionStringBuilder.ConnectionString);
    }

    private static void PopulateAccountsTable(string connectionString)
    {
        var accounts = new List<Account>();
        string path = @"C:\Projects\Training\MeterReading\Test_Accounts.csv";

        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        using var sr = new StreamReader(fs, Encoding.UTF8);

        while (!sr.EndOfStream)
        {
            var rows = sr.ReadLine()!.Split(',');
            if (rows[0].ToString() != "AccountId")
            {
                var accountId = int.Parse(rows[0].ToString());
                var firstName = rows[1].ToString();
                var lastName = rows[2].ToString();
                
                accounts.Add(new Account(accountId, firstName, lastName));
            }
        }

        using var connection = new SqliteConnection(connectionString);
        
        connection.Open();
        var populateTableCmd = connection.CreateCommand();
        foreach (var account in accounts)
        {
            populateTableCmd.CommandText = $"INSERT INTO Accounts VALUES ({account.AccountId}, '{account.FirstName}', '{account.LastName}')";
            populateTableCmd.ExecuteNonQuery();
        }
    }

    private static void CreateMeterReadingsTable(string connectionString)
    {
        using var connection = new SqliteConnection(connectionString);
        
        connection.Open();
        var createTableCmd = connection.CreateCommand();
        createTableCmd.CommandText = "CREATE TABLE MeterReadings(MeterReadingId INTEGER PRIMARY KEY AUTOINCREMENT, AccountId INTEGER NOT NULL, AuditId INTEGER NOT NULL, DateTime INTEGER NOT NULL, Value VARCHAR(5) NOT NULL, FOREIGN KEY(AccountId) REFERENCES Accounts(AccountId), FOREIGN KEY(AuditId) REFERENCES Audit(AuditId))";
        createTableCmd.ExecuteNonQuery();
    }

    private static void CreateAuditTable(string connectionString)
    {
        using var connection = new SqliteConnection(connectionString);
        
        connection.Open();
        var createTableCmd = connection.CreateCommand();
        createTableCmd.CommandText = "CREATE TABLE Audit(AuditId INTEGER PRIMARY KEY AUTOINCREMENT, FileName VARCHAR(50) NOT NULL, UploadedDateTimeStamp INTEGER NOT NULL, NumberOfSuccessfullyImportedRecords INTEGER NOT NULL, NumberOfFailedRecords INTEGER NOT NULL, FailedRecordDetails VARCHAR(1000))";
        createTableCmd.ExecuteNonQuery();
    }

    private static void CreateAccountsTable(string connectionString)
    {
        using var connection = new SqliteConnection(connectionString);
        
        connection.Open();
        var createTableCmd = connection.CreateCommand();
        createTableCmd.CommandText = "CREATE TABLE Accounts(AccountId INTEGER PRIMARY KEY ASC, FirstName VARCHAR(20) NOT NULL, LastName VARCHAR(20) NOT NULL)";
        createTableCmd.ExecuteNonQuery();
    }

    private static void DropTables(string connectionString)
    {
        using var connection = new SqliteConnection(connectionString);
        
        connection.Open();
        var delTableCmd = connection.CreateCommand();

        delTableCmd.CommandText = "DROP TABLE IF EXISTS MeterReadings";
        delTableCmd.ExecuteNonQuery();
        delTableCmd.CommandText = "DROP TABLE IF EXISTS Audit";
        delTableCmd.ExecuteNonQuery();
        delTableCmd.CommandText = "DROP TABLE IF EXISTS Accounts";
        delTableCmd.ExecuteNonQuery();
    }
}
