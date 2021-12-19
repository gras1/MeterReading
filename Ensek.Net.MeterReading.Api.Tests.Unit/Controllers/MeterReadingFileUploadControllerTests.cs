namespace Ensek.Net.MeterReading.Api.Tests.Unit.Controllers;

[ExcludeFromCodeCoverage]
public class MeterReadingFileUploadControllerTests
{
    private readonly MeterReadingFileUploadController _controller;
    private readonly IMeterReadingHandler _meterReadingHandlerFake;
    private readonly IFormFile _formFile;

    public MeterReadingFileUploadControllerTests()
    {
        _meterReadingHandlerFake = A.Fake<IMeterReadingHandler>();
        _controller = new MeterReadingFileUploadController(_meterReadingHandlerFake);

        var content = "Test content";
        var fileName = "test.csv";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;
        _formFile = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);
    }

    [Fact]
    public void Constructor_WhenIMeterReadingHandlerIsNull_ThrowsArgumentNullException()
    {
        Action act = () => new MeterReadingFileUploadController(default(IMeterReadingHandler)!);

        act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'meterReadingHandler')");
    }

    [Fact]
    public async Task PostAsync_WhenIFormFileIsNull_ReturnsBadRequestResult()
    {
        var result = await _controller.PostAsync(default(IFormFile)!);
        var badRequestResult = result as BadRequestResult;

        ((int)badRequestResult!.StatusCode).Should().Be(400);
    }

    [Fact]
    public async Task PostAsync_WhenIFormFileHasNoContent_ReturnsBadRequestResult()
    {
        var content = "";
        var fileName = "test.csv";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;
        var formFile = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);

        var result = await _controller.PostAsync(formFile);
        var badRequestResult = result as BadRequestResult;

        ((int)badRequestResult!.StatusCode).Should().Be(400);
    }

    [Fact]
    public async Task PostAsync_WhenHandleMeterReadingsThrowsFormFileToByteArrayConverterException_ReturnsBadRequestResult()
    {
        A.CallTo(() => _meterReadingHandlerFake.HandleMeterReadings(_formFile)).ThrowsAsync(new FormFileToByteArrayConverterException());
        var result = await _controller.PostAsync(_formFile);
        var badRequestResult = result as BadRequestResult;

        ((int)badRequestResult!.StatusCode).Should().Be(400);
    }

    [Fact]
    public async Task PostAsync_WhenHandleMeterReadingsThrowsFileProcessorException_ReturnsBadRequestResult()
    {
        A.CallTo(() => _meterReadingHandlerFake.HandleMeterReadings(_formFile)).ThrowsAsync(new FileProcessorException());
        var result = await _controller.PostAsync(_formFile);
        var badRequestResult = result as BadRequestResult;

        ((int)badRequestResult!.StatusCode).Should().Be(400);
    }

    [Fact]
    public async Task PostAsync_WhenHandleMeterReadingsThrowsException_Returns500StatusCodeResult()
    {
        A.CallTo(() => _meterReadingHandlerFake.HandleMeterReadings(_formFile)).ThrowsAsync(new Exception());
        var result = await _controller.PostAsync(_formFile);
        var badRequestResult = result as StatusCodeResult;

        ((int)badRequestResult!.StatusCode).Should().Be(500);
    }

    [Fact]
    public async Task PostAsync_WithValidIFormFile_ReturnsOkObjectResultWithMeterReadingFileUploadResponse()
    {
        var expectedResponse = new MeterReadingFileUploadResponse();
        A.CallTo(() => _meterReadingHandlerFake.HandleMeterReadings(_formFile)).Returns(expectedResponse);
        var result = await _controller.PostAsync(_formFile);
        var okObjectResult = result as OkObjectResult;

        ((int)okObjectResult!.StatusCode!).Should().Be(200);
        ((MeterReadingFileUploadResponse)okObjectResult.Value!).Should().BeEquivalentTo(expectedResponse);
    }
}
