using System.ComponentModel.DataAnnotations;
using RentalPeAPI.Payments.Domain.Model.Enums;

namespace RentalPeAPI.Payments.Interfaces.REST.Resources.payments;


public record PaymentMethodResource(
    [Required] PaymentMethodType Type,
    string? Label,
    [StringLength(4, MinimumLength = 4)] string? Last4);