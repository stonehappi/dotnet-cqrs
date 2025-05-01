namespace DotnetCqrs.Features.Users.Handlers;

public interface ICreateUserHandler
{
    Task HandleAsync(CreateUserCommand command);
}

public class CreateUserHandler(IUsersRepository repo) : ICreateUserHandler
{
    public async Task HandleAsync(CreateUserCommand command)
    {
        var user = new UsersEntity
        {
            Username = command.Username,
            Email = command.Email,
            StartDate = command.StartDate,
            StartTime = command.StartTime
        };
        await repo.AddAsync(user);
        await repo.CommitAsync();
    }
}