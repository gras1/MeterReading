namespace Ensek.Net.MeterReading.Api.Converters;

public class FormFileToByteArrayConverter : IFormFileToByteArrayConverter
{
    public async Task<byte[]> Convert(IFormFile fileToConvert)
    {
        if (fileToConvert == null || fileToConvert.Length == 0)
        {
            throw new FormFileToByteArrayConverterException(nameof(fileToConvert) + " cannot be null or empty");
        }

        var filePath = Path.GetTempFileName();
        using var stream = System.IO.File.Create(filePath);
        await fileToConvert.CopyToAsync(stream);
        using var br = new BinaryReader(stream, Encoding.ASCII, true);
        var arrayLength = (int)fileToConvert.Length;
        br.BaseStream.Position = 0;
        byte[] meterReadingFileBytes = br.ReadBytes(arrayLength);
        return meterReadingFileBytes;
    }
}
