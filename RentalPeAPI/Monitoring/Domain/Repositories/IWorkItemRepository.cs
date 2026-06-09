// Monitoring/Domain/Repositories/IWorkItemRepository.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using RentalPeAPI.Monitoring.Domain.Entities;

namespace RentalPeAPI.Monitoring.Domain.Repositories;

public interface IWorkItemRepository
{
    // Crear un nuevo work item
    Task AddAsync(WorkItem workItem);

    // Buscar uno por su Id (PK)
    Task<WorkItem?> FindByIdAsync(int id);

    // Listar todas las tareas asociadas a un espacio
    Task<IEnumerable<WorkItem>> ListBySpaceIdAsync(long spaceId);

    // Eliminar una tarea por su ID
    Task DeleteAsync(int id);

    // Sumar todos los precios de las tareas asociadas a un espacio
    Task<decimal> SumPricesBySpaceIdAsync(long spaceId);
}
