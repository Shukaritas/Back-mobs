using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using RentalPeAPI.Property.Application.Internal.CommandServices;
using RentalPeAPI.Property.Application.Internal.QueryServices;
using RentalPeAPI.Property.Application.Services;
using RentalPeAPI.Property.Interfaces.Rest.Transform;
using RentalPeAPI.Property.Interfaces.Rest.Resources;

namespace RentalPeAPI.Property.Interfaces.Rest.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize] // Requiere JWT en todos los endpoints
    public class SpaceController : ControllerBase
    {
        private readonly SpaceAppService _spaceAppService;

        public SpaceController(SpaceAppService spaceAppService)
        {
            _spaceAppService = spaceAppService;
        }
        
        [HttpGet]
        [Authorize(Roles = "Homeowner,Remodeler")]
        public async Task<ActionResult<IEnumerable<SpaceResource>>> GetAllSpaces()
        {
            var spaces = await _spaceAppService.ListSpacesAsync(new ListSpacesQuery(null, null));
            var resources = spaces.Select(SpaceResourceAssembler.ToResource);
            return Ok(resources);
        }
        
        [HttpGet("{id}")]
        [Authorize(Roles = "Homeowner,Remodeler")]
        public async Task<ActionResult<SpaceResource>> GetSpaceById(long id)
        {
            var space = await _spaceAppService.GetSpaceByIdAsync(new GetSpaceByIdQuery(id));
            if (space == null) return NotFound();
            return Ok(SpaceResourceAssembler.ToResource(space));
        }
        
        [HttpPost]
        [Authorize(Roles = "Homeowner")]
        public async Task<ActionResult<SpaceResource>> CreateSpace([FromBody] CreateSpaceResource resource)
        {
            var command = SpaceCommandAssembler.ToCommand(resource);
            var space = await _spaceAppService.CreateSpaceAsync(command);
            var resultResource = SpaceResourceAssembler.ToResource(space);
            return CreatedAtAction(nameof(GetSpaceById), new { id = space.Id }, resultResource);
        }
        
        [HttpPut("{id}")]
        [Authorize(Roles = "Homeowner")]
        public async Task<IActionResult> UpdateSpace(long id, [FromBody] UpdateSpaceResource resource)
        {
            var command = SpaceCommandAssembler.ToCommand(id, resource);
            var updated = await _spaceAppService.UpdateSpaceAsync(command);
            if (updated == null) return NotFound();
            return NoContent();
        }

        // ✅ DELETE: api/v1/space/{id} - Solo Homeowners pueden eliminar sus espacios
        [HttpDelete("{id}")]
        [Authorize(Roles = "Homeowner")]
        public async Task<IActionResult> DeleteSpace(long id)
        {
            var deleted = await _spaceAppService.DeleteSpaceAsync(new DeleteSpaceCommand(id));
            if (!deleted) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Acepta un proyecto publicado por parte de un Remodeler.
        /// Solo accesible para usuarios con rol "Remodeler".
        /// El RemodelerId se extrae del token JWT (ClaimTypes.NameIdentifier).
        /// 
        /// POST /api/v1/spaces/{id}/accept
        /// </summary>
        /// <param name="id">ID del espacio a aceptar</param>
        /// <returns>200 OK con SpaceResource actualizado</returns>
        /// <response code="200">Proyecto aceptado exitosamente</response>
        /// <response code="400">SpaceId inválido o ya ha sido aceptado</response>
        /// <response code="401">Token JWT inválido o no autorizado</response>
        /// <response code="403">Usuario no tiene rol "Remodeler"</response>
        /// <response code="404">Espacio no encontrado</response>
        [HttpPost("{id}/accept")]
        [Authorize(Roles = "Remodeler")] // Solo Remodelers pueden aceptar proyectos
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AcceptProject(long id)
        {
            // Extraer el ID del Remodeler desde el token JWT usando ClaimTypes.NameIdentifier
            var remodelIdClaim = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(remodelIdClaim) || !Guid.TryParse(remodelIdClaim, out var remodelerId))
                return Unauthorized(new { error = "Token JWT inválido o sin NameIdentifier." });

            // Validar que el rol sea exactamente "Remodeler"
            var roleClaim = User.FindFirstValue(System.Security.Claims.ClaimTypes.Role);
            if (string.IsNullOrEmpty(roleClaim) || !roleClaim.Equals("Remodeler", StringComparison.OrdinalIgnoreCase))
                return Forbid();

            try
            {
                var command = new AcceptSpaceCommand(id, remodelerId);
                var result = await _spaceAppService.AcceptProjectAsync(command);

                if (result == null)
                    return NotFound(new { error = $"Espacio con ID {id} no encontrado." });

                var resultResource = SpaceResourceAssembler.ToResource(result);
                return Ok(new 
                { 
                    message = "Proyecto aceptado exitosamente.", 
                    data = resultResource 
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Error al aceptar el proyecto: {ex.Message}" });
            }
        }
    }
}