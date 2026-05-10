using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalPeAPI.User.Domain;

namespace RentalPeAPI.User.Infrastructure.Persistence.EFC.Configuration;

public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        // Nombre de tabla; con UseSnakeCase se convertirá a user_payment_methods
        builder.ToTable("UserPaymentMethods");

        builder.HasKey(pm => pm.Id);

        builder.Property(pm => pm.Type)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(pm => pm.Number)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(pm => pm.Expiry)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(pm => pm.Cvv)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasOne(pm => pm.User)
            .WithMany(u => u.PaymentMethods)
            .HasForeignKey(pm => pm.UserId);
    }
}