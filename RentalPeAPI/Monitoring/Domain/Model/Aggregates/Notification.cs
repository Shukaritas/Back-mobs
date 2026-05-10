
using System;

namespace RentalPeAPI.Monitoring.Domain.Entities;

/// <summary>
/// Notificación de un evento en una obra (Space).
/// Alineada con el modelo DDD, sin dependencias circulares hacia Property.
/// </summary>
public class Notification
{
    public int Id { get; set; }

    // Identificador del usuario que recibe la notificación (Guid from User BC)
    public Guid UserId { get; set; }

    // Referencia al espacio (Space) - reemplaza el obsoleto ProjectId
    public long SpaceId { get; set; }

    // Puede existir o no incidente asociado: por eso nullable
    public int? IncidentId { get; set; }

    // Destinatario textual (email, nombre, etc.) para lógica interna
    public string Recipient { get; set; } = string.Empty;

    // "message" en el JSON
    public string Message { get; set; } = string.Empty;

    // Tipo de notificación interno
    public string Type { get; set; } = "InApp"; 

    // Estado interno
    public string Status { get; set; } = "unread";

    // Fecha de creación
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Si más adelante tienes envío por mail/push, puedes usarlo
    public DateTime? SentAt { get; set; }

    public Notification() { }

    // Ctor principal alineado con la nueva estructura
    public Notification(Guid userId, long spaceId, string message,
        int? incidentId = null,
        string? recipient = null,
        string type = "InApp",
        string status = "unread")
    {
        UserId = userId;
        SpaceId = spaceId;
        Message = message;

        IncidentId = incidentId;
        Recipient = recipient ?? string.Empty;
        Type = type;
        Status = status;

        CreatedAt = DateTime.UtcNow;
        SentAt = null;
    }
}
