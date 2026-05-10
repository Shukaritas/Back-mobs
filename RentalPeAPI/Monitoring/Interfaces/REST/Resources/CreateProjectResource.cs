// Monitoring/Interfaces/REST/Resources/CreateProjectResource.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Resources;

// Usamos propiedades con init para que funcione bien con el binder de ASP.NET
public record CreateProjectResource
{
    [Required]
    [JsonPropertyName("userId")]
    public Guid UserId { get; init; }   // obligatorio

    [Required]
    [JsonPropertyName("name")]
    public string Name { get; init; } = default!;

    [Required]
    [JsonPropertyName("description")]
    public string Description { get; init; } = default!;

    [Required]
    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; init; }   // obligatorio

    // OPCIONALES (como en el dbjson: pueden ser null)
    [JsonPropertyName("propertyId")]
    public int? PropertyId { get; init; }

    [JsonPropertyName("endDate")]
    public DateTime? EndDate { get; init; }

    // Constructor vacío para el binder (en realidad, el record ya lo soporta,
    // pero si quieres dejarlo por claridad, no pasa nada)
    public CreateProjectResource() { }

    // Constructor completo (opcional). OJO: como PropertyId y EndDate son opcionales,
    // mejor no obligarlos en este ctor; si no lo usas, puedes quitarlo.
    public CreateProjectResource(
        Guid userId,
        string name,
        string description,
        DateTime startDate,
        int? propertyId = null,
        DateTime? endDate = null)
    {
        UserId = userId;
        Name = name;
        Description = description;
        StartDate = startDate;
        PropertyId = propertyId;
        EndDate = endDate;
    }
}