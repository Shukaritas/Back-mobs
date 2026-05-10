using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
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
public class IoTDevicesController : ControllerBase
{
    private readonly IMediator _mediator;

    public IoTDevicesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Crea un nuevo dispositivo IoT para un espacio específico.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateDevice([FromBody] CreateIoTDeviceResource resource)
    {
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
    /// </summary>
    [HttpGet("space/{spaceId:long}")]
    public async Task<IActionResult> ListDevicesBySpace(long spaceId)
    {
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