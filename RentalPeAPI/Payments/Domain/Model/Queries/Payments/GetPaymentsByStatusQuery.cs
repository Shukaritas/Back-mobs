using RentalPeAPI.Payments.Domain.Model.Enums;

namespace RentalPeAPI.Payments.Domain.Model.Queries.Payments;

public sealed record GetPaymentsByStatusQuery(PaymentStatus Status, int? UserId = null);