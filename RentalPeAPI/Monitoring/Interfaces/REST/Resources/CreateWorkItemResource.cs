// Monitoring/Interfaces/REST/Resources/CreateWorkItemResource.cs
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Resources;

public class CreateWorkItemResource
{
    [Required]
    [JsonPropertyName("projectId")]
    public int ProjectId { get; init; }          // -> "projectId"

    [JsonPropertyName("incidentId")]
    public int? IncidentId { get; init; }        // -> "incidentId" (puede ser null si no hay incidente)

    [Required]
    [JsonPropertyName("assignedToUserId")]
    public int AssignedToUserId { get; init; }   // -> "assignedToUserId"

    [Required]
    [JsonPropertyName("description")]
    public string Description { get; init; } = default!; // -> "description"

    public CreateWorkItemResource() { }

    public CreateWorkItemResource(int projectId, int? incidentId, int assignedToUserId, string description)
    {
        ProjectId = projectId;
        IncidentId = incidentId;
        AssignedToUserId = assignedToUserId;
        Description = description;
    }
}