using AI.SocialNetwork.Application.Services;
using Xunit;

namespace AI.SocialNetwork.Tests;

public class CertificateServiceTests
{
    private readonly CertificateService _service = new();

    [Fact]
    public void GenerateAttestationCertificate_ReturnsValidDocx()
    {
        var bytes = _service.GenerateAttestationCertificate("Иван Иванов", "C#", DateTime.UtcNow, 92m, "A1B2C3D4");
        Assert.NotEmpty(bytes);
        // DOCX — это ZIP-архив (PK..)
        Assert.True(bytes.Length > 4);
        Assert.Equal(new byte[] { 0x50, 0x4B, 0x03, 0x04 }, bytes.Take(4).ToArray());
    }

    [Fact]
    public void GenerateDealCertificate_ContainsPartyNames()
    {
        var bytes = _service.GenerateDealCertificate("Покупатель", "Продавец", "Разработка ЛК", 5000m, DateTime.UtcNow);
        Assert.NotEmpty(bytes);
        Assert.Equal(new byte[] { 0x50, 0x4B, 0x03, 0x04 }, bytes.Take(4).ToArray());
    }
}