using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RentalPeAPI.Monitoring.Application.Internal.CommandServices;
using RentalPeAPI.Monitoring.Application.Internal.QueryServices;
using RentalPeAPI.Monitoring.Interfaces.REST.Resources;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Controllers;

[ApiController]
[Route("api/v1/monitoring/[controller]")]
public class IoTDevicesController : ControllerBase
{
    private readonly IMediator _mediator;

    public IoTDevicesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateDevice([FromBody] CreateIoTDeviceResource resource)
    {
        var command = new CreateIoTDeviceCommand(
            resource.ProjectId,
            resource.Type,
            resource.Name,
            resource.SerialNumber
        );

        var device = await _mediator.Send(command);

        var deviceResource = new IoTDeviceResource(
            device.Id,
            device.ProjectId,
            device.Type,
            device.Status,
            device.InstalledAt
        );

        return CreatedAtAction(
            nameof(ListDevicesByProject),
            new { projectId = device.ProjectId },
            deviceResource
        );
    }

    [HttpGet("project/{projectId:long}")]
    public async Task<IActionResult> ListDevicesByProject(long projectId)
    {
        var query = new ListIoTDevicesByProjectQuery(projectId);
        var devices = await _mediator.Send(query);

        var resources = devices.Select(d => new IoTDeviceResource(
            d.Id,
            d.ProjectId,
            d.Type,
            d.Status,
            d.InstalledAt
        ));

        return Ok(resources);
    }
}