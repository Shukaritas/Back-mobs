// Monitoring/Interfaces/REST/Controllers/TasksController.cs
using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RentalPeAPI.Monitoring.Application.Internal.CommandServices;
using RentalPeAPI.Monitoring.Interfaces.REST.Resources;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Controllers;

/// <summary>
/// Controlador para la gestión de tareas (WorkItems) vinculadas a espacios (Spaces).
/// Reemplaza la lógica temporal de conversión de bytes por IDs de tipo Guid.
/// </summary>
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
    /// Crea una nueva orden de trabajo (work item) vinculada a un espacio específico.
    /// El remodelador asignado es identificado por su Guid.
    /// </summary>
    /// <param name="resource">DTO con los datos de la nueva tarea</param>
    /// <returns>ID de la tarea creada</returns>
    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateWorkItemResource resource)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new CreateWorkItemCommand(
            resource.SpaceId,
            resource.AssignedToRemodelerId,
            resource.Description
        );

        var taskId = await _mediator.Send(command);

        return CreatedAtAction(
            nameof(GetTaskById),
            new { id = taskId },
            new { taskId }
        );
    }

    /// <summary>
    /// Obtiene los detalles de una tarea específica.
    /// (Endpoint de prueba/confirmación)
    /// </summary>
    [HttpGet("{id:int}")]
    public IActionResult GetTaskById(int id)
    {
        return Ok($"Tarea {id} creada y lista para ser consultada.");
    }
}