using DotnetCqrs.Features.Users.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace DotnetCqrs.Features.Users;

[ApiController]
[Route("[controller]")]
public class UsersController(
    ICreateUserHandler createHandler,
    IGetUserByIdHandler getHandler,
    IGetAllUsersHandler getAllUsersHandler,
    IUpdateUserHandler updateHandler,
    IDeleteUserHandler deleteHandler)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await getAllUsersHandler.HandleAsync(new GetAllUsersQuery());
        return Ok(users);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await getHandler.HandleAsync(new GetUserByIdQuery(id));
        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        var command = new CreateUserCommand(request.Username, request.Email, request.StartDate, request.StartTime);
        await createHandler.HandleAsync(command);
        return NoContent();
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest request)
    {
        var command = new UpdateUserCommand(id, request.Username, request.Email, request.StartDate, request.StartTime);
        await updateHandler.HandleAsync(command);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await deleteHandler.HandleAsync(id);
        return NoContent();
    }
}