using DotnetCqrs.Infrastructure.Configuration;

namespace DotnetCqrs.Features.Users.Handlers;

public interface IUpdateUserHandler
{
    Task HandleAsync(UpdateUserCommand command);
}

public class UpdateUserHandler(IUsersRepository repo) : IUpdateUserHandler
{
    public async Task HandleAsync(UpdateUserCommand command)
    {
        var user = await repo.GetSingleAsync(e => e.Id == command.Id);
        if (user == null) throw new NotFoundEx();
        user.Username = command.Username;
        user.Email = command.Email;
        user.StartDate = command.StartDate;
        user.StartTime = command.StartTime;
        repo.Update(user);
        await repo.CommitAsync();
    }
}