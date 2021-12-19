namespace Ensek.Net.MeterReading.Api.Processing;

public interface IFileProcessor
{
    ImportFile Process(string meterReadingFileName, byte[] meterReadingFileBytes);
}