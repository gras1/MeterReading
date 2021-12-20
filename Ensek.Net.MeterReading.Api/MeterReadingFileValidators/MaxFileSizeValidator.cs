namespace Ensek.Net.MeterReading.Api.MeterReadingFileValidators;

public class MaxFileSizeValidator : IValidator
{
    public bool Validate(IFormFile meterReadingFile)
    {
        if (meterReadingFile.Length >= 2097152)
        {
            return false;
        }
        return true;
    }
}
