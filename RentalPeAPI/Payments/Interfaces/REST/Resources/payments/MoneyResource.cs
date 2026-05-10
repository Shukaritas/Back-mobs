using System.ComponentModel.DataAnnotations;
using RentalPeAPI.Payments.Domain.Model.Enums;

namespace RentalPeAPI.Payments.Interfaces.REST.Resources.payments;

public record MoneyResource(
    [Range(0, double.MaxValue)] decimal Amount,
    [Required] Currency Currency);