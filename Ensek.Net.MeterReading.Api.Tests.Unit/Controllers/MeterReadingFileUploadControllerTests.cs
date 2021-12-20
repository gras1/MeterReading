namespace Ensek.Net.MeterReading.Api.Tests.Unit.Controllers;

[ExcludeFromCodeCoverage]
public class MeterReadingFileUploadControllerTests
{
    private readonly MeterReadingFileUploadController _controller;
    private readonly IMeterReadingHandler _meterReadingHandlerFake;
    private readonly IFormFile _formFile;
    private readonly IValidator _fileValidatorFake;
    private readonly IEnumerable<IValidator> _fileValidators;

    public MeterReadingFileUploadControllerTests()
    {
        var content = "Test content";
        var fileName = "test.csv";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;
        _formFile = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);
        _meterReadingHandlerFake = A.Fake<IMeterReadingHandler>();
        _fileValidatorFake = A.Fake<IValidator>();
        A.CallTo(() => _fileValidatorFake.Validate(_formFile)).Returns(true);
        _fileValidators = new List<IValidator>{ _fileValidatorFake };
        _controller = new MeterReadingFileUploadController(_fileValidators, _meterReadingHandlerFake);
    }

    [Fact]
    public void Constructor_WhenIMeterReadingHandlerIsNull_ThrowsArgumentNullException()
    {
        Action act = () => new MeterReadingFileUploadController(_fileValidators, default(IMeterReadingHandler)!);

        act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'meterReadingHandler')");
    }

    [Fact]
    public void Constructor_WhenIEnumerableIValidatorIsNull_ThrowsArgumentNullException()
    {
        Action act = () => new MeterReadingFileUploadController(default(IEnumerable<IValidator>)!, _meterReadingHandlerFake);

        act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'validators')");
    }

    [Fact]
    public async Task PostAsync_WhenIFormFileFailsValidation_ReturnsBadRequestResult()
    {
        A.CallTo(() => _fileValidatorFake.Validate(_formFile)).Returns(false);

        var result = await _controller.PostAsync(_formFile);
        var statusCodeResult = result as StatusCodeResult;

        statusCodeResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task PostAsync_WhenHandleMeterReadingsThrowsFormFileToByteArrayConverterException_ReturnsBadRequestResult()
    {
        A.CallTo(() => _meterReadingHandlerFake.HandleMeterReadings(_formFile)).ThrowsAsync(new FormFileToByteArrayConverterException());
        
        var result = await _controller.PostAsync(_formFile);
        var statusCodeResult = result as StatusCodeResult;

        statusCodeResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task PostAsync_WhenHandleMeterReadingsThrowsFileProcessorException_ReturnsBadRequestResult()
    {
        A.CallTo(() => _meterReadingHandlerFake.HandleMeterReadings(_formFile)).ThrowsAsync(new FileProcessorException());
        
        var result = await _controller.PostAsync(_formFile);
        var statusCodeResult = result as StatusCodeResult;

        statusCodeResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task PostAsync_WhenHandleMeterReadingsThrowsException_Returns500StatusCodeResult()
    {
        A.CallTo(() => _meterReadingHandlerFake.HandleMeterReadings(_formFile)).ThrowsAsync(new Exception());
        
        var result = await _controller.PostAsync(_formFile);
        var statusCodeResult = result as StatusCodeResult;

        statusCodeResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task PostAsync_WithValidIFormFile_ReturnsOkObjectResultWithMeterReadingFileUploadResponse()
    {
        var expectedResponse = new MeterReadingFileUploadResponse();
        A.CallTo(() => _meterReadingHandlerFake.HandleMeterReadings(_formFile)).Returns(expectedResponse);
        
        var result = await _controller.PostAsync(_formFile);
        var objectResult = result as ObjectResult;

        ((int)objectResult!.StatusCode!).Should().Be(200);
        ((MeterReadingFileUploadResponse)objectResult.Value!).Should().BeEquivalentTo(expectedResponse);
    }
}
