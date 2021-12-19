namespace Ensek.Net.MeterReading.Api.Tests.Unit.Converters;

[ExcludeFromCodeCoverage]
public class FormFileToByteArrayConverterTests
{
    [Fact]
    public void FormFileToByteArrayConverter_Implements_IFormFileToByteArrayConverter()
    {
        typeof(IFormFileToByteArrayConverter).IsAssignableFrom(typeof(FormFileToByteArrayConverter));
    }

    [Fact]
    public async Task Convert_WithValidIFormFile_ReturnsPopulatedByteArray()
    {
        var content = "Test content";
        var fileName = "test.csv";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;
        var formFile = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);
        var formFileToByteArrayConverter = new FormFileToByteArrayConverter();
        var expectedResponse = new byte[12]{84, 101, 115, 116, 32, 99, 111, 110, 116, 101, 110, 116}; //'Test content' as byte array

        var actualResponse = await formFileToByteArrayConverter.Convert(formFile);

        actualResponse.Should().BeEquivalentTo(expectedResponse);
    }
    
    [Fact]
    public void Convert_WithNullIFormFile_ThrowsFormFileToByteArrayConverterException()
    {
        var formFileToByteArrayConverter = new FormFileToByteArrayConverter();
        
        Func<Task> act = async () => await formFileToByteArrayConverter.Convert(default(IFormFile)!);

        act.Should().ThrowAsync<FormFileToByteArrayConverterException>();
    }
    
    [Fact]
    public void Convert_WithNoContentIFormFile_ThrowsFormFileToByteArrayConverterException()
    {
        var content = "";
        var fileName = "test.csv";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;
        var formFile = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);
        var formFileToByteArrayConverter = new FormFileToByteArrayConverter();
        
        Func<Task> act = async () => await formFileToByteArrayConverter.Convert(formFile);

        act.Should().ThrowAsync<FormFileToByteArrayConverterException>();
    }
}
