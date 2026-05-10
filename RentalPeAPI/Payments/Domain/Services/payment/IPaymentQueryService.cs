using RentalPeAPI.Payments.Domain.Model.Aggregates;
using RentalPeAPI.Payments.Domain.Model.Queries.Payments;

namespace RentalPeAPI.Payments.Domain.Services.payment;

public interface IPaymentQueryService
{
    Task<Payment?> Handle(GetPaymentByIdQuery query);

    Task<IEnumerable<Payment>> Handle(GetPaymentsByUserIdQuery query);

    Task<Payment?> Handle(GetPaymentByReferenceQuery query);

    Task<IEnumerable<Payment>> Handle(GetPaymentsByStatusQuery query);
    
    Task<IEnumerable<Payment>> Handle(GetPaymentsByProjectIdQuery query);

    Task<Payment?> Handle(GetPaymentsByProjectAndInstallmentQuery query);
}