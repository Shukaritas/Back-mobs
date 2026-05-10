namespace RentalPeAPI.Payments.Domain.Model.Enums;

public enum PaymentStatus
{
    PENDING   = 1,
    AUTHORIZED = 2,
    SETTLED   = 3,
    FAILED    = 4,
    CANCELLED = 5,
    REFUNDED  = 6
}