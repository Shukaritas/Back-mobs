// Monitoring/Domain/Repositories/IReadingRepository.cs
using RentalPeAPI.Monitoring.Domain.Entities;
using System.Threading.Tasks;

namespace RentalPeAPI.Monitoring.Domain.Repositories;

/// <summary>
/// Repositorio para la entidad Reading.
/// </summary>
public interface IReadingRepository
{
    Task AddAsync(Reading reading);
    Task<Reading?> FindLatestByDeviceIdAsync(long deviceId);
}