namespace Ensek.Net.MeterReading.Api.MeterReadingFileValidators;

public class NullValidator : IValidator
{
    public bool Validate(IFormFile meterReadingFile)
    {
        if (meterReadingFile == null)
        {
            return false;
        }
        return true;
    }
}
