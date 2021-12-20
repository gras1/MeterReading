namespace Ensek.Net.MeterReading.Api.Tests.Unit.Validators;

public class ContentTypeValidatorTests
{
    [Fact]
    public void Validate_WhenIFormFileHasInvalidContentType_ReturnsFalse()
    {
        var content = "Test content";
        var fileName = "test.csv";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;
        var formFile = new FormFile(stream, 0, stream.Length, "id_from_form", fileName){
            Headers = new HeaderDictionary(),
            ContentType = "text/plain"
        };
        var validator = new ContentTypeValidator();

        var actualResponse = validator.Validate(formFile);

        actualResponse.Should().Be(false);
    }

    [Fact]
    public void Validate_WhenIFormFileHasValidContentType_ReturnsTrue()
    {
        var content = "Test content";
        var fileName = "test.csv";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;
        var formFile = new FormFile(stream, 0, stream.Length, "id_from_form", fileName){
            Headers = new HeaderDictionary(),
            ContentType = "text/csv"
        };
        var validator = new ContentTypeValidator();

        var actualResponse = validator.Validate(formFile);

        actualResponse.Should().Be(true);
    }
}
