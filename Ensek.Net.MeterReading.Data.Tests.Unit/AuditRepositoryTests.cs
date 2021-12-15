namespace Ensek.Net.MeterReading.Data.Tests.Unit;

[ExcludeFromCodeCoverage]
public class AuditRepositoryTests
{
    private IOptions<DatabaseOptions> _iOptionsFake;

    public AuditRepositoryTests()
    {
        _iOptionsFake = A.Fake<IOptions<DatabaseOptions>>();
        _iOptionsFake.Value.ConnectionString = "test";
    }

    [Fact]
    public void CreateNewAuditRecord_WhenFilenameIsNull_ThrowsArgumentNullException()
    {
        var auditRepository = new AuditRepository(_iOptionsFake);

        Action act = () => auditRepository.CreateNewAuditRecord(default(string)!);

        act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'fileName')");
    }
    
    [Fact]
    public void CreateNewAuditRecord_WhenFilenameIsEmpty_ThrowsArgumentException()
    {
        var auditRepository = new AuditRepository(_iOptionsFake);

        Action act = () => auditRepository.CreateNewAuditRecord(string.Empty);

        act.Should().Throw<ArgumentException>().WithMessage("Required input fileName was empty. (Parameter 'fileName')");
    }
}
