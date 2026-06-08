// Monitoring/Application/Internal/EventHandlers/UpdateWorkItemStatusCommandHandler.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RentalPeAPI.Monitoring.Application.ACL;
using RentalPeAPI.Monitoring.Application.Internal.CommandServices;
using RentalPeAPI.Monitoring.Domain.Entities;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Property.Domain.Repositories;
using RentalPeAPI.Shared.Domain.Repositories;

namespace RentalPeAPI.Monitoring.Application.Internal.EventHandlers;

/// <summary>
/// Manejador del comando UpdateWorkItemStatusCommand.
/// Valida que solo el remodelador asignado al espacio pueda cambiar el estado de la tarea.
/// PASO 5B: Despacha automáticamente una notificación cuando la tarea cambia a "COMPLETED".
/// 
/// REGLA DE CONGELAMIENTO DIFERENCIADO PARA TAREAS:
/// - Si el estado del espacio NO es "Published" ni "Accepted", lanza excepción.
/// - Las tareas SÍ se congelan cuando el espacio está en "Finished" (COMPLETED).
/// </summary>
public class UpdateWorkItemStatusCommandHandler : IRequestHandler<UpdateWorkItemStatusCommand, WorkItem?>
{
    private readonly IWorkItemRepository _workItemRepository;
    private readonly ISpaceRepository _spaceRepository;
    private readonly IPropertyContextFacade _propertyFacade;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public UpdateWorkItemStatusCommandHandler(
        IWorkItemRepository workItemRepository,
        ISpaceRepository spaceRepository,
        IPropertyContextFacade propertyFacade,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _workItemRepository = workItemRepository;
        _spaceRepository = spaceRepository;
        _propertyFacade = propertyFacade;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<WorkItem?> Handle(UpdateWorkItemStatusCommand command, CancellationToken cancellationToken)
    {
        // 1. Obtener la tarea
        var workItem = await _workItemRepository.FindByIdAsync(command.TaskId);
        if (workItem == null)
            throw new KeyNotFoundException($"WorkItem con ID {command.TaskId} no encontrado.");

        // 2. Obtener el espacio
        var space = await _spaceRepository.FindByIdAsync(workItem.SpaceId);
        if (space == null)
            throw new KeyNotFoundException($"Space con ID {workItem.SpaceId} no encontrado.");

        // 3. Validación de permisos: Solo el remodelador asignado puede cambiar el estado
        if (space.RemodelerId == null || space.RemodelerId != command.RequestingUserId)
            throw new UnauthorizedAccessException(
                $"El usuario {command.RequestingUserId} no tiene permisos para cambiar el estado de la tarea. " +
                $"Solo el remodelador asignado al espacio (RemodelerId: {space.RemodelerId}) puede hacerlo.");

        // ===== VALIDACIÓN DE CONGELAMIENTO PARA TAREAS =====
        // Obtener el estado del espacio desde la fachada ACL
        var spaceStatus = await _propertyFacade.GetSpaceStatusAsync(workItem.SpaceId);
        
        // REGLA ESTRICTA: Si el estado NO es "Published" ni "Accepted", no se pueden actualizar tareas
        if (string.IsNullOrEmpty(spaceStatus) || 
            (spaceStatus != "Published" && spaceStatus != "Accepted"))
        {
            throw new InvalidOperationException(
                "No se pueden modificar tareas: El espacio está completado o cancelado.");
        }

        // 4. Verificar si el estado anterior era distinto de COMPLETED y ahora lo es
        bool transitioningToCompleted = !workItem.Status.Equals("COMPLETED", StringComparison.OrdinalIgnoreCase) 
                                       && command.Status.Equals("COMPLETED", StringComparison.OrdinalIgnoreCase);

        // 5. Actualizar el estado de la tarea usando el método de dominio
        // Nota: UpdateProgress mantiene las fechas existentes
        workItem.UpdateProgress(command.Status, workItem.PlannedStartDate, workItem.PlannedEndDate);

        // 6. Persistir cambios
        await _unitOfWork.CompleteAsync();

        // PASO 5B: Si la tarea transicionó exitosamente a COMPLETED, despachar notificación
        if (transitioningToCompleted)
        {
            var notificationCommand = new CreateNotificationCommand(
                space.HomeownerId,
                space.Id,
                "Hito Completado",
                $"Se ha completado con éxito la tarea: '{workItem.Title}'."
            );
            await _mediator.Send(notificationCommand);
        }

        // 7. Retornar la entidad actualizada
        return workItem;
    }
}
