using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalPeAPI.Payments.Domain.Model.Aggregates;

namespace RentalPeAPI.Payments.Infrastructure.Persistence.EFC.configuration.extensions;

public static class PaymentsModelBuilderExtensions
{
    public static void ApplyPaymentsConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Payment>(ConfigurePayment);
    }

    private static void ConfigurePayment(EntityTypeBuilder<Payment> b)
    {
        b.HasKey(p => p.Id);
        b.Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();

        b.Property(p => p.SpaceId).IsRequired();
        b.Property(p => p.PayerUserId).IsRequired();
        b.Property(p => p.PayeeUserId).IsRequired();
        b.Property(p => p.Installment).IsRequired();
        b.Property(p => p.Status).HasConversion<int>().IsRequired();
        b.Property(p => p.Reference).HasMaxLength(100);
        b.Property(p => p.Date).IsRequired();

        b.HasIndex(p => p.SpaceId);
        b.HasIndex(p => p.PayerUserId);
        b.HasIndex(p => p.PayeeUserId);
        b.HasIndex(p => p.Status);
        b.HasIndex(p => new { p.SpaceId, p.Installment });
        b.HasIndex(p => p.Reference);

        b.OwnsOne(p => p.Money, m =>
        {
            m.WithOwner().HasForeignKey("Id");
            m.Property(x => x.Amount).HasPrecision(18, 2).IsRequired();
            m.Property(x => x.Currency).HasConversion<int>().IsRequired();
        });

        b.OwnsOne(p => p.Method, m =>
        {
            m.WithOwner().HasForeignKey("Id");
            m.Property(x => x.Type).HasConversion<int>().IsRequired();
            m.Property(x => x.Label).HasMaxLength(100);
            m.Property(x => x.Last4).HasMaxLength(4);
        });

        b.Navigation(p => p.Money).IsRequired();
        b.Navigation(p => p.Method).IsRequired();
    }
}