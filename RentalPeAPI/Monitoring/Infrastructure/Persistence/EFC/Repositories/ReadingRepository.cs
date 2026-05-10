// Monitoring/Infrastructure/Persistence/EFC/Repositories/ReadingRepository.cs
using System.Linq; 
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RentalPeAPI.Monitoring.Domain.Entities;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Shared.Infrastructure.Persistence.EFC.Configuration; 

namespace RentalPeAPI.Monitoring.Infrastructure.Persistence.EFC.Repositories;

public class ReadingRepository : IReadingRepository
{
    private readonly AppDbContext _context;

    public ReadingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Reading reading) 
    {
        await _context.Readings.AddAsync(reading);
    }

    public async Task<Reading?> FindLatestByDeviceIdAsync(int deviceId)
    {
        return await _context.Readings
            .Where(r => r.IoTDeviceId == deviceId)
            .OrderByDescending(r => r.Timestamp)
            .FirstOrDefaultAsync();
    }
}