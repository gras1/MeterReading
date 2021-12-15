namespace Ensek.Net.MeterReading.Data;

public class BaseRepository
{
    private readonly DatabaseOptions _options;

    public BaseRepository(IOptions<DatabaseOptions> databaseOptions)
    {
        Guard.Against.Null(databaseOptions, nameof(databaseOptions));
        _options = databaseOptions.Value;
    }

    public string ConnectionString
    {
        get 
        {
            return _options.ConnectionString!;
        }
    }
}
