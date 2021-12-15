namespace Ensek.Net.MeterReading.Data.Tests.Unit;

[ExcludeFromCodeCoverage]
public class BaseRepositoryTests
{
    [Fact]
    public void Constructor_WhenIOptionsIsNull_ThrowsArgumentNullException()
    {
        Action act = () => new BaseRepository(default(IOptions<DatabaseOptions>)!);

        act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'databaseOptions')");
    }

    [Fact]
    public void Constructor_WhenIOptionsIsNotNull_SetsConnectionString()
    {
        var options = A.Fake<IOptions<DatabaseOptions>>();
        options.Value.ConnectionString = "test";

        var baseRepo = new BaseRepository(options);

        baseRepo.ConnectionString.Should().Be("test");
    }
}