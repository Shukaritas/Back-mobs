using RentalPeAPI.Payments.Domain.Model.Aggregates;
using RentalPeAPI.Payments.Domain.Model.Enums;
using RentalPeAPI.Shared.Domain.Repositories;

namespace RentalPeAPI.Payments.Domain.Repositories;

public interface IPaymentRepository : IBaseRepository<Payment>
{
    Task<IEnumerable<Payment>> FindByPayerUserIdAsync(Guid payerId);

    Task<IEnumerable<Payment>> FindByPayeeUserIdAsync(Guid payeeId);

    Task<Payment?> FindByReferenceAsync(string reference);

    Task<IEnumerable<Payment>> FindByStatusAsync(PaymentStatus status);

    Task<IEnumerable<Payment>> FindByStatusAndPayerUserIdAsync(PaymentStatus status, Guid payerId);
    
    Task<IEnumerable<Payment>> FindBySpaceIdAsync(long spaceId);

    Task<Payment?> FindBySpaceAndInstallmentAsync(long spaceId, int installment);
}