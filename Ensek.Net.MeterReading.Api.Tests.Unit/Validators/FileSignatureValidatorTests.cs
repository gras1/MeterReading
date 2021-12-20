namespace Ensek.Net.MeterReading.Api.Tests.Unit.Validators;

public class FileSignatureValidatorTests
{
    [Fact]
    public void Validate_WhenIFormFileHasInvalidFileSignature_ReturnsFalse()
    {
        var invalidFileSignatureByteOrderMark = new byte[] { 0x66, 0x66, 0x66 };
        var content = "Test content";
        var fileName = "test.csv";
        var stream = new MemoryStream();
        stream.Write(invalidFileSignatureByteOrderMark, 0, invalidFileSignatureByteOrderMark.Length); 
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;
        var formFile = new FormFile(stream, 0, stream.Length, "id_from_form", fileName){
            Headers = new HeaderDictionary(),
            ContentType = "text/csv"
        };
        var validator = new FileSignatureValidator();

        var actualResponse = validator.Validate(formFile);

        actualResponse.Should().Be(false);
    }

    [Fact]
    public void Validate_WhenIFormFileHasInvalidFileSignature_ReturnsTrue()
    {
        var utf8ByteOrderMark = new byte[] { 0xEF, 0xBB, 0xBF };
        var content = "Test content";
        var contentBytes = Encoding.ASCII.GetBytes(content);
        byte[] rv = new byte[utf8ByteOrderMark.Length + contentBytes.Length];
        System.Buffer.BlockCopy(utf8ByteOrderMark, 0, rv, 0, utf8ByteOrderMark.Length);
        System.Buffer.BlockCopy(contentBytes, 0, rv, utf8ByteOrderMark.Length, contentBytes.Length);
        var fileName = "test.csv";
        var stream = new MemoryStream();
        stream.Write(rv, 0, rv.Length);
        stream.Position = 0;
        var formFile = new FormFile(stream, 0, stream.Length, "id_from_form", fileName){
            Headers = new HeaderDictionary(),
            ContentType = "text/csv"
        };
        var validator = new FileSignatureValidator();

        var actualResponse = validator.Validate(formFile);

        actualResponse.Should().Be(true);
    }
}
