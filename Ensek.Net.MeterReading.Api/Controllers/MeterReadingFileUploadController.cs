namespace Ensek.Net.MeterReading.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MeterReadingFileUploadController : ControllerBase
{
    private readonly IMeterReadingHandler _meterReadingHandler;
    
    public MeterReadingFileUploadController(IMeterReadingHandler meterReadingHandler)
    {
        Guard.Against.Null(meterReadingHandler, nameof(meterReadingHandler));
        _meterReadingHandler = meterReadingHandler;
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync(IFormFile meterReadingFile)
    {
        if (meterReadingFile == null || meterReadingFile.Length == 0)
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
