
using RentalPeAPI.Monitoring.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RentalPeAPI.Monitoring.Domain.Repositories;

/// <summary>
/// Repositorio para la entidad Notification.
/// </summary>
public interface INotificationRepository
{
    Task AddAsync(Notification notification);
    Task<Notification?> FindByIdAsync(int id);
    Task<IEnumerable<Notification>> ListBySpaceIdAsync(long spaceId);
}