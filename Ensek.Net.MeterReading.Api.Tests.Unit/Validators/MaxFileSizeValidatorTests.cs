namespace Ensek.Net.MeterReading.Api.Tests.Unit.Validators;

[ExcludeFromCodeCoverage]
public class MaxFileSizeValidatorTests
{
    [Fact]
    public void Validate_WhenIFormFileContentIsTooLarge_ReturnsFalse()
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var content = new string(Enumerable.Repeat(chars, 2_100_000).Select(s => s[random.Next(s.Length)]).ToArray());
        var fileName = "test.csv";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;
        var formFile = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);
        var validator = new MaxFileSizeValidator();

        var actualResponse = validator.Validate(formFile);
        
        actualResponse.Should().Be(false);
    }
    
    [Fact]
    public void Validate_WhenIFormFileContentSizeIsAcceptable_ReturnsTrue()
    {
        var content = "Test content";
        var fileName = "test.csv";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;
        var formFile = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);
        var validator = new MaxFileSizeValidator();

        var actualResponse = validator.Validate(formFile);

        actualResponse.Should().Be(true);
    }
}
