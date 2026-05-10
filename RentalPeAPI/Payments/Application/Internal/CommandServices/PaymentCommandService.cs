using RentalPeAPI.Payments.Domain.Model.Commands.payments;
using RentalPeAPI.Payments.Domain.Repositories;
using RentalPeAPI.Payments.Domain.Services.payment;
using RentalPeAPI.Shared.Domain.Repositories;

namespace RentalPeAPI.Payments.Application.Internal.CommandServices;

public class PaymentCommandService(
    IPaymentRepository paymentRepository,
    IUnitOfWork unitOfWork) : IPaymentCommandService
{
    public async Task<Domain.Model.Aggregates.Payment?> Handle(CreatePaymentCommand command)
    {
        if (!string.IsNullOrWhiteSpace(command.Reference))
        {
            var duplicated = await paymentRepository.FindByReferenceAsync(command.Reference!);
            if (duplicated is not null) return null;
        }

        var payment = new Domain.Model.Aggregates.Payment(
            spaceId:      command.SpaceId,
            payerUserId:  command.PayerUserId,
            payeeUserId:  command.PayeeUserId,
            installment:  command.Installment,
            money:        command.Money,
            method:       command.Method,
            reference:    command.Reference,
            date:         command.Date ?? DateTimeOffset.UtcNow
        );

        try
        {
            await paymentRepository.AddAsync(payment);
            await unitOfWork.CompleteAsync();
            return payment;
        }
        catch
        {
            return null;
        }
    }

    public async Task<Domain.Model.Aggregates.Payment?> Handle(InitiatePaymentCommand command)
    {
        var payment = await paymentRepository.FindByIdAsync(command.PaymentId);
        if (payment is null) return null;

        try
        {
            payment.InitiatePayment();
            paymentRepository.Update(payment);
            await unitOfWork.CompleteAsync();
            return payment;
        }
        catch
        {
            return null;
        }
    }

    public async Task<Domain.Model.Aggregates.Payment?> Handle(ConfirmPaymentCommand command)
    {
        var payment = await paymentRepository.FindByIdAsync(command.PaymentId);
        if (payment is null) return null;

        try
        {
            payment.ConfirmPayment();
            paymentRepository.Update(payment);
            await unitOfWork.CompleteAsync();
            return payment;
        }
        catch
        {
            return null;
        }
    }

    public async Task<Domain.Model.Aggregates.Payment?> Handle(CancelPaymentCommand command)
    {
        var payment = await paymentRepository.FindByIdAsync(command.PaymentId);
        if (payment is null) return null;

        try
        {
            payment.CancelPayment();
            paymentRepository.Update(payment);
            await unitOfWork.CompleteAsync();
            return payment;
        }
        catch
        {
            return null;
        }
    }

    public async Task<Domain.Model.Aggregates.Payment?> Handle(RefundPaymentCommand command)
    {
        var payment = await paymentRepository.FindByIdAsync(command.PaymentId);
        if (payment is null) return null;

        try
        {
            payment.RefundPayment();
            paymentRepository.Update(payment);
            await unitOfWork.CompleteAsync();
            return payment;
        }
        catch
        {
            return null;
        }
    }
}