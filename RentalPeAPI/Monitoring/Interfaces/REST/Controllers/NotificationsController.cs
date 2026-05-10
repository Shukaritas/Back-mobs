// Monitoring/Interfaces/REST/Controllers/NotificationsController.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RentalPeAPI.Monitoring.Application.Internal.QueryServices;
using RentalPeAPI.Monitoring.Interfaces.REST.Resources;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Controllers;

/// <summary>
/// Controlador para el acceso a notificaciones de espacios (Spaces).
/// </summary>
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
    /// GET: Obtiene el historial de notificaciones para un espacio específico.
    /// </summary>
    [HttpGet("space/{spaceId:long}")]
    public async Task<IActionResult> ListNotificationsBySpace(long spaceId)
    {
        var query = new ListNotificationsQuery(spaceId);
        var notifications = await _mediator.Send(query);

        // Mapear entidad de dominio -> DTO para el front
        var resources = notifications.Select(n => new NotificationResource(
            n.Id,
            n.UserId,
            n.SpaceId,
            n.Message,
            n.CreatedAt
        ));

        return Ok(resources);
    }
}