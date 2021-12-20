namespace Ensek.Net.MeterReading.Api.MeterReadingFileValidators;

public class ZeroFileSizeValidator : IValidator
{
    public bool Validate(IFormFile meterReadingFile)
    {
        if (meterReadingFile.Length == 0)
        {
            return false;
        }
        return true;
    }
}
