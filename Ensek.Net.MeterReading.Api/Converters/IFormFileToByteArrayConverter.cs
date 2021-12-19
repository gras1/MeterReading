namespace Ensek.Net.MeterReading.Api.Converters;

public interface IFormFileToByteArrayConverter
{
    Task<byte[]> Convert(IFormFile fileToConvert);
}
