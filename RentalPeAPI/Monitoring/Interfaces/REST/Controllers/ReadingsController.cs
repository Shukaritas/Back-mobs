// Monitoring/Interfaces/REST/Controllers/ReadingsController.cs
using System;
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
/// Controlador para la ingesta y consulta de lecturas de telemetría de dispositivos IoT.
/// </summary>
[ApiController]
[Route("api/v1/monitoring/[controller]")]
[Authorize] // ← CRÍTICO: Requiere autenticación JWT
public class ReadingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReadingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// POST: Ingesta de telemetría de dispositivos IoT vinculados a un espacio.
    /// Solo accesible para usuarios autenticados.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Homeowner,Remodeler")]
    public async Task<IActionResult> IngestReading([FromBody] IngestReadingResource resource)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Validar que el usuario autenticado tenga un NameIdentifier válido
        var userIdClaim = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized(new { error = "Token JWT inválido o sin NameIdentifier." });

        var command = new IngestReadingCommand(
            resource.SpaceId,
            resource.IoTDeviceId,
            resource.MetricName,
            resource.Value,
            resource.Unit,
            resource.Timestamp
        );

        var success = await _mediator.Send(command);

        return success ? Accepted() : StatusCode(500, "No se pudo registrar la lectura.");
    }

    /// <summary>
    /// GET: Obtiene la última lectura registrada para un dispositivo IoT.
    /// </summary>
    [HttpGet("device/{iotDeviceId:long}/latest")]
    [Authorize(Roles = "Homeowner,Remodeler")]
    public async Task<IActionResult> GetLatestByDevice(long iotDeviceId)
    {
        // Validar que el usuario autenticado tenga un NameIdentifier válido
        var userIdClaim = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized(new { error = "Token JWT inválido o sin NameIdentifier." });

        var query = new GetLatestReadingByDeviceQuery(iotDeviceId);
        var reading = await _mediator.Send(query);

        if (reading is null) return NotFound();

        return Ok(reading);
    }
}