using MediatR; 
using Microsoft.AspNetCore.Mvc;
using RentalPeAPI.User.Application.Internal.CommandServices;
using RentalPeAPI.User.Application.Internal.QueryServices;
using RentalPeAPI.User.Interfaces.REST.Resources;

namespace RentalPeAPI.User.Interfaces.REST.Controllers;

[ApiController]
[Route("api/[controller]")] 
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> RegisterUser([FromBody] RegisterUserResource resource)
    {
        var command = new RegisterUserCommand(
            resource.FullName,
            resource.Email,
            resource.Password,
            resource.Phone,
            resource.Role,
            resource.Photo
        );

        var userDto = await _mediator.Send(command);

        // Ahora Swagger sabe que la respuesta es UserDto
        return CreatedAtAction(nameof(GetUserById), new { userId = userDto.Id }, userDto);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginResource resource)
    {
        try
        {
            var query = new LoginQuery(resource.Email, resource.Password);
            var authDto = await _mediator.Send(query);
            
            return Ok(authDto); 
        }
        catch (Exception ex)
        {
            return Unauthorized(new { message = ex.Message }); 
        }
    }
    
    [HttpGet("{userId:guid}")] 
    public async Task<ActionResult<UserDto>> GetUserById(Guid userId)
    {
        var query = new GetUserByIdQuery(userId);
        var userDto = await _mediator.Send(query);

        if (userDto is null) return NotFound();

        return Ok(userDto);
    }
    
    [HttpPost("{userId:guid}/payment-methods")]
    public async Task<ActionResult<UserDto>> AddPaymentMethod(Guid userId, [FromBody] AddPaymentMethodResource resource)
    {
        var command = new AddPaymentMethodCommand(
            userId,
            resource.Type,
            resource.Number,
            resource.Expiry,
            resource.Cvv
        );

        var userDto = await _mediator.Send(command);
        return Ok(userDto);
    }
}