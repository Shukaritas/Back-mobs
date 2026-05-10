using RentalPeAPI.Payments.Domain.Model.Aggregates;
using RentalPeAPI.Payments.Domain.Model.Queries.Payments;
using RentalPeAPI.Payments.Domain.Repositories;
using RentalPeAPI.Payments.Domain.Services.payment;

namespace RentalPeAPI.Payments.Application.Internal.QueryServices;

public class PaymentQueryService(IPaymentRepository paymentRepository) : IPaymentQueryService
{
    public async Task<Payment?> Handle(GetPaymentByIdQuery query)
        => await paymentRepository.FindByIdAsync(query.Id);

    public async Task<IEnumerable<Payment>> Handle(GetPaymentsByUserIdQuery query)
        => await paymentRepository.FindByPayerUserIdAsync(new Guid(query.UserId, 0, 0, new byte[8]));

    public async Task<Payment?> Handle(GetPaymentByReferenceQuery query)
        => await paymentRepository.FindByReferenceAsync(query.Reference);

    public async Task<IEnumerable<Payment>> Handle(GetPaymentsByStatusQuery query)
        => await paymentRepository.FindByStatusAsync(query.Status);

    public async Task<IEnumerable<Payment>> Handle(GetPaymentsByProjectIdQuery query)
        => await paymentRepository.FindBySpaceIdAsync((long)query.ProjectId);

    public async Task<Payment?> Handle(GetPaymentsByProjectAndInstallmentQuery query)
        => await paymentRepository.FindBySpaceAndInstallmentAsync(query.ProjectId, query.Installment);
}