namespace Ensek.Net.MeterReading.Api.Converters;

public class FormFileToByteArrayConverter : IFormFileToByteArrayConverter
{
    public async Task<byte[]> Convert(IFormFile fileToConvert)
    {
        var filePath = Path.GetTempFileName();
        using var stream = System.IO.File.Create(filePath);
        await fileToConvert.CopyToAsync(stream);
        using var br = new BinaryReader(stream, Encoding.ASCII, true);
        var numBytes = new FileInfo(fileToConvert.Name).Length;
        byte[] meterReadingFileBytes = br.ReadBytes((int)numBytes);
        return meterReadingFileBytes;
    }
}
