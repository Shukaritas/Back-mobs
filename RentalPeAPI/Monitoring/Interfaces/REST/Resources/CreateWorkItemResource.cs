// Monitoring/Interfaces/REST/Resources/CreateWorkItemResource.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Resources;

/// <summary>
/// DTO para la creación de una tarea (WorkItem) vinculada a un espacio (Space).
/// Regla de Negocio: Puede ser creada por el dueño del espacio o por un remodelador.
/// El estado inicial se asigna automáticamente como "PENDING".
/// Por seguridad, el CreatedByUserId se extrae del JWT, no del cliente.
/// </summary>
public class CreateWorkItemResource
{
    [Required(ErrorMessage = "SpaceId es requerido")]
    [JsonPropertyName("spaceId")]
    public long SpaceId { get; set; }

    [Required(ErrorMessage = "Title es requerido")]
    [StringLength(200, ErrorMessage = "Title no puede exceder 200 caracteres")]
    [JsonPropertyName("title")]
    public string Title { get; set; } = default!;

    [StringLength(500, ErrorMessage = "Description no puede exceder 500 caracteres")]
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "PlannedStartDate es requerido")]
    [JsonPropertyName("plannedStartDate")]
    public DateTime PlannedStartDate { get; set; }

    [Required(ErrorMessage = "PlannedEndDate es requerido")]
    [JsonPropertyName("plannedEndDate")]
    public DateTime PlannedEndDate { get; set; }

    public CreateWorkItemResource() { }

    public CreateWorkItemResource(
        long spaceId,
        string title,
        string description,
        DateTime plannedStartDate,
        DateTime plannedEndDate)
    {
        SpaceId = spaceId;
        Title = title;
        Description = description;
        PlannedStartDate = plannedStartDate;
        PlannedEndDate = plannedEndDate;
    }
}