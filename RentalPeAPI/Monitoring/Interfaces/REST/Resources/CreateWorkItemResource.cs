// Monitoring/Interfaces/REST/Resources/CreateWorkItemResource.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Resources;

/// <summary>
/// DTO para la creación de una tarea (WorkItem) vinculada a un espacio (Space).
/// Las tareas ya no dependen de un incidente; se crean directamente para un espacio.
/// </summary>
public class CreateWorkItemResource
{
    [Required(ErrorMessage = "SpaceId es requerido")]
    [JsonPropertyName("spaceId")]
    public long SpaceId { get; set; }

    [Required(ErrorMessage = "AssignedToRemodelerId es requerido")]
    [JsonPropertyName("assignedToRemodelerId")]
    public Guid AssignedToRemodelerId { get; set; }

    [Required(ErrorMessage = "Description es requerida")]
    [StringLength(500, ErrorMessage = "Description no puede exceder 500 caracteres")]
    [JsonPropertyName("description")]
    public string Description { get; set; } = default!;

    public CreateWorkItemResource() { }

    public CreateWorkItemResource(long spaceId, Guid assignedToRemodelerId, string description)
    {
        SpaceId = spaceId;
        AssignedToRemodelerId = assignedToRemodelerId;
        Description = description;
    }
}