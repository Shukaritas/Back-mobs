using System;

namespace RentalPeAPI.Monitoring.Domain.Entities;

/// <summary>
/// Entidad de dominio que representa una tarea (WorkItem) vinculada a un espacio (Space).
/// Regla de Negocio: Una tarea puede ser creada por el dueño del espacio o por un remodelador,
/// pero solo el remodelador asignado al espacio puede cambiar su estado.
/// </summary>
public class WorkItem
{
    // PK - Autogenerado
    public int Id { get; set; }

    // FK - Referencia al espacio (Space)
    public long SpaceId { get; set; }
    
    // FK - Usuario que subió la tarea (dueño o remodelador)
    public Guid CreatedByUserId { get; set; }

    // Propiedades del WorkItem
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }

    // Estado y auditoría
    public string Status { get; set; } = "PENDING";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }

    public WorkItem() { }

    /// <summary>
    /// Constructor sobrecargado - Compatible con código legado.
    /// Será reemplazado gradualmente por el constructor principal.
    /// </summary>
    [Obsolete("Use the full constructor with Title and PlannedDates instead.")]
    public WorkItem(long spaceId, Guid createdByUserId, string description)
    {
        if (spaceId <= 0)
            throw new ArgumentException("Space ID is required.", nameof(spaceId));
        if (createdByUserId == Guid.Empty)
            throw new ArgumentException("CreatedByUserId is required.", nameof(createdByUserId));

        SpaceId = spaceId;
        CreatedByUserId = createdByUserId;
        Title = "Sin título"; // Valor por defecto
        Description = description;
        PlannedStartDate = DateTime.UtcNow;
        PlannedEndDate = DateTime.UtcNow.AddDays(1);
        Status = "PENDING";
        CreatedAt = DateTime.UtcNow;
        CompletedAt = null;
    }

    /// <summary>
    /// Constructor principal para crear una nueva tarea.
    /// El estado inicial siempre es "PENDING".
    /// </summary>
    public WorkItem(
        long spaceId,
        Guid createdByUserId,
        string title,
        string description,
        DateTime plannedStartDate,
        DateTime plannedEndDate)
    {
        if (spaceId <= 0)
            throw new ArgumentException("Space ID is required.", nameof(spaceId));
        if (createdByUserId == Guid.Empty)
            throw new ArgumentException("CreatedByUserId is required.", nameof(createdByUserId));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));
        if (plannedStartDate >= plannedEndDate)
            throw new ArgumentException("PlannedStartDate must be before PlannedEndDate.", nameof(plannedStartDate));

        SpaceId = spaceId;
        CreatedByUserId = createdByUserId;
        Title = title;
        Description = description;
        PlannedStartDate = plannedStartDate;
        PlannedEndDate = plannedEndDate;
        Status = "PENDING";
        CreatedAt = DateTime.UtcNow;
        CompletedAt = null;
    }

    /// <summary>
    /// Actualiza el estado de la tarea.
    /// Si el nuevo estado es "COMPLETED", asigna automáticamente CompletedAt = DateTime.UtcNow.
    /// </summary>
    public void UpdateStatus(string newStatus)
    {
        if (string.IsNullOrWhiteSpace(newStatus))
            throw new ArgumentException("Status cannot be empty.", nameof(newStatus));

        Status = newStatus.ToUpper();

        if (Status == "COMPLETED" && !CompletedAt.HasValue)
        {
            CompletedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Método heredado para compatibilidad. Usa UpdateStatus internamente.
    /// </summary>
    public void MarkCompleted()
    {
        UpdateStatus("COMPLETED");
    }
}