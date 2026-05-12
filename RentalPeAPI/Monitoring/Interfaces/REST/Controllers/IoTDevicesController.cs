using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalPeAPI.Monitoring.Application.Internal.CommandServices;
using RentalPeAPI.Monitoring.Application.Internal.QueryServices;
using RentalPeAPI.Monitoring.Interfaces.REST.Resources;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Controllers;

/// <summary>
/// Controlador para dispositivos IoT vinculados a espacios (Spaces).
/// </summary>
[ApiController]
[Route("api/v1/monitoring/[controller]")]
[Authorize] // ← CRÍTICO: Requiere autenticación JWT
public class IoTDevicesController : ControllerBase
{
    private readonly IMediator _mediator;

    public IoTDevicesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Crea un nuevo dispositivo IoT para un espacio específico.
    /// Solo accesible para usuarios autenticados (ambos roles).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Homeowner,Remodeler")]
    public async Task<IActionResult> CreateDevice([FromBody] CreateIoTDeviceResource resource)
    {
        // Validar que el usuario autenticado tenga un NameIdentifier válido
        var userIdClaim = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized(new { error = "Token JWT inválido o sin NameIdentifier." });

        var command = new CreateIoTDeviceCommand(
            resource.SpaceId,
            resource.Type,
            resource.Name,
            resource.SerialNumber
        );

        var device = await _mediator.Send(command);

        var deviceResource = new IoTDeviceResource(
            device.Id,
            device.SpaceId,
            device.Type,
            device.Status,
            device.InstalledAt
        );

        return CreatedAtAction(
            nameof(ListDevicesBySpace),
            new { spaceId = device.SpaceId },
            deviceResource
        );
    }

    /// <summary>
    /// Lista todos los dispositivos IoT de un espacio específico.
    /// Solo accesible para usuarios autenticados (ambos roles).
    /// </summary>
    [HttpGet("space/{spaceId:long}")]
    [Authorize(Roles = "Homeowner,Remodeler")]
    public async Task<IActionResult> ListDevicesBySpace(long spaceId)
    {
        // Validar que el usuario autenticado tenga un NameIdentifier válido
        var userIdClaim = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized(new { error = "Token JWT inválido o sin NameIdentifier." });

        var query = new ListIoTDevicesBySpaceQuery(spaceId);
        var devices = await _mediator.Send(query);

        var resources = devices.Select(d => new IoTDeviceResource(
            d.Id,
            d.SpaceId,
            d.Type,
            d.Status,
            d.InstalledAt
        ));

        return Ok(resources);
    }
}