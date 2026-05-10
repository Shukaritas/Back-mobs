using System;

namespace RentalPeAPI.Monitoring.Domain.Entities;

public class Project
{
    // PK generada por la BD
    public int Id { get; set; }

    // Puede no tener propiedad asociada (en tu dbjson hay propertyId: null)
    public int? PropertyId { get; set; }

    // Relación con el usuario (AppUser.Id es Guid en tu BC de User)
    public Guid UserId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // En el dbjson usas "active" para los proyectos creados
    public string Status { get; set; } = "active";

    public DateTime StartDate { get; set; }

    // Puede ser null (proyecto en curso)
    public DateTime? EndDate { get; set; }

    // Fecha de creación del proyecto (en el dbjson: createdAt)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Project() { }

    public Project(
        Guid userId,
        string name,
        string description,
        DateTime startDate,
        int? propertyId = null,
        DateTime? endDate = null,
        string status = "active")
    {
        UserId = userId;
        Name = name;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
        PropertyId = propertyId;
        Status = string.IsNullOrWhiteSpace(status) ? "active" : status;
        CreatedAt = DateTime.UtcNow;
    }
}