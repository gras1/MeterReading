namespace Ensek.Net.MeterReading.Api.MeterReadingFileValidators;

public class FileExtensionValidator : IValidator
{
    public bool Validate(IFormFile meterReadingFile)
    {
        var fileExtension = Path.GetExtension(meterReadingFile.FileName).ToLower();
        if (fileExtension != ".csv")
        {
            return false;
        }
        return true;
    }
}
