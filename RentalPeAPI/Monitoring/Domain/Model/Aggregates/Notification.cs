
using System;

namespace RentalPeAPI.Monitoring.Domain.Entities;

public class Notification
{
    public int Id { get; set; }

    // "userId" en el JSON
    public int UserId { get; set; }

    // "projectId" en el JSON
    public int ProjectId { get; set; }

    // Puede existir o no incidente asociado: por eso nullable
    public int? IncidentId { get; set; }

    // Destinatario textual (email, nombre, etc.) para lógica interna
    public string Recipient { get; set; } = string.Empty;

    // "message" en el JSON
    public string Message { get; set; } = string.Empty;

    // Tipo de notificación interno (no lo usa el dbjson)
    public string Type { get; set; } = "InApp"; 

    // Estado interno (no está en el dbjson)
    public string Status { get; set; } = "unread";

    // "createdAt" en el JSON
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Si más adelante tienes envío por mail/push, puedes usarlo
    public DateTime? SentAt { get; set; }

    public Notification() { }

    // Ctor principal alineado al dbjson
    public Notification(int userId, int projectId, string message,
        int? incidentId = null,
        string? recipient = null,
        string type = "InApp",
        string status = "unread")
    {
        UserId = userId;
        ProjectId = projectId;
        Message = message;

        IncidentId = incidentId;
        Recipient = recipient ?? string.Empty;
        Type = type;
        Status = status;

        CreatedAt = DateTime.UtcNow;
        SentAt = null;
    }
}
