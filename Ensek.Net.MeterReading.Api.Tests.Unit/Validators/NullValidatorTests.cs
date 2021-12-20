namespace Ensek.Net.MeterReading.Api.Tests.Unit.Validators;

[ExcludeFromCodeCoverage]
public class NullValidatorTests
{
    [Fact]
    public void Validate_WhenIFormFileIsNull_ReturnsFalse()
    {
        var validator = new NullValidator();
        var actualResponse = validator.Validate(default(IFormFile)!);
        actualResponse.Should().Be(false);
    }
    
    [Fact]
    public void Validate_WhenIFormFileIsNotNull_ReturnsTrue()
    {
        var validator = new NullValidator();
        var actualResponse = validator.Validate(A.Fake<IFormFile>());
        actualResponse.Should().Be(true);
    }
}
