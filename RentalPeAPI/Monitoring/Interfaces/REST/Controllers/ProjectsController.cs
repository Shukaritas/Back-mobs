// Monitoring/Interfaces/REST/Controllers/ProjectsController.cs
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RentalPeAPI.Monitoring.Application.Internal.CommandServices;
using RentalPeAPI.Monitoring.Domain.Repositories;
using RentalPeAPI.Monitoring.Interfaces.REST.Resources;

namespace RentalPeAPI.Monitoring.Interfaces.REST.Controllers;

[ApiController]
[Route("api/v1/monitoring/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IProjectRepository _projectRepository;

    public ProjectsController(IMediator mediator, IProjectRepository projectRepository)
    {
        _mediator = mediator;
        _projectRepository = projectRepository;
    }

    /// <summary>
    /// Crea un nuevo proyecto.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectResource resource)
    {
        var command = new CreateProjectCommand(
            resource.UserId,
            resource.Name,
            resource.Description,
            resource.StartDate,
            resource.PropertyId,
            resource.EndDate
        );

        var projectId = await _mediator.Send(command);

        // Opcional: solo devolvemos el id, pero la ruta GetProjectById ya devolverá el ProjectResource
        return CreatedAtAction(nameof(GetProjectById), new { id = projectId }, new { id = projectId });
    }

    /// <summary>
    /// GET: Obtiene los detalles de un proyecto (formato amigable para el front).
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProjectById(int id)
    {
        var project = await _projectRepository.FindByIdAsync(id);

        if (project is null) return NotFound();

        var resource = new ProjectResource(
            project.Id,
            project.PropertyId,
            project.UserId,
            project.Name,
            project.Description,
            project.Status,
            project.StartDate,
            project.EndDate,
            project.CreatedAt
        );

        return Ok(resource);
    }
}
