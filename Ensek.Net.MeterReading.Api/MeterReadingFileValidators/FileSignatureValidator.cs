namespace Ensek.Net.MeterReading.Api.MeterReadingFileValidators;

public class FileSignatureValidator : IValidator
{
    public bool Validate(IFormFile meterReadingFile)
    {
        var acceptableTextFileSignatures = new List<byte[]>
        {
            new byte[] { 0xEF, 0xBB, 0xBF }, //UTF-8 byte order mark, commonly seen in text files
            new byte[] { 0xFF, 0xFE }, //UTF-16LE byte order mark, commonly seen in text files
            new byte[] { 0xFE, 0xFF }, //UTF-16BE byte order mark, commonly seen in text files
            new byte[] { 0xFF, 0xFE, 0x00, 0x00 }, //UTF-32LE byte order mark for text
            new byte[] { 0x00, 0x00, 0xFE, 0xFF }, //UTF-32BE byte order mark for text
            new byte[] { 0x2B, 0x2F, 0x76, 0x38 }, //UTF-7 byte order mark for text
            new byte[] { 0x2B, 0x2F, 0x76, 0x39 }, //UTF-7 byte order mark for text
            new byte[] { 0x2B, 0x2F, 0x76, 0x2B }, //UTF-7 byte order mark for text
            new byte[] { 0x2B, 0x2F, 0x76, 0x2F }, //UTF-7 byte order mark for text
            new byte[] { 0x0E, 0xFE, 0xFF }, //SCSU byte order mark for text
            new byte[] { 0xDD, 073, 066, 073 } //UTF-EBCDIC byte order mark for text
        };
        var filePath = Path.GetTempFileName();
        using var stream = System.IO.File.Create(filePath);
        meterReadingFile.CopyTo(stream);
        bool signatureFound = false;
        foreach (var acceptableTextFileSignature in acceptableTextFileSignatures)
        {
            stream.Position = 0;
            var reader = new BinaryReader(stream);
            var headerBytes = reader.ReadBytes(acceptableTextFileSignature.Length);
            if (acceptableTextFileSignature.SequenceEqual(headerBytes))
            {
                signatureFound = true;
            }
        }
        return signatureFound;
    }
}
