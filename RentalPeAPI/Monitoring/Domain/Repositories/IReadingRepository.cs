// Monitoring/Domain/Repositories/IReadingRepository.cs
using RentalPeAPI.Monitoring.Domain.Entities;

using System.Threading.Tasks;
namespace RentalPeAPI.Monitoring.Domain.Repositories;

public interface IReadingRepository
{
    
    Task AddAsync(Reading reading);
    
    
    Task<Reading?> FindLatestByDeviceIdAsync(int deviceId);
}