// Monitoring/Infrastructure/Persistence/EFC/Repositories/ProjectRepository.cs
using Microsoft.EntityFrameworkCore;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Monitoring.Domain.Entities;
using RentalPeAPI.Shared.Infrastructure.Persistence.EFC.Configuration; 
using System.Threading.Tasks;

namespace RentalPeAPI.Monitoring.Infrastructure.Persistence.EFC.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _context;

    public ProjectRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Project project)
    {
        
        await _context.Projects.AddAsync(project); 
    }

    public async Task<Project?> FindByIdAsync(int id)
    {
        return await _context.Projects.FindAsync(id);
    }
}