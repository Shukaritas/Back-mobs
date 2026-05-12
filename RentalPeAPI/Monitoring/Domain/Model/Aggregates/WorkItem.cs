using System;

namespace RentalPeAPI.Monitoring.Domain.Entities;

/// <summary>
/// Entidad de dominio que representa una tarea (WorkItem) vinculada a un espacio (Space).
/// Regla de Negocio: Una tarea puede ser creada por el dueño del espacio o por un remodelador,
/// pero solo el remodelador asignado al espacio puede cambiar su estado.
/// 
/// Fechas planificadas: pueden ser nulas (generalmente cuando el Homeowner crea sin fechas específicas).
/// PhotoUrl: opcional para documentar visualmente la tarea.
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
    public string? PhotoUrl { get; set; } // Opcional
    public DateTime? PlannedStartDate { get; set; } // Opcional
    public DateTime? PlannedEndDate { get; set; } // Opcional

    // Estado y auditoría
    public string Status { get; set; } = "PENDING";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }


    public WorkItem() { }

    /// <summary>
    /// Constructor principal para crear una nueva tarea según reglas de DDD.
    /// El estado inicial siempre es "PENDING" al crear.
    /// </summary>
    public WorkItem(
        long spaceId,
        Guid createdByUserId,
        string title,
        string description,
        string? photoUrl = null,
        DateTime? plannedStartDate = null,
        DateTime? plannedEndDate = null)
    {
        if (spaceId <= 0)
            throw new ArgumentException("Space ID is required.", nameof(spaceId));
        if (createdByUserId == Guid.Empty)
            throw new ArgumentException("CreatedByUserId is required.", nameof(createdByUserId));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));
        
        // Validar que si se proporciona una fecha, la otra también debe estar presente y válida
        if (plannedStartDate.HasValue && plannedEndDate.HasValue)
        {
            if (plannedStartDate >= plannedEndDate)
                throw new ArgumentException("PlannedStartDate must be before PlannedEndDate.", nameof(plannedStartDate));
        }
        else if (plannedStartDate.HasValue || plannedEndDate.HasValue)
        {
            throw new ArgumentException("Both PlannedStartDate and PlannedEndDate must be provided together.", nameof(plannedStartDate));
        }

        SpaceId = spaceId;
        CreatedByUserId = createdByUserId;
        Title = title;
        Description = description;
        PhotoUrl = photoUrl;
        PlannedStartDate = plannedStartDate;
        PlannedEndDate = plannedEndDate;
        Status = "PENDING";
        CreatedAt = DateTime.UtcNow;
        CompletedAt = null;
    }

    /// <summary>
    /// Constructor compatibilidad para código legado (sin fechas planificadas).
    /// </summary>
    [Obsolete("Use the full constructor instead.")]
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
        PhotoUrl = null;
        PlannedStartDate = null;
        PlannedEndDate = null;
        Status = "PENDING";
        CreatedAt = DateTime.UtcNow;
        CompletedAt = null;
    }

    /// <summary>
    /// Método de dominio para editar contenido de la tarea (textos y foto).
    /// Disponible para quien creó la tarea.
    /// </summary>
    public void EditContent(string title, string description, string? photoUrl)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required.", nameof(description));

        Title = title;
        Description = description;
        PhotoUrl = photoUrl;
    }

    /// <summary>
    /// Método de dominio para actualizar el estado y fechas de la tarea.
    /// Si el estado es "COMPLETED", asigna automáticamente CompletedAt = DateTime.UtcNow.
    /// Solo disponible para el remodelador asignado al espacio.
    /// </summary>
    public void UpdateProgress(string status, DateTime? startDate, DateTime? endDate)
    {
        if (string.IsNullOrWhiteSpace(status))
            throw new ArgumentException("Status cannot be empty.", nameof(status));

        // Validar que si se proporciona una fecha, la otra también debe estar presente y válida
        if (startDate.HasValue && endDate.HasValue)
        {
            if (startDate >= endDate)
                throw new ArgumentException("PlannedStartDate must be before PlannedEndDate.", nameof(startDate));
        }
        else if (startDate.HasValue || endDate.HasValue)
        {
            throw new ArgumentException("Both PlannedStartDate and PlannedEndDate must be provided together or both null.", nameof(startDate));
        }

        Status = status.ToUpper();
        PlannedStartDate = startDate;
        PlannedEndDate = endDate;

        // Si el estado es COMPLETED, asignar automáticamente la fecha de finalización
        if (Status == "COMPLETED" && !CompletedAt.HasValue)
        {
            CompletedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Método heredado para compatibilidad. Usa UpdateProgress internamente.
    /// </summary>
    [Obsolete("Use UpdateProgress instead.")]
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
    /// Método heredado para compatibilidad.
    /// </summary>
    [Obsolete("Use UpdateProgress with 'COMPLETED' status instead.")]
    public void MarkCompleted()
    {
        UpdateStatus("COMPLETED");
    }
}