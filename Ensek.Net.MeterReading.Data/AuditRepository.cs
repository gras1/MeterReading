namespace Ensek.Net.MeterReading.Data;

public class AuditRepository : BaseRepository, IAuditRepository
{
    public AuditRepository(IOptions<DatabaseOptions> databaseOptions) : base(databaseOptions) { }

    public int CreateNewAuditRecord(string fileName)
    {
        Guard.Against.NullOrWhiteSpace(fileName, nameof(fileName));
        
        using var con = new SqliteConnection(base.ConnectionString);
        
        con.Open();

        var stm = $"INSERT INTO [Audit] VALUES ('{fileName}', strftime ('%s', 'now')); SELECT last_insert_rowid();";

        using var cmd = new SqliteCommand(stm, con);
        
        var rowid = cmd.ExecuteScalar() as string;

        Int32.TryParse(rowid, out int id);

        con.Close();
        
        return id;
    }

    public void UpdateAuditRecord(int auditId, int numberOfSuccessfullyImportedRecords,
        int numberOfFailedRecords, string failedRecordDetails)
    {
        throw new NotImplementedException();
    }
}