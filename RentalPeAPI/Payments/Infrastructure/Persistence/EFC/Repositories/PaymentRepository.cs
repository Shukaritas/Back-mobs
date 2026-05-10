using Microsoft.EntityFrameworkCore;
using RentalPeAPI.Payments.Domain.Model.Aggregates;
using RentalPeAPI.Payments.Domain.Model.Enums;
using RentalPeAPI.Payments.Domain.Repositories;
using RentalPeAPI.Shared.Infrastructure.Persistence.EFC.Configuration;
using RentalPeAPI.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace RentalPeAPI.Payments.Infrastructure.Persistence.EFC.Repositories;

public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Payment>> FindByPayerUserIdAsync(Guid payerId)
    {
        return await Context.Set<Payment>()
            .Where(p => p.PayerUserId == payerId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> FindByPayeeUserIdAsync(Guid payeeId)
    {
        return await Context.Set<Payment>()
            .Where(p => p.PayeeUserId == payeeId)
            .ToListAsync();
    }

    public async Task<Payment?> FindByReferenceAsync(string reference)
    {
        return await Context.Set<Payment>()
            .FirstOrDefaultAsync(p => p.Reference == reference);
    }

    public async Task<IEnumerable<Payment>> FindByStatusAsync(PaymentStatus status)
    {
        return await Context.Set<Payment>()
            .Where(p => p.Status == status)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> FindByStatusAndPayerUserIdAsync(PaymentStatus status, Guid payerId)
    {
        return await Context.Set<Payment>()
            .Where(p => p.Status == status && p.PayerUserId == payerId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> FindBySpaceIdAsync(long spaceId)
    {
        return await Context.Set<Payment>()
            .Where(p => p.SpaceId == spaceId)
            .ToListAsync();
    }

    public async Task<Payment?> FindBySpaceAndInstallmentAsync(long spaceId, int installment)
    {
        return await Context.Set<Payment>()
            .FirstOrDefaultAsync(p => p.SpaceId == spaceId && p.Installment == installment);
    }
}