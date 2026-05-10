// Monitoring/Interfaces/REST/Controllers/NotificationsController.cs
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RentalPeAPI.Monitoring.Application.Internal.QueryServices;
using RentalPeAPI.Monitoring.Interfaces.REST.Resources;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Controllers;

[ApiController]
[Route("api/v1/monitoring/[controller]")] // Ruta: /api/v1/monitoring/notifications
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// GET: Obtiene el historial de notificaciones para un proyecto,
    /// con el mismo shape que el "notifications" del db.json.
    /// </summary>
    [HttpGet("project/{projectId:int}")]
    public async Task<IActionResult> ListNotificationsByProject(int projectId)
    {
        var query = new ListNotificationsQuery(projectId);
        var notifications = await _mediator.Send(query);

        // Mapear entidad de dominio -> DTO para el front
        var resources = notifications.Select(n => new NotificationResource(
            n.Id,
            n.UserId,
            n.ProjectId,
            n.Message,
            n.CreatedAt
        ));

        return Ok(resources);
    }
}