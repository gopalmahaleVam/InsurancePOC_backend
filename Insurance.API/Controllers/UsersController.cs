
using Insurance.Application.Features.Users.Commands;
using Insurance.Application.Features.Users.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.API.Controllers
{
    [ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }
[HttpPost]
public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
{
    var command = new CreateUserCommand(
        dto.Username,
        dto.Email,
        dto.PasswordHash,
        dto.FirstName,
        dto.LastName,
        dto.PhoneNumber,
        dto.Role
    );

    var userId = await _mediator.Send(command);

    return CreatedAtAction(nameof(CreateUser), new { id = userId }, userId);
}


    // [HttpPut("{id}")]
    // public async Task<IActionResult> Update(int id, UpdateUserCommand command)
    // {
    //     command.Id = id;
    //     return await _mediator.Send(command) ? Ok() : NotFound();
    // }

    // [HttpDelete("{id}")]
    // public async Task<IActionResult> Delete(int id)
    //     => await _mediator.Send(new DeleteUserCommand { Id = id }) ? Ok() : NotFound();

    // [HttpGet("{id}")]
    // public async Task<IActionResult> Get(int id)
    //     => Ok(await _mediator.Send(new GetUserByIdQuery { Id = id }));

    // [HttpGet("search")]
    // public async Task<IActionResult> Search(string name)
    //     => Ok(await _mediator.Send(new SearchUserQuery { Name = name }));
}
}
