// Monitoring/Interfaces/REST/Controllers/WorkItemController.cs
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalPeAPI.Monitoring.Application.Internal.CommandServices;
using RentalPeAPI.Monitoring.Application.Internal.QueryServices;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Monitoring.Interfaces.REST.Resources;
using RentalPeAPI.Property.Domain.Repositories;
using RentalPeAPI.Shared.Domain.Repositories;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Controllers;

/// <summary>
/// Controlador para la gestión de tareas (WorkItems) vinculadas a espacios (Spaces).
/// Implementa operaciones CRUD con autenticación JWT y autorización por roles.
/// 
/// Reglas de Negocio:
/// - POST: Extrae UserId del JWT. Si Homeowner: Status="PENDING" e ignora fechas. Si Remodeler: permite Status y fechas.
/// - PUT: Si es creador: editar contenido (título, descripción, foto). Si es Remodeler+RemodelerId: actualizar estado.
/// - DELETE: Solo el creador exacto puede eliminar.
/// </summary>
[ApiController]
[Route("api/v1/monitoring/tasks")] // -> /api/v1/monitoring/tasks
[Tags("Tasks")]
[Authorize] // Requiere JWT válido en todos los endpoints
public class WorkItemController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IWorkItemRepository _workItemRepository;
    private readonly ISpaceRepository _spaceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public WorkItemController(
        IMediator mediator,
        IWorkItemRepository workItemRepository,
        ISpaceRepository spaceRepository,
        IUnitOfWork unitOfWork)
    {
        _mediator = mediator;
        _workItemRepository = workItemRepository;
        _spaceRepository = spaceRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Crea una nueva orden de trabajo (work item) vinculada a un espacio específico.
    /// 
    /// Reglas de negocio:
    /// - Si el usuario es "Homeowner": Status se fuerza a "PENDING" e ignora fechas planificadas del payload.
    /// - Si el usuario es "Remodeler": permite que el comando guarde el Status y fechas enviadas.
    /// 
    /// El creador se extrae automáticamente del token JWT (ClaimTypes.NameIdentifier).
    /// Devuelve 201 Created con el WorkItemResource completo (incluyendo ID autogenerado y CreatedAt).
    /// </summary>
    /// <param name="resource">DTO con spaceId, title, description, photoUrl, planned dates</param>
    /// <returns>201 Created con WorkItemResource completo</returns>
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

        // Extraer el rol del usuario
        var roleClaim = User.FindFirstValue(ClaimTypes.Role);
        var isHomeowner = string.Equals(roleClaim, "Homeowner", StringComparison.OrdinalIgnoreCase);
        var isRemodeler = string.Equals(roleClaim, "Remodeler", StringComparison.OrdinalIgnoreCase);

        // Aplicar reglas de negocio según rol
        DateTime? finalStartDate = resource.PlannedStartDate;
        DateTime? finalEndDate = resource.PlannedEndDate;

        if (isHomeowner)
        {
            // Homeowner: ignorar fechas planificadas del payload
            finalStartDate = null;
            finalEndDate = null;
        }

        var command = new CreateWorkItemCommand(
            resource.SpaceId,
            createdByUserId,
            resource.Title,
            resource.Description,
            resource.PhotoUrl,
            finalStartDate,
            finalEndDate
        );

        try
        {
            // Ejecutar el comando y obtener el ID
            var taskId = await _mediator.Send(command);

            // Recuperar la entidad completa creada
            var createdWorkItem = await _workItemRepository.FindByIdAsync(taskId);
            if (createdWorkItem == null)
                throw new KeyNotFoundException($"WorkItem recién creado con ID {taskId} no encontrado.");

            // Mapear a WorkItemResource
            var workItemResource = new WorkItemResource(
                createdWorkItem.Id,
                createdWorkItem.SpaceId,
                createdWorkItem.CreatedByUserId,
                createdWorkItem.Title,
                createdWorkItem.Description,
                createdWorkItem.PhotoUrl,
                createdWorkItem.PlannedStartDate,
                createdWorkItem.PlannedEndDate,
                createdWorkItem.Status,
                createdWorkItem.CreatedAt,
                createdWorkItem.CompletedAt
            );

            // Devolver CreatedAtAction con el recurso completo
            return CreatedAtAction(
                nameof(GetWorkItemById),
                new { id = taskId },
                workItemResource
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
    /// Actualiza una tarea existente con autorización granular por rol.
    /// 
    /// Reglas de Autorización ESTRICTAS:
    /// 1. Si el usuario es el CREADOR (tokenUserId == CreatedByUserId):
    ///    - SOLO puede editar: Title, Description, PhotoUrl
    ///    - Status, PlannedStartDate, PlannedEndDate son IGNORADOS
    /// 
    /// 2. Si el usuario es "Remodeler" Y es el RemodelerId del Space:
    ///    - Puede editar: Status, PlannedStartDate, PlannedEndDate
    ///    - Title, Description, PhotoUrl son IGNORADOS
    /// 
    /// 3. Si el usuario NO cumple ninguno: 403 Forbid
    /// 
    /// El usuario que realiza la acción se extrae del token JWT.
    /// </summary>
    /// <param name="id">ID de la tarea a actualizar</param>
    /// <param name="resource">Datos a actualizar (todos los campos opcionales)</param>
    /// <returns>200 OK con mensaje de confirmación</returns>
    /// <response code="200">Tarea actualizada exitosamente</response>
    /// <response code="400">Validación fallida o tarea/espacio no encontrados</response>
    /// <response code="401">Token JWT inválido o ausente</response>
    /// <response code="403">Usuario no tiene permisos para actualizar esta tarea</response>
    /// <response code="404">Tarea no encontrada</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateWorkItem(int id, [FromBody] UpdateWorkItemResource resource)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Extraer el ID del usuario desde el token JWT
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var requestingUserId))
            return Unauthorized(new { error = "Token JWT inválido o sin NameIdentifier." });

        // Extraer el rol del usuario
        var roleClaim = User.FindFirstValue(ClaimTypes.Role);
        var isHomeowner = string.Equals(roleClaim, "Homeowner", StringComparison.OrdinalIgnoreCase);
        var isRemodeler = string.Equals(roleClaim, "Remodeler", StringComparison.OrdinalIgnoreCase);

        try
        {
            // Obtener la tarea
            var workItem = await _workItemRepository.FindByIdAsync(id);
            if (workItem == null)
                return NotFound(new { error = $"WorkItem con ID {id} no encontrado." });

            // Obtener el espacio para validaciones de remodelador
            var space = await _spaceRepository.FindByIdAsync(workItem.SpaceId);
            if (space == null)
                return NotFound(new { error = $"Space con ID {workItem.SpaceId} no encontrado." });

            // Determinar permisos del usuario
            bool isCreator = workItem.CreatedByUserId == requestingUserId;
            bool isRemodellerOfSpace = isRemodeler && space.RemodelerId == requestingUserId;

            // Si NO es creador ni remodelador del espacio → 403 Forbid
            if (!isCreator && !isRemodellerOfSpace)
                return Forbid();

            bool hasEdited = false;

            // ============== LÓGICA POR ROL ==============
            
            // CASO 1: Si es el CREADOR (Homeowner o Remodeler que creó la tarea)
            if (isCreator)
            {
                // Homeowner solo puede editar contenido descriptivo
                if (isHomeowner || !isRemodeler) // Creador que es Homeowner
                {
                    // SOLO permitir cambios a: Title, Description, PhotoUrl
                    if (!string.IsNullOrWhiteSpace(resource.Title) ||
                        !string.IsNullOrWhiteSpace(resource.Description) ||
                        resource.PhotoUrl != null)
                    {
                        string finalTitle = !string.IsNullOrWhiteSpace(resource.Title) ? resource.Title : workItem.Title;
                        string finalDescription = !string.IsNullOrWhiteSpace(resource.Description) ? resource.Description : workItem.Description;
                        string? finalPhotoUrl = resource.PhotoUrl ?? workItem.PhotoUrl;

                        workItem.EditContent(finalTitle, finalDescription, finalPhotoUrl);
                        hasEdited = true;
                    }

                    // RECHAZAR intentos de cambiar Status o fechas (solo ignorar para Homeowner)
                    if (isHomeowner && (!string.IsNullOrWhiteSpace(resource.Status) ||
                        resource.PlannedStartDate.HasValue ||
                        resource.PlannedEndDate.HasValue))
                    {
                        return BadRequest(new 
                        { 
                            error = "Como Homeowner, no tienes permiso para cambiar Status o fechas planificadas. Solo puedes editar título, descripción y foto." 
                        });
                    }
                }
            }

            // CASO 2: Si es Remodeler asignado al espacio (NO es el creador)
            if (isRemodellerOfSpace && !isCreator)
            {
                // SOLO permitir cambios a: Status, PlannedStartDate, PlannedEndDate
                if (!string.IsNullOrWhiteSpace(resource.Status) ||
                    resource.PlannedStartDate.HasValue ||
                    resource.PlannedEndDate.HasValue)
                {
                    string finalStatus = !string.IsNullOrWhiteSpace(resource.Status) ? resource.Status : workItem.Status;
                    DateTime? finalStartDate = resource.PlannedStartDate ?? workItem.PlannedStartDate;
                    DateTime? finalEndDate = resource.PlannedEndDate ?? workItem.PlannedEndDate;

                    workItem.UpdateProgress(finalStatus, finalStartDate, finalEndDate);
                    hasEdited = true;
                }

                // RECHAZAR intentos de cambiar contenido descriptivo
                if (!string.IsNullOrWhiteSpace(resource.Title) ||
                    !string.IsNullOrWhiteSpace(resource.Description) ||
                    resource.PhotoUrl != null)
                {
                    return BadRequest(new 
                    { 
                        error = "Como Remodeler no creador, no tienes permiso para cambiar título, descripción o foto. Solo puedes actualizar el progreso." 
                    });
                }
            }

            // Si se realizaron cambios, persistirlos
            if (hasEdited)
            {
                await _unitOfWork.CompleteAsync();
            }

            return Ok(new 
            { 
                message = "Tarea actualizada exitosamente.", 
                taskId = id,
                changesApplied = hasEdited
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = $"Error al actualizar la tarea: {ex.Message}" });
        }
    }

    /// <summary>
    /// Elimina una tarea existente.
    /// 
    /// Regla de Autorización:
    /// - Solo el usuario que creó exactamente la tarea (CreatedByUserId == tokenUserId) puede eliminarla.
    /// - Si no es el creador: 403 Forbid.
    /// 
    /// El usuario que realiza la acción se extrae del token JWT.
    /// </summary>
    /// <param name="id">ID de la tarea a eliminar</param>
    /// <returns>204 No Content si se eliminó correctamente</returns>
    /// <response code="204">Tarea eliminada exitosamente</response>
    /// <response code="401">Token JWT inválido o ausente</response>
    /// <response code="403">Usuario no es el creador de la tarea</response>
    /// <response code="404">Tarea no encontrada</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteWorkItem(int id)
    {
        // Extraer el ID del usuario desde el token JWT
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var requestingUserId))
            return Unauthorized(new { error = "Token JWT inválido o sin NameIdentifier." });

        try
        {
            // Obtener la tarea
            var workItem = await _workItemRepository.FindByIdAsync(id);
            if (workItem == null)
                return NotFound(new { error = $"WorkItem con ID {id} no encontrado." });

            // Validar que solo el creador exacto pueda eliminar
            if (workItem.CreatedByUserId != requestingUserId)
                return Forbid();

            // Eliminar la tarea
            await _workItemRepository.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = $"Error al eliminar la tarea: {ex.Message}" });
        }
    }

    /// <summary>
    /// Actualiza el estado de una tarea existente (endpoint legado).
    /// Solo el remodelador asignado al espacio puede cambiar el estado.
    /// El usuario que realiza la acción se extrae del token JWT.
    /// 
    /// DEPRECATED: Usar PUT /api/v1/monitoring/tasks/{id:int} en su lugar.
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
    [Obsolete("Use PUT /api/v1/monitoring/tasks/{id:int} instead with full UpdateWorkItemResource.")]
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
        catch (UnauthorizedAccessException)
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

