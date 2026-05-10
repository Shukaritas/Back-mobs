// Monitoring/Interfaces/REST/Controllers/ReadingsController.cs
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RentalPeAPI.Monitoring.Application.Internal.CommandServices;
using RentalPeAPI.Monitoring.Application.Internal.QueryServices;
using RentalPeAPI.Monitoring.Interfaces.REST.Resources;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Controllers;

[ApiController]
[Route("api/v1/monitoring/[controller]")]
public class ReadingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReadingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// POST: Ingesta de telemetría de dispositivos IoT.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> IngestReading([FromBody] IngestReadingResource resource)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new IngestReadingCommand(
            resource.ProjectId,     // 👈 OJO: este es el orden correcto del command
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
    [HttpGet("device/{iotDeviceId:int}/latest")]
    public async Task<IActionResult> GetLatestByDevice(int iotDeviceId)
    {
        var query = new GetLatestReadingByDeviceQuery(iotDeviceId);
        var reading = await _mediator.Send(query);

        if (reading is null) return NotFound();

        return Ok(reading);
    }
}