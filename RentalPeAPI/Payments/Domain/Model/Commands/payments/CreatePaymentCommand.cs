using RentalPeAPI.Payments.Domain.Model.ValueObjects;

namespace RentalPeAPI.Payments.Domain.Model.Commands.payments;

public sealed record CreatePaymentCommand(
    long SpaceId,
    Guid PayerUserId,
    Guid PayeeUserId,
    int Installment,
    Money Money,
    PaymentMethodSummary Method,
    string? Reference = null,
    DateTimeOffset? Date = null);