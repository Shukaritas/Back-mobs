
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using RentalPeAPI.Monitoring.Domain.Entities;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Shared.Infrastructure.Persistence.EFC.Configuration; 
using System.Linq;
namespace RentalPeAPI.Monitoring.Infrastructure.Persistence.EFC.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;

    public NotificationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
    }

    public async Task<Notification?> FindByIdAsync(int id)
    {
        return await _context.Notifications.FindAsync(id);
    }
    public async Task<IEnumerable<Notification>> ListByProjectIdAsync(int projectId)
    {
        return await _context.Notifications
            .Where(n => n.ProjectId == projectId) // Filtra por la FK ProjectId
            .ToListAsync();
    }
}