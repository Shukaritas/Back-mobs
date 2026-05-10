namespace RentalPeAPI.Payments.Interfaces.REST.Resources.payments;

public record PaymentResource(
    int Id,
    long SpaceId,
    Guid PayerUserId,
    Guid PayeeUserId,
    int Installment,
    decimal Amount,
    DateTimeOffset Date,
    string Status,
    string CurrencySymbol);