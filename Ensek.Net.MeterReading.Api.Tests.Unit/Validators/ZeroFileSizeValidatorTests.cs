namespace Ensek.Net.MeterReading.Api.Tests.Unit.Validators;

[ExcludeFromCodeCoverage]
public class ZeroFileSizeValidatorTests
{
    [Fact]
    public void Validate_WhenIFormFileHasNoContent_ReturnsFalse()
    {
        var content = "";
        var fileName = "test.csv";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;
        var formFile = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);
        var validator = new ZeroFileSizeValidator();

        var actualResponse = validator.Validate(formFile);
        
        actualResponse.Should().Be(false);
    }
    
    [Fact]
    public void Validate_WhenIFormFileHasContent_ReturnsTrue()
    {
        var content = "Test content";
        var fileName = "test.csv";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;
        var formFile = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);
        var validator = new ZeroFileSizeValidator();

        var actualResponse = validator.Validate(formFile);
        
        actualResponse.Should().Be(true);
    }
}
