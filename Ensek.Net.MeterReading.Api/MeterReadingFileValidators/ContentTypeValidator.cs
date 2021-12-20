namespace Ensek.Net.MeterReading.Api.MeterReadingFileValidators;

public class ContentTypeValidator : IValidator
{
    public bool Validate(IFormFile meterReadingFile)
    {
        if (meterReadingFile.ContentType.ToLower() != "text/csv")
        {
            return false;
        }
        return true;
    }
}
