using AI.SocialNetwork.Application.Contracts;
using AI.SocialNetwork.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AI.SocialNetwork.Web.Controllers;

// A7: инвойсы (выставление и оплата)
[ApiController]
[Route("api/invoices")]
[Authorize(Roles = "admin,moderator")]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoicesController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Invoice>>> GetAll()
    {
        return Ok(await _invoiceService.GetAllAsync());
    }

    [HttpPost]
    public async Task<ActionResult<Invoice>> Create(CreateInvoiceRequest request)
    {
        var invoice = await _invoiceService.CreateAsync(request.DealId, request.Number, request.Total, request.PlatformFee, request.TaxRate);
        return invoice == null ? BadRequest() : CreatedAtAction(nameof(GetAll), null, invoice);
    }

    [HttpPost("{invoiceId:long}/pay")]
    public async Task<ActionResult<Invoice>> MarkPaid(long invoiceId)
    {
        var invoice = await _invoiceService.MarkPaidAsync(invoiceId);
        return invoice == null ? NotFound() : Ok(invoice);
    }
}

public record CreateInvoiceRequest(long? DealId, string Number, decimal Total, decimal PlatformFee = 0, decimal TaxRate = 0);