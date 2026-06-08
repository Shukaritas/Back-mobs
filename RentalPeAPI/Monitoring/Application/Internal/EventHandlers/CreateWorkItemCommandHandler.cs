// Monitoring/Application/Internal/EventHandlers/CreateWorkItemCommandHandler.cs
using MediatR;
using RentalPeAPI.Monitoring.Domain.Entities;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Monitoring.Application.Internal.CommandServices;
using RentalPeAPI.Monitoring.Application.ACL;
using RentalPeAPI.Shared.Domain.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RentalPeAPI.Monitoring.Application.Internal.EventHandlers;

/// <summary>
/// Manejador del comando CreateWorkItemCommand.
/// Crea una nueva tarea (WorkItem) y la persiste en la base de datos.
/// 
/// REGLA DE CONGELAMIENTO DIFERENCIADO PARA TAREAS:
/// - Si el estado del espacio NO es "Published" ni "Accepted", lanza excepción.
/// - Las tareas SÍ se congelan cuando el espacio está en "Finished" (COMPLETED).
/// - No se pueden modificar tareas si el espacio está completado o cancelado.
/// </summary>
public class CreateWorkItemCommandHandler : IRequestHandler<CreateWorkItemCommand, int>
{
    private readonly IWorkItemRepository _workItemRepository;
    private readonly IPropertyContextFacade _propertyFacade;
    private readonly IUnitOfWork _unitOfWork;

    public CreateWorkItemCommandHandler(
        IWorkItemRepository workItemRepository,
        IPropertyContextFacade propertyFacade,
        IUnitOfWork unitOfWork)
    {
        _workItemRepository = workItemRepository;
        _propertyFacade = propertyFacade;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateWorkItemCommand command, CancellationToken cancellationToken)
    {
        // ===== VALIDACIÓN DE CONGELAMIENTO PARA TAREAS =====
        // Obtener el estado del espacio desde la fachada ACL
        var spaceStatus = await _propertyFacade.GetSpaceStatusAsync(command.SpaceId);
        
        // REGLA ESTRICTA: Si el estado NO es "Published" ni "Accepted", no se pueden crear tareas
        if (string.IsNullOrEmpty(spaceStatus) || 
            (spaceStatus != "Published" && spaceStatus != "Accepted"))
        {
            throw new InvalidOperationException(
                "No se pueden modificar tareas: El espacio está completado o cancelado.");
        }

        // Crear el WorkItem a partir del comando
        var workItem = new WorkItem(
            command.SpaceId,
            command.CreatedByUserId,
            command.Title,
            command.Description,
            command.PhotoUrl,
            command.PlannedStartDate,
            command.PlannedEndDate
        );

        // Guardar en la BD
        await _workItemRepository.AddAsync(workItem);
        await _unitOfWork.CompleteAsync();

        // Devolver el Id generado
        return workItem.Id;
    }
}