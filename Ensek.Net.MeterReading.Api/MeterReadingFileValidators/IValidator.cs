namespace Ensek.Net.MeterReading.Api.MeterReadingFileValidators;

public interface IValidator
{
    bool Validate(IFormFile meterReadingFile);
}
