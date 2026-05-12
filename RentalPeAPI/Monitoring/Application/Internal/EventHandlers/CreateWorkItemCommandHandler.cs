// Monitoring/Application/Internal/EventHandlers/CreateWorkItemCommandHandler.cs
using MediatR;
using RentalPeAPI.Monitoring.Domain.Entities;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Monitoring.Application.Internal.CommandServices;
using RentalPeAPI.Shared.Domain.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RentalPeAPI.Monitoring.Application.Internal.EventHandlers;

/// <summary>
/// Manejador del comando CreateWorkItemCommand.
/// Crea una nueva tarea (WorkItem) y la persiste en la base de datos.
/// </summary>
public class CreateWorkItemCommandHandler : IRequestHandler<CreateWorkItemCommand, int>
{
    private readonly IWorkItemRepository _workItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateWorkItemCommandHandler(
        IWorkItemRepository workItemRepository,
        IUnitOfWork unitOfWork)
    {
        _workItemRepository = workItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateWorkItemCommand command, CancellationToken cancellationToken)
    {
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