using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalPeAPI.Property.Domain.Aggregates.Entities;

namespace RentalPeAPI.Property.Infrastructure.Persistence.EFC.Configuration;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("services");

        // Clave primaria
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasColumnName("id");

        // Nombre del servicio
        builder.Property(s => s.Name)
            .IsRequired()
            .HasColumnName("name");

        // Clave foránea hacia Space
        builder.Property(s => s.SpaceId)
            .HasColumnName("space_id");

        // Relación con Space
        builder.HasOne(s => s.Space)
            .WithMany(sp => sp.Services)
            .HasForeignKey(s => s.SpaceId)
            .HasConstraintName("fk_spaces_services_space_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}