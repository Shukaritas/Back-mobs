using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using RentalPeAPI.Payments.Domain.Model.Commands.payments;
using RentalPeAPI.Payments.Domain.Model.Enums;
using RentalPeAPI.Payments.Domain.Model.Queries.Payments;
using RentalPeAPI.Payments.Domain.Services.payment;
using RentalPeAPI.Payments.Interfaces.REST.Resources.payments;
using RentalPeAPI.Payments.Interfaces.REST.Transform;
using RentalPeAPI.Resources;
using Swashbuckle.AspNetCore.Annotations;

namespace RentalPeAPI.Payments.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Payments")]
public class PaymentsController(
    IPaymentCommandService commandService,
    IPaymentQueryService queryService,
    IStringLocalizer<SharedResource> localizer) : ControllerBase
{
    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Gets a payment by id", OperationId = "GetPaymentById")]
    [SwaggerResponse(200, "Payment found", typeof(PaymentResource))]
    [SwaggerResponse(404, "Payment not found")]
    public async Task<IActionResult> GetPaymentById(int id)
    {
        var result = await queryService.Handle(new GetPaymentByIdQuery(id));
        if (result is null) return NotFound();
        return Ok(PaymentResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Creates a payment", OperationId = "CreatePayment")]
    [SwaggerResponse(201, "Payment created", typeof(PaymentResource))]
    [SwaggerResponse(400, "Invalid request")]
    [SwaggerResponse(409, "Payment with this reference already exists")]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentResource resource)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var command = CreatePaymentCommandFromResourceAssembler.ToCommandFromResource(resource);

        try
        {
            var result = await commandService.Handle(command);
            if (result is null)
                return Conflict(localizer["PaymentsReferenceDuplicated"].Value);

            return CreatedAtAction(
                nameof(GetPaymentById),
                new { id = result.Id },
                PaymentResourceFromEntityAssembler.ToResourceFromEntity(result));
        }
        catch (Exception ex) when (ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
        {
            return Conflict(localizer["PaymentsReferenceDuplicated"].Value);
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpPost("{id:int}/initiate")]
    [SwaggerOperation(Summary = "Initiates a payment", OperationId = "InitiatePayment")]
    [SwaggerResponse(200, "Payment initiated", typeof(PaymentResource))]
    [SwaggerResponse(400, "Invalid request")]
    public async Task<IActionResult> InitiatePayment(int id)
    {
        var result = await commandService.Handle(new InitiatePaymentCommand(id));
        if (result is null) return BadRequest();
        return Ok(PaymentResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    [HttpPost("{id:int}/confirm")]
    [SwaggerOperation(Summary = "Confirms a payment", OperationId = "ConfirmPayment")]
    [SwaggerResponse(200, "Payment confirmed", typeof(PaymentResource))]
    [SwaggerResponse(400, "Invalid request")]
    public async Task<IActionResult> ConfirmPayment(int id)
    {
        var result = await commandService.Handle(new ConfirmPaymentCommand(id));
        if (result is null) return BadRequest();
        return Ok(PaymentResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    [HttpPost("{id:int}/cancel")]
    [SwaggerOperation(Summary = "Cancels a payment", OperationId = "CancelPayment")]
    [SwaggerResponse(200, "Payment cancelled", typeof(PaymentResource))]
    [SwaggerResponse(400, "Invalid request")]
    public async Task<IActionResult> CancelPayment(int id)
    {
        var result = await commandService.Handle(new CancelPaymentCommand(id));
        if (result is null) return BadRequest();
        return Ok(PaymentResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    [HttpPost("{id:int}/refund")]
    [SwaggerOperation(Summary = "Refunds a payment", OperationId = "RefundPayment")]
    [SwaggerResponse(200, "Payment refunded", typeof(PaymentResource))]
    [SwaggerResponse(400, "Invalid request")]
    public async Task<IActionResult> RefundPayment(int id)
    {
        var result = await commandService.Handle(new RefundPaymentCommand(id));
        if (result is null) return BadRequest();
        return Ok(PaymentResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets payments by query",
        Description = "Filter by userId, status or reference",
        OperationId = "GetPaymentsFromQuery")]
    [SwaggerResponse(200, "Payment(s) found", typeof(IEnumerable<PaymentResource>))]
    [SwaggerResponse(400, "Invalid request")]
    public async Task<IActionResult> GetPaymentsFromQuery(
        [FromQuery] int? userId,
        [FromQuery] PaymentStatus? status,
        [FromQuery] string? reference)
    {
        if (!string.IsNullOrWhiteSpace(reference))
            return await GetPaymentByReference(reference!);

        if (status.HasValue)
            return await GetPaymentsByStatus(status.Value, userId);

        if (userId.HasValue)
            return await GetPaymentsByUserId(userId.Value);

        // fallback, por ejemplo, todos los PENDING
        var all = await queryService.Handle(new GetPaymentsByStatusQuery(PaymentStatus.PENDING, null));
        var resources = all.Select(PaymentResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    private async Task<IActionResult> GetPaymentsByUserId(int userId)
    {
        var result = await queryService.Handle(new GetPaymentsByUserIdQuery(userId));
        var resources = result.Select(PaymentResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    private async Task<IActionResult> GetPaymentByReference(string reference)
    {
        var result = await queryService.Handle(new GetPaymentByReferenceQuery(reference));
        if (result is null) return NotFound();
        return Ok(PaymentResourceFromEntityAssembler.ToResourceFromEntity(result));
    }

    private async Task<IActionResult> GetPaymentsByStatus(PaymentStatus status, int? userId)
    {
        var result = await queryService.Handle(new GetPaymentsByStatusQuery(status, userId));
        var resources = result.Select(PaymentResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }
}