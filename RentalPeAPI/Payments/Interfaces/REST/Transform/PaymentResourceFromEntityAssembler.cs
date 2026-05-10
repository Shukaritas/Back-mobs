using RentalPeAPI.Payments.Domain.Model.Aggregates;
using RentalPeAPI.Payments.Domain.Model.Enums;
using RentalPeAPI.Payments.Interfaces.REST.Resources.payments;

namespace RentalPeAPI.Payments.Interfaces.REST.Transform;

public static class PaymentResourceFromEntityAssembler
{
    public static PaymentResource ToResourceFromEntity(Payment entity)
    {
        var statusString   = entity.Status.ToString().ToLowerInvariant();
        var currencySymbol = ToCurrencySymbol(entity.Money.Currency);

        return new PaymentResource(
            entity.Id,
            entity.SpaceId,
            entity.PayerUserId,
            entity.PayeeUserId,
            entity.Installment,
            entity.Money.Amount,
            entity.Date,
            statusString,
            currencySymbol);
    }

    private static string ToCurrencySymbol(Currency currency) =>
        currency switch
        {
            Currency.USD => "$",
            Currency.PEN => "S/.",
            _            => currency.ToString()
        };
}