// Monitoring/Domain/Repositories/IProjectRepository.cs
using RentalPeAPI.Monitoring.Domain.Entities;
using System.Threading.Tasks;

namespace RentalPeAPI.Monitoring.Domain.Repositories;

public interface IProjectRepository
{
    Task AddAsync(Project project);
    Task<Project?> FindByIdAsync(int id);
}