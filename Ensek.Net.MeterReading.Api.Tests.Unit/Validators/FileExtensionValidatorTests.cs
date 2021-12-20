namespace Ensek.Net.MeterReading.Api.Tests.Unit.Validators;

public class FileExtensionValidatorTests
{
    [Fact]
    public void Validate_WhenIFormFileHasInvalidFileExtension_ReturnsFalse()
    {
        var content = "Test content";
        var fileName = "test.txt";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;
        var formFile = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);
        var validator = new FileExtensionValidator();

        var actualResponse = validator.Validate(formFile);

        actualResponse.Should().Be(false);
    }

    [Fact]
    public void Validate_WhenIFormFileHasValidFileExtension_ReturnsTrue()
    {
        var content = "Test content";
        var fileName = "test.csv";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;
        var formFile = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);
        var validator = new FileExtensionValidator();

        var actualResponse = validator.Validate(formFile);

        actualResponse.Should().Be(true);
    }
}
