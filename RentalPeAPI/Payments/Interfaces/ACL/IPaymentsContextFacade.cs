using RentalPeAPI.Payments.Domain.Model.Enums;

namespace RentalPeAPI.Payments.Interfaces.ACL;

public interface IPaymentsContextFacade
{
    /// <summary>
    /// Crea un pago asociado a un proyecto (Monitoring.Project).
    /// Devuelve el Id del payment creado o 0 si falla.
    /// </summary>
    Task<int> CreatePayment(
        int userId,
        int projectId,
        int installment,
        decimal amount,
        Currency currency,
        PaymentMethodType methodType,
        string? methodLabel,
        string? methodLast4,
        string? reference,
        DateTimeOffset? date);

    /// <summary>
    /// Obtiene el Id de un pago por su referencia.
    /// Devuelve 0 si no existe.
    /// </summary>
    Task<int> FetchPaymentIdByReference(string reference);
}