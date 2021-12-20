namespace Ensek.Net.MeterReading.Api.Controllers;

[ApiController]
public class MeterReadingFileUploadController : ControllerBase
{
    private readonly IEnumerable<IValidator> _validators;
    private readonly IMeterReadingHandler _meterReadingHandler;
    
    public MeterReadingFileUploadController(IEnumerable<IValidator> validators, IMeterReadingHandler meterReadingHandler)
    {
        Guard.Against.Null(validators, nameof(validators));
        Guard.Against.Null(meterReadingHandler, nameof(meterReadingHandler));
        _validators = validators;
        _meterReadingHandler = meterReadingHandler;
    }

    /// <summary>
    /// Imports a csv file containing meter readings.
    /// </summary>
    /// <response code="200">Returns a populated MeterReadingFileUploadResponse</response>
    /// <response code="400">If the request is null or not formed correctly</response>
    /// <response code="500">An unhandled exception occurred</response>
    [HttpPost("meter-reading-uploads")]
    public async Task<IActionResult> PostAsync(IFormFile meterReadingFile)
    {
        bool fileIsValid = true;
        foreach (var validator in _validators)
        {
            fileIsValid &= validator.Validate(meterReadingFile);
            if (!fileIsValid)
            {
                break;
            }
        }
        if (!fileIsValid)
        {
            return new BadRequestResult();
        }

        try
        {
            return new OkObjectResult(await _meterReadingHandler.HandleMeterReadings(meterReadingFile));
        }
        catch (FormFileToByteArrayConverterException)
        {
            return new BadRequestResult();
        }
        catch (FileProcessorException)
        {
            return new BadRequestResult();
        }
        catch (Exception ex)
        {
            //TODO: log unhandled exception
            return new StatusCodeResult(500);
        }
    }
}
