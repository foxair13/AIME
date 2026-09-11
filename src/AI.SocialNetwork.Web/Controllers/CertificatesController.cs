using AI.SocialNetwork.Application.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AI.SocialNetwork.Web.Controllers;

// C5: генерация и скачивание Word-документов (сертификаты/акты)
[ApiController]
[Route("api/certificates")]
[Authorize]
public class CertificatesController : ControllerBase
{
    private readonly ICertificateService _certificateService;

    public CertificatesController(ICertificateService certificateService)
    {
        _certificateService = certificateService;
    }

    [HttpGet("attestation")]
    public IActionResult GetAttestationCertificate(
        [FromQuery] string userName,
        [FromQuery] string skillName,
        [FromQuery] decimal score,
        [FromQuery] string? certificateNumber)
    {
        var bytes = _certificateService.GenerateAttestationCertificate(
            userName,
            skillName,
            DateTime.UtcNow,
            score,
            string.IsNullOrWhiteSpace(certificateNumber) ? Guid.NewGuid().ToString("N")[..8].ToUpperInvariant() : certificateNumber);

        return File(bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            $"serifikat_{skillName}.docx".Replace(' ', '_'));
    }

    [HttpGet("deal")]
    public IActionResult GetDealCertificate(
        [FromQuery] string buyerName,
        [FromQuery] string sellerName,
        [FromQuery] string dealTitle,
        [FromQuery] decimal amount)
    {
        var bytes = _certificateService.GenerateDealCertificate(buyerName, sellerName, dealTitle, amount, DateTime.UtcNow);
        return File(bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            $"akt_{dealTitle}.docx".Replace(' ', '_'));
    }
}