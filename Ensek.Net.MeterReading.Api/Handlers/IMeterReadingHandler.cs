namespace Ensek.Net.MeterReading.Api.Handlers;

public interface IMeterReadingHandler
{
    Task<MeterReadingFileUploadResponse> HandleMeterReadings(IFormFile meterReadingFile);
}
