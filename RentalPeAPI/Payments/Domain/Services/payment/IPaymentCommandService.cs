using RentalPeAPI.Payments.Domain.Model.Aggregates;
using RentalPeAPI.Payments.Domain.Model.Commands.payments;

namespace RentalPeAPI.Payments.Domain.Services.payment;

public interface IPaymentCommandService
{
    Task<Payment?> Handle(CreatePaymentCommand command);

    Task<Payment?> Handle(InitiatePaymentCommand command);

    Task<Payment?> Handle(ConfirmPaymentCommand command);

    Task<Payment?> Handle(CancelPaymentCommand command);

    Task<Payment?> Handle(RefundPaymentCommand command);
}