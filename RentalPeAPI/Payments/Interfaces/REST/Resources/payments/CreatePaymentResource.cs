using System.ComponentModel.DataAnnotations;
using RentalPeAPI.Payments.Domain.Model.Enums;

namespace RentalPeAPI.Payments.Interfaces.REST.Resources.payments;

public record CreatePaymentResource(
    [Required] long SpaceId,
    [Required] Guid PayerUserId,
    [Required] Guid PayeeUserId,
    [Required] int Installment,
    [Range(0, double.MaxValue)] decimal Amount,
    [Required] Currency Currency,

    DateTimeOffset? Date);