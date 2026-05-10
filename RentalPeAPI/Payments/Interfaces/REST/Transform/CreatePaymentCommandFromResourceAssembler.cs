using RentalPeAPI.Payments.Domain.Model.Commands.payments;
using RentalPeAPI.Payments.Domain.Model.Enums;
using RentalPeAPI.Payments.Domain.Model.ValueObjects;
using RentalPeAPI.Payments.Interfaces.REST.Resources.payments;

namespace RentalPeAPI.Payments.Interfaces.REST.Transform;

public static class CreatePaymentCommandFromResourceAssembler
{
    public static CreatePaymentCommand ToCommandFromResource(CreatePaymentResource resource)
    {
        var money  = new Money(resource.Amount, resource.Currency);
        var method = new PaymentMethodSummary(
            type:  PaymentMethodType.CARD,
            label: null,
            last4: null);

        return new CreatePaymentCommand(
            SpaceId:      resource.SpaceId,
            PayerUserId:  resource.PayerUserId,
            PayeeUserId:  resource.PayeeUserId,
            Installment:  resource.Installment,
            Money:        money,
            Method:       method,
            Reference:    null,
            Date:         resource.Date
        );
    }
}