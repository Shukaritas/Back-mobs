// Monitoring/Application/Internal/CommandServices/CreateWorkItemCommandHandler.cs
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RentalPeAPI.Monitoring.Domain.Entities;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Shared.Domain.Repositories;

namespace RentalPeAPI.Monitoring.Application.Internal.CommandServices;

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