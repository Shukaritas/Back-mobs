using Microsoft.AspNetCore.Mvc;
using RentalPeAPI.Property.Application.Internal.CommandServices;
using RentalPeAPI.Property.Application.Internal.QueryServices;
using RentalPeAPI.Property.Application.Services;
using RentalPeAPI.Property.Interfaces.Rest.Transform;
using RentalPeAPI.Property.Interfaces.Rest.Resources;

namespace RentalPeAPI.Property.Interfaces.Rest.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SpaceController : ControllerBase
    {
        private readonly SpaceAppService _spaceAppService;

        public SpaceController(SpaceAppService spaceAppService)
        {
            _spaceAppService = spaceAppService;
        }

        // ✅ GET: api/v1/space
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SpaceResource>>> GetAllSpaces()
        {
            var spaces = await _spaceAppService.ListSpacesAsync(new ListSpacesQuery(null, null));
            var resources = spaces.Select(SpaceResourceAssembler.ToResource);
            return Ok(resources);
        }

        // ✅ GET: api/v1/space/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<SpaceResource>> GetSpaceById(long id)
        {
            var space = await _spaceAppService.GetSpaceByIdAsync(new GetSpaceByIdQuery(id));
            if (space == null) return NotFound();
            return Ok(SpaceResourceAssembler.ToResource(space));
        }

        // ✅ POST: api/v1/space
        [HttpPost]
        public async Task<ActionResult<SpaceResource>> CreateSpace([FromBody] CreateSpaceResource resource)
        {
            var command = SpaceCommandAssembler.ToCommand(resource);
            var space = await _spaceAppService.CreateSpaceAsync(command);
            var resultResource = SpaceResourceAssembler.ToResource(space);
            return CreatedAtAction(nameof(GetSpaceById), new { id = space.Id }, resultResource);
        }

        // ✅ PUT: api/v1/space/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSpace(long id, [FromBody] UpdateSpaceResource resource)
        {
            var command = SpaceCommandAssembler.ToCommand(id, resource);
            var updated = await _spaceAppService.UpdateSpaceAsync(command);
            if (updated == null) return NotFound();
            return NoContent();
        }

        // ✅ DELETE: api/v1/space/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpace(long id)
        {
            var deleted = await _spaceAppService.DeleteSpaceAsync(new DeleteSpaceCommand(id));
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}