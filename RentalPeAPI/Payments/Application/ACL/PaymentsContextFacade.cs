using RentalPeAPI.Payments.Domain.Model.Commands.payments;
using RentalPeAPI.Payments.Domain.Model.Enums;
using RentalPeAPI.Payments.Domain.Model.Queries.Payments;
using RentalPeAPI.Payments.Domain.Model.ValueObjects;
using RentalPeAPI.Payments.Domain.Services.payment;
using RentalPeAPI.Payments.Interfaces.ACL;

namespace RentalPeAPI.Payments.Application.ACL;

public class PaymentsContextFacade(
    IPaymentCommandService paymentCommandService,
    IPaymentQueryService paymentQueryService
) : IPaymentsContextFacade
{
    public async Task<int> CreatePayment(
        int userId,
        int projectId,
        int installment,
        decimal amount,
        Currency currency,
        PaymentMethodType methodType,
        string? methodLabel,
        string? methodLast4,
        string? reference,
        DateTimeOffset? date)
    {
        var money = new Money(amount, currency);
        var method = new PaymentMethodSummary(methodType, methodLabel, methodLast4);

        var command = new CreatePaymentCommand(
            SpaceId: projectId,
            PayerUserId: (Guid)(object)userId,
            PayeeUserId: (Guid)(object)userId,
            Installment: installment,
            Money: money,
            Method: method,
            Reference: reference,
            Date: date
        );

        var payment = await paymentCommandService.Handle(command);
        return payment?.Id ?? 0;
    }

    public async Task<int> FetchPaymentIdByReference(string reference)
    {
        if (string.IsNullOrWhiteSpace(reference)) return 0;

        var query = new GetPaymentByReferenceQuery(reference.Trim());
        var payment = await paymentQueryService.Handle(query);
        return payment?.Id ?? 0;
    }
}