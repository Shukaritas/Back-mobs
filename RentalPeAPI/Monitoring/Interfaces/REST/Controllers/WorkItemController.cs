// Monitoring/Interfaces/REST/Controllers/WorkItemController.cs
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
/// Controlador para la gestión de tareas (WorkItems) vinculadas a espacios (Spaces).
/// Implementa operaciones CRUD con autenticación JWT y autorización por roles.
/// - Crear: Requiere autenticación (Homeowner o Remodeler)
/// - Obtener: Requiere autenticación
/// - Actualizar estado: Solo Remodelers pueden cambiar el estado
/// </summary>
[ApiController]
[Route("api/v1/monitoring/[controller]")] // -> /api/v1/monitoring/workitems
[Tags("Tasks")]
[Authorize] // Requiere JWT válido en todos los endpoints
public class WorkItemController : ControllerBase
{
    private readonly IMediator _mediator;

    public WorkItemController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Crea una nueva orden de trabajo (work item) vinculada a un espacio específico.
    /// El creador se extrae automáticamente del token JWT (ClaimTypes.NameIdentifier).
    /// </summary>
    /// <param name="resource">DTO con spaceId, title, description, planned dates</param>
    /// <returns>201 Created con el ID de la tarea creada</returns>
    /// <response code="201">Tarea creada exitosamente</response>
    /// <response code="400">Validación fallida o datos inválidos</response>
    /// <response code="401">Token JWT inválido o ausente</response>
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> CreateWorkItem([FromBody] CreateWorkItemResource resource)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Extraer el ID del usuario desde el token JWT
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var createdByUserId))
            return Unauthorized(new { error = "Token JWT inválido o sin NameIdentifier." });

        var command = new CreateWorkItemCommand(
            resource.SpaceId,
            createdByUserId,
            resource.Title,
            resource.Description,
            resource.PlannedStartDate,
            resource.PlannedEndDate
        );

        try
        {
            var taskId = await _mediator.Send(command);

            return CreatedAtAction(
                nameof(GetWorkItemById),
                new { id = taskId },
                new { taskId, message = "Tarea creada exitosamente." }
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = $"Error al crear la tarea: {ex.Message}" });
        }
    }

    /// <summary>
    /// Obtiene los detalles completos de una tarea específica.
    /// Requiere autenticación válida.
    /// </summary>
    /// <param name="id">ID de la tarea a obtener</param>
    /// <returns>200 OK con WorkItemResource si existe</returns>
    /// <response code="200">Tarea encontrada</response>
    /// <response code="401">Token JWT inválido o ausente</response>
    /// <response code="404">Tarea no encontrada</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetWorkItemById(int id)
    {
        try
        {
            var query = new GetWorkItemByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound(new { error = $"WorkItem con ID {id} no encontrado." });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = $"Error al obtener la tarea: {ex.Message}" });
        }
    }

    /// <summary>
    /// Actualiza el estado de una tarea existente.
    /// Solo el remodelador asignado al espacio puede cambiar el estado.
    /// El usuario que realiza la acción se extrae del token JWT.
    /// </summary>
    /// <param name="id">ID de la tarea a actualizar</param>
    /// <param name="resource">DTO con el nuevo estado (sin incluir userId)</param>
    /// <returns>200 OK si se actualizó correctamente</returns>
    /// <response code="200">Estado actualizado exitosamente</response>
    /// <response code="400">Validación fallida o tarea/espacio no encontrados</response>
    /// <response code="401">Token JWT inválido o ausente</response>
    /// <response code="403">Usuario no tiene permisos para cambiar el estado (no es el remodelador asignado)</response>
    /// <response code="404">Tarea no encontrada</response>
    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "Remodeler")] // Solo remodeladores pueden cambiar estado
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateWorkItemStatus(int id, [FromBody] UpdateWorkItemStatusResource resource)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Extraer el ID del usuario desde el token JWT
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var requestingUserId))
            return Unauthorized(new { error = "Token JWT inválido o sin NameIdentifier." });

        var command = new UpdateWorkItemStatusCommand(
            id,
            resource.Status,
            requestingUserId
        );

        try
        {
            var success = await _mediator.Send(command);

            if (success)
                return Ok(new { message = "Estado de la tarea actualizado exitosamente." });

            return BadRequest(new { error = "No fue posible actualizar el estado de la tarea." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = $"Error al actualizar estado: {ex.Message}" });
        }
    }
}

