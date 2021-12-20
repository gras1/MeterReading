namespace Ensek.Net.MeterReading.Api.Tests.Integration.Controllers;
public static class TestHelper
{
    public static IConfiguration GetTestConfiguration()
        =>  new ConfigurationBuilder()
            .AddJsonFile("testsettings.json")
            .Build();
}

[ExcludeFromCodeCoverage]
public class MeterReadingFileUploadControllerTests : IDisposable
{
    private readonly SqliteConnection _dbConnection;
    private int MaxAuditId { get; set; }
    private int MaxMeterReadingId { get; set; }
    private HttpClient _client;

    public MeterReadingFileUploadControllerTests()
    {
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => {
                builder.ConfigureAppConfiguration(config =>
                {
                    config.AddConfiguration(TestHelper.GetTestConfiguration());
                });
             });
        var clientOptions = new WebApplicationFactoryClientOptions();
        clientOptions.AllowAutoRedirect = true;
        clientOptions.BaseAddress = new Uri("http://localhost:5139");
        _client = application.CreateClient(clientOptions);

        string connectionString = TestHelper.GetTestConfiguration().GetSection("DatabaseOptions").GetSection("ConnectionString").Value;
        _dbConnection = new SqliteConnection(connectionString);

        _dbConnection.Open();

        var stm = $"SELECT Max([AuditId]) FROM [Audit];";
        using var maxAuditIdCmd = new SqliteCommand(stm, _dbConnection);
        var reader = maxAuditIdCmd.ExecuteReader();
        int maxAuditId = 0;
        while (reader.Read())
        {
            Int32.TryParse(Convert.ToString(reader[0]), out maxAuditId);
        }
        maxAuditIdCmd.Dispose();
        reader.Close();
        MaxAuditId = maxAuditId;
        
        stm = $"SELECT Max([MeterReadingId]) FROM [MeterReadings];";
        using var maxMeterReadingIdCmd = new SqliteCommand(stm, _dbConnection);
        reader = maxMeterReadingIdCmd.ExecuteReader();
        int maxMeterReadingId = 0;
        while (reader.Read())
        {
            Int32.TryParse(Convert.ToString(reader[0]), out maxMeterReadingId);
        }
        maxMeterReadingIdCmd.Dispose();
        reader.Close();
        MaxMeterReadingId = maxMeterReadingId;
        
        _dbConnection.Close();
    }

    public void Dispose()
    {
        _dbConnection.Open();
        
        var stm = $"DELETE FROM [MeterReadings] WHERE [MeterReadingId] > {MaxMeterReadingId};";
        using var deleteMeterReadingsCmd = new SqliteCommand(stm, _dbConnection);
        deleteMeterReadingsCmd.ExecuteNonQuery();
        deleteMeterReadingsCmd.Dispose();
        
        stm = $"DELETE FROM [Audit] WHERE [AuditId] > {MaxAuditId};";
        using var deleteAuditsCmd = new SqliteCommand(stm, _dbConnection);
        deleteAuditsCmd.ExecuteNonQuery();
        deleteAuditsCmd.Dispose();
        
        _dbConnection.Close();
    }

    [Fact]
    public async Task MeterReadingUploads()
    {
        var expectedMeterReadingFileUploadResponse = new MeterReadingFileUploadResponse {
            ImportFailureReasons = new List<Shared.Dtos.ImportFileAudit>(),
            NumberOfRecordsFailedToImport = 0,
            NumberOfRecordsSuccessfullyImported = 3
        };
        string path = Directory.GetCurrentDirectory();
        string testFilePath = path + "\\Test_Meter_Readings.csv";
        var content = new MultipartFormDataContent();
        var file_content = new ByteArrayContent(File.ReadAllBytes(testFilePath));
        file_content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
        file_content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            FileName = "Test_Meter_Readings.csv",
            Name = "meterReadingFile",
        };
        content.Add(file_content);

        var httpResponseMessage = await _client.PostAsync("/meter-reading-uploads", content);
        var response = await httpResponseMessage.Content.ReadAsStringAsync();
        var actualMeterReadingFileUploadResponse = JsonSerializer.Deserialize<MeterReadingFileUploadResponse>(response);

        ((int)httpResponseMessage.StatusCode).Should().Be(200);
        actualMeterReadingFileUploadResponse.Should().BeEquivalentTo(expectedMeterReadingFileUploadResponse);
    }
}
