// Monitoring/Interfaces/REST/Controllers/TasksController.cs
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RentalPeAPI.Monitoring.Application.Internal.CommandServices;
using RentalPeAPI.Monitoring.Interfaces.REST.Resources;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Controllers;

[ApiController]
[Route("api/v1/monitoring/[controller]")] // -> /api/v1/monitoring/tasks
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public TasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Crea una nueva orden de trabajo (work item), usualmente en respuesta a un incidente.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateWorkItemResource resource)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Convertir int a Guid (generar un GUID basado en el ID del usuario)
        var assignedToGuid = new Guid(resource.AssignedToUserId, 0, 0, new byte[8]);

        var command = new CreateWorkItemCommand(
            resource.ProjectId,
            assignedToGuid,
            resource.Description
        );

        var taskId = await _mediator.Send(command);

        return CreatedAtAction(
            nameof(GetTaskById),
            new { id = taskId },
            new { taskId } // el body de respuesta
        );
    }

    /// <summary>
    /// Solo endpoint de prueba: confirma que la tarea fue creada.
    /// </summary>
    [HttpGet("{id:int}")]
    public IActionResult GetTaskById(int id)
    {
        return Ok($"Tarea {id} creada y lista para ser consultada.");
    }
}