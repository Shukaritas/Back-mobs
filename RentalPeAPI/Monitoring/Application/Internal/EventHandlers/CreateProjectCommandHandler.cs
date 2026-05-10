
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RentalPeAPI.Monitoring.Domain.Entities;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Shared.Domain.Repositories;

namespace RentalPeAPI.Monitoring.Application.Internal.CommandServices;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, int>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProjectCommandHandler(IProjectRepository projectRepository, IUnitOfWork unitOfWork)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateProjectCommand command, CancellationToken cancellationToken)
    {
    
        var project = new Project(
            command.UserId,
            command.Name,
            command.Description,
            command.StartDate,
            command.PropertyId,
            command.EndDate
        );

        await _projectRepository.AddAsync(project);
        await _unitOfWork.CompleteAsync();

        return project.Id;
    }
}
